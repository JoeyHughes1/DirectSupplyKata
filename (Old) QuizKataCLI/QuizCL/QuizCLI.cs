using System;
using System.IO;
using System.Net;
using System.Text.Json;

/// <summary>
/// Runs a command line interface for the Quiz program.
/// </summary>
internal class QuizCLI
{

    /// <summary>
    /// Prints the prompt for the user to enter the number of their answer choice.
    /// </summary>
    private static void PrintPromptForAnswer()
    {
        Console.Write("Write the number of your choice: ");
    }



    /// <summary>
    /// This takes a question and returns a shuffled array of it's answer choices.
    /// </summary>
    /// <param name="q">The Question to shuffle the answers for.</param>
    /// <returns>The shuffled array of the answer strings.</returns>
    private static string[] GetAnswerChoices(Question q)
    {
        // Create an array to hold all of the answers, including the correct one.
        string[] allAnswers = new string[q.IncorrectAnswers.Length + 1];

        // Copy the incorrect answers into the array, and add the correct answer at the end.
        Array.Copy(q.IncorrectAnswers, allAnswers, q.IncorrectAnswers.Length);
        allAnswers[^1] = q.CorrectAnswer;

        // Shuffle the order of the answers in the array
        Random random = new Random();
        for (int i = allAnswers.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            string temp = allAnswers[i];
            allAnswers[i] = allAnswers[j];
            allAnswers[j] = temp;
        }

        // Return out shuffled array of all the answers
        return allAnswers;
    }



    /// <summary>
    /// This checks the given string answer (Case-insensitive) against the correct answer from the given Question.
    /// </summary>
    /// <param name="q">The question to check the answer for.</param>
    /// <param name="maybeCorrect">The answer that might be correct.</param>
    /// <returns>True if the given answer equals the Question's correct answer (Case-insensitive).</returns>
    private static bool IsCorrect(Question q, string maybeCorrect) => (!String.IsNullOrEmpty(maybeCorrect) && maybeCorrect.ToLower().Equals(q.CorrectAnswer.ToLower()));



    /// <summary>
    /// This, given a quiz, will start asking the quiz questions on the command line
    /// and keep score.
    /// </summary>
    /// <param name="quiz">The Quiz to take.</param>
    private static void TakeQuiz(Quiz quiz)
    {
        // Get the questions from the Quiz
        List<Question> list = quiz.Questions;

        // The number they get correct, for calculating score.
        int numCorrect = 0;

        // Print the information about the Quiz
        Console.WriteLine($"-----\nCategory: {quiz.Category}\nTitle: {quiz.Title}\n");

        // Ask each question
        for (int i = 0; i < list.Count; i++)
        {
            // --- Print the question and answers
            // Get the question and the answer choices
            Question question = list[i];
            string[] choices = GetAnswerChoices(question);

            // Write the prompt out with an extra newline
            Console.WriteLine($"---\n(Q{i + 1}) {question.Prompt}:\n");

            // Print out the choices.
            for (int j = 0; j < choices.Length; j++)
                Console.WriteLine($"{j + 1}: {choices[j]}");

            // --- Get the answer from the user
            // Keep reading until they enter a valid answer choice.
            bool gotItCorrect;
            while (true)
            {
                // Read an answer from the user.
                PrintPromptForAnswer();
                string? answer = Console.ReadLine();

                // If the answer read was null, something went wrong
                if (answer == null)
                {
                    Console.WriteLine("Your answer could not be read. Exiting.");
                    return;
                }

                // Try to convert the answer to a number and check their choice
                try
                {
                    // Check their answer against the correct answer
                    gotItCorrect = IsCorrect(question, choices[Convert.ToInt32(answer) - 1]);
                    break;
                }
                catch (Exception)
                {
                    // If their answer was not the right format or out of range, try again
                    Console.WriteLine("Please enter the number of one of the answer choices.");
                }
            }

            // --- Print either that they were correct, or show them the right answer
            Console.Clear();
            Console.Write(gotItCorrect ? "\nCorrect!\n\n" : "\nIncorrect. ");

            // If they got it right, increase the tally. If they didn't, print the right answer
            if (gotItCorrect)
                numCorrect++;
            else
                Console.WriteLine($"The correct answer was: \n\n{question.Prompt}\n-> {question.CorrectAnswer}\n");
        }

        // --- After the quiz, display the results
        float percentScore = 100 * ((float)numCorrect) / list.Count;
        Console.WriteLine($"-----\nAll finished!\nYour score is: \n\n({numCorrect} / {list.Count}) = {percentScore:0.00}%");
        Console.WriteLine("Thank you for taking the quiz!");
        Console.WriteLine("Press Enter to go back to the Category Selection.");
        Console.ReadLine();
    }



    /// <summary>
    /// Prints all the categories in the list of categories.
    /// The 0th category is listed as choice 1.
    /// </summary>
    /// <param name="list">The list of category names.</param>
    private static void PrintCategoryList(List<string> list)
    {
        // Print top of the category list
        Console.WriteLine("The Categories that quizzes may be selected from are: ");

        // Print each category on a line
        for (int i = 0; i < list.Count; i++)
        {
            Console.WriteLine($"({i + 1}) {list[i]}");
        }

        // Print the last option as the option to leave
        Console.WriteLine($"({list.Count + 1}) **Exit the program");
    }



    /// <summary>
    /// This prints out the list of Quizzes that can be chosen from within a
    /// certain category. The category name is given so we can print a header that
    /// lists what the category is.
    /// </summary>
    /// <param name="category">The category name for all the quizzes.</param>
    /// <param name="list">The list of Quiz objects.</param>
    private static void PrintQuizList(string category, List<string> list)
    {
        // Print top of the quiz list
        Console.WriteLine($"The Quizzes in the \"{category}\" category are: ");

        // Print each quiz on a line
        for (int i = 0; i < list.Count; i++)
        {
            Console.WriteLine($"({i + 1}) {list[i]}");
        }

        // Print the last option as the option to go back
        Console.WriteLine($"({list.Count + 1}) **Go back to categories");
    }



    /// <summary>
    /// This gets user input, assuming the input wanted is an index into an array.
    /// So, a number. So, if the user inputs 1, that is interpreted as an index 0 into an array.
    /// Given the size of the array, this will keep asking until a valid option is given.
    /// </summary>
    /// <param name="prompt">The initial prompt for input.</param>
    /// <param name="retryPrompt">The message to show when an incorrect value is entered.</param>
    /// <param name="sizeOfList">
    /// The size of the list being indexed into by the user. 
    /// Essentially, the highest acceptable value from the user.
    /// </param>
    /// <returns>The final choice the user made.</returns>
    private static int GetUserInput(string prompt, string retryPrompt, int sizeOfList)
    {
        int choice;
        while (true)
        {
            // Read an answer from the user.
            Console.Write(prompt);
            string? answer = Console.ReadLine();

            // If the answer read was null, something went wrong
            if (answer == null)
            {
                Console.WriteLine("Your answer could not be read. Something went wrong, exiting.");
                return -1;
            }

            // Try to convert the answer to a number and check their choice
            try
            {
                // Try to convert to an integer
                choice = Convert.ToInt32(answer) - 1;

                // If their choice was in the range of indices, break, we're good.
                if (choice >= 0 && choice < sizeOfList)
                    break;
            }
            catch (Exception) { }

            // If their answer was not the right format or out of range, try again
            Console.WriteLine(retryPrompt);
        }

        return choice;
    }



    /// <summary>
    /// The Main function for the program. Runs the command line interface for interacting with quizzes.
    /// </summary>
    private async static Task Main()
    {
        // Intro
        Console.WriteLine("Welcome to the Quiz Program!");

        // Create the QuizManager. This will create Random Quizzes in all the appropriate categories.
        QuizManager manager = new();


        //// Testing for adding, deleting and editing quizzes
        //Quiz newQuiz = new Quiz("Epic new Category", "Super swag quiz", 20);
        //newQuiz.AddQuestion("What is the capital of swagtown", "Mr. Swags house", "loserville", "not swag shaggy");
        //manager.AddQuiz(newQuiz);

        //Quiz otherQuiz = new Quiz("Lame sad Category", "Super lame quiz", 20);
        //otherQuiz.AddQuestion("What is the capital of loserville", "loser face mansion", "swagville", "not swag shaggy");
        //manager.AddQuiz(otherQuiz);

        //Quiz editedQuiz = new Quiz("Lame sad Category", "Super mega lame quiz", 50);
        //editedQuiz.AddQuestion("What is the capital of loserville", "loser face high school", "swagville", "not swag shaggy");
        //manager.EditQuiz("Lame sad Category", "Super lame quiz", editedQuiz);



        // The main loop of picking Quizzes
        while (true)
        {
            // Print the category list
            List<string> categoryList = manager.Categories;
            PrintCategoryList(categoryList);

            // Get the user's choice of category
            int catChoice = GetUserInput("Choose a category (#): ", "Please enter the number of one of the categories.", categoryList.Count + 1);

            // If they chose the last option, that means leave.
            if (catChoice == categoryList.Count) break;

            // Load the quizzes for that Category
            List<string> quizList = manager.GetQuizzesInCategory(categoryList[catChoice]);

            // Print the list of quiz choices:
            Console.Clear();
            PrintQuizList(categoryList[catChoice], quizList);

            // Get the user's choice of quiz
            int quizChoice = GetUserInput("Choose a quiz (#): ", "Please enter the number of one of the quizzes.", quizList.Count + 1);

            // If they chose the last option, that means go back to choosing categories
            if (quizChoice == quizList.Count)
            {
                Console.Clear();
                continue;
            }

            // Now we have their quiz choice. Get the Quiz object from the manager.
            Quiz? quizToTake = manager.GetQuiz(categoryList[catChoice], quizList[quizChoice]).Result;

            // If we got null, something went wrong.
            if (quizToTake == null)
            {
                Console.WriteLine("Something has gone wrong, we couldn't find the quiz. Exiting.");
                return;
            }

            // Now take the quiz!
            Console.Clear();
            TakeQuiz(quizToTake);
            Console.Clear();
        }

        // They have chosen to leave. Say goodbye.
        Console.Clear();
        Console.WriteLine("I hope you had fun! See you next time!");

    }
}