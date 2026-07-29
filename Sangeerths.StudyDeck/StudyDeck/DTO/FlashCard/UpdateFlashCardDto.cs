namespace StudyDeck.DTO.FlashCard;

public class UpdateFlashCardDto
{
    public int FlashCardId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}
