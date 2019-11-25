using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public List<UserDto> UserList()
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                return unitOfWork.User.UserList().Select(x => new UserDto
                {
                    Id = x.Id,
                    Name = x.FullName,
                    EMail = x.Email,
                    Status = ProjectUserStatus.Free 
                }).ToList();
            }
        }

        public List<UserDto> UserList(List<StatusRole> roles)
        {
            if (roles == null || roles.Count == 0) return new List<UserDto>();
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                return unitOfWork.User.UserList(roles).Select(x => new UserDto
                {
                    Id = x.Id,
                    Name = x.FullName,
                    EMail = x.Email,
                    Status = ProjectUserStatus.Free
                }).ToList();
            }
        }
    }
}
