using Spectre.Console;
using StudyDeck.Database;
using StudyDeck.Validation;
using System.Globalization;
namespace StudyDeck.Services;

    public class ReportService
    {
        private readonly InputValidator _validator;
        private readonly ReportSession _reportSession;
        private readonly ConsoleUI _consoleUI;

        public ReportService()
        {
            _validator = new InputValidator();
        _reportSession = new ReportSession();
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

            var reports = _reportSession.GetStudySessionsPivotReport(year);
            var table = new Table()
       .Border(TableBorder.Rounded)
       .Title($"[yellow]Study Sessions Per Stack - {year}[/]");

            table.AddColumn("Stack");
            table.AddColumn("Jan");
            table.AddColumn("Feb");
            table.AddColumn("Mar");
            table.AddColumn("Apr");
            table.AddColumn("May");
            table.AddColumn("Jun");
            table.AddColumn("Jul");
            table.AddColumn("Aug");
            table.AddColumn("Sep");
            table.AddColumn("Oct");
            table.AddColumn("Nov");
            table.AddColumn("Dec");

            foreach (var report in reports)
            {
                table.AddRow(
                    report.StackName,
                    report.Jan.ToString(),
                    report.Feb.ToString(),
                    report.Mar.ToString(),
                    report.Apr.ToString(),
                    report.May.ToString(),
                    report.Jun.ToString(),
                    report.Jul.ToString(),
                    report.Aug.ToString(),
                    report.Sep.ToString(),
                    report.Oct.ToString(),
                    report.Nov.ToString(),
                    report.Dec.ToString());
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

            var reports = _reportSession.MonthlyStudyReport(year);
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


