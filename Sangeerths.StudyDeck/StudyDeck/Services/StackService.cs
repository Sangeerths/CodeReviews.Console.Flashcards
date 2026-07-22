using Spectre.Console;
using StudyDeck.Database;
using StudyDeck.Models;
using StudyDeck.Validation;

namespace StudyDeck.Services
{
    public class StackService
    {
        private readonly DbSession _db;
        private readonly InputValidator _validator = new InputValidator();
        private readonly ConsoleUI _consoleUI;

        public StackService()
        {
            _db = new DbSession();
            _consoleUI = new ConsoleUI();
        }

        public void InsertStack()
        {
            string? stackName = _validator.ValidateStackName();
            if (stackName == null)
            {
                AnsiConsole.MarkupLine("[bold red]Insertion operation Failed..[/]");
                _consoleUI.Pause();
                return;
            }

            if (_db.IsStackExists(stackName) > 0)
            {
                AnsiConsole.MarkupLine($"[bold red]Stack '{stackName}' already exists.[/]");
                _consoleUI.Pause();
                return;
            }

            _db.InsertStack(stackName);
            AnsiConsole.MarkupLine($"[bold green]Stack '{stackName}' inserted successfully![/]");
            _consoleUI.Pause();


        }

        public void DeleteStack()
        {
            string? stackName = _validator.ValidateStackName();
            if (stackName == null)
            {
                AnsiConsole.MarkupLine("[bold red]Deletion operation Failed..[/]");
                _consoleUI.Pause();
                return;
            }

            if (_db.IsStackExists(stackName) > 0)
            {
                _db.DeleteStack(stackName);
                AnsiConsole.MarkupLine($"[bold green]Stack '{stackName}' deleted successfully![/]");

            }
            else
            {
                AnsiConsole.MarkupLine($"[bold red]Stack '{stackName}' does not exist.[/]");
            }

            _consoleUI.Pause();
        }

        public void UpdateStack()
        {
            string? oldName = _validator.ValidateStackNameUpdate("Enter the current stack name. Press ESC to go back.");
            if (oldName == null)
            {
                AnsiConsole.MarkupLine("[bold red]Update operation Failed..[/]");
                _consoleUI.Pause();
                return;
            }

            if (_db.IsStackExists(oldName) <= 0)
            {
                AnsiConsole.MarkupLine($"[bold red]Stack '{oldName}' does not exist.[/]");
                _consoleUI.Pause();
                return;
            }

            string? newName = _validator.ValidateStackNameUpdate("Enter the new stack name. Press ESC to go back.");
            if (newName == null)
            {
                AnsiConsole.MarkupLine("[bold red]Update operation Failed..[/]");
                _consoleUI.Pause();
                return;
            }

            if (_db.IsStackExists(newName) > 0)
            {
                AnsiConsole.MarkupLine($"[bold red]Stack '{newName}' already exists.[/]");
                _consoleUI.Pause();
                return;
            }

            _db.UpdateStack(oldName, newName);
            AnsiConsole.MarkupLine($"[bold green]Stack '{oldName}' renamed to '{newName}'.[/]");
            _consoleUI.Pause();
        }

        public void ViewAllStacks()
        {
            List<StackCard> stacks = _db.GetAllStacks();

            if (stacks.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No stacks found.[/]");
                _consoleUI.Pause();
                return;
            }

            var table = new Table();
            table.AddColumn("Stack Id");
            table.AddColumn("Stack Name");

            foreach (var stack in stacks)
            {
                table.AddRow(stack.StackId.ToString(), stack.StackName);
            }

            AnsiConsole.Write(table);
            _consoleUI.Pause();
        }
    }
}