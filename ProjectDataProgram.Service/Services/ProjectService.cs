using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ProjectDataProgram.Core.DataBase;
using ProjectDataProgram.Core.Repositories;
using ProjectDataProgram.DAL.Unit.Contracts;
using ProjectDataProgram.Service.Dtos;
using ProjectDataProgram.Service.Services.Contracts;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation.PredefinedTransformations;

namespace ProjectDataProgram.Service.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public ProjectService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));

            _unitOfWorkFactory = unitOfWorkFactory;
        }

        private EntityOperationResult<Project> Check(ProjectDto project)
        {
            if (project.Name == string.Empty)
                return EntityOperationResult<Project>
                    .Failure()
                    .AddError("Не указано наименование");
            if (project.ContractorCompany == string.Empty)
                return EntityOperationResult<Project>
                    .Failure()
                    .AddError("Не указано наименование компании-исполнителя");
            if (project.CustomerCompany == string.Empty)
                return EntityOperationResult<Project>
                    .Failure()
                    .AddError("Не указано наименование компании-заказчика");
            if (project.DateBegin == null || project.DateEnd == null)
                return EntityOperationResult<Project>
                    .Failure()
                    .AddError("Не указан период");
            if (project.SupervisorUser == null)
                return EntityOperationResult<Project>
                    .Failure()
                    .AddError("Не указаны руководитель проекта");
            
            return null;
        }

        public async Task<EntityOperationResult<Project>> CreateItemAsync(ProjectDto projectCreateDto)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var result = Check(projectCreateDto);
                if (result != null) return result;
                if (projectCreateDto.ProjectUsers == null || projectCreateDto.ProjectUsers.Count == 0)
                    return EntityOperationResult<Project>
                        .Failure()
                        .AddError("Не указаны исполнители");
                for (int i = 0; i < projectCreateDto.ProjectUsers.Count; i++)
                {
                    if (projectCreateDto.ProjectUsers[i].User == null)
                        return EntityOperationResult<Project>
                            .Failure()
                            .AddError("Не указан исполнитель в списке");
                }

                try
                {
                    var project = new Project
                    {
                        Name = projectCreateDto.Name,
                        CustomerCompany = projectCreateDto.CustomerCompany,
                        ContractorCompany = projectCreateDto.CustomerCompany,
                        Priority = projectCreateDto.Priority,
                        DateBegin = projectCreateDto.DateBegin,
                        DateEnd = projectCreateDto.DateEnd,
                        SupervisorUserId = projectCreateDto.SupervisorUser.Id
                    };
                    
                    var entity = await unitOfWork.Project.AddAsync(project);
                    await unitOfWork.CompleteAsync();

                    var projectUserList = new List<ProjectUser>();
                    for (int i = 0; i < projectCreateDto.ProjectUsers.Count; i++)
                    {
                        projectUserList.Add(new ProjectUser
                        {
                            ProjectId = entity.Id,
                            UserId = projectCreateDto.ProjectUsers[i].User.Id
                        });
                    }
                    unitOfWork.Project.AddAsync(projectUserList);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<Project>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<Project>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<Project>> EditItemAsync(ProjectDto projectEditDto)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var result = Check(projectEditDto);
                if (result != null) return result;
                var project = unitOfWork.Project.GetById(projectEditDto.Id);
                for (int i = 0; i < projectEditDto.ProjectUsers.Count; i++)
                {
                    if (projectEditDto.ProjectUsers[i].User == null)
                        return EntityOperationResult<Project>
                            .Failure()
                            .AddError("Не указан исполнитель в списке");
                    if (projectEditDto.ProjectUsers[i].Status == ProjectUserStatus.New)
                    {
                        if (project.ProjectUsers.Count(x => x.UserId == projectEditDto.ProjectUsers[i].User.Id) > 0)
                            return EntityOperationResult<Project>
                                .Failure()
                                .AddError($"Пользователь {projectEditDto.ProjectUsers[i].User} уже есть в базе");
                    }
                }

                try
                {
                    project.Name = projectEditDto.Name;
                    project.CustomerCompany = projectEditDto.CustomerCompany;
                    project.ContractorCompany = projectEditDto.CustomerCompany;
                    project.Priority = projectEditDto.Priority;
                    project.DateBegin = projectEditDto.DateBegin;
                    project.DateEnd = projectEditDto.DateEnd;
                    project.SupervisorUserId = projectEditDto.SupervisorUser.Id;

                    unitOfWork.Project.Update(project);
                    await unitOfWork.CompleteAsync();

                    var projectUserTemp = projectEditDto.ProjectUsers
                                                        .Where(x => x.Status == ProjectUserStatus.New)
                                                        .ToList();
                    var projectUserList = new List<ProjectUser>();
                    for (int i = 0; i < projectUserTemp.Count; i++)
                    {
                        projectUserList.Add(new ProjectUser
                        {
                            ProjectId = project.Id,
                            UserId = projectUserTemp[i].User.Id
                        });
                    }
                    projectUserTemp = projectEditDto.ProjectUsers
                                                        .Where(x => x.Status == ProjectUserStatus.Delete)
                                                        .ToList();
                    var projectUserDeleteList = unitOfWork.Project.GetOProjectUsers(projectEditDto.ProjectUsers
                                                                .Select(x => x.User.Id).ToList(), project.Id);
                    unitOfWork.Project.Update(projectUserList, projectUserDeleteList);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<Project>.Success(project);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<Project>.Failure().AddError(ex.Message);
                }
            }
        }

        private List<ProjectDto> GetConvertProjectList(List<Project> projectList)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                List<ProjectDto> projectResultList = new List<ProjectDto>();
                List<User> users = unitOfWork.User.UserList(new List<StatusRole>() {StatusRole.Employee});

                for (int i = 0; i < projectList.Count; i++)
                {
                    ProjectDto project = new ProjectDto
                    {
                        Id = projectList[i].Id,
                        Name = projectList[i].Name,
                        ContractorCompany = projectList[i].ContractorCompany,
                        CustomerCompany = projectList[i].CustomerCompany,
                        DateBegin = projectList[i].DateBegin,
                        DateEnd = projectList[i].DateEnd,
                        Priority = projectList[i].Priority,
                        SupervisorUser = new UserDto
                        {
                            Id = projectList[i].SupervisorUser.Id,
                            Name = projectList[i].SupervisorUser.FullName,
                            EMail = projectList[i].SupervisorUser.Email
                        }
                    };
                    for (int j = 0; j < projectList[i].ProjectUsers.Count; j++)
                    {
                        User user = users.FirstOrDefault(x => x.Id == projectList[i].ProjectUsers[j].UserId);
                        if (user == null) continue;

                        project.ProjectUsers.Add(new ProjectUserDto
                        {
                            Project = project,
                            User = new UserDto
                            {
                                Id = user.Id,
                                Name = user.FullName,
                                EMail = user.Email
                            }
                        });
                    }

                    projectResultList.Add(project);
                }

                return projectResultList;
            }
        }

        public List<ProjectDto> ProjectAll()
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                List<Project> projectList = unitOfWork.Project.GetAll().ToList();
                var projectResultList = GetConvertProjectList(projectList);

                return projectResultList;
            }
        }

        public List<ProjectDto> ProjectAll(ProjectFilterDto projectFilterDto)
        {
            if (projectFilterDto == null) return new List<ProjectDto>();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                if (!projectFilterDto.IsPriority)
                    projectFilterDto.Priority = null;
                if (!projectFilterDto.IsSpervisorUser)
                    projectFilterDto.SpervisorUserId = null;
                if (!projectFilterDto.IsUser)
                    projectFilterDto.UserId = null;
                List<Project> projectList = unitOfWork.Project.GetFilter(
                    name: projectFilterDto.Name,
                    contractorCompany: projectFilterDto.ContractorCompany,
                    customerCompany: projectFilterDto.CustomerCompany,
                    isDatePeriod: projectFilterDto.IsDatePeriod,
                    dateBegin: projectFilterDto.DateBegin,
                    dateEnd: projectFilterDto.DateEnd,
                    priority: projectFilterDto.Priority,
                    supervisorUserId: projectFilterDto.SpervisorUserId,
                    userId: projectFilterDto.UserId
                ).ToList();
                var projectResultList = GetConvertProjectList(projectList);

                return projectResultList;
            }
        }

        public ProjectDto GetProject(int id)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                Project project = unitOfWork.Project.GetById(id);
                List<User> users = unitOfWork.User.UserList(new List<StatusRole>(){ StatusRole.Employee });
                ProjectDto projectDto = new ProjectDto
                {
                    Id = project.Id,
                    Name = project.Name,
                    ContractorCompany = project.ContractorCompany,
                    CustomerCompany = project.CustomerCompany,
                    DateBegin = project.DateBegin,
                    DateEnd = project.DateEnd,
                    Priority = project.Priority
                };
                projectDto.SupervisorUser = new UserDto
                {
                    Id = project.SupervisorUser.Id,
                    Name = project.SupervisorUser.FullName,
                    EMail = project.SupervisorUser.Email
                };
                for (int i = 0; i < project.ProjectUsers.Count; i++)
                {
                    User user = users.FirstOrDefault(x => x.Id == project.ProjectUsers[i].UserId);
                    if (user == null) continue;
                    ProjectUserDto projectUserDto = new ProjectUserDto
                    {
                        User = new UserDto
                        {
                            Id = user.Id,
                            Name = user.FullName,
                            EMail = user.Email
                        }
                    };
                    projectDto.ProjectUsers.Add(projectUserDto);
                }

                return projectDto;
            }
        }
    }
}
