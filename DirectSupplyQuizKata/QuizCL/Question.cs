using System;


/// <summary>
/// The Question class represents a question that will be answered within a Quiz. 
/// Each Question will have a prompt, and answer choices. There will always be
/// one correct answer choice and zero or more incorrect answer choices.
/// </summary>
public class Question
{

	/// <summary>
	/// The prompt for this question. The question which the user will answer.
	/// Publically gettable.
	/// </summary>
	public string Prompt { get; private set; }

	/// <summary>
	/// The string of the correct answer to the prompt.
	/// Publically gettable.
	/// </summary>
	public string CorrectAnswer { get; private set; }

	/// <summary>
	/// The array of incorrect answers to the prompt. Could be empty.
	/// </summary>
	public string[] IncorrectAnswers {  get; private set; }



	/// <summary>
	/// This sets the fields of this question based on the values of the parameters.
	/// This copies values, so the client will not be able to later edit any values.
	/// </summary>
	/// <param name="newPrompt">The prompt to give the question.</param>
	/// <param name="correctAnswer">The string of the correct answer choice.</param>
	/// <param name="incorrectAnswers">The array of the incorrect answers. Can be any length.</param>
	/// <exception cref="ArgumentNullException">
	/// If any of the parameters are null or have empty strings,
	/// the ArgumentNullException is thrown.
	/// </exception>
	public Question(string newPrompt, string correctAnswer, params string?[] incorrectAnswers)
	{
		// Check the provided arguments for being null (or empty string)
		if (String.IsNullOrEmpty(newPrompt) || String.IsNullOrEmpty(correctAnswer) 
				|| incorrectAnswers == null)
			throw new ArgumentNullException("All arguments must be non-null and non-empty.");

		// Make sure all the incorrect answers are valid strings
		foreach (string? answer in incorrectAnswers)
		{
			if (String.IsNullOrEmpty(answer))
				throw new ArgumentNullException("Incorrect answers must be non-null and non-empty.");
		}

        // We have validated, copy over the prompt and the correct answer. This is safe, as strings are immutable
        Prompt = newPrompt;
        CorrectAnswer = correctAnswer;

		// Do a shallow array copy from the parameter into the private field in this class.
		IncorrectAnswers = (string[]) incorrectAnswers.Clone();
	}

}
