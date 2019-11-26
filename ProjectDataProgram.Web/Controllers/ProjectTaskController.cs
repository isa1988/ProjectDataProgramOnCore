using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectDataProgram.Core.Repositories;
using ProjectDataProgram.Service.Dtos;
using ProjectDataProgram.Service.Services.Contracts;
using ProjectDataProgram.Web.Models;

namespace ProjectDataProgram.Web.Controllers
{
    public class ProjectTaskController : Controller
    {
        private readonly UserManager<ProjectDataProgram.Core.DataBase.User> _userManager;
        private readonly IProjectTaskService _service;
        private readonly IProjectService _serviceProject;
        private readonly IUserService _serviceUser;

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
        }
        public IActionResult Index()
        {
            return View();
        }

        private List<UserModel> GetUsers(List<StatusRole> statusRoles)
        {
            return _serviceUser.UserList(statusRoles).Select(x =>
                new UserModel
                {
                    Id = x.Id,
                    Email = x.EMail,
                    FullName = x.Name
                }).ToList();
        }

        private List<UserModel> GetUsers(List<ProjectUserDto> userDtos)
        {
            return userDtos.Select(x =>
                new UserModel
                {
                    Id = x.User.Id,
                    Email = x.User.EMail,
                    FullName = x.User.Name
                }).ToList();
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
                projectTask.AuthorList = new SelectList(GetUsers(new List<StatusRole>()
                {
                    StatusRole.AdminAupervisor,
                    StatusRole.ProjectManager
                }), "Id", "FullName");
            }

            projectTask.ExecutorList = new SelectList(GetUsers(project.ProjectUsers), "Id", "FullName");
            projectTask.ProjectId = projectId.Value;
            return View(projectTask);
        }

        // POST: Organization/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectTaskModel request)
        {

            var dto = new ProjectTaskDto
            {
                Name = request.Name,
                Comment = request.Comment,
                Priority = request.Priority,
                Status = request.Status,
                Project = new ProjectDto
                {
                    Id = request.ProjectId
                },
                Author = new UserDto
                {
                    Id = request.AuthorId
                },
                Executor = new UserDto
                {
                    Id = request.ExecutorId
                }
            };
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
                    request.AuthorList = new SelectList(GetUsers(new List<StatusRole>()
                    {
                        StatusRole.AdminAupervisor,
                        StatusRole.ProjectManager
                    }), "Id", "FullName");
                }

                request.ExecutorList = new SelectList(GetUsers(project.ProjectUsers), "Id", "FullName");
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
            var projectTask = new ProjectTaskModel
            {
                Id = id.Value,
                Name = projectTaskDto.Name,
                Comment = projectTaskDto.Comment,
                Priority = projectTaskDto.Priority,
                Status = projectTaskDto.Status,
                ProjectId = projectTaskDto.Project.Id,
                AuthorId = projectTaskDto.Author.Id,
                ExecutorId = projectTaskDto.Executor.Id
            };
            var project = _serviceProject.GetProject(projectTask.ProjectId);
            if (User.IsInRole("AdminAupervisor"))
            {
                projectTask.AuthorList = new SelectList(GetUsers(new List<StatusRole>()
                {
                    StatusRole.AdminAupervisor,
                    StatusRole.ProjectManager
                }), "Id", "FullName");
            }

            projectTask.ExecutorList = new SelectList(GetUsers(project.ProjectUsers), "Id", "FullName");
            return View(projectTask);
        }

        // POST: Organization/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProjectTaskModel request)
        {
            var dto = new ProjectTaskDto
            {
                Id = request.Id,
                Name = request.Name,
                Comment = request.Comment,
                Priority = request.Priority,
                Status = request.Status,
                Project = new ProjectDto
                {
                    Id = request.ProjectId
                },
                Author = new UserDto
                {
                    Id = request.AuthorId
                },
                Executor = new UserDto
                {
                    Id = request.ExecutorId
                }
            };
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
                    request.AuthorList = new SelectList(GetUsers(new List<StatusRole>()
                    {
                        StatusRole.AdminAupervisor,
                        StatusRole.ProjectManager
                    }), "Id", "FullName");
                }

                request.ExecutorList = new SelectList(GetUsers(project.ProjectUsers), "Id", "FullName");
                foreach (var resultError in result.Errors)
                {
                    ModelState.AddModelError("Error", resultError);
                }
                return View(request);
            }
        }
    }
}