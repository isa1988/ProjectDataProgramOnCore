using System;
using System.Collections.Generic;
using System.Text;
using ProjectDataProgram.DAL.Core.DataBase;

namespace ProjectDataProgram.DAL.Core.Repositories
{
    public interface IProjectUsersRepository
    {
        void Add(ProjectUser value);
        void Add(List<ProjectUser> list);
        void Delete(ProjectUser value);
        void Delete(List<ProjectUser> list);
    }
}
