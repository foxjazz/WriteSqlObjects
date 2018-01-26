using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using System.IO;
using System.Net.Sockets;

namespace WriteSqlObjects
{
    public class SqlWriter
    {
        private string connectionString;
        private const string basepath = @"c:\db\";
        private string Serverstr;
        private string Database;
        private string subPath;
        private HashSet<string> schemas;
        public SqlWriter(string env, string database)
        {
            var dbconn = new DBConnectionHot(env);
            if (database == "service")
            {
                connectionString = dbconn.serviceConnstr;
                System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);

                Serverstr = builder.DataSource;
                Database = builder.InitialCatalog;
                
                subPath = basepath + Serverstr + "\\" + Database; // your code goes here
            }

            if (database.ToLower() == "importdata")
            {
                connectionString = dbconn.ImportDataConnstr;
                System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);

                Serverstr = builder.DataSource;
                Database = builder.InitialCatalog;

                subPath = basepath + Serverstr + "\\" + Database; // your code goes here
            }

            if (database.ToLower() == "statebridge")
            {
                connectionString = dbconn.StatebridgeConnstr;
                System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);

                Serverstr = builder.DataSource;
                Database = builder.InitialCatalog;

                subPath = basepath + Serverstr + "\\" + Database; // your code goes here
            }


        }

        private void addPath(string path)
        {
            if (!Directory.Exists(subPath + "\\" + path))
                Directory.CreateDirectory(subPath + "\\" + path);
        }
        public IDbConnection Connection => new SqlConnection(connectionString);

        public void WriteSql(string type)
        {
            string foldertype;
            string paramtype;
            switch (type)
            {
                case "procs":
                    paramtype = "procedures";
                    foldertype = "Stored Procedures";
                    break;
                case "tables":
                    paramtype = "tables";
                    foldertype = "tables";
                    break;
                case "views":
                    paramtype = "views";
                    foldertype = "views";
                    break;
                default:
                    return;
            }


            schemas = new HashSet<string>();
            var procNames = new List<Procedure>();
            var defs = new List<Definition>();
            string sql =
                $@"SELECT SchemaName = s.name, objectName = pr.name , pr.object_id FROM  sys.{paramtype} pr 
                INNER JOIN sys.schemas s ON pr.schema_id = s.schema_id where s.name <> 'dbo'";
            string sql2 = $@"SELECT SchemaName = s.name, objectName = pr.name , pr.object_id FROM  sys.{paramtype} pr 
                INNER JOIN sys.schemas s ON pr.schema_id = s.schema_id ";
            if(Connection.Database != "Service")
                sql = sql2;
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                procNames = Connection.Query<Procedure>(sql).ToList();   


            }
            sql = "select definition from sys.sql_modules where object_id = @id";
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                foreach (var pname in procNames)
                {
                    if (schemas.Add(pname.schemaname + "\\" + foldertype))
                        addPath(pname.schemaname + "\\" + foldertype);
                    defs = Connection.Query<Definition>(sql, new {id = pname.object_id}).ToList();
                    string def = defs[0].definition;
                    File.WriteAllText(subPath + @"\" + pname.schemaname + @"\" + foldertype + @"\"  + pname.objectName + ".sql", def);
                }
            }


        }

       


    }
}
