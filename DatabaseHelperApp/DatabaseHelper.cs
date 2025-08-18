using System;
using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace DatabaseHelperApp
{
    public class DatabaseHelper
    {
        private static readonly object _lock = new object();
        private static readonly HashSet<string> _registeredProviders = new HashSet<string>();

        private static void RegisterProvider(string providerName)
        {
            lock (_lock)
            {
                if (_registeredProviders.Contains(providerName))
                {
                    return;
                }

                switch (providerName)
                {
                    case "Microsoft.Data.Sqlite":
                        DbProviderFactories.RegisterFactory(providerName, SqliteFactory.Instance);
                        break;
                    case "System.Data.SqlClient":
                        DbProviderFactories.RegisterFactory(providerName, SqlClientFactory.Instance);
                        break;
                    case "MySql.Data.MySqlClient":
                        DbProviderFactories.RegisterFactory(providerName, MySqlClientFactory.Instance);
                        break;
                    case "Oracle.ManagedDataAccess.Client":
                        DbProviderFactories.RegisterFactory(providerName, OracleClientFactory.Instance);
                        break;
                }

                _registeredProviders.Add(providerName);
            }
        }

        private readonly DbProviderFactory _providerFactory;
        private readonly string _connectionString;

        public DatabaseHelper(string providerName, string connectionString)
        {
            RegisterProvider(providerName);

            try
            {
                _providerFactory = DbProviderFactories.GetFactory(providerName);
                _connectionString = connectionString;
            }
            catch (Exception ex)
            {
                // Handle provider not found exception
                throw new ArgumentException($"Could not get a factory for provider '{providerName}'. Please make sure the provider is registered correctly.", ex);
            }
        }

        public DbConnection CreateConnection()
        {
            var connection = _providerFactory.CreateConnection();
            if (connection == null)
            {
                throw new InvalidOperationException("The provider factory did not create a connection object.");
            }
            connection.ConnectionString = _connectionString;
            return connection;
        }

        public int ExecuteNonQuery(string commandText, CommandType commandType, params DbParameter[] parameters)
        {
            using (var connection = CreateConnection())
            {
                using (var command = _providerFactory.CreateCommand())
                {
                    if (command == null)
                    {
                        throw new InvalidOperationException("The provider factory did not create a command object.");
                    }
                    command.Connection = connection;
                    command.CommandText = commandText;
                    command.CommandType = commandType;
                    command.Parameters.AddRange(parameters);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public object? ExecuteScalar(string commandText, CommandType commandType, params DbParameter[] parameters)
        {
            using (var connection = CreateConnection())
            {
                using (var command = _providerFactory.CreateCommand())
                {
                    if (command == null)
                    {
                        throw new InvalidOperationException("The provider factory did not create a command object.");
                    }
                    command.Connection = connection;
                    command.CommandText = commandText;
                    command.CommandType = commandType;
                    command.Parameters.AddRange(parameters);

                    connection.Open();
                    return command.ExecuteScalar();
                }
            }
        }

        public DbDataReader ExecuteReader(string commandText, CommandType commandType, params DbParameter[] parameters)
        {
            var connection = CreateConnection();
            using (var command = _providerFactory.CreateCommand())
            {
                if (command == null)
                {
                    throw new InvalidOperationException("The provider factory did not create a command object.");
                }
                command.Connection = connection;
                command.CommandText = commandText;
                command.CommandType = commandType;
                command.Parameters.AddRange(parameters);

                connection.Open();
                // CommandBehavior.CloseConnection will close the connection when the reader is closed
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public DataTable GetDataTable(string commandText, CommandType commandType, params DbParameter[] parameters)
        {
            using (var connection = CreateConnection())
            {
                using (var command = _providerFactory.CreateCommand())
                {
                    if (command == null)
                    {
                        throw new InvalidOperationException("The provider factory did not create a command object.");
                    }
                    command.Connection = connection;
                    command.CommandText = commandText;
                    command.CommandType = commandType;
                    command.Parameters.AddRange(parameters);

                    using (var adapter = _providerFactory.CreateDataAdapter())
                    {
                        if (adapter == null)
                        {
                            throw new InvalidOperationException("The provider factory did not create a data adapter object.");
                        }
                        adapter.SelectCommand = command;
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        public DbParameter CreateParameter(string name, object value)
        {
            var parameter = _providerFactory.CreateParameter();
            if (parameter == null)
            {
                throw new InvalidOperationException("The provider factory did not create a parameter object.");
            }
            parameter.ParameterName = name;
            parameter.Value = value;
            return parameter;
        }
    }
}
