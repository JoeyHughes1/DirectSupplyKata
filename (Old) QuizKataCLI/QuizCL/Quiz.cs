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
	/// The Category of the Quiz. 
	/// </summary>
	public string Category { get; private set; }

	/// <summary>
	/// The Title of this Quiz.
	/// </summary>
	public string Title { get; private set; }

	/// <summary>
	/// The underlying integer time value for the TimeLimit property.
	/// </summary>
	private int timeLimit;

	/// <summary>
	/// The time limit in seconds for this Quiz.
	/// </summary>
	public int TimeLimit 
	{
		get => timeLimit;
		private protected set
		{
            if (value <= 0)
                throw new ArgumentOutOfRangeException("Time limit must be a positive integer");
			timeLimit = value;
        }
	}



	/// <summary>
	/// This constructs a quiz object with an empty questions list, but with
	/// the given category and title.
	/// </summary>
	/// <param name="category">The name of the category of this Quiz</param>
	/// <param name="title">The title of this quiz.</param>
	public Quiz(string category, string title, int timeLimit)
	{
		// Check for null or empty category or title.
		if(String.IsNullOrEmpty(category) || String.IsNullOrEmpty(title))
			throw new ArgumentNullException("The category and title must both be non-null and non-empty");

		Category = category;
		Title = title;
		TimeLimit = timeLimit;
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
