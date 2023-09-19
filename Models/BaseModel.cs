using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using test_project.Services;

namespace test_project.Models;

public class BaseModel
{

    protected MySqlConnection _connection;
    private readonly string _table = "todos"; 
    protected List<string> _accessibleFields;

    public BaseModel(IConfiguration configuration, string table, List<string> accessibleFields) 
    {
        var dbService = new DatabaseService(configuration);

        if (!(_connection is MySqlConnection) ) {
       
            _connection = dbService.GetMySqlConnection();
        }

        if (_connection is MySqlConnection && dbService.getConfigurationConnectionString() != _connection.ConnectionString) {
            _connection.Open();
        }

        _table = table;
        _accessibleFields = accessibleFields;
    }

    public List<Dictionary<String, dynamic>> Get(Dictionary<String, dynamic>? fields = null, bool withSoftDeleted = false)
    {
        string Query = $"SELECT * FROM {_table} ";

        if (fields != null && fields.Count > 0) {
            int index = 0;   

            foreach (KeyValuePair<string, dynamic> kvp in fields) {
                if (index == 0) {
                    Query += " WHERE ";
                }
                Query += " " + kvp.Key + "=" + kvp.Value;

                if (index > 0 && index < (fields.Count - 1)) {
                    Query += " AND ";
                }

                index++;
            }

            Query += " AND soft_deleted = false ";
        } else {
            Query += " WHERE soft_deleted = false ";
        }


        return ExecuteQuery(Query, "select");
    }

    public Dictionary<String, dynamic> Store(Dictionary<String, dynamic> fields)
    {
        string Query = $"INSERT into {_table} ";
        string[] columns = new string[fields.Count];
        string[] values = new string[fields.Count];

        int index = 0;

         if (fields != null && fields.Count > 0) {
            foreach (KeyValuePair<string, dynamic> kvp in fields) {
                columns[index] = kvp.Key;
                values[index] = "'" + kvp.Value.ToString() + "'";

                index++;
            }
        }

        Query += "(" + string.Join(",", columns) + ") VALUES " + "(" + string.Join(",", values) + ")";

        ExecuteQuery(Query);

        return GetLatest();
    }

    public void Update(Dictionary<String, dynamic> fields, Dictionary<String, dynamic>? conditions = null)
    {
        string Query = $"UPDATE {_table} SET ";
        string SetQuery = "";
        string ConditionQuery = "";
        
        int index1 = 0;
        foreach (KeyValuePair<string, dynamic> kvp in fields) {
            SetQuery += kvp.Key + "=" + kvp.Value + " ";

            if (fields.Count > 1 && index1 < (fields.Count - 1)) {
                SetQuery += ",";
            }

            index1++;
        }

        Query += SetQuery;

        if (conditions != null && conditions.Count > 0) {
            int index2 = 0;

            foreach (KeyValuePair<string, dynamic> kvp in conditions) {
                ConditionQuery += kvp.Key + "=" + kvp.Value;

                if (conditions.Count > 1 && index2 < (conditions.Count - 1)) {
                    ConditionQuery += " and ";
                }

                index2++;
            }

            Query += " WHERE " + ConditionQuery;
        }

        Console.WriteLine(Query);

        ExecuteQuery(Query);
    }

    public void Delete(Dictionary<String, dynamic> conditions)
    {
        string Query = $"DELETE from {_table} ";
        string ConditionQuery = "";
        int index = 0;

        foreach (KeyValuePair<string, dynamic> kvp in conditions) {
            ConditionQuery += kvp.Key + "=" + kvp.Value;

            if (index != (conditions.Count - 1)) {
                ConditionQuery += " and ";
            }

            index++;
        }

        Query += " WHERE " + ConditionQuery;

        ExecuteQuery(Query);
    }
    
    // SOft delete
    public void Archive(Dictionary<String, dynamic> conditions)
    {
        Dictionary<string, dynamic> fields = new Dictionary<string, dynamic>();
        fields["soft_deleted"] = true;

        Update(fields, conditions);
    }

    public List<Dictionary<String, dynamic>> Search(string field, dynamic value)
    {
        string Query = $"SELECT * FROM {_table} where {field} LIKE '%{value}%' and soft_deleted = false";

        return ExecuteQuery(Query, "select");
    }

    public Dictionary<string, dynamic> GetLatest()
    {
        string Query = $"SELECT * FROM {_table} order by id desc limit 1";

        return ExecuteQuery(Query, "select")[0];
    }

    protected List<Dictionary<String, dynamic>> ExecuteQuery(String query, String? queryType = "void")
    {
        List<Dictionary<String, dynamic>> rows = new List<Dictionary<String, dynamic>>();

        using MySqlCommand cmd = new MySqlCommand(query, _connection);
        using MySqlDataReader reader = cmd.ExecuteReader();

        if (queryType == "select") {
            List<string> returnedFields = new List<string>();

            if (reader.HasRows) {
                for (int i=0; i< reader.FieldCount; i++) {
                    returnedFields.Add(reader.GetName(i));
                }
            }

            while(reader.Read()) {
                Dictionary<string, dynamic> row = new Dictionary<string, dynamic>();

                foreach(string field in returnedFields) {
                    row[field] = reader[field];
                }

                rows.Add(row);

            }
        }

        reader.Close();

        return rows;
    }
}