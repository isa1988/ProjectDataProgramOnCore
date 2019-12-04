using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectDataProgram.Core.Repositories;
using ProjectDataProgram.Service.Dtos;
using ProjectDataProgram.Service.Services.Contracts;
using ProjectDataProgram.Web.Helper;
using ProjectDataProgram.Web.Models;

namespace ProjectDataProgram.Web.Controllers
{
    public class ProjectTaskController : Controller
    {
        private readonly UserManager<ProjectDataProgram.Core.DataBase.User> _userManager;
        private readonly IProjectTaskService _service;
        private readonly IProjectService _serviceProject;
        private readonly IUserService _serviceUser;
        private readonly ClassHelper _helper;
        public ProjectTaskController(UserManager<ProjectDataProgram.Core.DataBase.User> userManager,
            IProjectTaskService service, IProjectService serviceProject, IUserService serviceUser)
        {
            if (userManager == null)
                throw new ArgumentNullException(nameof(userManager));
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            if (serviceProject == null)
                throw new ArgumentNullException(nameof(serviceProject));
            if (serviceUser == null)
                throw new ArgumentNullException(nameof(serviceUser));
            _userManager = userManager;
            _service = service;
            _serviceProject = serviceProject;
            _serviceUser = serviceUser;
            _helper = new ClassHelper();
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "AdminAupervisor, ProjectManager")]
        public IActionResult Create(int? projectId)
        {
            if (!projectId.HasValue)
            {
                return NotFound();
            }
            var projectTask = new ProjectTaskModel();

            var project = _serviceProject.GetProject(projectId.Value);
            if (User.IsInRole("AdminAupervisor"))
            {
                projectTask.AuthorList = new SelectList(_helper.GetUsers(new List<StatusRole>()
                {
                    StatusRole.AdminAupervisor,
                    StatusRole.ProjectManager
                }, _serviceUser), "Id", "FullName");
            }

            projectTask.ExecutorList = new SelectList(_helper.GetUsers(project.ProjectUsers), "Id", "FullName");
            projectTask.ProjectId = projectId.Value;
            return View(projectTask);
        }

        // POST: Organization/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectTaskModel request)
        {
            var dto = Mapper.Map<ProjectTaskDto>(request); 
            
            if (!User.IsInRole("AdminAupervisor"))
            {
                dto.Author.Id = int.Parse(_userManager.GetUserId(User));
            }
            var result = await _service.CreateItemAsync(dto);

            if (result.IsSuccess)
            {
                if (User.IsInRole("AdminAupervisor"))
                {
                    return RedirectToAction("IndexAdmin", "Project");
                }
                else
                {
                    return RedirectToAction("IndexPM", "Project");
                }
            }
            else
            {
                var project = _serviceProject.GetProject(request.ProjectId);
                if (User.IsInRole("AdminAupervisor"))
                {
                    request.AuthorList = new SelectList(_helper.GetUsers(new List<StatusRole>()
                    {
                        StatusRole.AdminAupervisor,
                        StatusRole.ProjectManager
                    }, _serviceUser), "Id", "FullName");
                }

                request.ExecutorList = new SelectList(_helper.GetUsers(project.ProjectUsers), "Id", "FullName");
                foreach (var resultError in result.Errors)
                {
                    ModelState.AddModelError("Error", resultError);
                }
                return View(request);
            }
        }


        [Authorize(Roles = "AdminAupervisor, ProjectManager")]
        public IActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            var projectTaskDto = _service.GetProjectTaskById(id.Value);
            var projectTask = Mapper.Map<ProjectTaskModel>(projectTaskDto);
            var project = _serviceProject.GetProject(projectTask.ProjectId);
            if (User.IsInRole("AdminAupervisor"))
            {
                projectTask.AuthorList = new SelectList(_helper.GetUsers(new List<StatusRole>()
                {
                    StatusRole.AdminAupervisor,
                    StatusRole.ProjectManager
                }, _serviceUser), "Id", "FullName");
            }

            projectTask.ExecutorList = new SelectList(_helper.GetUsers(project.ProjectUsers), "Id", "FullName");
            return View(projectTask);
        }

        // POST: Organization/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProjectTaskModel request)
        {
            var dto = Mapper.Map<ProjectTaskDto>(request); 
            if (!User.IsInRole("AdminAupervisor"))
            {
                dto.Author.Id = int.Parse(_userManager.GetUserId(User));
            }
            var result = await _service.EditItemAsync(dto);

            if (result.IsSuccess)
            {
                if (User.IsInRole("AdminAupervisor"))
                {
                    return RedirectToAction("IndexAdmin", "Project");
                }
                else
                {
                    return RedirectToAction("IndexPM", "Project");
                }
            }
            else
            {
                var project = _serviceProject.GetProject(request.ProjectId);
                if (User.IsInRole("AdminAupervisor"))
                {
                    request.AuthorList = new SelectList(_helper.GetUsers(new List<StatusRole>()
                    {
                        StatusRole.AdminAupervisor,
                        StatusRole.ProjectManager
                    }, _serviceUser), "Id", "FullName");
                }

                request.ExecutorList = new SelectList(_helper.GetUsers(project.ProjectUsers), "Id", "FullName");
                foreach (var resultError in result.Errors)
                {
                    ModelState.AddModelError("Error", resultError);
                }
                return View(request);
            }
        }

        [Authorize(Roles = "AdminAupervisor, ProjectManager")]
        public IActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            var projectTaskDto = _service.GetProjectTaskById(id.Value);
            var projectTask = Mapper.Map<ProjectTaskModel>(projectTaskDto);

            return View(projectTask);
        }

        // POST: Organization/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ProjectTaskModel request)
        {
            var dto = Mapper.Map<ProjectTaskDto>(request);

            var result = await _service.DeleteItemAsync(dto);

            if (result.IsSuccess)
            {
                if (User.IsInRole("AdminAupervisor"))
                {
                    return RedirectToAction("IndexAdmin", "Project");
                }
                else
                {
                    return RedirectToAction("IndexPM", "Project");
                }
            }
            else
            {
                return View(request);
            }
        }
    }
}