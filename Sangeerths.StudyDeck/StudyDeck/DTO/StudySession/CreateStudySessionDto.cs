namespace StudyDeck.DTO.StudySession;

public class CreateStudySessionDto
{
    public int StackId { get; set; }
    public string StackName { get; set; } = string.Empty;
    public double Score { get; set; }
}
