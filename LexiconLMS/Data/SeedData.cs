using Bogus;
using LexiconLMS.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Data
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services, string adminPW)
        {
            // "Handle resources", "use service until the 'using' is used up"
            using var db = new ApplicationDbContext(services.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            //if (db.Users.Any())
            //{
            //    db.Users.RemoveRange(db.Users);
            //    db.Roles.RemoveRange(db.Roles);
            //    db.Courses.RemoveRange(db.Courses);
            //    db.RoleClaims.RemoveRange(db.RoleClaims);
            //    db.UserLogins.RemoveRange(db.UserLogins);
            //    db.UserClaims.RemoveRange(db.UserClaims);
            //    db.UserRoles.RemoveRange(db.UserRoles);
            //    db.UserTokens.RemoveRange(db.UserTokens);
            //    db.Activities.RemoveRange(db.Activities);
            //    db.ActivityTypes.RemoveRange(db.ActivityTypes);
            //    db.Documents.RemoveRange(db.Documents);

            //    await db.SaveChangesAsync();
            //}

            var fake = new Faker("en");
            var random = new Random();

            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            var roleNames = new[] { "Teacher", "Student" };

            foreach (var roleName in roleNames)
            {
                if (await roleManager.RoleExistsAsync(roleName))
                {
                    continue;
                }

                var role = new IdentityRole { Name = roleName };
                var result = await roleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    throw new Exception(string.Join("\n", result.Errors));
                }
            }

            // Seed Admin

            var adminEmail = "admin@lms.se";

            var foundUser = await userManager.FindByEmailAsync(adminEmail);

            if (foundUser != null)
            {
                return;
            }

            var admin = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "LMS"
            };

            var addAdminResult = await userManager.CreateAsync(admin, adminPW);

            if (!addAdminResult.Succeeded)
            {
                throw new Exception(string.Join("\n", addAdminResult.Errors));
            }

            var adminUser = await userManager.FindByNameAsync(adminEmail);

            foreach (var role in roleNames)
            {
                if (await userManager.IsInRoleAsync(adminUser, role))
                {
                    continue;
                }

                var addToRoleResult = await userManager.AddToRoleAsync(adminUser, role);

                if (!addToRoleResult.Succeeded)
                {
                    throw new Exception(string.Join("\n", addToRoleResult.Errors));
                }
            }

            // Seed Courses

            var courses = new List<Course>();
            string[] courseNames =
            {
                "Video Game Development",
                "Web Design",
                "Marketing",
                "Accounting",
                "Business Law",
                "Archaeology",
                "Philosophy",
                "Data Engineering",
                "Journalism",
                "Interior Design"
            };

            foreach (var name in courseNames)
            {
                var course = new Course
                {
                    Name = name,
                    Description = fake.Lorem.Sentences(),
                    StartTime = fake.Date.Soon()
                };
                courses.Add(course);
            }

            db.AddRange(courses);
            await db.SaveChangesAsync();

            // Seed Modules

            var modules = new List<Module>();
            string[] moduleNames =
            {
                $"Data Management",
                $"Microsoft Office",
                $"Essay Research",
                $"Essay Writing"
            };

            foreach (var course in courses)
            {
                var moduleStartTime = course.StartTime;
                var moduleLength = TimeSpan.FromDays(25);
                var moduleEndTime = moduleStartTime + moduleLength;

                for (int i = 0; i < moduleNames.Length; i++)
                {
                    var module = new Module
                    {
                        Name = moduleNames[i],
                        Description = fake.Lorem.Sentences(),
                        StartTime = moduleStartTime,
                        EndTime = moduleEndTime,
                        CourseId = course.Id
                    };
                    modules.Add(module);

                    moduleStartTime += moduleLength;
                    moduleEndTime += moduleLength;
                }
            }

            db.AddRange(modules);
            await db.SaveChangesAsync();

            // Seed Activity Types

            var activityTypes = new List<ActivityType>();
            string[] types = { "E-Learning", "Assignment", "Lecture", "Group Meeting" };

            for (int i = 0; i < types.Length; i++)
            {
                var t = types[i];
                var atype = new ActivityType { Name = t };
                activityTypes.Add(atype);
            }

            db.AddRange(activityTypes);
            await db.SaveChangesAsync();

            // Seed Activities

            var activities = new List<Activity>();

            foreach (var module in modules)
            {
                var activityStartTime = module.StartTime;
                var moduleEndTime = module.EndTime;

                var activityLength = TimeSpan.FromDays(2.5);
                var activityEndTime = activityStartTime + activityLength;

                for (int i = 0; i < 10; i++)
                {
                    var activityType = new ActivityType();
                    var randomId = random.Next(1, 5);
                    var randomAssignment = random.Next(1, 4);

                    if (randomAssignment == 1)
                        activityType = activityTypes.Where(a => a.Name == "Assignment").FirstOrDefault();
                    else
                        activityType = activityTypes.FirstOrDefault(a => a.Id == randomId);

                    var activity = new Activity
                    {
                        Name = $"{fake.Commerce.Product()} {activityType.Name}",
                        Description = fake.Lorem.Sentences(),
                        StartTime = activityStartTime,
                        EndTime = activityEndTime,
                        ModuleId = module.Id,
                        ActivityTypeId = activityType.Id
                    };
                    activities.Add(activity);

                    activityStartTime = activityEndTime;
                    activityEndTime += activityLength;
                }
            }

            db.AddRange(activities);
            await db.SaveChangesAsync();

            // Seed Students
            var students = new List<AppUser>();

            for (int i = 0; i < 20; i++)
            {
                var fName = fake.Name.FirstName();
                var lName = fake.Name.LastName();
                var studentEmail = fake.Internet.Email($"{fName} {lName}");

                var student = new AppUser
                {
                    UserName = studentEmail,
                    FirstName = fName,
                    LastName = lName,
                    Email = studentEmail,
                    CourseId = courses[random.Next(courses.Count)].Id
                };

                var addStudentResult = await userManager.CreateAsync(student, adminPW);

                if (!addStudentResult.Succeeded)
                {
                    throw new Exception(string.Join("\n", addStudentResult.Errors));
                }

                var studentUser = await userManager.FindByNameAsync(studentEmail);

                if (await userManager.IsInRoleAsync(studentUser, "Student"))
                {
                    continue;
                }

                var addToRoleResult = await userManager.AddToRoleAsync(studentUser, "Student");

                if (!addToRoleResult.Succeeded)
                {
                    throw new Exception(string.Join("\n", addToRoleResult.Errors));
                }

                students.Add(student);
            }

            // Seed Documents

            //var documents = new List<Document>();

            //for (int i = 0; i < 30; i++)
            //{
            //    // TODO: Make it so that the documents only get either a course, module or activity (and the rest are not set)
            //    var document = new Document
            //    {
            //        Name = fake.Company.CatchPhrase(),
            //        Description = fake.Lorem.Sentences(),
            //        UploadTime = fake.Date.Soon(),
            //        AppUserId = students[random.Next(students.Count)].Id,
            //        CourseId = courses[random.Next(courses.Count)].Id,
            //        ModuleId = modules[random.Next(modules.Count)].Id,
            //        ActivityId = activities[random.Next(activities.Count)].Id
            //    };

            //    documents.Add(document);
            //}

            //db.AddRange(documents);
            //await db.SaveChangesAsync();
        }
    }
}
