using System;
using System.Text.Json.Serialization;
using QuizKata.Server;


/// <summary>
/// The Quiz object holds a list of Questions, a category, and a title.
/// The category and title can only be changed by going through the QuizManager.
/// </summary>
public class Quiz
{
    /// <summary>
    /// The list of Question objects for this quiz.
    /// This needs to have a public set to allow for JSON serialization into a Quiz to work.
    /// </summary>
    public List<Question> Questions { get; set; } = [];

    /// <summary>
    /// The underlying category for this Quiz.
    /// </summary>
    private string _category;

    /// <summary>
    /// The Category of the Quiz. 
    /// </summary>
    public string Category 
    {
        get => _category;
        private set
        {
            ArgumentNullException.ThrowIfNullOrEmpty(value);
            _category = value;
        }
    }

    /// <summary>
    /// The underlying title of this Quiz.
    /// </summary>
    private string _title;

    /// <summary>
    /// The title of this Quiz.
    /// </summary>
    public string Title
    {
        get => _title;
        private set
        {
            ArgumentNullException.ThrowIfNullOrEmpty(value);
            _title = value;
        }
    }

    /// <summary>
    /// The underlying integer time value for the TimeLimit property.
    /// </summary>
    private protected int _timeLimit;

    /// <summary>
    /// The time limit in seconds for this Quiz.
    /// </summary>
    public int TimeLimit 
	{
		get => _timeLimit;
		private protected set
		{
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
            _timeLimit = value;
        }
	}



    /// <summary>
    /// This constructs a quiz object with an empty questions list, and with
    /// the given category, title, and time limit.
    /// </summary>
    /// <param name="category">The name of the category of this Quiz</param>
    /// <param name="title">The title of this quiz.</param>
    /// <param name="timeLimit">The time limit for this Quiz.</param>
    public Quiz(string category, string title, int timeLimit) : this([], category, title, timeLimit) { }




    /// <summary>
    /// This constructs a quiz object with an existing questions list, and with
    /// the given category, title, and time limit.
    /// </summary>
    /// <param name="questions">The list of Questions to give this Quiz.</param>
    /// <param name="category">The name of the category of this Quiz</param>
    /// <param name="title">The title of this quiz.</param>
    /// <param name="timeLimit">The time limit for this Quiz.</param>
    [JsonConstructor]
    public Quiz(List<Question> questions, string category, string title, int timeLimit)
    {
        Questions = questions;
        Category = category;
        Title = title;
        TimeLimit = timeLimit;
    }
}
