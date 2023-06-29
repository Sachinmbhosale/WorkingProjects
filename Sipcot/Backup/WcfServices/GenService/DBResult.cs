using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace GenServiceDataContracts
{
    public class DBResult
    {
        public DBResult()
        {
            ResultDS = new DataSet();
            ErrorState = 0;
            ErrorSeverity = 0;
            Msg = string.Empty;
        }
        public DataSet ResultDS { get; set; }
        public int ErrorState { get; set; }
        public int ErrorSeverity { get; set; }
        public string Msg { get; set; }
    }
}
