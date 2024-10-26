using System;
using System.IO;

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
            string[] choices = question.getAnswerChoices();

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
                    gotItCorrect = question.IsCorrect(choices[Convert.ToInt32(answer) - 1]);
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
            Console.WriteLine(gotItCorrect ? "\nCorrect!\n" : "\nIncorrect.\n");

            // If they got it right, increase the tally. If they didn't, print the right answer
            if (gotItCorrect)
                numCorrect++;
            else
                Console.WriteLine($"The correct answer was: \n\n{question.Prompt}\n-> {question.CorrectAnswer}\n");
        }

        // --- After the quiz, display the results
        float percentScore = 100 * ((float)numCorrect) / quiz.NumQuestions;
        Console.WriteLine($"Your score is: \n\n({numCorrect} / {quiz.NumQuestions}) = {percentScore:0.00}%");
        Console.WriteLine("Thank you for taking the quiz!");
    }



    /// <summary>
    /// The Main function for the program. Runs the command line interface for interacting with quizzes.
    /// </summary>
    private async static Task Main()
    {
        Console.WriteLine("Welcome to the Quiz.");

        // Create a Quiz
        //Quiz quiz = new Quiz("Minecraft", "The ultimat minecraft quiz! EP1C STYL3!!");
        //quiz.AddQuestion("What is the fastest material for pickaxes?", "Gold", "Wood", "Diamond", "Netherite", "Iron");
        //quiz.AddQuestion("Who is the creepy man with white eyes that looks like Steve?", "Herobrine", "Not so good feeling maker man", "DanTDM", "Jeff");
        //quiz.AddQuestion("What is the final boss's name?", "The Ender Dragon", "The Shulker", "The Evoker");
        RandomQuiz quiz = new RandomQuiz("Science & Nature");
        await quiz.RandomizeQuestions();

        TakeQuiz(quiz);
    }
}