using ICC.Predictor.Interfaces.Connection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICC.Predictor.DataAccess.Common
{
    public class BaseDataAccess
    {
        protected readonly string _ConnectionString;
        protected readonly string _Schema;

        public BaseDataAccess(IPostgre postgre)
        {
            _ConnectionString = postgre.ConnectionString;
            _Schema = postgre.Schema;
        }
    }
}
