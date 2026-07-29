using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudyDeck.DTO.StudySession;
using StudyDeck.Models;

namespace StudyDeck.Database;

public class StudingSession
{
    private readonly string _connectionString;
    public StudingSession()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found in appsettings.json.");
    }

    public void InsertStudySession(CreateStudySessionDto createStudySessionDto)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        string query = @"
            INSERT INTO StudySessions (StackId, StackName, Score)
            VALUES (@StackId, @StackName, @Score)";

        using var command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@StackId", createStudySessionDto.StackId);
        command.Parameters.AddWithValue("@StackName", createStudySessionDto.StackName);
        command.Parameters.AddWithValue("@Score", createStudySessionDto.Score);
        command.ExecuteNonQuery();
    }

    public List<StudySession> GetAllStudySessions()
    {
        var sessions = new List<StudySession>();
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand(
            @"SELECT 
            StudySessionId,
            StackId,
            StackName,
            SessionDate,
            Score
          FROM StudySessions
          ORDER BY SessionDate DESC",
            connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            sessions.Add(new StudySession
            {
                StudySessionId = reader.GetInt32(
                    reader.GetOrdinal("StudySessionId")),

                StackId = reader.GetInt32(
                    reader.GetOrdinal("StackId")),

                StackName = reader.GetString(
                    reader.GetOrdinal("StackName")),

                SessionDate = reader.GetDateTime(
                    reader.GetOrdinal("SessionDate")),

                Score = reader.GetDecimal(
                    reader.GetOrdinal("Score"))
            });
        }

        return sessions;
    }
}
