using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudyDeck.DTO.Stack;
using StudyDeck.Models;

namespace StudyDeck.Database;

internal class StackSession
{
    private readonly string _connectionString;
    public StackSession()
    {
        var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found in appsettings.json.");
    }

    public void InsertStack(string stackName)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand("INSERT INTO Stacks (StackName) VALUES (@stackName)", connection);
        command.Parameters.AddWithValue("@stackName", stackName);
        command.ExecuteNonQuery();
    }

    public void DeleteStack(string stackName)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand("DELETE FROM Stacks WHERE StackName = @Name", connection);
        command.Parameters.AddWithValue("@Name", stackName);
        command.ExecuteNonQuery();
    }

    public void UpdateStack(UpdateStackDto updateStack)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand("UPDATE Stacks SET StackName = @NewName WHERE StackName = @OldName", connection);
        command.Parameters.AddWithValue("@NewName", updateStack.NewName);
        command.Parameters.AddWithValue("@OldName", updateStack.OldName);
        command.ExecuteNonQuery();
    }

    public int IsStackExists(string stackName)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand("SELECT COUNT(*) FROM Stacks WHERE StackName = @stackName", connection);
        command.Parameters.AddWithValue("@stackName", stackName);
        return (int)command.ExecuteScalar();
    }

    public int GetStackId(string stackName)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand("SELECT StackId FROM Stacks WHERE StackName = @Name", connection);
        command.Parameters.AddWithValue("@Name", stackName);
        var result = command.ExecuteScalar();
        return result == null ? -1 : (int)result;
    }

    public List<StackCard> GetAllStacks()
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand("SELECT * FROM Stacks ORDER BY StackName", connection);
        using var reader = command.ExecuteReader();
        var stacks = new List<StackCard>();
        while (reader.Read())
            stacks.Add(new StackCard
            {
                StackId = reader.GetInt32(0),
                StackName = reader.GetString(1)
            });

        return stacks;
    }
}
