using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LexiconLMS.Data;
using LexiconLMS.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using LexiconLMS.Models.ViewModels.Student;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using LexiconLMS.Models.ViewModels.Activity;

namespace LexiconLMS.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<AppUser> userManager;
        private readonly IWebHostEnvironment web;

        public ActivitiesController(ApplicationDbContext db, UserManager<AppUser> userManager, IWebHostEnvironment web)
        {
            this.db = db;
            this.userManager = userManager;
            this.web = web;
        }

        // GET: Activities
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Index()
        {
            var model = await db.Activities.Include(a => a.ActivityType)
                .Include(a => a.Module)
                .OrderBy(a => a.ModuleId)
                .ThenBy(a => a.StartTime)
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentUpload(int id)
        {
            var model = await db.Activities.Include(a => a.ActivityType)
                .Include(a => a.Module)
                .ThenInclude(a => a.Course)
                .ThenInclude(a => a.AppUsers)
                .Where(a => a.Id == id)
                .Select(a => new StudentUploadViewModel
                {
                    Activity = a,
                    Course = a.Module.Course
                })
                .FirstOrDefaultAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentUpload(int id, Document model, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var userId = userManager.GetUserId(User);

                var activity = await db.Activities.Include(a => a.ActivityType)
                        .Include(a => a.Module)
                        .ThenInclude(a => a.Course)
                        .ThenInclude(a => a.AppUsers)
                        .Where(a => a.Id == id).FirstOrDefaultAsync();

                string path = Path.Combine(web.WebRootPath, $"uploads/Assignments/{activity.Name}");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = Path.GetFileName(file.FileName);

                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                model = new Document
                {
                    Name = fileName.Split(".")[0],
                    Description = activity.Description,
                    UploadTime = DateTime.Now,
                    IsFinished = true,
                    CourseId = activity.Module.Course.Id,
                    ModuleId = activity.Module.Id,
                    ActivityId = activity.Id,
                    AppUserId = userId,
                    FilePath = $"/uploads/Assignments/{activity.Name}/{fileName}"
                };

                db.Add(model);
                await db.SaveChangesAsync();
                return RedirectToAction("Student", "AppUsers");
            }

            return View(model);
        }

        // GET: Activities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await db.Activities
                .Include(a => a.ActivityType)
                .Include(a => a.Module)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (activity == null)
            {
                return NotFound();
            }


            // Get all students that have uploaded a document to an Activity of type 'Assignment'
            var students = await db.Documents
                .Where(d => d.ActivityId == activity.Id)
                .Where(f => f.IsFinished == true)
                .Select(s => s.AppUser)
                .ToListAsync();

            var viewmodel = new ActivityDetailsViewModel
            {
                Activity = activity,
                Students = students
            };

            return View(viewmodel);
        }

        // GET: Activities/Create
        public IActionResult Create()
        {
            ViewData["ActivityTypeId"] = new SelectList(db.ActivityTypes, "Id", "Id");
            ViewData["ModuleId"] = new SelectList(db.Modules, "Id", "Id");
            return View();
        }

        // POST: Activities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,StartTime,EndTime,ModuleId,ActivityTypeId")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                db.Add(activity);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActivityTypeId"] = new SelectList(db.ActivityTypes, "Id", "Id", activity.ActivityTypeId);
            ViewData["ModuleId"] = new SelectList(db.Modules, "Id", "Id", activity.ModuleId);
            return View(activity);
        }

        // GET: Activities/Edit/5
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await db.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }
            ViewData["ActivityTypeId"] = new SelectList(db.ActivityTypes, "Id", "Id", activity.ActivityTypeId);
            ViewData["ModuleId"] = new SelectList(db.Modules, "Id", "Id", activity.ModuleId);
            return View(activity);
        }

        // POST: Activities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,StartTime,EndTime,ModuleId,ActivityTypeId")] Activity activity)
        {
            if (id != activity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(activity);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActivityExists(activity.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActivityTypeId"] = new SelectList(db.ActivityTypes, "Id", "Id", activity.ActivityTypeId);
            ViewData["ModuleId"] = new SelectList(db.Modules, "Id", "Id", activity.ModuleId);
            return View(activity);
        }

        // GET: Activities/Delete/5
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await db.Activities
                .Include(a => a.ActivityType)
                .Include(a => a.Module)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        // POST: Activities/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[HttpPost]
        [Authorize(Roles = "Teacher")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {


            var activity = await db.Activities.FindAsync(id);
            var moduleId = activity.ModuleId;
            var documents = await db.Documents.Where(d => d.ActivityId == id).ToListAsync();

            foreach (var item in documents)
            {
                db.Documents.Remove(item);
            }

            db.Activities.Remove(activity);
            await db.SaveChangesAsync();
            
            return RedirectToAction(
                "Edit",
                "Modules",
                new { id = moduleId });
        }

        private bool ActivityExists(int id)
        {
            return db.Activities.Any(e => e.Id == id);
        }
    }
}
