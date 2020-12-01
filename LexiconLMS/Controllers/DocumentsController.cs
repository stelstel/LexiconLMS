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
using LexiconLMS.Models.ViewModels.Document;
using Microsoft.AspNetCore.Identity;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace LexiconLMS.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<AppUser> userManager;
        private readonly IWebHostEnvironment web;

        public DocumentsController(ApplicationDbContext db, UserManager<AppUser> userManager, IWebHostEnvironment web)
        {
            this.db = db;
            this.userManager = userManager;
            this.web = web;
        }

        // GET: Documents
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = db.Documents.Include(d => d.Activity).Include(d => d.AppUser).Include(d => d.Course).Include(d => d.Module);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Upload Course Document
        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public async Task<IActionResult> UploadCourseDoc(int? id)
        {
            var courses = await db.Courses.ToListAsync();
            var course = courses.Where(a => a.Id == id).FirstOrDefault();

            var model = new UploadCourseDocumentViewModel
            {
                Course = course
            };

            return View(model);
        }

        // POST: Upload Course Document
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCourseDoc(int? id, Document model, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var userId = userManager.GetUserId(User);

                var courses = await db.Courses.ToListAsync();
                var course = courses.Where(a => a.Id == id).FirstOrDefault();

                string path = Path.Combine(web.WebRootPath, $"uploads/{course.Name}");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = Path.GetFileName(file.FileName);

                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var newModel = new Document
                {
                    Name = fileName.Split(".")[0],
                    Description = model.Description,
                    Course = course,
                    CourseId = course.Id,
                    UploadTime = DateTime.Now,
                    AppUserId = userId
                };

                db.Add(newModel);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Upload Module Document
        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public async Task<IActionResult> UploadModuleDoc(int? id)
        {
            var modules = await db.Modules.Include(m => m.Course).ToListAsync();
            var module = modules.Where(a => a.Id == id).FirstOrDefault();

            var model = new UploadModuleDocumentViewModel
            {
                Module = module,
                Course = module.Course
            };

            return View(model);
        }

        // POST: Upload Module Document
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadModuleDoc(int? id, Document document, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var userId = userManager.GetUserId(User);

                var modules = await db.Modules.Include(m => m.Course).ToListAsync();
                var module = modules.Where(a => a.Id == id).FirstOrDefault();

                string path = Path.Combine(web.WebRootPath, $"uploads/{module.Course.Name}/{module.Name}");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = Path.GetFileName(file.FileName);

                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var model = new Document
                {
                    Name = fileName.Split(".")[0],
                    Description = document.Description,
                    Module = module,
                    ModuleId = module.Id,
                    CourseId = module.CourseId,
                    UploadTime = DateTime.Now,
                    AppUserId = userId
                };

                db.Add(model);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(document);
        }

        // GET: Get Upload Module Document
        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public async Task<IActionResult> UploadActivityDoc(int? id)
        {
            var activities = await db.Activities.ToListAsync();
            var activity = activities.Where(a => a.Id == id).FirstOrDefault();
            var module = await db.Modules.Where(a => a.Id == activity.ModuleId).FirstOrDefaultAsync();
            var courses = await db.Courses.ToListAsync();
            var course = courses.Where(c => c.Id == module.CourseId).FirstOrDefault();

            var model = new UploadActivityDocumentViewModel
            {
                Id = id,
                Activity = activity,
                Module = module,
                Course = course
            };

            return View(model);
        }

        // POST: Get Upload Course Document
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadActivityDoc(int id, Document document, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var userId = userManager.GetUserId(User);
                var upload = DateTime.Now;

                var course = await db.Activities.Include(a => a.Module).ThenInclude(a => a.Course)
                    .Where(a => a.Id == id)
                    .Where(a => a.ModuleId == document.ModuleId)
                    .Select(a => a.Module.Course).FirstOrDefaultAsync();

                var module = await db.Modules.Where(m => m.Id == document.ModuleId).FirstOrDefaultAsync();

                var activity = await db.Activities.Where(a => a.Id == id).FirstOrDefaultAsync();

                string path = Path.Combine(web.WebRootPath, $"uploads/{course.Name}/{module.Name}/Activities/{activity.Name}");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = Path.GetFileName(file.FileName);

                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var model = new Document
                {
                    Name = fileName.Split(".")[0],
                    Description = document.Description,
                    ModuleId = document.ModuleId,
                    ActivityId = document.ActivityId,
                    CourseId = course.Id,
                    UploadTime = upload,
                    AppUserId = userId,
                };

                db.Add(model);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(document);
        }

        // GET: Documents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await db.Documents
                .Include(d => d.Activity)
                .Include(d => d.AppUser)
                .Include(d => d.Course)
                .Include(d => d.Module)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // GET: Documents/Create
        public IActionResult Create()
        {
            ViewData["ActivityId"] = new SelectList(db.Activities, "Id", "Id");
            ViewData["AppUserId"] = new SelectList(db.Users, "Id", "Id");
            ViewData["CourseId"] = new SelectList(db.Courses, "Id", "Id");
            ViewData["ModuleId"] = new SelectList(db.Modules, "Id", "Id");
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,UploadTime,AppUserId,CourseId,ModuleId,ActivityId")] Document document)
        {
            if (ModelState.IsValid)
            {
                db.Add(document);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActivityId"] = new SelectList(db.Activities, "Id", "Id", document.ActivityId);
            ViewData["AppUserId"] = new SelectList(db.Users, "Id", "Id", document.AppUserId);
            ViewData["CourseId"] = new SelectList(db.Courses, "Id", "Id", document.CourseId);
            ViewData["ModuleId"] = new SelectList(db.Modules, "Id", "Id", document.ModuleId);
            return View(document);
        }

        // GET: Documents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await db.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            ViewData["ActivityId"] = new SelectList(db.Activities, "Id", "Id", document.ActivityId);
            ViewData["AppUserId"] = new SelectList(db.Users, "Id", "Id", document.AppUserId);
            ViewData["CourseId"] = new SelectList(db.Courses, "Id", "Id", document.CourseId);
            ViewData["ModuleId"] = new SelectList(db.Modules, "Id", "Id", document.ModuleId);
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,UploadTime,AppUserId,CourseId,ModuleId,ActivityId")] Document document)
        {
            if (id != document.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(document);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentExists(document.Id))
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
            ViewData["ActivityId"] = new SelectList(db.Activities, "Id", "Id", document.ActivityId);
            ViewData["AppUserId"] = new SelectList(db.Users, "Id", "Id", document.AppUserId);
            ViewData["CourseId"] = new SelectList(db.Courses, "Id", "Id", document.CourseId);
            ViewData["ModuleId"] = new SelectList(db.Modules, "Id", "Id", document.ModuleId);
            return View(document);
        }

        // GET: Documents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await db.Documents
                .Include(d => d.Activity)
                .Include(d => d.AppUser)
                .Include(d => d.Course)
                .Include(d => d.Module)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var document = await db.Documents.FindAsync(id);
            db.Documents.Remove(document);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentExists(int id)
        {
            return db.Documents.Any(e => e.Id == id);
        }

        // GET: Download Course Document
        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<IActionResult> DownloadCourseDoc(int? id)
        {
            var courses = await db.Courses.ToListAsync();
            var course = courses.Where(a => a.Id == id).FirstOrDefault();
            var directoryPath = Path.Combine(web.WebRootPath, $"uploads/{course.Name}");

            ICollection<Document> docs = await db.Documents
                .Where(d => d.Course == course)
                //.Where(d => d.ModuleId == null)
                //.Where(d => d.ActivityId == null)
                .ToListAsync();

            // ONLY course documents. NOT module or activity documents (SearchOption.AllDirectories to see the rest)
            ICollection<string> fileList = Directory.GetFiles(directoryPath, "*.*", SearchOption.TopDirectoryOnly);

            var model = new DownloadCourseDocumentViewModel
            {
                Course = course,
                DirectoryPath = directoryPath,
                fileList = fileList
            };

            return View(model);
        }

        // POST: Download Course Document
        [Authorize(Roles = "Student, Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> DownloadCourseDoc(int id, /*Document model, IFormFile file*/ string file)
        public async Task<IActionResult> DownloadCourseDoc(int id, Document model, IFormFile file)
        {
            //var fil = TempData["fil"];

            if (ModelState.IsValid)
            {
                var userId = userManager.GetUserId(User);

                var courses = await db.Courses.ToListAsync();
                var course = courses.Where(a => a.Id == id).FirstOrDefault();
                var filePath = Path.Combine(web.WebRootPath, $"uploads/{course.Name}/{file}");
                //var filePath = Path.Combine(web.WebRootPath, file.ToString());

                var memory = new MemoryStream();

                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0;

                return File(memory, GetContType(filePath), Path.GetFileName(filePath));
            }

            //return View(model);
            return null;
        }

        [HttpGet]
        public string GetContType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
