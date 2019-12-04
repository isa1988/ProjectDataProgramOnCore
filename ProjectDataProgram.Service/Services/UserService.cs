using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using ProjectDataProgram.Core.Repositories;
using ProjectDataProgram.DAL.Unit.Contracts;
using ProjectDataProgram.Service.Dtos;
using ProjectDataProgram.Service.Services.Contracts;

namespace ProjectDataProgram.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public UserService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));

            _unitOfWorkFactory = unitOfWorkFactory;
        }

        private List<UserDto> GetUserInProjectAndAll(List<Core.DataBase.User> users, 
                                                     ProjectDto projectDto, bool isRegistr)
        {
            if (!isRegistr)
            {

                return Mapper.Map<List<UserDto>>(users);
            }
            else
            {
                var userList = Mapper.Map<List<UserDto>>(users);
                UserDto tempDto = null;
                for (int i = 0; i < userList.Count; i++)
                {
                    if (projectDto != null)
                    {
                        tempDto = projectDto.ProjectUsers.FirstOrDefault(x => x.ProjectUserId == userList[i].Id);
                    }

                    if (tempDto != null)
                    {
                        userList[i].Status = ProjectUserStatus.Save;
                        userList[i].ProjectUserId = userList[i].Id;
                        userList[i].Id = tempDto.Id;
                    }
                    else
                    {
                        userList[i].Status = ProjectUserStatus.Free;
                        userList[i].ProjectUserId = userList[i].Id;
                        userList[i].Id = 0;
                    }
                }

                return userList;
            }
        }

        public List<UserDto> UserList(ProjectDto projectDto)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var userList = unitOfWork.User.UserList();
                return GetUserInProjectAndAll(userList, null, false);
            }
        }

        public List<UserDto> UserList(List<StatusRole> roles, bool isRegistr, ProjectDto projectDto = null)
        {
            if (roles == null || roles.Count == 0) return new List<UserDto>();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var userList = unitOfWork.User.UserList(roles);
                return GetUserInProjectAndAll(userList, projectDto, isRegistr);
            }
        }
    }
}
