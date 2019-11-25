using System;
using System.Collections.Generic;
using System.Text;
using ProjectDataProgram.Core.DataBase;

namespace ProjectDataProgram.Core.Repositories
{
    public interface IUserRepository
    {
        List<User> UserList();
        List<User> UserList(List<StatusRole> roles);
    }

    public enum StatusRole : int
    {
        AdminAupervisor = 1,
        ProjectManager = 2,
        Employee = 3
    }
}
