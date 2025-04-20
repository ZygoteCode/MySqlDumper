using MySql.Data.MySqlClient;
using System.Data;
using System.Text;

namespace MySqlDumper
{
    public class SqlDumper
    {
        private string _ip, _user, _password, _database, _port;
        private int _timeout = 99999999;

        public SqlDumper(string ip, string database, string user = "", string password = "", string port = "3306")
        {
            _ip = ip;
            _database = database;
            _user = user;
            _password = password;
            _port = port;
        }

        public void DumpDatabase(string filePath)
        {
            using (MySqlConnection conn = new MySqlConnection(
                $"server={_ip};user={_user};password={_password};database={_database};port={_port};" +
                $"Connection Timeout={_timeout};default command timeout={_timeout}"
            ))
            {
                conn.Open();

                DataTable tables = conn.GetSchema("Tables");
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"CREATE DATABASE IF NOT EXISTS `{_database}`;");
                sb.AppendLine($"USE `{_database}`;");

                Parallel.ForEach(tables.AsEnumerable(), row =>
                {
                    string tableName = row["TABLE_NAME"].ToString();

                    using (MySqlConnection innerConn = new MySqlConnection(
                        $"server={_ip};user={_user};password={_password};database={_database};port={_port};" +
                        $"Connection Timeout={_timeout};default command timeout={_timeout}"
                    ))
                    {
                        innerConn.Open();

                        using (MySqlCommand cmd = new MySqlCommand($"SHOW CREATE TABLE `{tableName}`;", innerConn))
                        {
                            cmd.CommandTimeout = _timeout;

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string createTable = reader["Create Table"].ToString();

                                    lock (sb)
                                    {
                                        sb.AppendLine(createTable + ";");
                                    }
                                }
                            }
                        }

                        using (MySqlCommand cmd = new MySqlCommand($"SELECT * FROM `{tableName}`;", innerConn))
                        {
                            cmd.CommandTimeout = _timeout;

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                List<string> batchInserts = new List<string>();

                                while (reader.Read())
                                {
                                    StringBuilder insert = new StringBuilder($"INSERT INTO `{tableName}` VALUES(");

                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        if (i > 0)
                                        {
                                            insert.Append(",");
                                        }

                                        if (reader.IsDBNull(i))
                                        {
                                            insert.Append("NULL");
                                        }
                                        else
                                        {
                                            object value = reader.GetValue(i);

                                            if (value is string)
                                            {
                                                insert.Append($"'{((string)value).Replace("'", "''")}'");
                                            }
                                            else if (value is DateTime)
                                            {
                                                insert.Append($"'{((DateTime)value).ToString("yyyy-MM-dd")}'");
                                            }
                                            else if (value is decimal || value is float || value is double)
                                            {
                                                insert.Append(value.ToString().Replace(",", "."));
                                            }
                                            else
                                            {
                                                insert.Append(value.ToString());
                                            }
                                        }
                                    }

                                    insert.Append(");");
                                    batchInserts.Add(insert.ToString());

                                    if (batchInserts.Count >= 1000)
                                    {
                                        lock (sb)
                                        {
                                            sb.AppendLine(string.Join("\n", batchInserts));
                                        }

                                        batchInserts.Clear();
                                    }
                                }

                                if (batchInserts.Count > 0)
                                {
                                    lock (sb)
                                    {
                                        sb.AppendLine(string.Join("\n", batchInserts));
                                    }
                                }
                            }
                        }

                        lock (sb)
                        {
                            sb.AppendLine();
                        }
                    }
                });

                using (StreamWriter file = new StreamWriter(filePath))
                {
                    file.Write(sb.ToString());
                }
            }
        }
    }
}
