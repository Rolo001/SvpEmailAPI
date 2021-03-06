﻿namespace Svp.Data.Classes
{
    using Svp.Data.Classes.CommandFactory;
    using Svp.Data.Classes.Exceptions;
    using Svp.Data.Interfaces;
    using Svp.Data.Model;
    using System;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    public class UserGateway : IUserGateway
    {
        private readonly string connectionString;
        private readonly IUserSqlCommandFactory userCommand;

        public UserGateway(IConnectionStringProvider provider, IUserSqlCommandFactory UserCommand)
        {
            this.connectionString = provider.GetDbConnectionString();
            userCommand = UserCommand;
        }
        
        public async Task AddUser(User request) // method to sve/read into/from datastore or database
        {
            try
            {
                using (var connection = new SqlConnection(this.connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        var command = this.userCommand.AddUserSqlCommand(request);
                        command.Connection = connection;
                        command.Transaction = transaction;

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new DataGatewayException(e);
            }
        }      
        
        public async Task<string> GetUserDetail(int ID) // method to sve/read into/from datastore or database
        {
            string result;
            try
            {
                using (var connection = new SqlConnection(this.connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        var command = this.userCommand.RetrieveUserSqlCommand(ID);
                        command.Connection = connection;
                        command.Transaction = transaction;

                        var reader = await command.ExecuteReaderAsync();
                        reader.Read();
                        result = reader.GetValue(0).ToString();
                        reader.Close();
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw new DataGatewayException(e);
            }
        }
    }
}
