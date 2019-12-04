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

        private void GetProjectDtoToProject(ProjectDto projectDto, Project project)
        {
            project.Name = projectDto.Name;
            project.CustomerCompany = projectDto.CustomerCompany;
            project.ContractorCompany = projectDto.CustomerCompany;
            project.Priority = projectDto.Priority;
            project.DateBegin = projectDto.DateBegin;
            project.DateEnd = projectDto.DateEnd;
            project.SupervisorUserId = projectDto.SupervisorUser.Id;
        }

        private void SetProjectIdToList(List<ProjectUser> users, int id)
        {
            for (int i = 0; i < users.Count; i++)
            {
                users[i].ProjectId = id;
            }
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

                try
                {
                    var project = new Project();
                    GetProjectDtoToProject(projectCreateDto, project);
                    
                    var entity = await unitOfWork.Project.AddAsync(project);
                    await unitOfWork.CompleteAsync();

                    var projectUserList = Mapper.Map<List<ProjectUser>>(projectCreateDto.ProjectUsers
                                            .Where(x => x.Status == ProjectUserStatus.New)
                                            .ToList());
                    SetProjectIdToList(projectUserList, entity.Id);
                    unitOfWork.Project.UpdateProjectUsers(projectUserList);
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
                    if (projectEditDto.ProjectUsers[i].Status == ProjectUserStatus.New)
                    {
                        if (project.ProjectUsers.Count(x => x.UserId == projectEditDto.ProjectUsers[i].Id) > 0)
                            return EntityOperationResult<Project>
                                .Failure()
                                .AddError($"Пользователь {projectEditDto.ProjectUsers[i].Name} уже есть в базе");
                    }
                }

                try
                {
                    GetProjectDtoToProject(projectEditDto, project);

                    unitOfWork.Project.Update(project);
                    await unitOfWork.CompleteAsync();

                    var projectUserList = Mapper.Map<List<ProjectUser>>(projectEditDto.ProjectUsers
                                                        .Where(x => x.Status == ProjectUserStatus.New)
                                                        .ToList());
                    SetProjectIdToList(projectUserList, project.Id);
                    List<int> deleteList = projectEditDto.ProjectUsers
                        .Where(x => x.Status == ProjectUserStatus.Delete)
                        .Select(x => x.Id).ToList();
                    var projectUserDeleteList = new List<ProjectUser>();
                    if (deleteList?.Count > 0)
                        projectUserDeleteList = project.ProjectUsers
                            .Where(y => deleteList.Any(x => x == y.Id)).ToList();
                    SetProjectIdToList(projectUserDeleteList, project.Id);
                    unitOfWork.Project.UpdateProjectUsers(projectUserList, projectUserDeleteList);
                    await unitOfWork.CompleteAsync();
                    return EntityOperationResult<Project>.Success(project);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<Project>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<Project>> DeleteItemAsync(ProjectDto projectDeleteDto)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var project = unitOfWork.Project.GetById(projectDeleteDto.Id);

                if (project.ProjectUsers.Any(x => projectDeleteDto.ProjectTasks.Count > 0) )
                    return EntityOperationResult<Project>
                        .Failure()
                        .AddError($"Есть задания удалите сперва их");

                try
                {
                    SetProjectIdToList(project.ProjectUsers, project.Id);
                    unitOfWork.Project.UpdateProjectUsers(null, project.ProjectUsers);
                    unitOfWork.Project.Delete(project);
                    await unitOfWork.CompleteAsync();

                    return EntityOperationResult<Project>.Success(project);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<Project>.Failure().AddError(ex.Message);
                }
            }
        }

        public List<ProjectDto> ProjectAll()
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                List<Project> projectList = unitOfWork.Project.GetAll().ToList();
                var projectResultList = Mapper.Map<List<ProjectDto>>(projectList);
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
                var projectResultList = Mapper.Map<List<ProjectDto>>(projectList);

                return projectResultList;
            }
        }

        public ProjectDto GetProject(int id)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                Project project = unitOfWork.Project.GetById(id);
                
                return Mapper.Map<ProjectDto>(project);
            }
        }
    }
}
