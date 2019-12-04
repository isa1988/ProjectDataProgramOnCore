using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ProjectDataProgram.Core.Repositories;
using ProjectDataProgram.Service.Dtos;
using ProjectDataProgram.Service.Services.Contracts;
using ProjectDataProgram.Web.Models;

namespace ProjectDataProgram.Web.Helper
{
    class ClassHelper
    {
        public List<UserModel> GetUsers(List<StatusRole> statusRoles,
            IUserService serviceUser)
        {
            return Mapper.Map<List<UserModel>>(serviceUser.UserList(roles: statusRoles, isRegistr: false));

        }
        public List<ProjectUserModel> GetUsers(List<StatusRole> statusRoles,
            IUserService serviceUser, ProjectDto projectDto = null)
        {
            return Mapper.Map<List<ProjectUserModel>>(
                serviceUser.UserList(roles: statusRoles, isRegistr: true, projectDto: projectDto));
        }
        public List<UserModel> GetUsers(List<UserDto> userDtos)
        {
            return Mapper.Map<List<UserModel>>(userDtos);
        }
    }
}
