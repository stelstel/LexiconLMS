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
using LexiconLMS.Models.ViewModels.Teacher;

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
            var userCourse = await GetUserCourseAsync(userId);

            var model = await db.Activities.Include(a => a.ActivityType)
                .Include(a => a.Module)
                .ThenInclude(a => a.Course)
                .Where(a => a.Module.Course == userCourse && a.ActivityType.Name == "Assignment")
                .Select(a => new AssignmentListViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    IsFinished = false
                })
                .ToListAsync();

            return model;
        }

        private async Task<Course> GetUserCourseAsync(string userId)
        {
            return await db.Users.Include(a => a.Course)
                .Where(a => a.Id == userId)
                .Select(a => a.Course)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ModuleListViewModel>> GetStudentModuleListAsync()
        {
            var userId = userManager.GetUserId(User);
            var userCourse = await GetUserCourseAsync(userId);

            var model = await db.Modules.Include(a => a.Course)
                .Where(a => a.Course.Id == userCourse.Id)
                .Select(a => new ModuleListViewModel
                {
                    Id = a.Id,
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
            var userCourse = await GetUserCourseAsync(userId);

            var model = await db.Activities.Include(a => a.ActivityType)
                .Include(a => a.Module)
                .ThenInclude(a => a.Course)
                .Where(a => a.Module.Course.Id == userCourse.Id)
                .Select(a => new ActivityListViewModel
                {
                    Id = a.Id,
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
        [Authorize(Roles = "Teacher")]
        public IActionResult CreateUser()
        {
             return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateUser(CreateUserViewModel newUser)
        {
            if (ModelState.IsValid)
            {
                if (AppUserEmailExists(newUser.Email))
                {
                    ModelState.AddModelError(string.Empty, $"User '{newUser.Email}' already exists");
                    return View();
                }

                var addUser = new AppUser
                {
                    UserName = newUser.Email,
                    Email = newUser.Email,
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    Course = await CreateCourseSelectList(newUser)
                };

                var createResult = await userManager.CreateAsync(addUser, newUser.Password);

                if (createResult.Succeeded)
                {
                    var createdUser = await userManager.FindByNameAsync(newUser.Email);

                    // Add role(s)
                    // Before adding role, verify that roles haven't have been applied yet.
                    // All users have role Student
                    if (!await userManager.IsInRoleAsync(createdUser, "Student"))
                    {
                        await AddAppUserToRoleAsync(createdUser, "Student");

                        // Add Teacher role if applicable
                        if (newUser.IsTeacher)
                        {
                            if (!await userManager.IsInRoleAsync(createdUser, "Teacher"))
                            {
                                await AddAppUserToRoleAsync(createdUser, "Teacher");
                            }
                        }
                    }
                }

                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }

        private async Task<IdentityResult> AddAppUserToRoleAsync(AppUser createdUser, String role)
        {
            var addToRoleResult = await userManager.AddToRoleAsync(createdUser, role);
            if (!addToRoleResult.Succeeded)
            {
                throw new Exception(string.Join("\n", addToRoleResult.Errors));
            }
            return addToRoleResult;
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

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Student()
        {
            var userId = userManager.GetUserId(User);
            var moduleList = await GetStudentModuleListAsync();
            var activityList = await GetStudentActivityListAsync();
            var assignmentList = await GetStudentAssignmentsAsync();
            var current = await Current();

            var appUser = await db.Users
                .Include(a => a.Course)
                .ThenInclude(a => a.Modules)
                .ThenInclude(a => a.Activities)
                .Include(a => a.Course)
                .ThenInclude(a => a.AppUsers)
                .FirstOrDefaultAsync(a => a.Id == userId);

            var model = new StudentViewModel
            {
                AssignmentList = assignmentList,
                ModuleList = moduleList,
                ActivityList = activityList,
                AppUser = appUser,
                CurrentViewModel = current
            };

            return View(model);
        }

        public async Task<CurrentViewModel> Current()
        {
            var userId = userManager.GetUserId(User);

            var userCourse = await db.Users.Include(a => a.Course)
                .ThenInclude(c => c.AppUsers)
                .Where(a => a.Id == userId)
                .Select(a => a.Course)
                .FirstOrDefaultAsync();

            var timeNow = DateTime.Now;

            var modules = await db.Modules.Include(a => a.Course)
               .Where(a => a.Course.Id == userCourse.Id)
               .ToListAsync();

            var currentModule = modules.OrderBy(t => Math.Abs((t.StartTime - timeNow).Ticks)).First();

            var activities = await db.Activities
               .Where(a => a.ModuleId == currentModule.Id)
               .ToListAsync();

            var currentActivity = new Activity();

            if (activities.Count > 0)
                currentActivity = activities.OrderBy(t => Math.Abs((t.StartTime - timeNow).Ticks)).First();
            else
                currentActivity.Name = "No current activities";

            var assignments = await db.Activities.Include(a => a.ActivityType)
                .Include(a => a.Module)
                .ThenInclude(a => a.Course)
                .Where(a => a.Module.Course.Id == userCourse.Id && a.ActivityType.Name == "Assignment")
                .ToListAsync();

            var currentAssignment = new List<Activity>();

            if (assignments.Count > 0)
            {
                assignments.OrderBy(t => Math.Abs((t.StartTime - timeNow).Ticks));

                for (int i = 0; i < assignments.Count && i < 3; i++)
                {
                    currentAssignment.Add(assignments[i]);
                }
            }

            var model = new CurrentViewModel
            {
                Course = userCourse,
                Module = currentModule,
                Activity = currentActivity,
                Assignments = currentAssignment
            };

            return model;
        }

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

        // Teacher Course Page
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Teacher(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await db.Courses.FirstOrDefaultAsync(m => m.Id == id);

            var current = await CurrentTeacher(id);

            var model = new TeacherViewModel
            {
                Course = course,
                TeacherCurrentViewModel = current,
            };

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // Teacher Home Page
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TeacherHome()
        {
            var courses = await db.Courses.ToListAsync();

            var model = new TeacherHomeViewModel
            {
                Courses = courses
            };

            return View(model);
        }

        public async Task<TeacherCurrentViewModel> CurrentTeacher(int? id)
        {
            var timeNow = DateTime.Now;

            var course = await db.Courses.Include(c => c.AppUsers)
                .Include(c => c.Modules)
                .ThenInclude(c => c.Activities)
                .ThenInclude(c => c.ActivityType)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            var module = course.Modules.OrderBy(t => Math.Abs((t.StartTime - timeNow).Ticks)).First();

            var activity = module.Activities.OrderBy(t => Math.Abs((t.StartTime - timeNow).Ticks)).First();

            var assignments = module.Activities.Where(c => c.ActivityType.Name == "Assignment")
                .OrderBy(t => Math.Abs((t.StartTime - timeNow).Ticks)).ToList();

            if (assignments.Count < 3)
                assignments = assignments.Take(assignments.Count).ToList();
            else
                assignments = assignments.Take(3).ToList();

            // Percentage of the students that is finished - NOT IMPLEMENTED
            double finished = 0.0;

            var model = new TeacherCurrentViewModel
            {
                Course = course,
                Module = module,
                Activity = activity,
                Assignments = assignments,
                Finished = $"{finished} %"
            };

            return model;
        }

        private bool AppUserExists(string id)
        {
            return db.Users.Any(e => e.Id == id);
        }

        private bool AppUserEmailExists(string email)
        {
            return db.Users.Any(e => e.Email == email);
        }
        private async Task<Course> CreateCourseSelectList(CreateUserViewModel newUser)
        {
            Course course = null;

            // newUser.CourseId is null if nothing selected in course dropdown list
            if (newUser.CourseId != null && !newUser.IsTeacher)
            {
                course = await db.Courses.FirstOrDefaultAsync(c => c.Id == newUser.CourseId);
            }

            return course;
        }

    }
}

