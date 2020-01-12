using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace LoginAPI
{
    public class MySQLHelper : IDatabaseHelper
    {
        public IConfiguration _config;
        public MySQLHelper(IConfiguration config)
        {
            _config = config;
        }
        public string CnnVal(string name) 
        {
            return _config.GetConnectionString(name);
        }
        public IDbConnection Connection
        {
            get
            {
                return new MySqlConnection(_config.GetConnectionString("myDB"));
            }
        }
    }
}
