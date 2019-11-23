using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectDataProgram.DAL.Unit.Contracts
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork MakeUnitOfWork();
    }
}
