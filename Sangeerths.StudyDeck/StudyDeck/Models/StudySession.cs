namespace StudyDeck.Models;

public class StudySession
{
    public int StudySessionId { get; set; }
    public int StackId { get; set; }
    public string StackName { get; set; } = string.Empty;
    public DateTime SessionDate { get; set; }
    public decimal Score { get; set; }
}
