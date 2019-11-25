using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ProjectDataProgram.Core.DataBase;
using ProjectDataProgram.Service.Dtos;

namespace ProjectDataProgram.Service.Services.Contracts
{
    public interface IProjectService
    {
        Task<EntityOperationResult<Project>> CreateItemAsync(ProjectDto basketCreateDto);
        Task<EntityOperationResult<Project>> EditItemAsync(ProjectDto basketEditto);
        List<ProjectDto> ProjectAll();
        List<ProjectDto> ProjectAll(ProjectFilterDto projectFilterDto);
        ProjectDto GetProject(int id);
    }
}
