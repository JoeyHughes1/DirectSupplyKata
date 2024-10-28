using System;
using System.Text.Json.Serialization;


/// <summary>
/// The Question class represents a question that will be answered within a Quiz. 
/// Each Question will have a prompt, and answer choices. There will always be
/// one correct answer choice and zero or more incorrect answer choices.
/// </summary>
public class Question
{
	/// <summary>
	/// Underlying prompt for the question.
	/// </summary>
	private string _prompt;

	/// <summary>
	/// The prompt for this question. The question which the user will answer.
	/// Publically gettable.
	/// </summary>
	public string Prompt 
	{ 
		get => _prompt; 
		private set
		{
            ArgumentNullException.ThrowIfNullOrEmpty(value);
            _prompt = value;
        }
	}

	/// <summary>
	/// Underlying correct answer to the question.
	/// </summary>
	private string _correctAnswer;

	/// <summary>
	/// The string of the correct answer to the prompt.
	/// Publically gettable.
	/// </summary>
	public string CorrectAnswer
    {
        get => _correctAnswer;
        private set
        {
            ArgumentNullException.ThrowIfNullOrEmpty(value);
            _correctAnswer = value;
        }
    }

	/// <summary>
	/// The underlying incorrect answers.
	/// </summary>
    private string[] _incorrectAnswers;

	/// <summary>
	/// The array of incorrect answers to the prompt. Could be empty.
	/// </summary>
	public string[] IncorrectAnswers
	{
		get => _incorrectAnswers;
		private set
		{
			// Incorrect answer array can't be null
            ArgumentNullException.ThrowIfNull(value);

			// All of the incorrect answer strings must not be null or empty.
			// I left the manual throwing to have a specific message, since this is a little less obvious.
            foreach (string? answer in value)
            {
                if (String.IsNullOrEmpty(answer))
                    throw new ArgumentNullException(nameof(value), "Incorrect answer strings must be non-null and non-empty.");
            }
			_incorrectAnswers = (string[])value.Clone();
        }
	}



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
	[JsonConstructor]
	public Question(string prompt, string correctAnswer, params string?[] incorrectAnswers)
	{
        Prompt = prompt;
        CorrectAnswer = correctAnswer;
		// Use null forgiving operator to ease the compiler, IncorrectAnswers will do its own checks.
		IncorrectAnswers = incorrectAnswers!;
	}

}
