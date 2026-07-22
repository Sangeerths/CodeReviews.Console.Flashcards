using Spectre.Console;
using StudyDeck.Database;
using StudyDeck.Models;
using StudyDeck.Validation;

namespace StudyDeck.Services
{
    public class FlashCardService
    {
        private readonly InputValidator _validator;
        private readonly DbSession _dbSession;
        private readonly ConsoleUI _consoleUI;

        public FlashCardService()
        {
            _validator = new InputValidator();
            _dbSession = new DbSession();
            _consoleUI = new ConsoleUI();
        }

        public void InsertFlashCard()
        {
            string stackName = _validator.ValidateStackName();
            if (string.IsNullOrEmpty(stackName))
            {
                AnsiConsole.MarkupLine("[bold red]FlashCard insertion Failed.[/]");
                _consoleUI.Pause();
                return;
            }

            int stackId = _dbSession.GetStackId(stackName);
            if(stackId == -1)
            {
                AnsiConsole.MarkupLine($"[bold red]Stack '{stackName}' does not exist.[/]");
                _consoleUI.Pause();
                return;
            }
            
            FlashCard? flashCard = _validator.ValidateFlashCard();
            if (flashCard == null)
            {
                AnsiConsole.MarkupLine("[bold red]Flash Card insertion unsuccessful, please try again.[/]");
                _consoleUI.Pause();
                return;
            }

            _dbSession.InsertFlashCard(stackId, flashCard.Question, flashCard.Answer);
            AnsiConsole.MarkupLine("[bold green]Flash Card inserted successfully![/]");
            _consoleUI.Pause();
        }

        public void DeleteFlashCard()
        {
            string stackName = _validator.ValidateStackName();
            if (string.IsNullOrEmpty(stackName))
            {
                AnsiConsole.MarkupLine("[bold red]FlashCard Deletion Unsuccessful..[/]");
                _consoleUI.Pause();
                return;
            }

            if (_dbSession.IsStackExists(stackName) <= 0)
            {
                AnsiConsole.MarkupLine($"[bold red]Stack '{stackName}' does not exist.[/]");
                _consoleUI.Pause();
                return;
            }
            
            int flashcardId = _validator.ValidateId();
            if(flashcardId ==-1)
            {
                AnsiConsole.MarkupLine($"[bold red] FlashCard  with id {flashcardId} doesnt exist[/]");
                _consoleUI.Pause();
                return;
            }

            if (_dbSession.IsFlashCardExists(flashcardId) <= 0)
            {
                AnsiConsole.MarkupLine("[bold red]FlashCard does not exist.[/]");
                _consoleUI.Pause();
                return;
            }

            _dbSession.DeleteFlashCard(flashcardId);
            AnsiConsole.MarkupLine("[bold green]Flash Card deleted successfully![/]");
            _consoleUI.Pause();
        }

        public void UpdateFlashCard()
        {
            string stackName = _validator.ValidateStackName();
            if (string.IsNullOrEmpty(stackName))
            {
                AnsiConsole.MarkupLine("[bold red]FlashCard Updation UnSuccessful[/]");
                _consoleUI.Pause();
                return;
            }

            int flashcardId = _validator.ValidateId();
            if (flashcardId == -1) 
            {
                AnsiConsole.MarkupLine($"[bold red] FlashCard  doesnt exist[/]");
                _consoleUI.Pause();
                return;
            }

            if (_dbSession.IsFlashCardExists(flashcardId) <= 0)
            {
                AnsiConsole.MarkupLine($"[bold red]FlashCard with id {flashcardId} does not exist.[/]");
                _consoleUI.Pause();
                return;
            }

            FlashCard? updated = _validator.ValidateFlashCard();
            if (updated == null)
            {
                AnsiConsole.MarkupLine("[bold red]Flash Card update unsuccessful, please try again.[/]");
                _consoleUI.Pause();
                return;
            }

            _dbSession.UpdateFlashCard(flashcardId, updated.Question, updated.Answer);
            AnsiConsole.MarkupLine("[bold green]Flash Card updated successfully![/]");
            _consoleUI.Pause();
        }

        public void ViewAllFlashCards( )
        {
            string stackName = _validator.ValidateStackName();
            if (string.IsNullOrEmpty(stackName))
            {
                AnsiConsole.MarkupLine("[bold red]FlashCard Operation Unsuccessful[/]");
                _consoleUI.Pause();
                return;
            }

            int stackId = _dbSession.GetStackId(stackName);
            if(stackId == -1)
            {
                AnsiConsole.WriteLine("[bold red]Wrong Stack[/]");
                _consoleUI.Pause();
                return;
            }

            List<FlashCard> cards = _dbSession.GetFlashCardsByStack(stackId);
            if (cards.Count == 0)
            {
                AnsiConsole.MarkupLine($"[yellow]No flashcards found in stack '{stackName}'.[/]");
                _consoleUI.Pause();
                return;
            }

            var table = new Table()
                .AddColumn("Id")
                .AddColumn("Front")
                .AddColumn("Back");

            foreach (var card in cards)
                table.AddRow(card.Id.ToString(), card.Question, card.Answer);

            AnsiConsole.Write(table);
            _consoleUI.Pause();
        }
    }
}