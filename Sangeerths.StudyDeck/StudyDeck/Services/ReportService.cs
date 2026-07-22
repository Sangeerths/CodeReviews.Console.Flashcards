using Spectre.Console;
using StudyDeck.Database;
using StudyDeck.Validation;
using System.Globalization;

namespace StudyDeck.Services
{
    public class ReportService
    {
        private readonly InputValidator _validator;
        private readonly DbSession _session;
        private readonly ConsoleUI _consoleUI;

        public ReportService()
        {
            _validator = new InputValidator();
            _session = new DbSession();
            _consoleUI = new ConsoleUI();
        }

        public void TotalSessionPerMonth()
        {
            int year = _validator.ValidateYear();
            if(year == -1)
            {
                AnsiConsole.MarkupLine("[bold red] Entered Year is wrong[/]");
                _consoleUI.Pause();
                return;
            }

            var reports = _session.MonthlyStudyReport(year);
            Table table = new Table().Border(TableBorder.Rounded).Title($"[yellow]Study Report - {year}[/]");
            table.AddColumn("[cyan]Month[/]");
            table.AddColumn(new TableColumn("[green]Sessions[/]").Centered());
            foreach (var report in reports)
            {
                table.AddRow(
                    CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(report.Month),
                    report.TotalSession.ToString());    
            }

            AnsiConsole.Write(table);
            _consoleUI.Pause();
        }

        public void AverageScorePerMonth()
        {
            int year = _validator.ValidateYear();
            if (year == -1)
            {
                _consoleUI.Pause();
                return;
            }

            var reports = _session.MonthlyStudyReport(year);
            Table table = new Table().Border(TableBorder.Rounded).Title($"[yellow]Average Session Score Report - {year}[/]");
            table.AddColumn("[cyan]Month[/]");
            table.AddColumn(new TableColumn("[green]Average Score[/]").Centered());
            foreach (var report in reports)
            {
                table.AddRow(
                    CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(report.Month),
                    $"{report.AverageScore:F1}%");
            }

            AnsiConsole.Write(table);
            _consoleUI.Pause();
        }
    }
}

