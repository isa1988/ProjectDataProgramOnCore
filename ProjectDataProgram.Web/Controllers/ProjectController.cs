using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectDataProgram.Core.Repositories;
using ProjectDataProgram.Web.Models;
using ProjectDataProgram.Service.Dtos;
using ProjectDataProgram.Service.Services.Contracts;

namespace ProjectDataProgram.Web.Controllers
{
    public class ProjectController : Controller
    {
        private readonly UserManager<ProjectDataProgram.Core.DataBase.User> _userManager;
        private readonly IProjectService _service;
        private readonly IUserService _serviceUser;

        public ProjectController(UserManager<ProjectDataProgram.Core.DataBase.User> userManager,
            IProjectService service, IUserService serviceUser)
        {
            if (userManager == null)
                throw new ArgumentNullException(nameof(userManager));
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            if (serviceUser == null)
                throw new ArgumentNullException(nameof(serviceUser));
            _userManager = userManager;
            _service = service;
            _serviceUser = serviceUser;
        }

        [Authorize(Roles = "AdminAupervisor")]
        public IActionResult IndexAdmin(List<ProjectDto> projectList)
        {
            if (projectList == null || projectList.Count == 0) projectList = _service.ProjectAll();
            ProjectIndexModel projectModl = new ProjectIndexModel();
            if (projectModl != null) projectModl.ProjectList = GetConvert(projectList);
            projectModl.Filter = new ProjectFilterModel();
            var supervisorUserList = _serviceUser.UserList(new List<StatusRole>() { StatusRole.AdminAupervisor, StatusRole.ProjectManager }).Select(x =>
                new ProjectUserModel
                {
                    Id = x.Id,
                    Email = x.EMail,
                    FullName = x.Name,
                    Status = x.Status
                }).ToList();
            projectModl.Filter.SpervisorUser = new SelectList(supervisorUserList, "Id", "FullName");
            supervisorUserList =_serviceUser.UserList(new List<StatusRole>() { StatusRole.Employee }).Select(x =>
                new ProjectUserModel
                {
                    Id = x.Id,
                    Email = x.EMail,
                    FullName = x.Name,
                    Status = x.Status
                }).ToList();
            projectModl.Filter.User = new SelectList(supervisorUserList, "Id", "FullName");
            return View(projectModl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexAdmin(ProjectIndexModel request)
        {
            var filter = new ProjectFilterDto
            {
                Name = request.Filter.Name,
                ContractorCompany = request.Filter.ContractorCompany,
                CustomerCompany = request.Filter.CustomerCompany,
                IsDatePeriod = request.Filter.IsDatePeriod,
                DateBegin = request.Filter.DateBegin,
                DateEnd = request.Filter.DateEnd,
                IsUser = request.Filter.IsUser,
                UserId = request.Filter.UserId,
                IsPriority = request.Filter.IsPriority,
                Priority = request.Filter.Priority,
                IsSpervisorUser = request.Filter.IsSpervisorUser,
                SpervisorUserId = request.Filter.SpervisorUserId
            };
            var result = _service.ProjectAll(filter);
            request.Filter = request.Filter;
            request.ProjectList = GetConvert(result);
            return View(request);
        }

        private List<ProjectModl> GetConvert(List<ProjectDto> projectList)
        {
            List<ProjectModl> projectResultList = new List<ProjectModl>();

            for (int i = 0; i < projectList.Count; i++)
            {
                var project = new ProjectModl
                {
                    Id = projectList[i].Id,
                    Name = projectList[i].Name,
                    ContractorCompany = projectList[i].ContractorCompany,
                    CustomerCompany = projectList[i].CustomerCompany,
                    DateBegin = projectList[i].DateBegin,
                    DateEnd = projectList[i].DateEnd,
                    Priority = projectList[i].Priority,
                    SupervisorUserId = projectList[i].SupervisorUser.Id
                };
                for (int j = 0; j < projectList[i].ProjectUsers.Count; j++)
                {
                    project.ProjectUsers.Add(new ProjectUserModel
                    {
                        Id = projectList[i].ProjectUsers[j].User.Id,
                        FullName = projectList[i].ProjectUsers[j].User.Name,
                        Email = projectList[i].ProjectUsers[j].User.EMail
                    });
                }
                projectResultList.Add(project);
            }

            return projectResultList;
        }
        

        [Authorize(Roles = "AdminAupervisor")]
        public IActionResult Create()
        {
            var project = new ProjectModl();

            var supervisorUserList = _serviceUser.UserList(new List<StatusRole>() { StatusRole.AdminAupervisor, StatusRole.ProjectManager }).Select(x =>
                new ProjectUserModel
                {
                    Id = x.Id,
                    Email = x.EMail,
                    FullName = x.Name,
                    Status = x.Status
                }).ToList();
            project.SupervisorUserList = new SelectList(supervisorUserList, "Id", "FullName");
            project.ProjectUsers = _serviceUser.UserList(new List<StatusRole>() {StatusRole.Employee}).Select(x =>
                new ProjectUserModel
                {
                        Id = x.Id,
                        Email = x.EMail,
                        FullName = x.Name,
                        Status = x.Status
                }).ToList();
            project.DateBegin = DateTime.Now;
            project.DateEnd = project.DateBegin.AddDays(14);
            return View(project);
        }

        // POST: Organization/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectModl request)
        {

            var dto = new ProjectDto
            {
                Name = request.Name,
                ContractorCompany = request.ContractorCompany,
                CustomerCompany = request.CustomerCompany,
                DateBegin = request.DateBegin,
                DateEnd = request.DateEnd,
                Priority = request.Priority,
                SupervisorUser = new UserDto
                {
                    Id = request.SupervisorUserId
                }
            };
            var userAddList = request.ProjectUsers.Where(x => x.IsAdd).ToList();
            for (int i = 0; i < userAddList.Count; i++)
            {
                dto.ProjectUsers.Add(new ProjectUserDto
                    {
                        User = new UserDto
                        {
                            Id = userAddList[i].Id
                        },
                        Status = ProjectUserStatus.New
                    });
            }

            var result = await _service.CreateItemAsync(dto);

            if (result.IsSuccess)
            {
                return RedirectToAction("IndexAdmin");
            }
            else
            {
                var supervisorUserList = _serviceUser.UserList(new List<StatusRole>() { StatusRole.AdminAupervisor, StatusRole.ProjectManager }).Select(x =>
                    new ProjectUserModel
                    {
                        Id = x.Id,
                        Email = x.EMail,
                        FullName = x.Name,
                        Status = x.Status
                    }).ToList();
                request.SupervisorUserList = new SelectList(supervisorUserList, "Id", "FullName");
                request.ProjectUsers = _serviceUser.UserList(new List<StatusRole>() { StatusRole.Employee }).Select(x =>
                    new ProjectUserModel
                    {
                            Id = x.Id,
                            Email = x.EMail,
                            FullName = x.Name,
                            Status = x.Status
                    }).ToList();
                request.DateBegin = DateTime.Now;
                request.DateEnd = request.DateBegin.AddDays(14);
                foreach (var resultError in result.Errors)
                {
                    ModelState.AddModelError("Error", resultError);
                }
                return View(request);
            }
        }


        [Authorize(Roles = "AdminAupervisor")]
        public IActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            var projectDto = _service.GetProject(id.Value);
            var project = new ProjectModl
            {
                Id = id.Value,
                Name = projectDto.Name,
                ContractorCompany = projectDto.ContractorCompany,
                CustomerCompany = projectDto.CustomerCompany,
                DateBegin = projectDto.DateBegin,
                DateEnd = projectDto.DateEnd,
                Priority = projectDto.Priority,
                SupervisorUserId = projectDto.SupervisorUser.Id
            };

            var supervisorUserList = _serviceUser.UserList(new List<StatusRole>() { StatusRole.AdminAupervisor, StatusRole.ProjectManager }).Select(x =>
                new ProjectUserModel
                {
                    Id = x.Id,
                    Email = x.EMail,
                    FullName = x.Name,
                    Status = x.Status
                }).ToList();
            project.SupervisorUserList = new SelectList(supervisorUserList, "Id", "FullName");
            
            var projectUsersAll = _serviceUser.UserList(new List<StatusRole>() { StatusRole.Employee }).Select(x =>
                  new ProjectUserModel
                  {
                      Id = x.Id,
                      Email = x.EMail,
                      FullName = x.Name,
                      Status = x.Status
                  }).ToList();
            for (int i = 0; i < projectUsersAll.Count; i++)
            {
                var proDto = projectDto.ProjectUsers.FirstOrDefault(x => x.User.Id == projectUsersAll[i].Id);
                projectUsersAll[i].Status = (proDto != null) ? ProjectUserStatus.Save : ProjectUserStatus.Free;
                project.ProjectUsers.Add(projectUsersAll[i]);
            }
            return View(project);
        }

        // POST: Organization/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProjectModl request)
        {
            var dto = new ProjectDto
            {
                Id = request.Id,
                Name = request.Name,
                ContractorCompany = request.ContractorCompany,
                CustomerCompany = request.CustomerCompany,
                DateBegin = request.DateBegin,
                DateEnd = request.DateEnd,
                Priority = request.Priority,
                SupervisorUser = new UserDto
                {
                    Id = request.SupervisorUserId
                }
            };
            var userAddList = request.ProjectUsers.Where(x => x.IsAdd).ToList();
            for (int i = 0; i < userAddList.Count; i++)
            {
                var projectUserDto = new ProjectUserDto
                {
                    User = new UserDto
                    {
                        Id = userAddList[i].Id
                    }
                };
                projectUserDto.Status = ProjectUserStatus.New;
                dto.ProjectUsers.Add(projectUserDto);
            }

            userAddList = request.ProjectUsers.Where(x => x.IsDelete).ToList();
            for (int i = 0; i < userAddList.Count; i++)
            {
                var projectUserDto = new ProjectUserDto
                {
                    User = new UserDto
                    {
                        Id = userAddList[i].Id
                    }
                };
                projectUserDto.Status = ProjectUserStatus.Delete;
                dto.ProjectUsers.Add(projectUserDto);
            }

            var result = await _service.EditItemAsync(dto);

            if (result.IsSuccess)
            {
                return RedirectToAction("IndexAdmin");
            }
            else
            {
                var supervisorUserList = _serviceUser.UserList(new List<StatusRole>() { StatusRole.AdminAupervisor, StatusRole.ProjectManager }).Select(x =>
                    new ProjectUserModel
                    {
                        Id = x.Id,
                        Email = x.EMail,
                        FullName = x.Name,
                        Status = x.Status
                    }).ToList();
                request.SupervisorUserList = new SelectList(supervisorUserList, "Id", "FullName");
                
                return View(request);
            }
        }
    }
}