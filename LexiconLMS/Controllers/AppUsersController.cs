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
using System.Data.Common;

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
            var timeNow = DateTime.Now;

            var documents = await db.Documents.Where(a => a.AppUserId == userId).Select(a => a.ActivityId).ToListAsync();

            var model = await db.Activities.Include(a => a.ActivityType)
                .Include(a => a.Module)
                .ThenInclude(a => a.Course)
                .Include(a => a.Documents)
                .Where(a => a.Module.Course == userCourse && a.ActivityType.Name == "Assignment")
                .Select(a => new AssignmentListViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    IsFinished = documents.Contains(a.Id)
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

                if (ModelState.IsValid)
                {
                    ViewBag.Result = "User created successfully!";
                    ViewBag.Email = newUser.Email;
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
        [Authorize(Roles = "Teacher")]
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
        [Authorize(Roles = "Teacher")]
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
            var timeNow = DateTime.Now;
            var moduleList = await GetStudentModuleListAsync();
            var module = moduleList.Find(y => y.IsCurrentModule);
            List<ActivityListViewModel> activityList = null;

            if (module != null)
            {
                activityList = await GetModuleActivityListAsync(module.Id);
            }

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
                AssignmentList = assignmentList.OrderBy(t => Math.Abs((t.EndTime - timeNow).Ticks)),
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

            var user = await db.Users.Include(a => a.Course)
                .ThenInclude(a => a.Modules)
                .ThenInclude(a => a.Activities)
                .ThenInclude(a => a.ActivityType)
                .Include(a => a.Documents)
                .Where(a => a.Id == userId)
                .FirstOrDefaultAsync();

            var timeNow = DateTime.Now;

            var module = user.Course.Modules.OrderBy(t => Math.Abs((t.StartTime - timeNow).Ticks)).First();
            var activity = module.Activities.OrderBy(t => Math.Abs((t.StartTime - timeNow).Ticks)).First();
            var documents = await db.Documents.Where(a => a.AppUserId == userId).Select(a => a.ActivityId).ToListAsync();

            var assignments = module.Activities.Where(a => a.ActivityType.Id == 3)
                .OrderBy(t => Math.Abs((t.EndTime - timeNow).Ticks)).Take(3)
                .Select(a => new CurrentAssignmentsViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    DueTime = a.EndTime,
                    IsFinished = documents.Contains(a.Id)
                })
                .ToList();

            var model = new CurrentViewModel
            {
                Course = user.Course,
                Module = module,
                Activity = activity,
                Assignments = assignments,
            };

            return model;
        }

        [Authorize(Roles = "Teacher")]
        //[Authorize(Roles = "Student")]
        public async Task<IActionResult> TeacherUserIndex(string sortOrder)
        {

            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CourseSortParm"] = sortOrder == "Course" ? "course_desc" : "Course";
            ViewData["EmailSortParm"] = sortOrder == "Email" ? "email_desc" : "Email";


            //var userList = await db.Users
            //    .OrderBy(u => u.LastName)
            //    .Include(a => a.Course)
            //    .ToListAsync();


            var userList = (from q in db.Users 
                            select q).Include("Course");
                        
            switch (sortOrder)
            {
                case "name_desc":
                    userList = userList.OrderByDescending(s => s.LastName);
                    break;
                case "Course":
                    userList = userList.OrderBy(s => s.Course.Name);
                    break;
                case "course_desc":
                    userList = userList.OrderByDescending(s => s.Course.Name);
                    break;
                case "Email":
                    userList = userList.OrderBy(s => s.Email);
                    break;
                case "email_desc":
                    userList = userList.OrderByDescending(s => s.Email);
                    break;
                default:
                    userList = userList.OrderBy(s => s.LastName);
                    break;
            }
            var userList2 = userList.ToList();

            


            var model = new List<AppUserListViewModel>();

            //foreach (var appUser in userList)
            foreach (var appUser in userList2)
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

            var current = await CurrentTeacher(id);
            var currentCourse = current.Course;

            if (current.Course.Modules.Count == 0)
                return View(new TeacherViewModel
                {
                    Current = new TeacherCurrentViewModel
                    {
                        Course = current.Course,
                        Module = null,
                        Activity = null,
                        Assignments = null,
                        Finished = null
                    },
                    AssignmentList = null,
                    ModuleList = null,
                    ActivityList = null
                });

            var assignmentList = await AssignmentListTeacher(id);
            var moduleList = await GetTeacherModuleListAsync(id);
            var module = moduleList.Find(y => y.IsCurrentModule);
            var activityList = new List<ActivityListViewModel>();

            if (module != null)
                activityList = await GetModuleActivityListAsync(module.Id);

            var model = new TeacherViewModel
            {
                Current = current,
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
        public async Task<IActionResult> TeacherHome(string sortOrder)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var courses = from c in db.Courses select c;

            switch (sortOrder)
            {
                case "name_desc":
                    courses = courses.OrderByDescending(s => s.Name);
                    break;
                case "Date":
                    courses = courses.OrderBy(s => s.StartTime);
                    break;
                case "date_desc":
                    courses = courses.OrderByDescending(s => s.StartTime);
                    break;
                default:
                    courses = courses.OrderBy(s => s.Name);
                    break;
            }
            var coursesList = courses.ToList();

            //var courses = await db.Courses
            //    .OrderBy(n => n.Name)
            //    .ToListAsync();

            var model = new TeacherHomeViewModel
            {
                Courses = coursesList
            };

            return View(model);
        }

        public async Task<TeacherCurrentViewModel> CurrentTeacher(int? id)
        {
            var course = db.Courses.Include(a => a.AppUsers)
                .Include(a => a.Modules)
                .ThenInclude(a => a.Activities)
                .FirstOrDefault(a => a.Id == id);

            var students = course.AppUsers.Count();

            //This happens for newly created courses without any modules
            if (course.Modules.Count == 0)
                return new TeacherCurrentViewModel { Course = course, Module = null, Activity = null, Assignments = null, Finished = null };

            var timeNow = DateTime.Now;
            var module = course.Modules.OrderBy(t => Math.Abs((t.StartTime - timeNow).Ticks)).First();

            // If no users assigned to course yet
            if (students == 0)
            {
                return new TeacherCurrentViewModel { Course = course, Module = module, Activity = null, Assignments = null, Finished = null };
            }
            var activity = module.Activities.OrderBy(t => Math.Abs((t.StartTime - timeNow).Ticks)).First();

            var assignments = await db.Activities.Where(c => c.ActivityType.Name == "Assignment" && c.Module.CourseId == id)
                .OrderBy(a => a.StartTime)
                .Select(a => new TeacherAssignmentsViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    DueTime = a.EndTime,
                    Finished = a.Documents.Where(d => d.IsFinished.Equals(true)).Count() * 100 / students
                })
                .ToListAsync();

            assignments = assignments.Count < 3 ? assignments.Take(assignments.Count).ToList() : assignments.Take(3).ToList();

            var model = new TeacherCurrentViewModel
            {
                Course = course,
                Module = module,
                Activity = activity,
                Assignments = assignments,
            };

            return model;
        }

        public async Task<List<TeacherAssignmentListViewModel>> AssignmentListTeacher(int? id)
        {
            var students = db.Courses.Find(id).AppUsers.Count();

            if (students == 0) return null;

            var assignments = await db.Activities
                .Where(a => a.ActivityType.Name == "Assignment" && a.Module.CourseId == id)
                .Select(a => new TeacherAssignmentListViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Finished = a.Documents.Where(d => d.IsFinished.Equals(true)).Count() * 100 / students
                })
                .OrderBy(v => v.StartTime)
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

