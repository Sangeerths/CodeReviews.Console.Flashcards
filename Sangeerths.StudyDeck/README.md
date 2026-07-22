# 📚 StudyDeck

StudyDeck is a console-based flashcard learning application built with **C# and .NET**. It allows users to create and manage study stacks, add flashcards, take interactive study sessions, and view performance reports.

The application uses **SQL Server** for data persistence and **Spectre.Console** to provide an interactive and user-friendly console interface.

---

## 🚀 Features

### 📂 Stack Management
- Create new study stacks
- View all available stacks
- Update stack names
- Delete stacks
- Prevent duplicate stack names
- Search stacks by name

### 🃏 Flashcard Management
- Add flashcards to a stack
- View all flashcards
- Update flashcards
- Delete flashcards
- Associate flashcards with stacks using `StackId`

### 🧠 Study Sessions
- Select a study stack
- Search for stacks before starting a session
- Paginated stack selection
- Answer flashcard questions interactively
- Automatically check answers
- Display the correct answer for incorrect responses
- Calculate the final score
- Save study session results
- Study the same stack again
- Choose another stack
- Exit the study session

### 📊 Reports
- View total study sessions
- View average scores
- Analyze study session data month-wise
- View all previous study sessions

### 🎨 Console UI
- Interactive menus using Spectre.Console
- Formatted tables
- Headers and banners
- Navigation using selection prompts
- Pagination for large lists
- Search functionality
- Pause functionality between operations

---

## 🛠️ Technologies Used

- **C#**
- **.NET 10**
- **SQL Server**
- **Microsoft.Data.SqlClient**
- **Spectre.Console**
- **ADO.NET**
- **Git & GitHub**

---

## 🏗️ Project Structure

```text
StudyDeck
│
├── Database
│   └── DbSession.cs
│
├── Models
│   ├── StackCard.cs
│   ├── FlashCard.cs
│   └── StudySession.cs
│
├── Services
│   ├── StackService.cs
│   ├── FlashCardService.cs
│   ├── StudySessionService.cs
│   ├── ReportService.cs
│   └── ConsoleUI.cs
│
├── Validation
│   └── InputValidator.cs
│
├── Controller
│   └── Menu.cs
│
└── Program.cs