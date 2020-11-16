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
            using (var db = new ApplicationDbContext(services.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
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

                    await db.SaveChangesAsync();
                }


                var fake = new Faker("sv");
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

                ///db.AddRange(courses);
               



                // Seed students
                var random = new Random();
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
                        Course = courses[random.Next(courses.Count)]
                };




                // Seed courses

                


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




                    //students.Add(student);
                }

                //db.AddRange(students);




                await db.SaveChangesAsync();
            }
        }


    }
}
