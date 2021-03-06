﻿using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using ProjectDataProgram.DAL.Data;
using ProjectDataProgram.DAL.Unit.Contracts;
using ProjectDataProgram.DAL.Unit;
using ProjectDataProgram.DAL.Data.Contracts;
using ProjectDataProgram.Core.DataBase;
using ProjectDataProgram.Service.Services.Contracts;
using ProjectDataProgram.Service.Services;

namespace ProjectDataProgram.Web
{
    public class Startup
    {
        private string _contentRootPath = "";

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _contentRootPath = env.ContentRootPath;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var connection = Configuration.GetConnectionString("DefaultConnection");
            if (connection.Contains("%CONTENTROOTPATH%"))
            {
                connection = connection.Replace("%CONTENTROOTPATH%", _contentRootPath);
            }
            services.AddDbContext<DataDbContext>(options => options.UseSqlServer(connection));
            services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
            var optionsBuilder = new DbContextOptionsBuilder<DataDbContext>();
            optionsBuilder.UseSqlServer(connection);
            services.AddSingleton<IDataDbContextFactory>(
                sp => new DataDbContextFactory(optionsBuilder.Options));

            // services.AddDbContext<IDataDbContextFactory>(options => options.UseSqlServer(connection));

            /*services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<DataDbContext>()
                .AddDefaultTokenProviders();*/
            services.AddDefaultIdentity<User>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 3;
                    //options.Password.RequiredUniqueChars = 1;
                })
                .AddRoles<Role>()
                .AddEntityFrameworkStores<DataDbContext>();

            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProjectTaskService, ProjectTaskService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            Mapper.Initialize(config =>
            {
                config.AddProfile<ProjectDataProgram.Web.MappingProfile>();
                config.AddProfile<ProjectDataProgram.Service.MappingProfile>();

            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            //new DataDbInitializer().SeedAsync(app).GetAwaiter();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
