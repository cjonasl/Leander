using System;
using System.IO;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using System.Collections;
using Leander.Nr1;

namespace CreateTriggers
{
    class Program
    {
        private static string _insertStatementIntoJonasLeander;

        static void Main(string[] args)
        {
            string connectionString, fileNameFullPathTablesToExclude, errorMessage;
            string[] excludeTables = null;
            int i, index, maxNumberOfColumns;
            object result;

            try
            {
                if (args.Length != 1 && args.Length != 2)
                {
                    Console.WriteLine("There should be one or two parameters to the program!");
                    Console.ReadKey();
                    return;
                }

                connectionString = args[0];

                if (args.Length == 2)
                {
                    fileNameFullPathTablesToExclude = args[1];

                    if (!File.Exists(fileNameFullPathTablesToExclude))
                    {
                        Console.WriteLine(string.Format("The file \"{0}\" does not exist!", fileNameFullPathTablesToExclude));
                        Console.ReadKey();
                        return;
                    }

                    excludeTables = Utility.ReturnRowsInFile(fileNameFullPathTablesToExclude, out errorMessage);

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        Console.WriteLine(string.Format("An error happened when \"Utility.ReturnRowsInFile\" was called: {0}", fileNameFullPathTablesToExclude));
                        Console.ReadKey();
                        return;
                    }
                }

                if (!ExecuteScalar(connectionString, "SELECT COUNT(*) FROM sys.tables WHERE name = 'JonasLeander'", out result, out errorMessage))
                {
                    Console.WriteLine("The given connection string does not work!");
                    Console.ReadKey();
                    return;
                }

                if ((int)result == 0)
                {
                    ArrayList tables, columns;

                    if (!DropAllCarlJonasTriggers(connectionString, out errorMessage)) //Should not exist any CarlJonas-triggers, but run this method anyway (if an error has happened before)
                    {
                        Console.WriteLine(errorMessage);
                        Console.ReadKey();
                        return;
                    }

                    if (!FillArrayListsTablesAndColumns(connectionString, out tables, out columns, out errorMessage))
                    {
                        Console.WriteLine(errorMessage);
                        Console.ReadKey();
                        return;
                    }

                    if (excludeTables != null)
                    {
                        i = 0;

                        while (string.IsNullOrEmpty(errorMessage) && i < excludeTables.Length)
                        {
                            index = tables.IndexOf(excludeTables[i]);

                            if (index == -1)
                                errorMessage = string.Format("The following table should be excluded, but it does not exist in the database (obs you need to give both schema name and table name, dot separated): {0}", excludeTables[i]);
                            else
                            {
                                tables.RemoveAt(index);
                                columns.RemoveAt(index);
                            }
                                
                            i++;
                        }

                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            Console.WriteLine(errorMessage);
                            Console.ReadKey();
                            return;
                        }
                    }

                    maxNumberOfColumns = 0;

                    for (i = 0; i < columns.Count; i++)
                    {
                        if (((ArrayList)columns[i]).Count > maxNumberOfColumns)
                            maxNumberOfColumns = ((ArrayList)columns[i]).Count;
                    }

                    if (!CreateTableJonasLeander(connectionString, maxNumberOfColumns, out errorMessage))
                    {
                        Console.WriteLine(errorMessage);
                        Console.ReadKey();
                        return;
                    }

                    if (!CreateTriggers(connectionString, tables, columns, maxNumberOfColumns, out errorMessage))
                    {
                        Console.WriteLine(errorMessage);
                        Console.ReadKey();
                        return;
                    }

                    Console.WriteLine("Triggers was created successfully!");
                    Console.ReadKey();
                }
                else
                {
                    if (!ExecuteNonQuery(connectionString, "DROP TABLE JonasLeander", out errorMessage))
                    {
                        Console.WriteLine(errorMessage);
                        Console.ReadKey();
                        return;
                    }

                    if (!DropAllCarlJonasTriggers(connectionString, out errorMessage))
                    {
                        Console.WriteLine(errorMessage);
                        Console.ReadKey();
                        return;
                    }

                    Console.WriteLine("Triggers was deleted successfully!");
                    Console.ReadKey();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("An exception happened! e.Message = {0}", e.Message));
                Console.ReadKey();
                return;
            }
        }

        private static bool ExecuteNonQuery(string connectionString, string cmdText, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();

                return true;
            }
            catch (Exception e)
            {
                errorMessage = string.Format("An error happened in method \"ExecuteNonQuery\" when running the following query: {0}. e.Message = {1}", cmdText, e.Message);
                return false;
            }
        }

        private static bool ExecuteScalar(string connectionString, string cmdText, out object result, out string errorMessage)
        {
            errorMessage = null;
            result = null;

            try
            {
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
                sqlConnection.Open();
                result = sqlCommand.ExecuteScalar();
                sqlConnection.Close();

                return true;
            }
            catch (Exception e)
            {
                errorMessage = string.Format("An error happened in method \"ExecuteScalar\" when running the following query: {0}. e.Message = {1}", cmdText, e.Message);
                return false;
            }
        }

        private static DataTable ReturnDataTable(string connectionString, string cmdText, out string errorMessage)
        {
            errorMessage = null;
            DataTable dataTable = null;

            try
            {
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
                dataTable = new DataTable();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dataTable);
            }
            catch (Exception e)
            {
                errorMessage = string.Format("An error happened in method \"ReturnDataTable\" when running the following query: {0}. e.Message = {1}", cmdText, e.Message);
                return null;
            }

            return dataTable;
        }

        private static bool DropAllCarlJonasTriggers(string connectionString, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                string cmdText = "SELECT name FROM sys.objects WHERE type = 'TR' AND name LIKE 'trCarlJonas%'";
                DataTable dataTable = ReturnDataTable(connectionString, cmdText, out errorMessage);

                if (!string.IsNullOrEmpty(errorMessage))
                    return false;

                if (dataTable.Rows.Count > 0)
                {
                    Console.WriteLine(string.Format("Drop {0} triggers", dataTable.Rows.Count.ToString()));

                    int n = 0;
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        if (!ExecuteNonQuery(connectionString, string.Format("DROP TRIGGER {0}", dataTable.Rows[i][0].ToString()), out errorMessage))
                        {
                            Console.WriteLine(errorMessage);
                            return false;
                        }                      

                        n++;
                        Console.Write("\r" + n.ToString());
                    }

                    Console.WriteLine();
                }           
            }
            catch (Exception e)
            {
                errorMessage = string.Format("An error happened in method \"DropAllCarlJonasTriggers\". e.Message = ", e.Message);
                return false;
            }

            return true;
        }

        private static ArrayList ReturnColumns(string connectionString, string object_id, out string errorMessage)
        {
            ArrayList arrayList = null;

            try
            {
                string cmdText = string.Format("SELECT name FROM sys.columns WHERE object_id = {0} ORDER BY column_id", object_id);
                DataTable dataTable = ReturnDataTable(connectionString, cmdText, out errorMessage);

                if (!string.IsNullOrEmpty(errorMessage))
                    return null;

                arrayList = new ArrayList();

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    arrayList.Add(dataTable.Rows[i][0].ToString());
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("An error happened in method \"ReturnColumns\". e.Message = ", e.Message);
                return null;
            }

            return arrayList;
        }

        private static bool FillArrayListsTablesAndColumns(string connectionString, out ArrayList tables, out ArrayList columns, out string errorMessage)
        {
            string cmdText, tableName;
            DataTable dataTable;
            int i;

            tables = null;
            columns = null;
            errorMessage = null;

            try
            {
                cmdText = "SELECT sch.name AS 'SchemaName', obj.name AS 'TableName', obj.object_id FROM sys.objects obj INNER JOIN sys.schemas sch ON obj.schema_id = sch.schema_id WHERE obj.type = 'U'";
                dataTable = ReturnDataTable(connectionString, cmdText, out errorMessage);

                if (!string.IsNullOrEmpty(errorMessage))
                    return false;

                tables = new ArrayList();
                columns = new ArrayList();

                i = 0;
                while (i < dataTable.Rows.Count && string.IsNullOrEmpty(errorMessage))
                {
                    tableName = string.Format("[{0}].[{1}]", dataTable.Rows[i][0].ToString(), dataTable.Rows[i][1].ToString());
                    tables.Add(tableName);
                    columns.Add(ReturnColumns(connectionString, dataTable.Rows[i][2].ToString(), out errorMessage));
                    i++;
                }
            }
            catch (Exception e)
            {
                errorMessage = string.Format("An error happened in method \"FillArrayListsTablesAndColumns\". e.Message = {0}", e.Message);
                return false;
            }

            if (!string.IsNullOrEmpty(errorMessage))
                return false;
            else
                return true;
        }

        private static bool CreateTableJonasLeander(string connectionString, int maxNumberOfColumns, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                StringBuilder sb = new StringBuilder("CREATE TABLE JonasLeander(ID int identity(1, 1) NOT NULL,\r\n [DateTime] datetime NOT NULL,\r\n [TableName] varchar(500) NOT NULL,\r\n [Type] varchar(25) NOT NULL,\r\n");
                StringBuilder sbInsertStatementIntoJonasLeander = new StringBuilder("INSERT INTO JonasLeander([DateTime], [TableName], [Type], ");

                for (int i = 1; i <= maxNumberOfColumns; i++)
                {
                    if (i == maxNumberOfColumns)
                    {
                        sb.Append(string.Format("c{0} varchar(max) NULL)", i.ToString()));
                        sbInsertStatementIntoJonasLeander.Append(string.Format("c{0})", i.ToString()));
                    }
                    else
                    {
                        sb.Append(string.Format("c{0} varchar(max) NULL,\r\n", i.ToString()));
                        sbInsertStatementIntoJonasLeander.Append(string.Format("c{0}, ", i.ToString()));
                    }
                }

                _insertStatementIntoJonasLeander = sbInsertStatementIntoJonasLeander.ToString();

                string cmdText = sb.ToString();

                if (!ExecuteNonQuery(connectionString, cmdText, out errorMessage))
                    return false;
            }
            catch (Exception e)
            {
                errorMessage = string.Format("An error happened in method \"CreateTableJonasLeander\". e.Message = ", e.Message);
                return false;
            }

            return true;
        }

        private static string ReturnSelectExpression(string tableName, ArrayList columns, string type, int index, int maxNumberOfColumns)
        {
            StringBuilder sb = new StringBuilder(string.Format("SELECT getdate(), '{0}', '{1}', ", tableName.Substring(7, tableName.Length - 8), type));

            ArrayList arrayList = (ArrayList)columns[index];

            for (int i = 0; i < arrayList.Count; i++)
            {
                if (i == (arrayList.Count - 1))
                {
                    sb.Append(string.Format("CAST({0} AS varchar(max))", arrayList[i]));

                    if (arrayList.Count < maxNumberOfColumns)
                    {
                        sb.Append(", ");

                        int n = maxNumberOfColumns - arrayList.Count;

                        for (int j = 0; j < n; j++)
                        {
                            if (j == (n - 1))
                            {
                                sb.Append("'No column'");
                            }
                            else
                            {
                                sb.Append("'No column', ");
                            }
                        }
                    }
                }
                else
                {
                    sb.Append(string.Format("CAST({0} AS varchar(max)), ", arrayList[i]));
                }
            }

            return sb.ToString();
        }

        private static bool CreateTriggers(string connectionString, ArrayList tables, ArrayList columns, int maxNumberOfColumns, out string errorMessage)
        {
            errorMessage = null;

            try
            {
                string selectExpression, cmdText;

                Console.WriteLine(string.Format("Create triggers for {0} tables", tables.Count.ToString()));

                int i;
                i = 1;
                while (i <= tables.Count && string.IsNullOrEmpty(errorMessage))
                {
                    selectExpression = ReturnSelectExpression((string)tables[i - 1], columns, "INSERT", i - 1, maxNumberOfColumns) + " FROM inserted";
                    cmdText = string.Format("CREATE TRIGGER trCarlJonasInsert{0} ON {1} AFTER INSERT AS BEGIN SET NOCOUNT ON {2} {3} END", i.ToString(), (string)tables[i - 1], _insertStatementIntoJonasLeander, selectExpression);
                    ExecuteNonQuery(connectionString, cmdText, out errorMessage);

                    selectExpression = ReturnSelectExpression((string)tables[i - 1], columns, "DELETE", i - 1, maxNumberOfColumns) + " FROM deleted";
                    cmdText = string.Format("CREATE TRIGGER trCarlJonasDelete{0} ON {1} AFTER DELETE AS BEGIN SET NOCOUNT ON {2} {3} END", i.ToString(), (string)tables[i - 1], _insertStatementIntoJonasLeander, selectExpression);
                    ExecuteNonQuery(connectionString, cmdText, out errorMessage);

                    selectExpression = ReturnSelectExpression((string)tables[i - 1], columns, "UPDATE_DELETE", i - 1, maxNumberOfColumns);
                    cmdText = string.Format("CREATE TRIGGER trCarlJonasUpdate{0} ON {1} AFTER UPDATE AS BEGIN SET NOCOUNT ON {2} {3} {4} {5} END", i.ToString(), (string)tables[i - 1], _insertStatementIntoJonasLeander, selectExpression + " FROM deleted ", _insertStatementIntoJonasLeander, selectExpression.Replace("UPDATE_DELETE", "UPDATE_INSERT") + " FROM inserted ");
                    ExecuteNonQuery(connectionString, cmdText, out errorMessage);

                    Console.Write("\r" + i.ToString());
                    i++;
                }

                Console.WriteLine();
            }
            catch (Exception e)
            {
                errorMessage = string.Format("An error happened in method \"CreateTriggers\". e.Message = {0}", e.Message);
                return false;
            }

            if (string.IsNullOrEmpty(errorMessage))
                return true;
            else
                return false;
        }
    }
}
