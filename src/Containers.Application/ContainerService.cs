using Containers.Models;
using Microsoft.Data.SqlClient;

namespace Containers.Application;

public class ContainerService : IContainerService
{
    private readonly String _connectionString;

    public ContainerService(string connectionString)
    {
        this._connectionString = connectionString;
    }
    public IEnumerable<Container> GetAllContainers()
    {
        List<Container> containers = [];
        
        string queryString = "SELECT * FROM Containers";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            
            connection.Open();
            
            //for SELECT queries
            SqlDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var container = new Container
                        {
                            ID = reader.GetInt32(0),
                            ContainerTypeId = reader.GetInt32(1),
                            IsHazardious = reader.GetBoolean(2),
                            Name = reader.GetString(3)
                        };
                        containers.Add(container);
                    }
                }
            }
            finally
            {
                reader.Close();
            }
        }
        
        return containers;
    }

    public bool Create(Container container)
    {
        const string insertString =
            "INSERT INTO Containers(ContainerTypeId, IsHazardious, Name) VALUES (@ContainerTypeId, @IsHazardious, @Name)";
        int countRowsAdd = -1;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(insertString, connection);
            command.Parameters.AddWithValue("@ContainerTypeId", container.ContainerTypeId);
            command.Parameters.AddWithValue("@IsHazardious", container.IsHazardious);
            command.Parameters.AddWithValue("@Name", container.Name);
            
            connection.Open();
            // "ExecuteNonQuery" => for update, insert, delete
            countRowsAdd = command.ExecuteNonQuery();
        }
        return countRowsAdd != -1;
    }
    
}