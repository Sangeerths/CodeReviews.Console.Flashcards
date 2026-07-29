using Spectre.Console;
using StudyDeck.Database;
using StudyDeck.DTO.FlashCard;
using StudyDeck.Models;
using StudyDeck.Validation;
namespace StudyDeck.Services;

public class FlashCardService
    {
        private readonly InputValidator _validator;
        private readonly FlashCardSession _flashCardSession;
        private readonly StackSession _stackSession;
        private readonly ConsoleUI _consoleUI;

        public FlashCardService()
        {
            _validator = new InputValidator();
            _flashCardSession = new FlashCardSession();
            _stackSession = new StackSession();
            _consoleUI = new ConsoleUI();
        }

        public void InsertFlashCard()
        {
            
            string stackName = _validator.ValidateStackName()!;
            if (string.IsNullOrEmpty(stackName))
            {
                AnsiConsole.MarkupLine("[bold red]FlashCard insertion Failed.[/]");
                _consoleUI.Pause();
                return;
            }

            int stackId = _stackSession.GetStackId(stackName);
            if(stackId == -1)
            {
                AnsiConsole.MarkupLine($"[bold red]Stack '{stackName}' does not exist.[/]");
                _consoleUI.Pause();
                return;
            }

            UpdateFlashCardDto? flashCard = _validator.ValidateFlashCard();
            if (flashCard == null)
            {
                AnsiConsole.MarkupLine("[bold red]Flash Card insertion unsuccessful, please try again.[/]");
                _consoleUI.Pause();
                return;
            }
            CreateFlashCardDto createFlashCardDto = new CreateFlashCardDto 
            {
                StackId = stackId,
                Question = flashCard.Question!, 
                Answer = flashCard.Answer! 
            };

        _flashCardSession.InsertFlashCard(createFlashCardDto);
            AnsiConsole.MarkupLine("[bold green]Flash Card inserted successfully![/]");
            _consoleUI.Pause();
        }

        public void DeleteFlashCard()
        {
            string stackName = _validator.ValidateStackName()!;
            if (string.IsNullOrEmpty(stackName))
            {
                AnsiConsole.MarkupLine("[bold red]FlashCard Deletion Unsuccessful..[/]");
                _consoleUI.Pause();
                return;
            }

            if (_stackSession.IsStackExists(stackName) <= 0)
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

            if (_flashCardSession.IsFlashCardExists(flashcardId) <= 0)
            {
                AnsiConsole.MarkupLine("[bold red]FlashCard does not exist.[/]");
                _consoleUI.Pause();
                return;
            }

        _flashCardSession.DeleteFlashCard(flashcardId);
            AnsiConsole.MarkupLine("[bold green]Flash Card deleted successfully![/]");
            _consoleUI.Pause();
        }

        public void UpdateFlashCard()
        {
            string stackName = _validator.ValidateStackName()!;
            if (string.IsNullOrEmpty(stackName))
            {
                AnsiConsole.MarkupLine("[bold red]FlashCard Updation UnSuccessful[/]");
                _consoleUI.Pause();
                return;
            }

            UpdateFlashCardDto updateFlashCardDto = new UpdateFlashCardDto();
            updateFlashCardDto.FlashCardId = _validator.ValidateId();
            if (updateFlashCardDto.FlashCardId == -1) 
            {
                AnsiConsole.MarkupLine($"[bold red] FlashCard  doesnt exist[/]");
                _consoleUI.Pause();
                return;
            }

            if (_flashCardSession.IsFlashCardExists(updateFlashCardDto.FlashCardId) <= 0)
            {
                AnsiConsole.MarkupLine($"[bold red]FlashCard with id {updateFlashCardDto.FlashCardId} does not exist.[/]");
                _consoleUI.Pause();
                return;
            }

            UpdateFlashCardDto? updated = _validator.ValidateFlashCard();
            if (updated == null)
            {
                AnsiConsole.MarkupLine("[bold red]Flash Card update unsuccessful, please try again.[/]");
                _consoleUI.Pause();
                return;
            }

        _flashCardSession.UpdateFlashCard(updateFlashCardDto);
            AnsiConsole.MarkupLine("[bold green]Flash Card updated successfully![/]");
            _consoleUI.Pause();
        }

        public void ViewAllFlashCards( )
        {
            string stackName = _validator.ValidateStackName()!;
            if (string.IsNullOrEmpty(stackName))
            {
                AnsiConsole.MarkupLine("[bold red]FlashCard Operation Unsuccessful[/]");
                _consoleUI.Pause();
                return;
            }

            int stackId = _stackSession.GetStackId(stackName);
            if(stackId == -1)
            {
                AnsiConsole.WriteLine("[bold red]Wrong Stack[/]");
                _consoleUI.Pause();
                return;
            }

            List<FlashCard> cards = _flashCardSession.GetFlashCardsByStack(stackId);
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

            for (int i = 0; i < cards.Count; i++)
            {
                table.AddRow(
                    (i + 1).ToString(),
                    cards[i].Question!,
                    cards[i].Answer!);
            }

            AnsiConsole.Write(table);
            _consoleUI.Pause();
        }
    }
