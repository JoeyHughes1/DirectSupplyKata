using System;

/// <summary>
/// The Quiz object holds a list of Questions, a category, and a title.
/// The category and title can only be changed by going through the QuizManager.
/// </summary>
public class Quiz
{
	/// <summary>
	/// The list of Question objects for this quiz.
	/// </summary>
	public List<Question> Questions {  get; private set; }

	/// <summary>
	/// Property of the amount of questions in this quiz. Read-only.
	/// </summary>
	public int NumQuestions
	{
		get => Questions.Count;
	}

	/// <summary>
	/// The Category of the Quiz. 
	/// </summary>
	public string Category { get; internal set; }

	/// <summary>
	/// The Title of this Quiz.
	/// </summary>
	public string Title { get; internal set; }



	/// <summary>
	/// This constructs a quiz object with an empty questions list, but with
	/// the given category and title.
	/// </summary>
	/// <param name="category">The name of the category of this Quiz</param>
	/// <param name="title">The title of this quiz.</param>
	public Quiz(string category, string title)
	{
		Category = category;
		Title = title;
        Questions = [];
	}



    /// <summary>
    /// Creates a new Question object and adds it to this Quiz.
    /// </summary>
    /// <param name="prompt">The prompt to give the question.</param>
    /// <param name="correctAnswer">The correct answer to give to the question.</param>
    /// <param name="incorrectAnswers">The list of incorrect answers to give to the question.</param>
    /// <exception cref="ArgumentNullException">If any of the parameters are null or have empty strings.</exception>
    public void AddQuestion(string prompt, string correctAnswer, params string?[] incorrectAnswers)
	{
        Questions.Add(new Question(prompt, correctAnswer, incorrectAnswers));
	}
}
