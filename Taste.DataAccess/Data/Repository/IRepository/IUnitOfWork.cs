using System;
using System.Collections.Generic;
using System.Text;

namespace Taste.DataAccess.Data.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        void Save();
    }
}
