using Spectre.Console;
using StudyDeck.Database;
using StudyDeck.DTO.Stack;
using StudyDeck.Models;
using StudyDeck.Validation;
namespace StudyDeck.Services;

public class StackService
    {
        private readonly StackSession _stackSession;
        private readonly InputValidator _validator = new InputValidator();
        private readonly ConsoleUI _consoleUI;

        public StackService()
        {
        _stackSession = new StackSession();
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

            if (_stackSession.IsStackExists(stackName) > 0)
            {
                AnsiConsole.MarkupLine($"[bold red]Stack '{stackName}' already exists.[/]");
                _consoleUI.Pause();
                return;
            }

            _stackSession.InsertStack(stackName);
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

            if (_stackSession.IsStackExists(stackName) > 0)
            {
                _stackSession.DeleteStack(stackName);
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
            UpdateStackDto updateStack = new UpdateStackDto();
            updateStack.OldName = _validator.ValidateStackNameUpdate("Enter the current stack name. Press ESC to go back.")!;
            if (updateStack.OldName == null)
            {
                AnsiConsole.MarkupLine("[bold red]Update operation Failed..[/]");
                _consoleUI.Pause();
                return;
            }

            if (_stackSession.IsStackExists(updateStack.OldName) <= 0)
            {
                AnsiConsole.MarkupLine($"[bold red]Stack '{updateStack.OldName}' does not exist.[/]");
                _consoleUI.Pause();
                return;
            }

            updateStack.NewName = _validator.ValidateStackNameUpdate("Enter the new stack name. Press ESC to go back.")!;
            if (updateStack.NewName == null)
            {
                AnsiConsole.MarkupLine("[bold red]Update operation Failed..[/]");
                _consoleUI.Pause();
                return;
            }

            if (_stackSession.IsStackExists(updateStack.NewName) > 0)
            {
                AnsiConsole.MarkupLine($"[bold red]Stack '{updateStack.NewName}' already exists.[/]");
                _consoleUI.Pause();
                return;
            }

            _stackSession.UpdateStack(updateStack);
            AnsiConsole.MarkupLine($"[bold green]Stack '{updateStack.OldName}' renamed to '{updateStack.NewName}'.[/]");
            _consoleUI.Pause();
        }

        public void ViewAllStacks()
        {
            List<StackCard> stacks = _stackSession.GetAllStacks();

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
