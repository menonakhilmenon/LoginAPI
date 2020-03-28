using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Dapper;

namespace LoginAPI
{
    public static class SQLExtensions 
    {
        public static DynamicParameters AddParameter(this DynamicParameters parameter, string name, object data)
        {
            parameter.Add(name, data);
            return parameter;
        }
    }
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
                return new MySqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }


        public async Task<int> CallStoredProcedureExec(string procedureName, DynamicParameters parameters)
        {
            using (var dbConnection = Connection)
            {
                try
                {
                    return await dbConnection.ExecuteAsync(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
                }
                catch (MySqlException e)
                {
                    Console.WriteLine(e.ToString());
                    return 0;
                }
            }
        }

        public async Task<int> CallStoredProcedureExec(IDbConnection dbConnection, string procedureName, DynamicParameters parameters)
        {
            try
            {
                return await dbConnection.ExecuteAsync(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.ToString());
                return 0;
            }
        }

        public async Task<IEnumerable<T>> CallStoredProcedureQuery<T>(string procedureName, DynamicParameters parameters)
        {
            try
            {
                using (var dbConnection = Connection)
                {
                    var res = (await SqlMapper.QueryAsync<T>(dbConnection, procedureName, parameters, commandType: CommandType.StoredProcedure));
                    return res;
                }
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }

        }

        public async Task<IEnumerable<T>> CallStoredProcedureQuery<T>(IDbConnection connection, string procedureName, DynamicParameters parameters)
        {
            try
            {
                var res = (await SqlMapper.QueryAsync<T>(connection, procedureName, parameters, commandType: CommandType.StoredProcedure));
                return res;
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }

        }


    }
}
