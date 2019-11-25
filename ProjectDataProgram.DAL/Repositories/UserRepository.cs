using Microsoft.EntityFrameworkCore;
using ProjectDataProgram.Core.DataBase;
using ProjectDataProgram.Core.Repositories;
using ProjectDataProgram.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectDataProgram.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        protected DataDbContext _context;
        protected DbSet<User> DbSet { get; set; }

        public UserRepository(DataDbContext context)
        {
            _context = context;
            DbSet = _context.Users;
        }
        public List<User> UserList()
        {
            return DbSet.ToList();
        }

        public List<User> UserList(List<StatusRole> roles)
        {
            List<int> roleList = _context.Roles.Where(x => roles.Any(y =>
                                x.Name.ToLower() == y.ToString().ToLower())).Select(x => x.Id).ToList();
            if (roleList == null && roleList.Count == 0) new List<User>();
            List<int> userIdList = _context.UserRoles.Where(x => roleList.Any(y => x.RoleId == y))
                                                      .Select(x => x.UserId).ToList();
            if (userIdList == null && userIdList.Count == 0) new List<User>();
            return DbSet.Where(x => userIdList.Any(y => x.Id == y)).ToList();
        }
    }
}
