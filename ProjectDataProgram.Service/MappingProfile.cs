using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using ProjectDataProgram.Core.DataBase;
using ProjectDataProgram.Service.Dtos;

namespace ProjectDataProgram.Service
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            UserMapping();
            ProjectTaskMapping();
            ProjectUserMapping();
            ProjectMapping();
        }

        public void ProjectTaskMapping()
        {
            CreateMap<ProjectTask, ProjectTaskDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => (StatusTask) s.Status))
                .ForMember(d => d.Project, o => o.MapFrom(s => new ProjectDto { Id = s.Project.Id }))
                .ForMember(d => d.Author, o => o.MapFrom(s => s.Author))
                .ForMember(d => d.Executor, o => o.MapFrom(s => s.Executor));
        }

        private void UserMapping()
        {
            CreateMap<User, UserDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.FullName))
                .ForMember(d => d.EMail, o => o.MapFrom(s => s.Email))
                .ForMember(d => d.Status, (o) => o.Ignore())
                .ForMember(d => d.ProjectUserId, (o) => o.Ignore());
        }

        private void ProjectMapping()
        {
            CreateMap<Project, ProjectDto>()
                .ForMember(d => d.ProjectTasks, o => o.MapFrom(s => s.Tasks))
                .ForMember(d => d.ProjectUsers, o => o.MapFrom(s => s.ProjectUsers));
        }

        private void ProjectUserMapping()
        {
            CreateMap<ProjectUser, UserDto>()
                .ForMember(d => d.EMail, o => o.MapFrom(s => s.User.Email))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.User.FullName))
                .ForMember(d => d.ProjectUserId, o => o.MapFrom(s => s.User.Id))
                .ForMember(d => d.Status, (o) => o.Ignore());


            CreateMap<UserDto, ProjectUser>()
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.ProjectUserId))
                .ForMember(d => d.User, (o) => o.Ignore())
                .ForMember(d => d.Project, (o) => o.Ignore())
                .ForMember(d => d.ProjectId, (o) => o.Ignore());
            
        }

    }
}
