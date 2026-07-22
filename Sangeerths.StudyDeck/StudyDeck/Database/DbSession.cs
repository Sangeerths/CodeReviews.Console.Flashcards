using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudyDeck.Models;

namespace StudyDeck.Database
{
    public class DbSession
    {
        private readonly string _connectionString;

        public DbSession()
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

        public void UpdateStack(string oldName, string newName)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand("UPDATE Stacks SET StackName = @NewName WHERE StackName = @OldName", connection);
            command.Parameters.AddWithValue("@NewName", newName);
            command.Parameters.AddWithValue("@OldName", oldName);
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

        public void InsertFlashCard(int stackId, string question, string answer)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand(
                "INSERT INTO FlashCards (StackId, FlashcardFront, FlashcardBack) VALUES (@StackId, @Question, @Answer)",
                connection);
            command.Parameters.AddWithValue("@StackId", stackId);
            command.Parameters.AddWithValue("@Question", question);
            command.Parameters.AddWithValue("@Answer", answer);
            command.ExecuteNonQuery();
        }

        public void DeleteFlashCard(int flashcardId)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand("DELETE FROM FlashCards WHERE FlashcardId = @Id", connection);
            command.Parameters.AddWithValue("@Id", flashcardId);
            command.ExecuteNonQuery();
        }

        public void UpdateFlashCard(int flashcardId, string question, string answer)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand(
                "UPDATE FlashCards SET FlashcardFront = @Question, FlashcardBack = @Answer WHERE FlashcardId = @Id",
                connection);
            command.Parameters.AddWithValue("@Question", question);
            command.Parameters.AddWithValue("@Answer", answer);
            command.Parameters.AddWithValue("@Id", flashcardId);
            command.ExecuteNonQuery();
        }

        public int IsFlashCardExists(int flashcardId)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand(
                "SELECT COUNT(*) FROM FlashCards WHERE FlashcardId = @Id", connection);
            command.Parameters.AddWithValue("@Id", flashcardId);
            return (int)command.ExecuteScalar();
        }

        public List<FlashCard> GetFlashCardsByStack(int stackId)
        {
            var cards = new List<FlashCard>();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand(
                "SELECT FlashcardId, StackId, FlashcardFront, FlashcardBack FROM FlashCards WHERE StackId = @stackId",
                connection);
            command.Parameters.AddWithValue("@StackId", stackId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                cards.Add(new FlashCard
                {
                    Id = reader.GetInt32(0),
                    StackId = reader.GetInt32(1),
                    Question = reader.GetString(2),
                    Answer = reader.GetString(3)
                });
            }

            return cards;
        }
        public void InsertStudySession(int stackId, string stackName, double score)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string query = @"
            INSERT INTO StudySessions (StackId, StackName, Score)
            VALUES (@StackId, @StackName, @Score)";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@StackId", stackId);
            command.Parameters.AddWithValue("@StackName", stackName);
            command.Parameters.AddWithValue("@Score", score);
            command.ExecuteNonQuery();
        }

        
        public List<MonthlyStudyReport> MonthlyStudyReport(int year)
        {
            List<MonthlyStudyReport> reports = new List<MonthlyStudyReport>();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            string query = @"
        SELECT
            MONTH(SessionDate) AS Month,
            COUNT(*) AS TotalSessions,
            AVG(Score) AS AverageScore
        FROM StudySessions
        WHERE YEAR(SessionDate) = @Year
        GROUP BY MONTH(SessionDate)
        ORDER BY Month;";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Year", year);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                reports.Add(new MonthlyStudyReport
                {
                    Year = year,
                    Month = reader.GetInt32(0),
                    TotalSession = reader.GetInt32(1),
                    AverageScore = Convert.ToDouble(reader.GetDecimal(2))
                });
            }

            return reports;
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
}