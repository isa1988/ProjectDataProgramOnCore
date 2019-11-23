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
        public ProjectRepository(DataDbContext context) : base(context)
        {
            DbSet = context.Projects;
        }

        public override IEnumerable<Project> GetAll()
        {
            return DbSet.Include(p => p.Tasks).Include(x => x.ProjectUsers).ToList();
        }
        
        public IEnumerable<Project> GetFilter(string name, string contractorCompany, 
                                      string customerCompany, int? priority, int? supervisorUserId, int? userId)
        {
            IQueryable<Project> result = DbSet.Include(p => p.Tasks).Include(x => x.ProjectUsers);
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
            if (userId.HasValue)
            {
                var projectUsers = _context.ProjectUsers.Where(x => x.UserId == userId.Value).ToList();
                if (projectUsers?.Count > 0)
                {
                    List<int> proIntIds = projectUsers.Select(x => x.ProjectId).ToList();
                    proIntIds = proIntIds.Distinct().ToList();
                    result = result.Where(x => proIntIds.Any(p => x.Id == p));
                }
            }

            return result.ToList();
        }

        public override Project GetById(int id)
        {
            return DbSet.Include(p => p.Tasks).Include(x => x.ProjectUsers).FirstOrDefault(p => p.Id == id);
        }

        public void AddAsync(List<ProjectUser> users)
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

        public void Update(List<ProjectUser> addUsers, List<ProjectUser> deleteUsers)
        {
            AddAsync(addUsers);
            DeleteUser(deleteUsers);
        }

        public void Delete(Project entity)
        {
            DeleteUser(entity.ProjectUsers);
            DbSet.Remove(entity);
        }
    }
}
