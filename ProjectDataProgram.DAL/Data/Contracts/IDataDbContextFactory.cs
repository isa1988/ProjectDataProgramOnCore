using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectDataProgram.DAL.Data.Contracts
{
    public interface IDataDbContextFactory
    {
        DataDbContext Create();
    }
}
