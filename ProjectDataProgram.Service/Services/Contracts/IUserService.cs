using System;
using System.Collections.Generic;
using System.Text;
using ProjectDataProgram.Core.Repositories;
using ProjectDataProgram.Service.Dtos;

namespace ProjectDataProgram.Service.Services.Contracts
{
    public interface IUserService
    {
        List<UserDto> UserList();
        List<UserDto> UserList(List<StatusRole> roles);
    }
}
