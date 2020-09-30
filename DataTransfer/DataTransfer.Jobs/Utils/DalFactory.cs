using BFES.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.Jobs.Utils
{
    public static class DalFactory
    {
        public static IDataBase GreateIDataBase(DatabaseSource DBSource)
        {
            return DataBaseFactory.GreateDataBaseFactory(DBSource.Connstr, DBSource.DBType);
        }
    }
}
