using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Dapper;
using LoginAPI.Helpers;

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
        private readonly DBConfig dbConfig;

        public MySQLHelper(DBConfig dBConfig)
        {
            this.dbConfig = dBConfig;
        }

        public string CnnVal(string name) 
        {
            return dbConfig.ConnectionString;
        }
        public IDbConnection Connection
        {
            get
            {
                return new MySqlConnection(dbConfig.ConnectionString);
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
