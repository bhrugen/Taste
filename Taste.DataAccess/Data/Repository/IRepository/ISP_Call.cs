using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Taste.DataAccess.Data.Repository.IRepository
{
    public interface ISP_Call: IDisposable
    {
        IEnumerable<T> ReturnList<T>(string procedureName, DynamicParameters param = null);

        void ExecuteWithoutReturn(string procedureName, DynamicParameters param = null);
        T ExecureReturnScaler<T>(string procedureName, DynamicParameters param = null);
    }
}
