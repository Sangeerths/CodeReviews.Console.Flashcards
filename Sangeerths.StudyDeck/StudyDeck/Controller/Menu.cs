using Spectre.Console;
using StudyDeck.Services;
using StudyDeck.Validation;

public class Menu
{
    private readonly FlashCardService _flashCardService;
    private readonly StackService _stackService;
    private readonly StudySessionService _studySessionService;
    private readonly ReportService _reportService;
    private readonly ConsoleUI _consoleUI;

    private enum MainMenuOption
    {
        ManageStacks,
        ManageFlashCards,
        StudySession,
        Report,
        Exit
    }

    private enum StackMenuOption
    {
        Insert,
        Delete,
        Update,
        ViewAll,
        GoBack
    }

    private enum FlashCardMenuOption
    {
        Insert,
        Delete,
        Update,
        ViewAll,
        GoBack
    }

    private enum StudySessionOption
    {
        StartSession,
        ViewAllSession,
        GoBack
    }
    private enum ReportOption
    {
        TotalSessions,
        AverageScore,
        GoBack
    }

    private T PromptEnum<T>(string title, params (string Label, T Value)[] options) where T : struct
    {
        var labels = new List<string>();
        var map = new Dictionary<string, T>();

        foreach (var (label, value) in options)
        {
            labels.Add(label);
            map[label] = value;
        }

        string selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .AddChoices(labels));

        return map[selected];
    }

    public Menu()
    {
        _flashCardService = new FlashCardService();
        _stackService = new StackService();
        _studySessionService = new StudySessionService();
        _reportService = new ReportService();
        _consoleUI = new ConsoleUI();
    }

    public void Start()
    {
        var running = true;

        while (running)
        {
            _consoleUI.ShowHeader();
            var choice = PromptEnum<MainMenuOption>(
                "Choose your operation:",
                ("Manage Stacks", MainMenuOption.ManageStacks),
                ("Manage FlashCards", MainMenuOption.ManageFlashCards),
                ("Study Session", MainMenuOption.StudySession),
                ("Report", MainMenuOption.Report),
                ("Exit", MainMenuOption.Exit));

            switch (choice)
            {
                case MainMenuOption.ManageStacks:
                    ManageStacksMenu();
                    break;
                case MainMenuOption.ManageFlashCards:
                    ManageFlashCardsMenu();
                    break;
                case MainMenuOption.StudySession:
                    ManageStudySession();
                    break;
                case MainMenuOption.Report:
                    ManageReport();
                    break;
                case MainMenuOption.Exit:
                    running = false;
                    break;
            }
        }

        AnsiConsole.MarkupLine("[grey]Goodbye![/]");
    }

    private void ManageStacksMenu()
    {
        var inSubMenu = true;

        while (inSubMenu)
        {
            _consoleUI.ShowHeader();
            var choice = PromptEnum<StackMenuOption>(
                "Choose Stack Operation:",
                ("Insert Stack", StackMenuOption.Insert),
                ("Delete Stack", StackMenuOption.Delete),
                ("Update Stack", StackMenuOption.Update),
                ("View All Stacks", StackMenuOption.ViewAll),
                ("Go Back", StackMenuOption.GoBack));

            try
            {
                switch (choice)
                {
                    case StackMenuOption.Insert:
                        _stackService.InsertStack();
                        break;
                    case StackMenuOption.Delete:
                        _stackService.DeleteStack();
                        break;
                    case StackMenuOption.Update:
                        _stackService.UpdateStack();
                        break;
                    case StackMenuOption.ViewAll:
                        _stackService.ViewAllStacks();
                        break;
                    case StackMenuOption.GoBack:
                        inSubMenu = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}");
                _consoleUI.Pause();
            }
        }
    }

    private void ManageFlashCardsMenu()
    {
        var inSubMenu = true;

        while (inSubMenu)
        {
            _consoleUI.ShowHeader();
            var choice = PromptEnum<FlashCardMenuOption>(
                "Choose FlashCard Operation:",
                ("Insert FlashCard", FlashCardMenuOption.Insert),
                ("Delete FlashCard", FlashCardMenuOption.Delete),
                ("Update FlashCard", FlashCardMenuOption.Update),
                ("View All FlashCards", FlashCardMenuOption.ViewAll),
                ("Go Back", FlashCardMenuOption.GoBack));

            try
            {
                switch (choice)
                {
                    case FlashCardMenuOption.Insert:
                        _flashCardService.InsertFlashCard();
                        break;
                    case FlashCardMenuOption.Delete:
                        _flashCardService.DeleteFlashCard();
                        break;
                    case FlashCardMenuOption.Update:
                        _flashCardService.UpdateFlashCard();
                        break;
                    case FlashCardMenuOption.ViewAll:
                        _flashCardService.ViewAllFlashCards();
                        break;
                    case FlashCardMenuOption.GoBack:
                        inSubMenu = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}");
                _consoleUI.Pause();
            }
        }
    }

    private void ManageStudySession()
    {
        var inSubMenu = true;
        while (inSubMenu)
        {
            _consoleUI.ShowHeader();
            var choice = PromptEnum<StudySessionOption>(
                "Choose StudySession Operation:",
                ("Start Study Session", StudySessionOption.StartSession),
                ("View All StudySession", StudySessionOption.ViewAllSession),
                ("Go Back", StudySessionOption.GoBack));

            try
            {
                switch (choice)
                {
                    case StudySessionOption.StartSession:
                        _studySessionService.StudySession();
                        break;
                    case StudySessionOption.ViewAllSession:
                        _studySessionService.ViewAllStudySession();
                        break;
                    case StudySessionOption.GoBack:
                        inSubMenu = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}");
                _consoleUI.Pause();
            }
        }

    }

    private void ManageReport()
    {
        var inSubMenu = true;
        while (inSubMenu)
        {
            _consoleUI.ShowHeader();
            var choice = PromptEnum<ReportOption>(
                "Choose an Operation",
                ("Total Session", ReportOption.TotalSessions),
                ("Average Score", ReportOption.AverageScore),
                ("Go Back", ReportOption.GoBack));

            try
            {
                switch (choice)
                {
                    case ReportOption.TotalSessions:
                        _reportService.TotalSessionPerMonth();
                        break;
                    case ReportOption.AverageScore:
                        _reportService.AverageScorePerMonth();
                        break;
                    case ReportOption.GoBack:
                        inSubMenu = false;
                        break;
                }

            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}");
                _consoleUI.Pause();
            }
        }

    }
}