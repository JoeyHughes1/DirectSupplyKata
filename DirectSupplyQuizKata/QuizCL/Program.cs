using System;
using System.IO;
using System.Xml.Linq;

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
    /// Prints out the Question prompt, and its answer choices in a random order.
    /// Returns the array of the answer choices.
    /// </summary>
    /// <param name="q">The question to write to console.</param>
    /// <returns>The array of answer choices in the order they were printed.</returns>
    private static string[] PrintQuestion(Question q)
    {
        // Write the prompt out with an extra newline
        Console.WriteLine($"---\n{q.Prompt}:\n");

        // Get a random sequence of choices.
        string[] choices = q.getAnswerChoices();

        // Print out the choices.
        for (int i = 0; i < choices.Length; i++)
        {
            Console.WriteLine($"{i}: {choices[i]}");
        }

        // Return the choice array so the rest of the code can know which choice was which.
        return choices;
    }



    /// <summary>
    /// The Main function for the program. Runs the command line interface for interacting with quizzes.
    /// </summary>
    private static void Main()
    {
        Console.WriteLine("Welcome to the Quiz.");

        // Create a Quiz
        Quiz quiz = new Quiz("Minecraft", "The ultimat minecraft quiz! EP1C STYL3!!");
        quiz.AddQuestion("What is the fastest material for pickaxes?", "Gold", "Wood", "Diamond", "Netherite", "Iron");
        quiz.AddQuestion("Who is the creepy man with white eyes that looks like Steve?", "Herobrine", "Not so good feeling maker man", "DanTDM", "Jeff");
        quiz.AddQuestion("What is the final boss's name?", "The Ender Dragon", "The Shulker", "The Evoker");


        // Get the questions from the Quiz
        List<Question> list = quiz.Questions;
        int numCorrect = 0;

        // Print the information about the Quiz
        Console.WriteLine($"-----\nCategory: {quiz.Category}\nTitle: {quiz.Title}\n");

        // Ask each question
        foreach (Question question in list)
        {
            // Print the question and get a hold of the choices
            string[] choices = PrintQuestion(question);

            // Keep reading until they enter a valid answer choice.
            bool tryAgain = true;
            while (tryAgain)
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
                    if (question.IsCorrect(choices[Convert.ToInt32(answer)]))
                    {
                        // If their answer was correct, print correct
                        Console.WriteLine("\nCorrect!");
                        numCorrect++;
                    }
                    else
                    {
                        // If their answer was incorrect, print false
                        Console.WriteLine("\nIncorrect.");
                    }

                    // Since we successfully got an answer, we are done reading.
                    tryAgain = false;
                }
                catch (Exception ex)
                {
                    // If their answer was not the right format or out of range, try again
                    Console.WriteLine("Please enter the number of one of the answer choices.");
                }
            }

            // After they have given their answer, print another newline, then go to the next.
            Console.WriteLine();
        }

        // After the quiz, display the results
        float percentScore = 100 * ((float)numCorrect) / quiz.NumQuestions;
        Console.WriteLine($"Your score is: \n\n({numCorrect} / {quiz.NumQuestions}) = {percentScore:0.00}%");
        Console.WriteLine("Thank you for taking the quiz!");
        
    }
}