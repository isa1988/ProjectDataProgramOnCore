using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ProjectDataProgram.Core.DataBase;
using ProjectDataProgram.DAL.Unit.Contracts;
using ProjectDataProgram.Service.Dtos;
using ProjectDataProgram.Service.Services.Contracts;

namespace ProjectDataProgram.Service.Services
{
    public class ProjectTaskService : IProjectTaskService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public ProjectTaskService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));

            _unitOfWorkFactory = unitOfWorkFactory;
        }

        private EntityOperationResult<ProjectTask> Check(ProjectTaskDto projectTask)
        {
            if (projectTask.Name == string.Empty)
                return EntityOperationResult<ProjectTask>
                    .Failure()
                    .AddError("Не указано наименование");
            if (projectTask.Author == null)
                return EntityOperationResult<ProjectTask>
                    .Failure()
                    .AddError("Не указан Автор");
            if (projectTask.Executor == null)
                return EntityOperationResult<ProjectTask>
                    .Failure()
                    .AddError("Не указан Исполнитель");
            
            return null;
        }

        public async Task<EntityOperationResult<ProjectTask>> CreateItemAsync(ProjectTaskDto projectTaskCreateDto)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var result = Check(projectTaskCreateDto);
                if (result != null) return result;
                if (projectTaskCreateDto.Project == null)
                    return EntityOperationResult<ProjectTask>
                        .Failure()
                        .AddError("Не указан Проект");
                try
                {
                    var project = new ProjectTask
                    {
                        Name = projectTaskCreateDto.Name,
                        AuthorId = projectTaskCreateDto.Author.Id,
                        ExecutorId = projectTaskCreateDto.Executor.Id,
                        Status = (int)projectTaskCreateDto.Status,
                        ProjectId = projectTaskCreateDto.Project.Id,
                        Priority = projectTaskCreateDto.Priority,
                        Comment = projectTaskCreateDto.Comment
                    };

                    var entity = await unitOfWork.ProjectTask.AddAsync(project);
                    await unitOfWork.CompleteAsync();
                    
                    return EntityOperationResult<ProjectTask>.Success(entity);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<ProjectTask>.Failure().AddError(ex.Message);
                }
            }
        }

        public async Task<EntityOperationResult<ProjectTask>> EditItemAsync(ProjectTaskDto projectTaskEditDto)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var result = Check(projectTaskEditDto);
                if (result != null) return result;
                var projectTask = unitOfWork.ProjectTask.GetById(projectTaskEditDto.Id);
                

                try
                {
                    projectTask.Name = projectTaskEditDto.Name;
                    projectTask.AuthorId = projectTaskEditDto.Author.Id;
                    projectTask.ExecutorId = projectTaskEditDto.Executor.Id;
                    projectTask.Status = (int) projectTaskEditDto.Status;
                    projectTask.ProjectId = projectTaskEditDto.Project.Id;
                    projectTask.Priority = projectTaskEditDto.Priority;
                    projectTask.Comment = projectTaskEditDto.Comment;

                    unitOfWork.ProjectTask.Update(projectTask);
                    await unitOfWork.CompleteAsync();

                    return EntityOperationResult<ProjectTask>.Success(projectTask);
                }
                catch (Exception ex)
                {
                    return EntityOperationResult<ProjectTask>.Failure().AddError(ex.Message);
                }
            }
        }

        public ProjectTaskDto GetProjectTaskById(int id)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var projectTask = unitOfWork.ProjectTask.GetById(id);
                if (projectTask == null) return null;
                var projectTaskDto = new ProjectTaskDto
                {
                    Id = projectTask.Id,
                    Name = projectTask.Name,
                    Priority = projectTask.Priority,
                    Status = (StatusTask)projectTask.Status,
                    Comment = projectTask.Comment,
                    Project = new ProjectDto
                    {
                        Id = projectTask.ProjectId
                    },
                    Author = new UserDto
                    {
                        Id = projectTask.Author.Id,
                        Name = projectTask.Author.FullName,
                        EMail = projectTask.Author.Email
                    },
                    Executor = new UserDto
                    {
                        Id = projectTask.Executor.Id,
                        Name = projectTask.Executor.FullName,
                        EMail = projectTask.Executor.Email
                    },
                };
                return projectTaskDto;
            }
        }
    }
}
