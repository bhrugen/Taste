using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Taste.DataAccess.Data.Repository.IRepository;

namespace Taste.DataAccess.Data.Repository
{
    class SP_Call : ISP_Call
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public T ExecureReturnScaler<T>(string procedureName, DynamicParameters param = null)
        {
            throw new NotImplementedException();
        }

        public void ExecuteWithoutReturn(string procedureName, DynamicParameters param = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> ReturnList<T>(string procedureName, DynamicParameters param = null)
        {
            throw new NotImplementedException();
        }
    }
}
