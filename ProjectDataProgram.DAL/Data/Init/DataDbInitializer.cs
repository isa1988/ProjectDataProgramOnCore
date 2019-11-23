
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using ProjectDataProgram.Core.DataBase;

namespace ProjectDataProgram.DAL.Data.Init
{
    public class DataDbInitializer
    {
        public async System.Threading.Tasks.Task SeedAsync(IApplicationBuilder app)
        {
            using (var score = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var userManager = score.ServiceProvider.GetRequiredService<UserManager<User>>();

                var roleManager = score.ServiceProvider.GetRequiredService<RoleManager<Role>>();

                if (!await roleManager.RoleExistsAsync("AdminSupervisor"))
                {
                    Role roleAdmin = new Role("AdminAupervisor");
                    await roleManager.CreateAsync(roleAdmin);
                }

                if (!await roleManager.RoleExistsAsync("ProjectManager"))
                {
                    Role roleUser = new Role("ProjectManager");
                    await roleManager.CreateAsync(roleUser);
                }

                if (!await roleManager.RoleExistsAsync("Employee"))
                {
                    Role roleUser = new Role("Employee");
                    await roleManager.CreateAsync(roleUser);
                }

                User admin = await userManager.FindByNameAsync("Admin");
                if (admin == null)
                {
                    var user = new User
                    {
                        UserName = "admin",
                        Login = "admin",
                        Email = "admin@test.ru",
                        FullName = "Тестов Тест Тестович",
                        //ImagePath = "images/admin.png"
                    };
                    var result = await userManager.CreateAsync(user, "123456");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "AdminAupervisor");
                    }
                }
            }
        }
    }
}
