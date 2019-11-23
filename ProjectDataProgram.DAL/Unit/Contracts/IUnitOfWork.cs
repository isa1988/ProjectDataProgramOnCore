﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ProjectDataProgram.Core.DataBase;
using ProjectDataProgram.Core.Repositories;

namespace ProjectDataProgram.DAL.Unit.Contracts
{
    public  interface IUnitOfWork : IDisposable
    {
        IProjectRepository Project { get; }
        ITaskRepository Task { get; }

        Task<int> CompleteAsync();
        void BeginTransaction();
        //void BeginTransaction(IsolationLevel level);
        void RollbackTransaction();
        void CommitTransaction();

        IRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity;
    }
}