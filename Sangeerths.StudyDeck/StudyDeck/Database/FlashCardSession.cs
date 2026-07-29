using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudyDeck.DTO.FlashCard;
using StudyDeck.Models;

namespace StudyDeck.Database;

public class FlashCardSession
{
    private readonly string _connectionString;
    public FlashCardSession()
    {

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found in appsettings.json.");
    }

    public void InsertFlashCard(CreateFlashCardDto createFlashCardDto)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand(
            "INSERT INTO FlashCards (StackId, FlashcardFront, FlashcardBack) VALUES (@StackId, @Question, @Answer)",
            connection);
        command.Parameters.AddWithValue("@StackId", createFlashCardDto.StackId);
        command.Parameters.AddWithValue("@Question", createFlashCardDto.Question);
        command.Parameters.AddWithValue("@Answer", createFlashCardDto.Answer);
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

    public void UpdateFlashCard(UpdateFlashCardDto updateFlashCardDto)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand(
            "UPDATE FlashCards SET FlashcardFront = @Question, FlashcardBack = @Answer WHERE FlashcardId = @Id",
            connection);
        command.Parameters.AddWithValue("@Question", updateFlashCardDto.Question);
        command.Parameters.AddWithValue("@Answer", updateFlashCardDto.Answer);
        command.Parameters.AddWithValue("@Id", updateFlashCardDto.FlashCardId);
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
}
