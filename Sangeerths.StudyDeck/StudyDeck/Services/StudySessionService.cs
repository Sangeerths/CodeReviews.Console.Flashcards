using Spectre.Console;
using StudyDeck.Database;
using StudyDeck.Models;
using StudyDeck.Validation;

namespace StudyDeck.Services
{
    public class StudySessionService
    {
        private readonly InputValidator _validator;
        private readonly DbSession _session;
        private readonly ConsoleUI _consoleUI;
        public StudySessionService()
        {
            _validator = new InputValidator();
            _session = new DbSession();
            _consoleUI = new ConsoleUI();
            
        }

        public void StudySession()
        {
            while (true)
            {
                _consoleUI.ShowHeader();
                var stacks = _session.GetAllStacks();
                AnsiConsole.MarkupLine("[yellow]Search stack (leave blank to show all):[/]");
                string search = Console.ReadLine() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(search))
                {
                    stacks = stacks
                        .Where(s => s.StackName.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if (!stacks.Any())
                {
                    AnsiConsole.MarkupLine("[red]No stacks found.[/]");
                    _consoleUI.Pause();
                    return;
                }

                var selectedStack = AnsiConsole.Prompt(
                    new SelectionPrompt<StackCard>()
                        .Title("Choose a stack")
                        .PageSize(10)
                        .MoreChoicesText("[grey](Use ↑ ↓ to scroll)[/]")
                        .UseConverter(s => s.StackName)
                        .AddChoices(stacks));
                bool studyCurrentStack = true;

                while (studyCurrentStack)
                {
                    _consoleUI.ShowHeader();
                    RunQuiz(selectedStack);
                    string choice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("What would you like to do?")
                            .AddChoices(
                                "Study Again",
                                "Choose Another Stack",
                                "Exit"));

                    switch (choice)
                    {
                        case "Study Again":
                            break;

                        case "Choose Another Stack":
                            studyCurrentStack = false;
                            break;

                        case "Exit":
                            return;
                    }
                }
            }
        }

        public double RunQuiz(StackCard card)
        {
            
            List<FlashCard> flashCards = _session.GetFlashCardsByStack(card.StackId);
            int correctCount = 0;
            int totalQuestions = flashCards.Count;
            if (!flashCards.Any()){
                return -1;
            }
            for(int i=0;i<totalQuestions;i++)
            {
            FlashCard flashCard = flashCards[i];
                AnsiConsole.Clear();
                _consoleUI.ShowHeader();
                AnsiConsole.Write(new Rule($"Question {i + 1} / {totalQuestions}").RuleStyle("yellow"));
                AnsiConsole.MarkupLine($"\n[cyan]{flashCard.Question}[/]\n");
                string userAnswer = AnsiConsole.Ask<string>("[green]Your Answer:[/]");
                bool isCorrect = userAnswer.Trim().Equals(flashCard.Answer.Trim(), StringComparison.OrdinalIgnoreCase);

                if (isCorrect)
                {
                    correctCount++;
                    AnsiConsole.MarkupLine("\n[green]✓ Correct![/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("\n[red]✗ Incorrect[/]");
                    AnsiConsole.MarkupLine($"[yellow]Correct Answer:[/] {flashCard.Answer}");
                }

                _consoleUI.Pause();
                AnsiConsole.Clear();
                _consoleUI.ShowHeader();
            }
            
            double score = (double)correctCount / totalQuestions * 100;
            AnsiConsole.Write(
                new Rule("[green]Study Session Complete[/]")
                    .RuleStyle("green"));
            AnsiConsole.MarkupLine($"[cyan]Total Questions:[/] {totalQuestions}");
            AnsiConsole.MarkupLine($"[green]Correct:[/] {correctCount}");
            AnsiConsole.MarkupLine($"[red]Wrong:[/] {totalQuestions - correctCount}");
            AnsiConsole.MarkupLine($"[yellow]Score:[/] {score:F1}%");
            string choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Save this study session?")
                    .AddChoices("Yes", "No"));

            if (choice == "Yes")
            {
                _session.InsertStudySession(card.StackId,card.StackName, score);
            }
            _consoleUI.Pause();
            AnsiConsole.Clear();
            _consoleUI.ShowHeader();
            return score;
        }

        public void ViewAllStudySession()
        {
            List<StudySession> sessions = _session.GetAllStudySessions();
            if (sessions.Count == 0)
            {
                AnsiConsole.MarkupLine(
                    "[yellow]No study sessions found.[/]");

                _consoleUI.Pause();
                return;
            }

            var table = new Table();

            table.AddColumn("Session Id");
            table.AddColumn("Stack Name");
            table.AddColumn("Session Date");
            table.AddColumn("Score");

            foreach (var session in sessions)
            {
                table.AddRow(
                    session.StudySessionId.ToString(),
                    session.StackName,
                    session.SessionDate.ToString("dd-MM-yyyy HH:mm"),
                    $"{session.Score:F1}%");
            }

            AnsiConsole.Write(table);

            _consoleUI.Pause();
        }
    }
}
