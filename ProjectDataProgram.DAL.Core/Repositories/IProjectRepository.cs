using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ProjectDataProgram.Core.DataBase;

namespace ProjectDataProgram.Core.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        IEnumerable<Project> GetFilter(string name, string contractorCompany,
            string customerCompany, bool isDatePeriod, DateTime dateBegin, DateTime dateEnd,
            int? priority, int? supervisorUserId, int? userId);

        void AddAsync(List<ProjectUser> users);

        void Update(List<ProjectUser> addUsers, List<ProjectUser> deleteUsers);

        List<ProjectUser> GetOProjectUsers(List<int> idUserList, int projectId);
    }
}
