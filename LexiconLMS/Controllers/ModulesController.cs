using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LexiconLMS.Data;
using LexiconLMS.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using LexiconLMS.Models.ViewModels;
using AutoMapper;
using System;
using System.Collections.Generic;

namespace LexiconLMS.Controllers
{
    public class ModulesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;

        public ModulesController(ApplicationDbContext context, IMapper mapper)
        {
            db = context;
            this.mapper = mapper;
        }

        // GET: Modules
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = db.Modules.Include(c => c.Course);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Modules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var module = await db.Modules
                .Include(c => c.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (module == null)
            {
                return NotFound();
            }

            // All activities in the module.
            var activities = await db.Activities.Where(a => a.ModuleId == module.Id).ToListAsync();

            // All activity Ids in the module.
            var activityIds = await db.Activities.Where(a => a.ModuleId == module.Id).Select(i => i.Id).ToListAsync();

            // All documents in the module.
            var documents = await db.Documents.Where(d => d.ModuleId == module.Id).Include(a => a.Activity).ToListAsync();

            // All documents in the activities in the module.
            foreach (var doc in await db.Documents.Include(a => a.Activity).ToListAsync())            
            {
                foreach (var i in activityIds)
                {
                    if (doc.ActivityId == i)
                    {
                        if (!documents.Contains(doc))
                        {
                            documents.Add(doc);
                        }
                        
                    }
                } 
            }

            var viewmodel = new ModuleDetailsViewModel
            {
                Module = module,
                Documents = documents
            };

            return View(viewmodel);
        }

        // GET: Modules/Create
        [Authorize(Roles = "Teacher")]
        public IActionResult Create(int? id)
        {
            if (id is null || !CourseExists((int) id))
            {
                return NotFound();
            }

            if (TempData["ValidationError"] != null)
            {
                ModelState.AddModelError("", (string)TempData["ValidationError"]);
            }

            var moduleDefaultStartTime = GetCourseStartTime((int)id).AddHours(8);

            var model = new ModuleActivityCreateViewModel
            {
                CourseId = (int)id,
                ModuleStartTime = moduleDefaultStartTime,
                ModuleEndTime = moduleDefaultStartTime.AddHours(1),
                ActivityStartTime = moduleDefaultStartTime,
                ActivityEndTime = moduleDefaultStartTime.AddMinutes(10)
            };
            return View(model);
        }

  

        // POST: Modules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create(ModuleActivityPostViewModel viewModel) 
        {
            if (ModelState.IsValid)
            {
                // TODO: perform id control comparison
                // TODO: Check if redirect problem stems from ajax 

                // Check module not is in same timespan as existing module for this course. Also validate other things for start and end time

                var errorMessage = "";
                if (!IsModuleTimeCorrect(ref errorMessage, viewModel.Module.CourseId, viewModel.Module.ModuleStartTime, viewModel.Module.ModuleEndTime, null))
                {
                    TempData["ValidationError"] = errorMessage;
                    return Json(new { redirectToUrl = Url.Action("Create", "Modules", new { id = viewModel.Module.CourseId }) });
                    // TODO: Create view is reset with default values. Can that be fixed?
                }

                var module = new Module
                {                   
                    CourseId = viewModel.Module.CourseId,
                    Name = viewModel.Module.ModuleName,
                    Description = viewModel.Module.ModuleDescription,
                    StartTime = viewModel.Module.ModuleStartTime,
                    EndTime = viewModel.Module.ModuleEndTime
                };

                db.Add(module);

                if (IsActivityOverlap(viewModel.Data))
                {
                    TempData["ValidationError"] = "Activity start and end times overlap";
                    return Json(new { redirectToUrl = Url.Action("Create", "Modules", new { id = module.Id }) });
                }

                // TODO: what if viewModel.Data == null (no activities added). Before NPE was thrown and we stayed on create page. Now returns to course page
                if (viewModel.Data != null)
                {
                    foreach (var item in viewModel.Data)
                    {
                        if (!IsActivityTimeCorrect(ref errorMessage, null, viewModel.Module.ModuleStartTime, item.ActivityStartTime, item.ActivityEndTime))
                        {
                            TempData["ValidationError"] = errorMessage;
                            return Json(new { redirectToUrl = Url.Action("Create", "Modules", new { id = viewModel.Module.CourseId }) });
                        }
                        var activity = new Activity
                        {
                            Name = item.ActivityName,
                            Description = item.ActivityDescription,
                            StartTime = item.ActivityStartTime,
                            EndTime = item.ActivityEndTime,
                            ActivityTypeId = item.ActivityTypeId,
                            Module = module
                        };
                        db.Add(activity);
                    }
                }

                await db.SaveChangesAsync();
                return Json(new { redirectToUrl = Url.Action("Teacher", "AppUsers", new { id = viewModel.Module.CourseId }) });
            }
            //ViewData["CourseId"] = new SelectList(db.Courses, "Id", "Id", module.CourseId); // What does this show? Which course it belongs to?
            return View(viewModel);
        }

        // GET: Modules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var module = await db.Modules.FindAsync(id);
            if (module == null)
            {
                return NotFound();
            }

            if (TempData["ValidationError"] != null)
            {        
                ModelState.AddModelError("", (string) TempData["ValidationError"]);
            }

            // List of activities so it can be displayed in the Edit View

            var activityList = await db.Activities.Include(t => t.ActivityType).Where(a => a.ModuleId == id).ToListAsync();
            var activityTypeList = activityList.Select(a => a.ActivityType).FirstOrDefault();

            var viewModel = new ModuleEditViewModel
            {
                CourseId = module.CourseId,
                ModuleId = module.Id,
                ModuleName = module.Name,
                ModuleDescription = module.Description,
                ModuleStartTime = module.StartTime,
                ModuleEndTime = module.EndTime,
                Activities = activityList,
                ActivityType = activityTypeList,
                ActivityStartTime = DateTime.Now,
                ActivityEndTime = DateTime.Now

            };

            ViewData["CourseId"] = new SelectList(db.Courses, "Id", "Id", module.CourseId);
            return View(viewModel);
        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(ModuleActivityPostViewModel viewModel)
        {
            var module = db.Modules.Find(viewModel.Module.ModuleId);

            // Validate start and end time
            var errorMessage = "";
            if (!IsModuleTimeCorrect(ref errorMessage, module.CourseId, viewModel.Module.ModuleStartTime, viewModel.Module.ModuleEndTime, viewModel.Module.ModuleId))
            {                
                TempData["ValidationError"] = errorMessage;
                return Json(new { redirectToUrl = Url.Action("Edit", "Modules", new { id = module.Id }) });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    module.Name = viewModel.Module.ModuleName;
                    module.Description = viewModel.Module.ModuleDescription;
                    module.StartTime = viewModel.Module.ModuleStartTime;
                    module.EndTime = viewModel.Module.ModuleEndTime;

                    db.Update(module);
                    await db.SaveChangesAsync();

                    if (IsActivityOverlap(viewModel.Data))
                    {
                        TempData["ValidationError"] = "Activity start and end times overlap";
                        return Json(new { redirectToUrl = Url.Action("Edit", "Modules", new { id = module.Id }) });
                    }

                    // TODO: what if viewModel.Data == null (no activities added). Before NPE was thrown and we stayed on create page. Now returns to course page
                    if (viewModel.Data != null)
                    {
                        foreach (var item in viewModel.Data)
                        {
                            if (!IsActivityTimeCorrect(ref errorMessage, viewModel.Module.ModuleId, viewModel.Module.ModuleStartTime, item.ActivityStartTime, item.ActivityEndTime))
                            {
                                TempData["ValidationError"] = errorMessage;
                                return Json(new { redirectToUrl = Url.Action("Edit", "Modules", new { id = module.Id }) });
                            }
                            var activity = new Activity
                            {
                                Name = item.ActivityName,
                                Description = item.ActivityDescription,
                                StartTime = item.ActivityStartTime,
                                EndTime = item.ActivityEndTime,
                                ActivityTypeId = item.ActivityTypeId,
                                Module = module
                            };
                            db.Add(activity);
                        }

                        await db.SaveChangesAsync();
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleExists(module.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Json(new { redirectToUrl = Url.Action("Teacher", "AppUsers", new {id = module.CourseId }) });
            }
            ViewData["CourseId"] = new SelectList(db.Courses, "Id", "Id", module.CourseId);
            return View(module);
        }

        // GET: Modules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var module = await db.Modules
                .Include(c => c.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (module == null)
            {
                return NotFound();
            }

            return View(module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var module = await db.Modules.FindAsync(id);
            db.Modules.Remove(module);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private DateTime GetCourseStartTime(int courseId)
        {
            return db.Courses
                .FirstOrDefault(c => c.Id == courseId)
                .StartTime;
        }
        private DateTime GetModuleStartTime(int moduleId)
        {
            return db.Modules
                .FirstOrDefault(m => m.Id == moduleId)
                .StartTime;
        }

        private bool IsModuleTimeCorrect(ref string errorMessage, int courseId, DateTime startTime, DateTime endTime, int? thisModuleId)
        {
            // Module starttime must be < module endtime
            if (endTime < startTime)
            {
                errorMessage = "Module end time is before its start time";
                return false;
            }
            //  Module ModuleStartTime must be >= course start time  
            var courseStartTime = GetCourseStartTime(courseId);
            if (startTime < courseStartTime)
            {
                errorMessage = $"Module start time is before course start time ({courseStartTime}) ";
                return false;
            }

            var modules = db.Modules
                .Where(m => m.CourseId == courseId)
                .ToList();

            foreach (var module in modules)
            {
                if (module.Id != thisModuleId)
                {
                    if ((startTime < module.StartTime && endTime > module.EndTime)        // timespan over existing module
                        || (startTime >= module.StartTime && startTime < module.EndTime)  // startTime within existing module
                        || (endTime > module.StartTime && endTime <= module.EndTime))     // endTime within existing module
                    {
                        errorMessage = $"Module time span interferes with module '{module.Name}'";

                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsActivityTimeCorrect(ref string errorMessage, int? moduleId, DateTime moduleStartTime, DateTime startTime, DateTime endTime)
        {
            // TODO all this could be in IsActivityOverlap where we have sorted it. No need for the loop below either.

            // activity starttime must be < module endtime
            if (endTime < startTime)
            {
                errorMessage = "Activity end time is before its start time";
                return false;
            }
            //  Activity start time must be >= module start time  

            if (startTime < moduleStartTime)
            {
                errorMessage = $"Activity start time is before module start time ({moduleStartTime}) ";
                return false;
            }

           
            if (moduleId != null)  // moduleId is null when creating a new module
            {
                var activities = db.Activities
                    .Where(a => a.ModuleId == moduleId)
                    .ToList();

                foreach (var activity in activities)
                {
                    if ((startTime < activity.StartTime && endTime > activity.EndTime)        // timespan over existing activity
                        || (startTime >= activity.StartTime && startTime < activity.EndTime)  // startTime within existing activity
                        || (endTime > activity.StartTime && endTime <= activity.EndTime))     // endTime within existing activity
                    {
                        errorMessage = $"Activity time span interferes with activity '{activity.Name}'";

                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsActivityOverlap(IEnumerable<ActivityPostViewModel> viewModelData)
        {
            if (viewModelData.Count<ActivityPostViewModel>() < 2) return false;  // no need if count < 2
            {
                var length = viewModelData.Count<ActivityPostViewModel>();
                var sortedData = viewModelData.OrderBy(s => s.ActivityStartTime).ToArray<ActivityPostViewModel>();

                for (var i = 1; i < length; i++)
                {
                    if (sortedData[i - 1].ActivityEndTime > sortedData[i].ActivityStartTime)
                    {
                        return true;
                    }
                }
                return false;
            }
        }


        private bool ModuleExists(int id)
        {
            return db.Modules.Any(e => e.Id == id);
        }

        private bool CourseExists(int id)
        {
            return db.Courses.Any(e => e.Id == id);
        }
    }
}
