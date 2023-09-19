using MySql.Data.MySqlClient;

namespace test_project.Services;

public class DatabaseService
{
    private readonly IConfiguration _configuration;

    public DatabaseService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public MySqlConnection GetMySqlConnection()
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        using MySqlConnection connection = new MySqlConnection(connectionString);

        return connection;
    }

    public string getConfigurationConnectionString()
    {
        return _configuration.GetConnectionString("DefaultConnection");
    }
} 