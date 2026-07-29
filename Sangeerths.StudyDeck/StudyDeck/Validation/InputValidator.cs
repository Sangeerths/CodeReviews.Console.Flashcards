using Spectre.Console;
using StudyDeck.DTO.FlashCard;
using System.Text;
namespace StudyDeck.Validation;

public class InputValidator
{
    private static string? ReadLineWithEscape()
    {
        StringBuilder buffer = new StringBuilder();

        while (true)
        {
            var keyInfo = Console.ReadKey(intercept: true);

            if (keyInfo.Key == ConsoleKey.Escape)
            {
                Console.WriteLine();
                return null;
            }

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return buffer.ToString();
            }

            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (buffer.Length > 0)
                {
                    buffer.Length--;
                    Console.Write("\b \b"); 
                }
                continue;
            }

            if (!char.IsControl(keyInfo.KeyChar))
            {
                buffer.Append(keyInfo.KeyChar);
                Console.Write(keyInfo.KeyChar); 
            }
        }
    }

    public string? ValidateStackName()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[bold yellow]Enter the name of the Stack (3-50 characters). Press ESC to go back.[/]");
            string? input = ReadLineWithEscape();
            if (input == null) return null; 

            string trimmed = input.Trim();

            if (trimmed.Length < 3 || trimmed.Length > 50)
            {
                AnsiConsole.MarkupLine("[bold red]Invalid stack name. Please enter a valid stack name (3-50 characters).[/]");
                continue;
            }

            return trimmed;
        }
    }

    public string? ValidateStackNameUpdate(string prompt)
    {
        while (true)
        {
            AnsiConsole.MarkupLine($"[bold yellow]{prompt}[/]");
            string? input = ReadLineWithEscape();
            if (input == null)
                return null;

            string trimmed = input.Trim();

            if (trimmed.Length < 3 || trimmed.Length > 50)
            {
                AnsiConsole.MarkupLine("[bold red]Invalid stack name.[/]");
                continue;
            }

            return trimmed;
        }
    }

    public UpdateFlashCardDto? ValidateFlashCard()
    {
        UpdateFlashCardDto flashCard = new UpdateFlashCardDto();

        while (true)
        {
            AnsiConsole.MarkupLine("[bold yellow]Enter the Front of the FlashCard. Press ESC to go back.[/]");
            string? input = ReadLineWithEscape();
            if (input == null) return null;

            string trimmed = input.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                AnsiConsole.MarkupLine("[bold red]Invalid input. The front of the flashcard cannot be empty.[/]");
                continue;
            }

            flashCard.Question = trimmed;
            break;
        }

        while (true)
        {
            AnsiConsole.MarkupLine("[bold yellow]Enter the Back of the FlashCard. Press ESC to go back.[/]");

            string? input = ReadLineWithEscape();
            if (input == null) return null;

            string trimmed = input.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                AnsiConsole.MarkupLine("[bold red]Invalid input. The back of the flashcard cannot be empty.[/]");
                continue;
            }

            flashCard.Answer = trimmed;
            break;
        }

        return flashCard;
    }

    public int ValidateId()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[bold yellow]Enter the Id. Press ESC to go back.[/]");
            string? input = ReadLineWithEscape();
            if (input == null) return -1; 

            if (!int.TryParse(input.Trim(), out int id) || id <= 0)
            {
                AnsiConsole.MarkupLine("[bold red]Invalid Id. Please enter a positive whole number.[/]");
                continue;
            }

            return id;
        }
    }
    public int ValidateYear()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[bold yellow]Enter the Year (e.g., 2026). Press ESC to go back.[/]");
            string? input = ReadLineWithEscape();
            if (input == null) return -1;

            if (!int.TryParse(input.Trim(), out int year))
            {
                AnsiConsole.MarkupLine("[bold red]Invalid input. Please enter a valid 4-digit year number.[/]");
                continue;
            }

            int currentYear = DateTime.Now.Year;
            if (year < 1 || year > currentYear)
            {
                AnsiConsole.MarkupLine($"[bold red]Invalid Year. Please enter a year between 1 and {currentYear}.[/]");
                continue;
            }

            return year;
        }
    } 
}
