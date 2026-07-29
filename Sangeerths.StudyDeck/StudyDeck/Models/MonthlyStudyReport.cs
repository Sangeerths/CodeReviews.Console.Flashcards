namespace StudyDeck.Models;

public class MonthlyStudyReport
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int TotalSession { get; set; }
    public double AverageScore { get; set; }
}
