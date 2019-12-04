using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ProjectDataProgram.Web.Models;
using ProjectDataProgram.Service.Dtos;

namespace ProjectDataProgram.Web
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            UserMapping();
            ProjectUsersMapping();
            ProjectTaskMapping();
            ProjectMapping();
            ProjectFilter();
        }

        public void ProjectTaskMapping()
        {
            CreateMap<ProjectTaskModel, ProjectTaskDto>()
                .ForMember(d => d.Executor, o => o.MapFrom(s => 
                                                    new UserDto{Id = s.ExecutorId, Name = s.ExecutorName}))
                .ForMember(d => d.Author, o => o.MapFrom(s => 
                                                    new UserDto{Id = s.AuthorId, Name = s.AuthorName}))
                .ForMember(d => d.Project, o => o.MapFrom(s => 
                                                    new ProjectDto{Id = s.ProjectId} ));
            CreateMap<ProjectTaskDto, ProjectTaskModel>()
                .ForMember(d => d.ExecutorId, o => o.MapFrom(s => s.Executor != null ? s.Executor.Id : 0))
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => s.Author != null ? s.Author.Id : 0))
                .ForMember(d => d.ExecutorName, o => o.MapFrom(s => s.Executor != null ? s.Executor.Name : string.Empty))
                .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.Author != null ? s.Author.Name : string.Empty))
                .ForMember(d => d.ProjectId, o => o.MapFrom(s => s.Project != null ? s.Project.Id : 0));
        }

        private void UserMapping()
        {
            CreateMap<UserModel, UserDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.FullName))
                .ForMember(d => d.EMail, o => o.MapFrom(s => s.Email));
            CreateMap<UserDto, UserModel>()
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.EMail));
        }

        private void ProjectMapping()
        {
            CreateMap<ProjectModl, ProjectDto>()
                .ForMember(d => d.SupervisorUser, o => o.MapFrom(
                    s => new UserDto{Id = s.SupervisorUserId, Name = s.SupervisorUserName}));
            CreateMap<ProjectDto, ProjectModl>()
                .ForMember(d => d.SupervisorUserId, o => o.MapFrom(
                    s => s.SupervisorUser != null ? s.SupervisorUser.Id : 0))
                .ForMember(d => d.SupervisorUserName, o => o.MapFrom(
                    s => s.SupervisorUser != null ? s.SupervisorUser.Name : string.Empty));
        }

        private void ProjectUsersMapping()
        {
            CreateMap<UserDto, ProjectUserModel>()
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.EMail))
                .ForMember(d => d.IsAdd, (o) => o.Ignore())
                .ForMember(d => d.IsDelete, (o) => o.Ignore());

            CreateMap<ProjectUserModel, UserDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.FullName))
                .ForMember(d => d.EMail, o => o.MapFrom(s => s.Email))
                .ForMember(d => d.Status, (o) => o.MapFrom(s => 
                            s.IsAdd ? ProjectUserStatus.New :
                            s.IsDelete ? ProjectUserStatus.Delete : ProjectUserStatus.Free));
        }

        private void ProjectFilter()
        {
            CreateMap<ProjectFilterDto, ProjectFilterModel>();
            CreateMap<ProjectFilterModel, ProjectFilterDto>();
        }
    }
}
