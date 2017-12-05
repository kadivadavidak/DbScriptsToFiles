using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace DbScriptsToGit
{
    class DataHandler
    {
        // Get DB connection information.
        internal static SqlConnection Connect()
        {
            var dbUserName = ConfigurationManager.AppSettings["ServerUserName"];
            var dbPassword = ConfigurationManager.AppSettings["ServerPassword"];
            var dbName = ConfigurationManager.AppSettings["DatabaseName"];
            var serverName = ConfigurationManager.AppSettings["ServerName"];

            string connetionString = $"Data Source={serverName};Initial Catalog={dbName};User ID={dbUserName};Password={dbPassword}";

            return new SqlConnection(connetionString);
        }

        // Get a list of the modules for the specified database
        internal static DataTable ModulesToDataSet(string databaseName)
        {
            try
            {
                var dt = new DataTable();
                var sql = $"SELECT M.[definition], '{databaseName}_' + OBJECT_SCHEMA_NAME(o.object_id, (SELECT database_id FROM sys.databases D WHERE D.NAME = '{databaseName}')) + '_' + O.[type] COLLATE DATABASE_DEFAULT +  '_' + O.[name] COLLATE DATABASE_DEFAULT [Name] FROM {databaseName}.sys.sql_modules M INNER JOIN {databaseName}.sys.objects O on O.object_id = M.object_id WHERE [type] NOT IN ('PK', 'UQ', 'FK', 'F')";
                var connection = Connect();
                var command = new SqlCommand(sql, connection);

                connection.Open();

                // Execute command
                var adpter = new SqlDataAdapter(command);

                // Populate data table with data
                adpter.Fill(dt);

                connection.Close();
                adpter.Dispose();

                return dt;
            }
            catch (Exception e)
            {
                Console.WriteLine("There was a problem geting module scripts from database.\n\nERROR: " + e);
                throw;
            }
        }

        // Get list of databases
        internal static List<string> DatabaseNamesToList()
        {
            try
            {
                var dt = new DataTable();
                // TechBrands, TechBrandsDW, SRO_SimplyMac, TARDIS
                var sql = "SELECT [name] FROM TechBrands.sys.databases WHERE database_id in (6, 15, 16, 19)";
                var connection = Connect();
                var command = new SqlCommand(sql, connection);
                var databaseNames = new List<string>();

                connection.Open();

                var adpter = new SqlDataAdapter(command);
                adpter.Fill(dt);

                //Get module definitions
                databaseNames = dt.AsEnumerable().Select(x => x[0].ToString()).ToList();

                connection.Close();
                adpter.Dispose();

                return databaseNames;
            }
            catch (Exception e)
            {
                Console.WriteLine("There was a problem geting module scripts from database.\n\nERROR: " + e);
                throw;
            }
        }
    }
}
