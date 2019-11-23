﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ProjectDataProgram.Core.DataBase;
using ProjectDataProgram.Core.Repositories;
using ProjectDataProgram.DAL.Data;

namespace ProjectDataProgram.DAL.Repositories
{
    class TaskRepository : Repository<Task>, ITaskRepository
    {
        public TaskRepository(DataDbContext context) : base(context)
        {
            DbSet = context.Tasks;
        }

        public override IEnumerable<Task> GetAll()
        {
            return DbSet.Include(p => p.Author).Include(x => x.Executor).Include(x => x.Project).ToList();
        }

        public override Task GetById(int id)
        {
            return DbSet.Include(p => p.Author).Include(x => x.Executor)
                        .Include(x => x.Project).FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Task> GetFilter(string name, int? authorId, int? executorId, int? projectId,
                                            int? priority, int? status)
        {
            IQueryable<Task> result = DbSet.Include(p => p.Author).Include(x => x.Executor).Include(x => x.Project);
            if (name?.Length > 0)
                result = result.Where(x => x.Name.ToLower().Contains(name.ToLower()));
            if (authorId.HasValue)
                result = result.Where(x => x.AuthorId == authorId.Value);
            if (executorId.HasValue)
                result = result.Where(x => x.ExecutorId == executorId.Value);
            if (priority.HasValue)
                result = result.Where(x => x.Priority == priority.Value);
            if (status.HasValue)
                result = result.Where(x => x.Status == status.Value);
            
            return result.ToList();
        }
    }
}
