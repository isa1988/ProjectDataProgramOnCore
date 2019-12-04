using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ProjectDataProgram.Core.DataBase;
using ProjectDataProgram.Service.Dtos;

namespace ProjectDataProgram.Service.Services.Contracts
{
    public interface IProjectTaskService
    {
        Task<EntityOperationResult<ProjectTask>> CreateItemAsync(ProjectTaskDto projectTaskCreateDto);
        Task<EntityOperationResult<ProjectTask>> EditItemAsync(ProjectTaskDto projectTaskEditto);
        Task<EntityOperationResult<ProjectTask>> DeleteItemAsync(ProjectTaskDto projectTaskDeleteDto);
        ProjectTaskDto GetProjectTaskById(int id);
    }
}
