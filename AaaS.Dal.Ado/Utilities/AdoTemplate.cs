using AaaS.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Dal.Ado.Utilities
{
    public class AdoTemplate
    {
        private readonly IConnectionFactory connectionFactory;

        public AdoTemplate(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, Func<IDataRecord, T> rowMapper, params QueryParameter[] queryParameter)
        {
            await using var connection = await connectionFactory.CreateConnectionAsync();
            await using var cmd = connection.CreateCommand();

            cmd.CommandText = sql;

            AddParameters(cmd, queryParameter);

            var executeReaderTask = cmd.ExecuteReaderAsync();
            var items = new List<T>();

            await using var reader = await executeReaderTask;
            while (reader.Read())
            {
                items.Add(rowMapper(reader));
            }
            return items;
        }

        private void AddParameters(DbCommand cmd, QueryParameter[] queryParameters)
        {
            foreach (var parameter in queryParameters)
            {
                DbParameter dbParam = cmd.CreateParameter();
                dbParam.ParameterName = parameter.Name;
                dbParam.Value = parameter.Value;
                cmd.Parameters.Add(dbParam);
            }
        }

        public async Task<T> QuerySingleAsync<T>(string sql, Func<IDataRecord, T> mapRowToPerson, params QueryParameter[] queryParameter)
        {
            return (await QueryAsync(sql, mapRowToPerson, queryParameter)).SingleOrDefault();
        }


        public async Task<int> ExecuteAsync(string sql, params QueryParameter[] queryParameters)
        {
            await using var connection = await connectionFactory.CreateConnectionAsync();
            await using var command = connection.CreateCommand();

            command.CommandText = sql;
            AddParameters(command, queryParameters);

            return await command.ExecuteNonQueryAsync();
        }

        public async Task<R> ExecuteScalarAsync<R>(string sql, params QueryParameter[] queryParameters)
        {
            await using var connection = await connectionFactory.CreateConnectionAsync();
            await using var command = connection.CreateCommand();

            command.CommandText = sql;
            AddParameters(command, queryParameters);

            return (R) await command.ExecuteScalarAsync();
        }
    }
}
