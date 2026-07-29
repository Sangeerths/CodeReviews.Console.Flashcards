using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudyDeck.DTO.StudySession;
using StudyDeck.Models;

namespace StudyDeck.Database;

public class ReportSession
{
    private readonly string _connectionString;
    public ReportSession()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found in appsettings.json.");
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



    public List<StudySessionPivotDto> GetStudySessionsPivotReport(int year)
    {
        List<StudySessionPivotDto> reports = new();

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        const string query = @"
        SELECT
            StackName,
            ISNULL([January], 0) AS Jan,
            ISNULL([February], 0) AS Feb,
            ISNULL([March], 0) AS Mar,
            ISNULL([April], 0) AS Apr,
            ISNULL([May], 0) AS May,
            ISNULL([June], 0) AS Jun,
            ISNULL([July], 0) AS Jul,
            ISNULL([August], 0) AS Aug,
            ISNULL([September], 0) AS Sep,
            ISNULL([October], 0) AS Oct,
            ISNULL([November], 0) AS Nov,
            ISNULL([December], 0) AS Dec
        FROM
        (
            SELECT
                StackName,
                DATENAME(MONTH, SessionDate) AS MonthName
            FROM StudySessions
            WHERE YEAR(SessionDate) = @Year
        ) AS SourceTable
        PIVOT
        (
            COUNT(MonthName)
            FOR MonthName IN
            (
                [January],
                [February],
                [March],
                [April],
                [May],
                [June],
                [July],
                [August],
                [September],
                [October],
                [November],
                [December]
            )
        ) AS PivotTable
        ORDER BY StackName;";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Year", year);

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            reports.Add(new StudySessionPivotDto
            {
                StackName = reader["StackName"].ToString()!,
                Jan = Convert.ToInt32(reader["Jan"]),
                Feb = Convert.ToInt32(reader["Feb"]),
                Mar = Convert.ToInt32(reader["Mar"]),
                Apr = Convert.ToInt32(reader["Apr"]),
                May = Convert.ToInt32(reader["May"]),
                Jun = Convert.ToInt32(reader["Jun"]),
                Jul = Convert.ToInt32(reader["Jul"]),
                Aug = Convert.ToInt32(reader["Aug"]),
                Sep = Convert.ToInt32(reader["Sep"]),
                Oct = Convert.ToInt32(reader["Oct"]),
                Nov = Convert.ToInt32(reader["Nov"]),
                Dec = Convert.ToInt32(reader["Dec"])
            });
        }

        return reports;
    }

}
