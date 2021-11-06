using AaaS.Common;
using AaaS.Dal.Ado.Utilities;
using AaaS.Dal.Interface;
using AaaS.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaaS.Dal.Ado
{
    public abstract class AdoClientDao : IClientDao
    {
        private readonly AdoTemplate template;
        protected abstract string LastInsertedIdQuery { get; }

        public AdoClientDao(IConnectionFactory factory)
        {
            template = new AdoTemplate(factory);
        }

        private Client MapRowToClient(IDataRecord row)
          => new()
          {
              Id = (int)row["id"],
              ApiKey = (string)row["api_key"],
              Name = (string)row["name"],
          };

        public IAsyncEnumerable<Client> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Client> FindByIdAsync(int id)
          => await template.QuerySingleAsync(
                "select * from Client where id=@id",
                MapRowToClient,
                new QueryParameter("@id", id));

        public async Task InsertAsync(Client client)
        {
            const string SQL_INSERT = "insert into Client (name, api_key) values (@name, @key)";

            client.Id = Convert.ToInt32(await template.ExecuteScalarAsync<object>($"{SQL_INSERT};{LastInsertedIdQuery}",
                 new QueryParameter("@name", client.Name),
                 new QueryParameter("@key", client.ApiKey)));
        }

        public Task<bool> UpdateAsync(Client person)
        {
            throw new NotImplementedException();
        }
    }
}
