using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectDataProgram.Core.DataBase;
using ProjectDataProgram.Core.Repositories;
using ProjectDataProgram.DAL.Data;

namespace ProjectDataProgram.DAL.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {

        private IQueryable<Project> GetInclude()
        {
            return DbSet.Include(y => y.SupervisorUser)
                        .Include(p => p.Tasks).ThenInclude(x => x.Author)
                        .Include(p => p.Tasks).ThenInclude(x => x.Executor)
                        .Include(x => x.ProjectUsers).ThenInclude(x => x.User);
        }

        public ProjectRepository(DataDbContext context) : base(context)
        {
            DbSet = context.Projects;
        }

        public override IEnumerable<Project> GetAll()
        {
            return GetInclude().ToList();
        }
        
        public IEnumerable<Project> GetFilter(string name, string contractorCompany, 
                                      string customerCompany, bool isDatePeriod, DateTime dateBegin, DateTime dateEnd,
                                      int? priority, int? supervisorUserId, int? userId)
        {
            IQueryable<Project> result = GetInclude();
            if (name?.Length > 0)
                result = result.Where(x => x.Name.ToLower().Contains(name.ToLower()));
            if (contractorCompany?.Length > 0)
                result = result.Where(x => x.ContractorCompany.ToLower().Contains(contractorCompany.ToLower()));
            if (customerCompany?.Length > 0)
                result = result.Where(x => x.CustomerCompany.ToLower().Contains(customerCompany.ToLower()));
            if (priority.HasValue)
                result = result.Where(x => x.Priority == priority.Value);
            if (supervisorUserId.HasValue)
                result = result.Where(x => x.SupervisorUserId == supervisorUserId.Value);
            if (isDatePeriod)
                result = result.Where(x => x.DateBegin.Date >= dateBegin.Date && x.DateEnd.Date < dateEnd.Date);

            if (userId.HasValue)
            {
                var projectUsers = _context.ProjectUsers.Where(x => x.UserId == userId.Value).ToList();
                if (projectUsers?.Count > 0)
                {
                    List<int> proIntIds = projectUsers.Select(x => x.ProjectId).ToList();
                    proIntIds = proIntIds.Distinct().ToList();
                    result = result.Where(x => proIntIds.Any(p => x.Id == p));
                }
                else
                {
                    return new List<Project>();
                }
            }

            return result.ToList();
        }

        public override Project GetById(int id)
        {
            return GetInclude().FirstOrDefault(p => p.Id == id);
        }

        private void AddUsers(List<ProjectUser> users)
        {
            if (users?.Count > 0)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    _context.ProjectUsers.Add(users[i]);
                }
            }
        }

        private void DeleteUser(List<ProjectUser> deleteUsers)
        {
            if (deleteUsers?.Count > 0)
            {
                for (int i = 0; i < deleteUsers.Count; i++)
                {
                    _context.ProjectUsers.Remove(deleteUsers[i]);
                }
            }
        }

        public void UpdateProjectUsers(List<ProjectUser> addUsers, List<ProjectUser> deleteUsers)
        {
            AddUsers(addUsers);
            DeleteUser(deleteUsers);
        }
        
        public void UpdateProjectUsers(List<ProjectUser> addUsers)
        {
            AddUsers(addUsers);
        }

        public void Delete(Project entity)
        {
            DeleteUser(entity.ProjectUsers);
            DbSet.Remove(entity);
        }

        public List<ProjectUser> GetOProjectUsers(List<int> idUserList, int projectId)
        {
            return _context.ProjectUsers.Where(x => idUserList.Any(y => x.UserId == y) &&
                                                    x.ProjectId == projectId).ToList();
        }
    }
}
