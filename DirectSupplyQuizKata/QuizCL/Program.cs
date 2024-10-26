using System;
using System.IO;

/// <summary>
/// Not too sure yet.
/// </summary>
internal class Program
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
        Console.WriteLine($"{q.Prompt}:\n");

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
        Console.WriteLine("Hello, World!");
        
        // Create a test question
        Question q1 = new Question("What is the capital of the United States?", "Washington, DC", "Charlotte, NC", "Salt Lake City, UT");


        // Print the question and get a hold of the choices
        string[] choices = PrintQuestion(q1);

        // Check if the answer they gave was correct
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
                if (q1.IsCorrect(choices[Convert.ToInt32(answer)]))
                {
                    // If their answer was correct, print correct
                    Console.WriteLine("Correct!");
                } else
                {
                    // If their answer was incorrect, print false
                    Console.WriteLine("Incorrect.");
                }
                tryAgain = false;
            }
            catch (Exception ex)
            {
                // If their answer was not the right format or out of range, try again
                Console.WriteLine("Please enter the number of one of the answer choices.");
            }
        }

        
    }
}