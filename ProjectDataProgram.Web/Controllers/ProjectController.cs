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
using AutoMapper;
using ProjectDataProgram.Web.Helper;

namespace ProjectDataProgram.Web.Controllers
{
    public class ProjectController : Controller
    {
        private readonly UserManager<ProjectDataProgram.Core.DataBase.User> _userManager;
        private readonly IProjectService _service;
        private readonly IUserService _serviceUser;
        private readonly ClassHelper _helper;

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
            _helper = new ClassHelper();
        }

        [Authorize(Roles = "AdminAupervisor")]
        public IActionResult IndexAdmin(List<ProjectDto> projectList)
        {
            if (projectList == null || projectList.Count == 0) projectList = _service.ProjectAll();
            ProjectIndexModel projectModl = new ProjectIndexModel();
            projectModl.ProjectList = Mapper.Map<List<ProjectModl>>(projectList);
            projectModl.Filter = new ProjectFilterModel();

            var supervisorUserList = _helper.GetUsers(new List<StatusRole>()
                {StatusRole.AdminAupervisor, StatusRole.ProjectManager}, _serviceUser);
            projectModl.Filter.SpervisorUser = new SelectList(supervisorUserList, "Id", "FullName");

            var employeeUserList = _helper.GetUsers(new List<StatusRole>()
                {StatusRole.Employee}, _serviceUser);    
            projectModl.Filter.User = new SelectList(employeeUserList, "Id", "FullName");

            return View(projectModl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexAdmin(ProjectIndexModel request)
        {
            var filter = Mapper.Map<ProjectFilterDto>(request.Filter); 
            
            request.ProjectList = Mapper.Map<List<ProjectModl>>(_service.ProjectAll(filter)) ;
            request.Filter = request.Filter;
            var supervisorUserList = _helper.GetUsers(new List<StatusRole>()
                {StatusRole.AdminAupervisor, StatusRole.ProjectManager}, _serviceUser);
            request.Filter.SpervisorUser = new SelectList(supervisorUserList, "Id", "FullName");

            var employeeUserList = _helper.GetUsers(new List<StatusRole>()
                {StatusRole.Employee}, _serviceUser);
            request.Filter.User = new SelectList(employeeUserList, "Id", "FullName");
            return View(request);
        }

        [Authorize(Roles = "ProjectManager")]
        public IActionResult IndexPM(List<ProjectDto> projectList)
        {
            if (projectList == null || projectList.Count == 0)
                projectList = _service.ProjectAll(new ProjectFilterDto
                {
                    IsSpervisorUser = true,
                    SpervisorUserId = int.Parse(_userManager.GetUserId(User))
                });
            ProjectIndexModel projectModl = new ProjectIndexModel();
            projectModl.ProjectList = Mapper.Map<List<ProjectModl>>(projectList);
            projectModl.Filter = new ProjectFilterModel();

            var supervisorUserList = _helper.GetUsers(new List<StatusRole>() 
                                    {StatusRole.Employee}, _serviceUser);
                 
            projectModl.Filter.User = new SelectList(supervisorUserList, "Id", "FullName");
            return View(projectModl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexPM(ProjectIndexModel request)
        {
            var filter = Mapper.Map<ProjectFilterDto>(request.Filter);
            filter.IsSpervisorUser = true;
            filter.SpervisorUserId = int.Parse(_userManager.GetUserId(User));
            request.ProjectList = Mapper.Map<List<ProjectModl>>(_service.ProjectAll(filter));
            request.Filter = request.Filter;
            var supervisorUserList = _helper.GetUsers(new List<StatusRole>()
                {StatusRole.Employee}, _serviceUser);

            request.Filter.User = new SelectList(supervisorUserList, "Id", "FullName");
            return View(request);
        }

        [Authorize(Roles = "Employee")]
        public IActionResult IndexEmp(List<ProjectDto> projectList)
        {
            if (projectList == null || projectList.Count == 0)
                projectList = _service.ProjectAll(new ProjectFilterDto
                {
                    IsUser = true,
                    UserId = int.Parse(_userManager.GetUserId(User))
                });
            ProjectIndexModel projectModl = new ProjectIndexModel();
            if (projectModl != null) projectModl.ProjectList = Mapper.Map<List<ProjectModl>>(projectList);
            projectModl.Filter = new ProjectFilterModel();
            return View(projectModl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexEmp(ProjectIndexModel request)
        {
            var filter = Mapper.Map<ProjectFilterDto>(request.Filter);
            filter.IsUser = true;
            filter.UserId = int.Parse(_userManager.GetUserId(User));
            request.ProjectList = Mapper.Map<List<ProjectModl>>(_service.ProjectAll(filter)) ;
            request.Filter = request.Filter;
            
            return View(request);
        }

        [Authorize(Roles = "AdminAupervisor")]
        public IActionResult Create()
        {
            var project = new ProjectModl();
            
            var supervisorUserList = _helper.GetUsers(new List<StatusRole>()
                                        {StatusRole.AdminAupervisor, StatusRole.ProjectManager}, _serviceUser);
            project.SupervisorUserList = new SelectList(supervisorUserList, "Id", "FullName");

            project.ProjectUsers = _helper.GetUsers(new List<StatusRole>()
                                        {StatusRole.Employee}, _serviceUser, null);
            project.DateBegin = DateTime.Now;
            project.DateEnd = project.DateBegin.AddDays(14);
            return View(project);
        }

        // POST: Organization/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectModl request)
        {

            var dto = Mapper.Map<ProjectDto>(request);
            var userAddList = request.ProjectUsers.Where(x => x.IsAdd).ToList();
            if (userAddList?.Count > 0)
                dto.ProjectUsers = Mapper.Map<List<UserDto>>(userAddList);

            var result = await _service.CreateItemAsync(dto);

            if (result.IsSuccess)
            {
                return RedirectToAction("IndexAdmin");
            }
            else
            {
                var supervisorUserList = _helper.GetUsers(new List<StatusRole>()
                                            {StatusRole.AdminAupervisor, StatusRole.ProjectManager}, _serviceUser);
                request.SupervisorUserList = new SelectList(supervisorUserList, "Id", "FullName");

                request.ProjectUsers = _helper.GetUsers(new List<StatusRole>()
                                                {StatusRole.Employee}, _serviceUser, null);
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
            var project = Mapper.Map<ProjectModl>(projectDto);

            var supervisorUserList = _helper.GetUsers(new List<StatusRole>()
                                        {StatusRole.AdminAupervisor, StatusRole.ProjectManager}, _serviceUser);
            project.SupervisorUserList = new SelectList(supervisorUserList, "Id", "FullName");
            
            project.ProjectUsers = _helper.GetUsers(new List<StatusRole>()
                                { StatusRole.Employee }, _serviceUser, projectDto);
            
            return View(project);
        }

        // POST: Organization/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProjectModl request)
        {
            var dto = Mapper.Map<ProjectDto>(request);
            var userList = request.ProjectUsers.Where(x => x.IsAdd || x.IsDelete).ToList();
            dto.ProjectUsers = new List<UserDto>();
            if (userList?.Count > 0)
                dto.ProjectUsers = Mapper.Map<List<UserDto>>(userList);
            
            var result = await _service.EditItemAsync(dto);

            if (result.IsSuccess)
            {
                return RedirectToAction("IndexAdmin");
            }
            else
            {
                var supervisorUserList = _helper.GetUsers(new List<StatusRole>()
                                    {StatusRole.AdminAupervisor, StatusRole.ProjectManager}, _serviceUser);
                request.SupervisorUserList = new SelectList(supervisorUserList, "Id", "FullName");

                request.ProjectUsers = _helper.GetUsers(new List<StatusRole>()
                                    {StatusRole.Employee}, _serviceUser, dto);

                return View(request);
            }
        }
        [Authorize(Roles = "AdminAupervisor")]
        public IActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            var projectDto = _service.GetProject(id.Value);
            var project = Mapper.Map<ProjectModl>(projectDto);
            project.ProjectUsers = Mapper.Map<List<ProjectUserModel>>(projectDto.ProjectUsers);

            return View(project);
        }

        // POST: Organization/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ProjectModl request)
        {
            var dto = Mapper.Map<ProjectDto>(request);

            var result = await _service.DeleteItemAsync(dto);

            if (result.IsSuccess)
            {
                return RedirectToAction("IndexAdmin");
            }
            else
            {
                request.ProjectUsers = Mapper.Map<List<ProjectUserModel>>(request.ProjectUsers);

                return View(request);
            }
        }
    }
}