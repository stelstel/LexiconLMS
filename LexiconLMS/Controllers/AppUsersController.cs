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
using LexiconLMS.Extensions;
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
                    return RedirectToAction(nameof(TeacherHome));
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
            var timeNow = DateTime.Now;

            var modules = await db.Modules.Include(a => a.Course)
                .Where(a => a.Course.Id == userCourse.Id)
                .Select(a => new ModuleListViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    IsCurrentModule = false
                })
                .OrderBy(m => m.StartTime)
                .ToListAsync();

            var currentModuleId = modules.OrderBy(t => Math.Abs((t.StartTime - timeNow).Ticks)).First().Id;

            SetCurrentModule(modules, currentModuleId);

            return modules;
        }

        public async Task<List<TeacherModuleViewModel>> GetTeacherModuleListAsync(int? id)
        {
            var timeNow = DateTime.Now;

            var modules = await db.Modules.Include(a => a.Course)
                .Where(a => a.Course.Id == id)
                .Select(a => new TeacherModuleViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    IsCurrentModule = false
                })
                .OrderBy(m => m.StartTime)
                .ToListAsync();

            var currentModuleId = modules.OrderBy(t => Math.Abs((t.StartTime - timeNow).Ticks)).First().Id;

            SetCurrentTeacherModule(modules, currentModuleId);

            return modules;
        }

        //************** Makes a list out of ALL activities for a student ********************
        public async Task<List<ActivityListViewModel>> GetStudentActivityListAsync()
        {
            var userId = userManager.GetUserId(User);
            var userCourse = await GetUserCourseAsync(userId);

            var model = await db.Activities
                .Include(a => a.ActivityType)
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

        public async Task<List<ActivityListViewModel>> GetTeacherActivityListAsync(int? id)
        {
            var model = await db.Activities
                .Include(a => a.ActivityType)
                .Include(a => a.Module)
                .ThenInclude(a => a.Course)
                .Where(a => a.Module.Course.Id == id)
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

        //******************************************* GetModuleActivityListAsync *******************
        // Makes a list out of activities belonging to a module
        private async Task<List<ActivityListViewModel>> GetModuleActivityListAsync(int id)
        {
            var model = await db.Activities
                .Include(a => a.ActivityType)
                .Where(a => a.Module.Id == id)
                .OrderBy(a => a.StartTime)
                //.Where(a => a.ActivityType.Id != 3) // Except "Assignments"
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

        //************************* GetActListAjax ******************************************
        public async Task<IActionResult> GetActListAjax(int? Id)
        {
            if (Id == null) return BadRequest();

            if (Request.IsAjax())
            {
                // Jag har modul Id (Id)
                // Ta reda på vilket kurs Id modulen har
                // Lista all moduler som har samma kurs id

                var module = await db.Modules
                    .FirstOrDefaultAsync(m => m.Id == Id);

                var courseId = module.CourseId;

                var modules = await db.Modules
                    .Where(ms => ms.CourseId == courseId)
                    .OrderBy(m => m.StartTime)
                    .ToListAsync();

                List<ModuleListViewModel> moduleList = new List<ModuleListViewModel>();

                foreach (var mod in modules)
                {
                    var modLVM = new ModuleListViewModel();
                    modLVM.Id = mod.Id;
                    modLVM.Name = mod.Name;
                    modLVM.StartTime = mod.StartTime;
                    modLVM.EndTime = mod.EndTime;
                    modLVM.IsCurrentModule = false;

                    moduleList.Add(modLVM);
                }

                SetCurrentModule(moduleList, (int)Id);

                StudentViewModel studVM = new StudentViewModel();

                studVM.ModuleList = moduleList;
                studVM.ActivityList = GetModuleActivityListAsync((int)Id).Result;

                return PartialView("StudentModuleAndActivityPartial", studVM);
            }

            return BadRequest();
        }

        public async Task<IActionResult> GetTeacherActivityAjax(int? id)
        {
            if (id == null) return BadRequest();

            if (Request.IsAjax())
            {
                var module = await db.Modules.FirstOrDefaultAsync(m => m.Id == id);

                var modules = await db.Modules
                    .Where(m => m.CourseId == module.CourseId)
                    .OrderBy(m => m.StartTime)
                    .Select(m => new TeacherModuleViewModel
                    {
                        Id = m.Id,
                        Name = m.Name,
                        StartTime = m.StartTime,
                        EndTime = m.EndTime,
                        IsCurrentModule = false
                    })
                    .ToListAsync();

                SetCurrentTeacherModule(modules, (int)id);

                var teacherModel = new TeacherViewModel()
                {
                    ModuleList = modules,
                    ActivityList = GetModuleActivityListAsync((int)id).Result
                };

                return PartialView("TeacherModuleAndActivityPartial", teacherModel);
            }

            return BadRequest();
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

                if (newUser.IsTeacher)
                {
                    newUser.CourseId = null;
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

            var appUser = await userManager.FindByIdAsync(id);
            var isTeacher = await userManager.IsInRoleAsync(appUser, "Teacher");

            var editUser = await db.Users
                .Where(u => u.Id == id)
                .Select(u => new EditUserViewModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    IsTeacher = isTeacher,
                    CourseId = u.CourseId
                })
                .FirstOrDefaultAsync();

            if (editUser == null)
            {
                return NotFound();
            }

            return View(editUser);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserViewModel editUser)
        {
            if (id != editUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var appUser = await db.Users.FindAsync(id);

                    if (editUser.CurrentPassword != null && editUser.Password != null)
                    {
                        var updatePasswordresult = await userManager.ChangePasswordAsync(appUser, editUser.CurrentPassword, editUser.Password);
                        if (!updatePasswordresult.Succeeded)
                        {
                            foreach (var err in updatePasswordresult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, err.Description);
                            }
                        }
                    }

                    // Don't update if password change gone wrong
                    if (ModelState.IsValid)
                    {
                        appUser.CourseId = editUser.CourseId;
                        appUser.FirstName = editUser.FirstName;
                        appUser.LastName = editUser.LastName;
                        db.Update(appUser);
                        await db.SaveChangesAsync();
                    }

                    if (ModelState.IsValid)
                    {
                        ViewBag.Result = "Update Successful!";
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUserExists(editUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }         

            return View(editUser);
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
            var module = moduleList.Find(y => y.IsCurrentModule);
            List<ActivityListViewModel> activityList = null;


            if (module != null)
            {
                activityList = await GetModuleActivityListAsync(module.Id);
            }

            //var activityList = await GetStudentActivityListAsync();
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
            var assignmentList = await AssignmentListTeacher(id);
            var moduleList = await GetTeacherModuleListAsync(id);
            var activityList = new List<ActivityListViewModel>();

            var module = moduleList.Find(y => y.IsCurrentModule);

            if (module != null)
                activityList = await GetModuleActivityListAsync(module.Id);

            var model = new TeacherViewModel
            {
                Course = course,
                TeacherCurrentViewModel = current,
                AssignmentList = assignmentList,
                ModuleList = moduleList,
                ActivityList = activityList
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

            // This happens for newly created courses without any modules
            if (course.Modules.Count == 0)
            {
                return new TeacherCurrentViewModel
                {
                    Course = course,
                    Module = null,
                    Activity = null,
                    Assignments = null,
                    Finished = null
                };
            }
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

        public async Task<List<TeacherAssignmentListViewModel>> AssignmentListTeacher(int? id)
        {
            // Percentage of the students that is finished - NOT IMPLEMENTED
            var finished = 0.0;

            var assignments = await db.Activities.Include(a => a.ActivityType).Include(a => a.Module).ThenInclude(a => a.Course)
                .Where(a => a.ActivityType.Name == "Assignment" && a.Module.Course.Id == id)
                .OrderBy(a => a.StartTime)
                .Select(a => new TeacherAssignmentListViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Finished = $"{finished} %",
                })
                .ToListAsync();

            return assignments;
        }

        public async Task<List<TeacherModuleViewModel>> ModuleListTeacher(int? id)
        {
            var modules = await db.Modules.Where(m => m.CourseId == id)
                .OrderBy(a => a.StartTime)
                .Select(a => new TeacherModuleViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                })
                .ToListAsync();

            return modules;
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


        //*************************************** SetCurrentModule **********************************************
        // Params:
        // modules,         List<ModuleListViewModel>,  containing the modules
        // currentModuleId, int,                        containing current module Id

        private List<ModuleListViewModel> SetCurrentModule(List<ModuleListViewModel> modules, int currentModuleId)
        {
            foreach (var module in modules)
            {
                if (module.Id == currentModuleId)
                {
                    module.IsCurrentModule = true;
                }
                else
                {
                    module.IsCurrentModule = false;
                }
            }

            return modules;
        }

        private List<TeacherModuleViewModel> SetCurrentTeacherModule(List<TeacherModuleViewModel> modules, int currentId)
        {
            foreach (var module in modules)
            {
                if (module.Id == currentId)
                {
                    module.IsCurrentModule = true;
                }
                else
                {
                    module.IsCurrentModule = false;
                }
            }

            return modules;
        }
    }
}

