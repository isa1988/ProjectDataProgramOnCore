﻿using ProjectDataProgram.Core.DataBase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataProgram.Core.Repositories
{
    public interface IRepository<T> where T : Entity
    {
        Task<T> AddAsync(T entity);
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync();
        T GetById(int id);
        void Update(T entity);
        void Delete(T entity);
    }
}
