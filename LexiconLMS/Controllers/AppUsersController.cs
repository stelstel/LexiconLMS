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
using LexiconLMS.Models.ViewModels;

namespace LexiconLMS.Controllers
{
    public class AppUsersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<AppUser> userManager;

        public AppUsersController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        // GET: Users
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Teacher"))
                {
                    return RedirectToAction(nameof(TeacherUserIndex));
                }
                return RedirectToAction(nameof(Student));
            }
            return View();
        }

        public async Task<List<AssignmentListViewModel>> GetStudentAssignmentsAsync()
        {
            var userId = userManager.GetUserId(User);

            var userCourseId = await db.Users.Include(a => a.Course)
                .Where(a => a.Id == userId)
                .Select(a => a.CourseId)
                .FirstOrDefaultAsync();

            var model = await db.Activities.Include(a => a.ActivityType)
                .Include(a => a.Module)
                .ThenInclude(a => a.Course)
                .Where(a => a.Module.CourseId == userCourseId && a.ActivityType.Name == "Assignment")
                .Select(a => new AssignmentListViewModel
                {
                    Name = a.Name,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    IsFinished = false
                })
                .ToListAsync();

            return model;
        }

        public async Task<List<ModuleListViewModel>> GetStudentModuleListAsync()
        {
            var userId = userManager.GetUserId(User);

            var userCourseId = await db.Users.Include(a => a.Course)
                .Where(a => a.Id == userId)
                .Select(a => a.CourseId)
                .FirstOrDefaultAsync();

            var model = await db.Modules.Include(a => a.Course)
                .Where(a => a.CourseId == userCourseId)
                .Select(a => new ModuleListViewModel
                {
                    Name = a.Name,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime
                })
                .ToListAsync();

            return model;
        }

        public async Task<List<ActivityListViewModel>> GetStudentActivityListAsync()
        {
            var userId = userManager.GetUserId(User);

            var userCourseId = await db.Users.Include(a => a.Course)
                .Where(a => a.Id == userId)
                .Select(a => a.CourseId)
                .FirstOrDefaultAsync();

            var model = await db.Activities.Include(a => a.ActivityType)
                .Include(a => a.Module)
                .ThenInclude(a => a.Course)
                .Where(a => a.Module.CourseId == userCourseId)
                .Select(a => new ActivityListViewModel
                {
                    Name = a.Name,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    ActivityType = a.ActivityType.Name
                })
                .ToListAsync();

            return model;
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var appUser = await db.Users
                .Include(a => a.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // GET: Users/Create
        //[Authorize(Roles = "Teacher")]
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(db.Set<Course>(), "Id", "Id");

            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                db.Add(appUser);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CourseId"] = new SelectList(db.Set<Course>(), "Id", "Id", appUser.CourseId);

            return View(appUser);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await db.Users.FindAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            ViewData["CourseId"] = new SelectList(db.Set<Course>(), "Id", "Id", appUser.CourseId);

            return View(appUser);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, AppUser appUser)
        {
            if (id != appUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(appUser);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUserExists(appUser.Id))
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

            ViewData["CourseId"] = new SelectList(db.Set<Course>(), "Id", "Id", appUser.CourseId);

            return View(appUser);
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await db.Users
               .Include(u => u.Course)
               .Include(u => u.Documents)
               .FirstOrDefaultAsync(u => u.Id == id);

            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var appUser = await db.Users
                .Include(u => u.Course)
                .Include(u => u.Documents)
                .FirstOrDefaultAsync(u => u.Id == id);

            db.Users.Remove(appUser);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //[Authorize(Roles = "Student")]
        public async Task<IActionResult> Student()
        {
            var userId = userManager.GetUserId(User);
            var moduleList = await GetStudentModuleListAsync();
            var activityList = await GetStudentActivityListAsync();
            var assignmentList = await GetStudentAssignmentsAsync();

            var appUser = await db.Users
                .Include(a => a.Course)
                .ThenInclude(a => a.Modules)
                .ThenInclude(a => a.Activities)
                .FirstOrDefaultAsync(a => a.Id == userId);

            var model = new StudentViewModel
            {
                AssignmentList = assignmentList,
                ModuleList = moduleList,
                ActivityList = activityList,
                AppUser = appUser
            };

            return View(model);
        }

        // Teacher: User Accounts Index


        //[Authorize(Roles = "Teacher")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> TeacherUserIndex()
        {
            var userList = await db.Users
                .OrderBy(u => u.LastName)
                .Include(a => a.Course)
                .ToListAsync();

            var model = new List<AppUserListViewModel>();
            foreach (var appUser in userList)
            {
                model.Add(new AppUserListViewModel
                {
                    AppUserId = appUser.Id,
                    FirstName = appUser.FirstName,
                    LastName = appUser.LastName,
                    Email = appUser.Email,
                    FullName = $"{appUser.FirstName} {appUser.LastName}",
                    Course = appUser.Course,
                    IsTeacher = await userManager.IsInRoleAsync(appUser, "Teacher")
                });
            }

            return View(model);

        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Teacher()
        {
            var userId = userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound();
            }

            var appUser = await db.Users
                .Include(a => a.Course)
                .FirstOrDefaultAsync(m => m.Id == userId);

            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        private bool AppUserExists(string id)
        {
            return db.Users.Any(e => e.Id == id);
        }
    }
}

