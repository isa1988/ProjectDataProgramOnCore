using Microsoft.EntityFrameworkCore.Storage;
using ProjectDataProgram.Core.DataBase;
using ProjectDataProgram.Core.Repositories;
using ProjectDataProgram.DAL.Data;
using ProjectDataProgram.DAL.Repositories;
using ProjectDataProgram.DAL.Unit.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataProgram.DAL.Unit
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataDbContext _context;
        private readonly ConcurrentDictionary<Type, object> _repositories;

        private IDbContextTransaction _transaction;

        private bool _disposed;

        public UnitOfWork(DataDbContext context)
        {
            _context = context;
            _repositories = new ConcurrentDictionary<Type, object>();

            Project = new ProjectRepository(context);
            Task = new TaskRepository(context);
            
        }

        public IProjectRepository Project { get; }
        public ITaskRepository Task { get; }
        public Task<int> CompleteAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        //public void BeginTransaction(IsolationLevel level)
        //{
        //    _transaction = _context.Database.BeginTransaction(level);
        //}

        public void CommitTransaction()
        {
            if (_transaction == null) return;

            _transaction.Commit();
            _transaction.Dispose();

            _transaction = null;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity
        {
            return _repositories.GetOrAdd(typeof(TEntity), (object)new Repository<TEntity>(_context)) as IRepository<TEntity>;
        }

        public void RollbackTransaction()
        {
            if (_transaction == null) return;

            _transaction.Rollback();
            _transaction.Dispose();

            _transaction = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _context.Dispose();

            _disposed = true;
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
