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
            if (db.Users.Any())
            {
                db.Users.RemoveRange(db.Users);
                db.Roles.RemoveRange(db.Roles);
                db.Courses.RemoveRange(db.Courses);
                db.RoleClaims.RemoveRange(db.RoleClaims);
                db.UserLogins.RemoveRange(db.UserLogins);
                db.UserClaims.RemoveRange(db.UserClaims);
                db.UserRoles.RemoveRange(db.UserRoles);
                db.UserTokens.RemoveRange(db.UserTokens);
                db.Activities.RemoveRange(db.Activities);
                db.ActivityTypes.RemoveRange(db.ActivityTypes);
                db.Documents.RemoveRange(db.Documents);

                await db.SaveChangesAsync();
            }


            var fake = new Faker("sv");
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

            // Seed admin
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



            // Seed courses

            var courses = new List<Course>();

            for (int i = 0; i < 3; i++)
            {
                var course = new Course
                {
                    Name = fake.Company.CatchPhrase(),
                    Description = fake.Lorem.Sentences(),
                    StartTime = fake.Date.Soon()
                };

                courses.Add(course);
            }

            db.AddRange(courses);
            await db.SaveChangesAsync();


            // Seed modules

            var modules = new List<Module>();

            for (int i = 0; i < 9; i++)
            {
                var tempTime = fake.Date.Soon();
                var tempTimeSpan = TimeSpan.FromDays(5);

                var module = new Module
                {
                    Name = fake.Company.CatchPhrase(),
                    Description = fake.Lorem.Sentences(),
                    StartTime = tempTime,
                    EndTime = tempTime + tempTimeSpan,
                    //Course = courses[random.Next(courses.Count)]
                    CourseId = courses[random.Next(courses.Count)].Id
                };

                modules.Add(module);
            }

            db.AddRange(modules);
            await db.SaveChangesAsync();


            // Seed Activity Types

            var activityTypes = new List<ActivityType>();
            string[] types = { "E-Learning", "Hand-in", "Lecture", "Group Meeting" };

            for (int i = 0; i < types.Length; i++)
            {
                var t = types[i];
                var atype = new ActivityType { Name = t };
                activityTypes.Add(atype);
            }

            db.AddRange(activityTypes);

            // TODO: Make sure all activitytypes are selected at least once

            // Seed activities

            var activities = new List<Activity>();

            for (int i = 0; i < 4; i++)
            {
                var tempTime = fake.Date.Soon();
                var tempTimeSpan = TimeSpan.FromDays(5);

                var activity = new Activity
                {
                    Name = fake.Company.CatchPhrase(),
                    Description = fake.Lorem.Sentences(),
                    StartTime = tempTime,
                    EndTime = tempTime + tempTimeSpan,
                    ModuleId = modules[random.Next(modules.Count)].Id,
                    ActivityType = activityTypes[i]
                };

                activities.Add(activity);
            }

            db.AddRange(activities);
            await db.SaveChangesAsync();


            // Seed students
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
                    //Course = courses[random.Next(courses.Count)]
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





            // Seed documents

            var documents = new List<Document>();


            for (int i = 0; i < 30; i++)
            {
                // TODO: Make it so that the documents only get either a course, module or activity (and the rest are not set)
                var document = new Document
                {
                    Name = fake.Company.CatchPhrase(),
                    Description = fake.Lorem.Sentences(),
                    UploadTime = fake.Date.Soon(),
                    AppUserId = students[random.Next(students.Count)].Id,
                    CourseId = courses[random.Next(courses.Count)].Id,
                    ModuleId = modules[random.Next(modules.Count)].Id,
                    ActivityId = activities[random.Next(activities.Count)].Id
                };

                documents.Add(document);

            }

            db.AddRange(documents);



            await db.SaveChangesAsync();
        }


    }
}
