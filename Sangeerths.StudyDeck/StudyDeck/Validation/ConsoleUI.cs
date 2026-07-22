using Spectre.Console;

namespace StudyDeck.Validation
{
    public class ConsoleUI
    {
        public void Pause()
        {
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[grey]Press Enter to continue...[/]");
            Console.ReadLine();
        }

        public void ShowHeader()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("StudyDeck")
                    .Centered()
                    .Color(Color.Cyan));
            AnsiConsole.Write(
                new Rule("[yellow]Flashcard Learning System[/]"));
            AnsiConsole.WriteLine();
        }
    }
}
