using ProjectDataProgram.DAL.Data.Contracts;
using ProjectDataProgram.DAL.Unit.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectDataProgram.DAL.Unit
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IDataDbContextFactory _applicationDbContextFactory;

        public UnitOfWorkFactory(IDataDbContextFactory applicationDbContextFactory)
        {
            _applicationDbContextFactory = applicationDbContextFactory;
        }

        public IUnitOfWork MakeUnitOfWork()
        {
            return new UnitOfWork(_applicationDbContextFactory.Create());
        }
    }
}
