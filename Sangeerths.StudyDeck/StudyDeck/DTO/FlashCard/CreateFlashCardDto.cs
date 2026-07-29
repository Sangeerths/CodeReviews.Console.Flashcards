namespace StudyDeck.DTO.FlashCard;

public class CreateFlashCardDto
{
    public int StackId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}
