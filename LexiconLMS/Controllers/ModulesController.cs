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

            return View(module);
        }

        // GET: Modules/Create
        [Authorize(Roles = "Teacher")]
        public IActionResult Create(int? id)
        {
            if (id is null || !CourseExists((int) id))
            {
                return NotFound();
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

        private DateTime GetCourseStartTime(int courseId)
        {
            return db.Courses
                .FirstOrDefault(c => c.Id == courseId)
                .StartTime;
        }
 
        private bool IsModuleTimeCorrect(int courseId, DateTime startTime, DateTime endTime, int? thisModuleId)
        {
            // Module starttime must be < module endtime
            if (endTime < startTime)
            {
                return false;
            }
            //  Module ModuleStartTime must be >= course start time  
            if (startTime < GetCourseStartTime(courseId))
            {
                return false;
            }

           
            var modules = db.Modules
                .Where(m => m.CourseId == courseId)
                .ToList();

            // TODO: when Edit Module: don't include *this* module in the check
            foreach (var module in modules)
            {
                if (module.Id != thisModuleId)
                {
                    if ((startTime >= module.StartTime && startTime < module.EndTime)
                        || (endTime > module.StartTime && endTime <= module.EndTime))
                    {
                        return false;
                    }
                }
            }

            return true;
        }



        // POST: Modules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create(ModuleActivityPostViewModel viewModel)  //  viewModel skapas med json data (Torbjörn fattar nu)
        {
            if (ModelState.IsValid)
            {
                // TODO: perform id control comparison
                // TODO: Check if redirect problem stems from ajax 

                // Check module not is in same timespan as existing module for this course.. Also validate other things for start and end time
                if (!IsModuleTimeCorrect(viewModel.Module.CourseId, viewModel.Module.ModuleStartTime, viewModel.Module.ModuleEndTime, null))
                {
                    // TODO: how do we get error feedback?
                    return View(viewModel);
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

                // TODO: time checks
                // (for each activity)
                // - ActivityStartTime must be >= ModuleStartTime
                // - ActivityEndTime must be <= ModuleEndTime
                // - ActivityStartTime and ActivityEndTime time span must not overlap any other activity in this module


                // TODO: what if viewModel.Data == null (no activities added). Before NPE was thrown and we stayed on create page. Now returns to course page
                if (viewModel.Data != null)
                {
                    foreach (var item in viewModel.Data)
                    {
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
                //return RedirectToAction(nameof(Index)); // TODO: change so it points to course dashboard
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


            // List of activities so it can be displayed in the Edit View

            var activityList = await db.Activities.Include(t => t.ActivityType).Where(a => a.ModuleId == id).ToListAsync();
            var activityTypeList = activityList.Select(a => a.ActivityType).FirstOrDefault();

            var viewModel = new ModuleEditViewModel
            {
                ModuleId = module.Id,
                ModuleName = module.Name,
                ModuleDescription = module.Description,
                ModuleStartTime = module.StartTime,
                ModuleEndTime = module.EndTime,
                Activities = activityList,
                ActivityType = activityTypeList

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

            //if (id != viewmodel.Module.Id)
            //{
            //    return NotFound();


            var module = db.Modules.Find(viewModel.Module.ModuleId);
            
            // Validate start and end time
            if (!IsModuleTimeCorrect(module.CourseId, viewModel.Module.ModuleStartTime, viewModel.Module.ModuleEndTime, viewModel.Module.ModuleId))
            {
                // TODO: how do we get error feedback?
                return View(viewModel);
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

                    // TODO: what if viewModel.Data == null (no activities added). Before NPE was thrown and we stayed on create page. Now returns to course page
                    if (viewModel.Data != null)
                    {
                        foreach (var item in viewModel.Data)
                        {
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
                //return RedirectToAction(nameof(Index));                 // TODO, byt ut till redirect to url json.
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
