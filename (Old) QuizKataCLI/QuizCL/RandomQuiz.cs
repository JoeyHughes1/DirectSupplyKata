using System;
using System.Net.Http.Json;
using System.Text;
using System.Web;


/// <summary>
/// The RandomQuiz class extends the Quiz class, adding the functionality to grab Questions from the 
/// OpenTriviaDB API. The Category must be one of the OpenTriviaDB Categories.
/// </summary>
public class RandomQuiz : Quiz
{
	/// <summary>
	/// Realistically, the URL for the API call should not grow above this amount of characters.
	/// A normal call with category and difficulty, with 2 digit category and amount on medium
	/// difficulty only reaches 67.
	/// </summary>
	private const int REALISTIC_URL_MAX = 70;

	/// <summary>
	/// The limit on how many questions we can request from the API. I doubt, any
	/// categories could even handle this much, but I want to at least keep it within this.
	/// </summary>
	private const int QUESTION_LIMIT = 100;

	/// <summary>
	/// Enum for the possible difficulties of the OpenTriviaDB quizzes.
	/// </summary>
    public enum Difficulty
    {
        Easy,
		Medium,
		Hard,
		Any
    }

    /// <summary>
    /// This is a record class for deserializing the OpenTDB JSON.
    /// This represents one question that was returned
    /// </summary>
    /// <param name="type">The type of question, multiple choice or true/false.</param>
    /// <param name="difficulty">The difficulty of the question. Correspond to the Difficulty enum.</param>
    /// <param name="category">The category of the question. Will match the Category stored in this RandomQuiz.</param>
    /// <param name="question">The question prompt.</param>
    /// <param name="correct_answer">The string of the correct answer.</param>
    /// <param name="incorrect_answers">The list of incorrect answer strings.</param>
    private record class OpenTDBQuestion(
		string? type = null,
		string? difficulty = null,
		string? category = null,
		string? question = null,
		string? correct_answer = null,
		List<string?>? incorrect_answers = null);
	
	/// <summary>
	/// This is a record class for deserializing the OpenTDB JSON.
	/// This represents the whole response. The whole response first gives a 
	/// response code which, for our purposes, will either be:
	/// 0: Success, all went well
	/// 1: Could not return results because number of questions was too high.
	/// 2: Invalid parameter in the url.
	/// 5: Rate limited. Too many requests too fast.
	/// </summary>
	/// <param name="response_code"></param>
	/// <param name="results"></param>
	private record class OpenTDBResponse(
		int? response_code = null,
		List<OpenTDBQuestion?>? results = null);

	/// <summary>
	/// Indexing into this array with the Difficulties enum value will give the string to place in the URL.
	/// </summary>
	private static readonly string[] difficultyStrings = { "easy", "medium", "hard" };

    /// <summary>
    /// The map from OpenTriviaDB Category Names to IDs used in the URL.
    /// <remark> 
	/// This is hardcoded for now, but it is possible to have this be checked as the program starts,
    /// in the case that OpenTriviaDB Updates their Category mapping. For now, this is fine.
	/// </remark>
    /// </summary>
    public static readonly Dictionary<string, int> OpenTriviaDB_CategoryToIDMap = new Dictionary<string, int>()
	{
        {"General Knowledge", 9},
		{"Entertainment: Books", 10},
		{"Entertainment: Film", 11},
		{"Entertainment: Music", 12},
		{"Entertainment: Musicals & Theatres", 13},
		{"Entertainment: Television", 14},
		{"Entertainment: Video Games", 15},
		{"Entertainment: Board Games", 16},
		{"Science & Nature", 17},
		{"Science: Computers", 18},
		{"Science: Mathematics", 19},
		{"Mythology", 20},
		{"Sports", 21},
		{"Geography", 22},
		{"History", 23},
		{"Politics", 24},
		{"Art", 25},
		{"Celebrities", 26},
		{"Animals", 27},
		{"Vehicles", 28},
		{"Entertainment: Comics", 29},
		{"Science: Gadgets", 30},
		{"Entertainment: Japanese Anime & Manga", 31},
		{"Entertainment: Cartoon & Animations", 32}
	};

	/// <summary>
	/// The HttpClient that interfaces with opentdb.com
	/// </summary>
	private static HttpClient OpenTriviaDBClient = new()
	{
		BaseAddress = new Uri("https://opentdb.com")
	};



	/// <summary>
	/// Checks the given category to see if it is one of the Open Trivia DB
	/// categories. If it isn't, an ArgumentException is thrown.
	/// </summary>
	/// <param name="category">The category to check.</param>
	/// <returns>The category that was given in, if it was valid.</returns>
	/// <exception cref="ArgumentException">If the given category is not one of the Open Trivia DB Categories.</exception>
	private static string ValidateCategory(string category)
	{
        if (!OpenTriviaDB_CategoryToIDMap.ContainsKey(category))
            throw new ArgumentException("Category is not one of the OpenTriviaDB Quiz Categories");
		return category;
    }



    /// <summary>
    /// Constructs a new RandomQuiz with an empty Question list with the given 
    /// category. The category must be one of the OpenTriviaDB Categories, or else
    /// an ArgumentException is thrown. The title is set to "Random Quiz".
    /// </summary>
    /// <param name="category">The category of the quiz. Must be one of the OpenTriviaDB categories.</param>
    /// <exception cref="ArgumentException">If the given category is not one of the Open Trivia DB Categories.</exception>
    public RandomQuiz(string category) : base(ValidateCategory(category), "Random Quiz", 1)
	{
	}



	/// <summary>
	/// This makes an HTTP request to the Open Trivia DB API to get the given number
	/// of questions (or default 10) in this Quiz's category (With either the given difficulty or the default medium).
	/// It then adds those questions to this Quiz's Question list.
	/// </summary>
	/// <param name="numQuestions">The number of questions to get. Default 10.</param>
	/// <param name="difficulty">The difficulty of the questions. Defaults to Difficulty.Any </param>
	/// <exception cref="ArgumentOutOfRangeException">If the number of questions is negative or above 100.</exception>
	public async Task RandomizeQuestions(int numQuestions = 10, Difficulty difficulty = Difficulty.Any)
	{
		// First, we check our number of questions. It should be positive and within our limit
		if (numQuestions < 0 || numQuestions > QUESTION_LIMIT)
			throw new ArgumentOutOfRangeException("Number of questions should be a small positive number (<= 100).");

		// First, we get the URL string ready
		// Make a StringBuilder to build our URL string.
		StringBuilder urlBuilder = new StringBuilder("api.php?amount=", REALISTIC_URL_MAX);

		// Add our amount of questions
		urlBuilder.Append($"{numQuestions}&category=");

		// Add our category number
		urlBuilder.Append($"{OpenTriviaDB_CategoryToIDMap[Category]}");

		// If we are giving a difficulty, add it
		if(difficulty != Difficulty.Any)
		{
			urlBuilder.Append($"&difficulty={ difficultyStrings[(int)difficulty] }");
        }

		// Get the final url from our builder.
		string url = urlBuilder.ToString();

		// We have the static HttpClient, so we use it to send a request.
		OpenTDBResponse? response = await OpenTriviaDBClient.GetFromJsonAsync<OpenTDBResponse>(url);


		// Handle if null response
		if (response == null)
		{
			Console.WriteLine("The response from OpenTDB was null.");
			return;
		}
		// Handle a non-success response code and give a fitting message to the console before returning.
		if (response.response_code != 0) 
		{
			switch (response.response_code)
			{
				case 1:
					Console.WriteLine("Number of questions was too high.");
					return;
				case 2:
					Console.WriteLine("The URL had an invalid parameter.");
					return;
				case 5:
					Console.WriteLine("Too many requests too fast. Try again in a few seconds.");
					return;
				default:
					Console.WriteLine($"We got a weird non-zero response code of: {response.response_code}");
					return;
			}
		}

		// If our list of questions is null, something went wrong.
		if(response.results == null)
		{
			Console.WriteLine("Our questions list was null.");
			return;
		}

		// If we are here, we had a response code of 0. We should have our questions in the list.
		// Now, before we start adding to our list, we clear out the current list of questions.
		Questions.Clear();

		// Loop through the list and make Questions and add them to our list.
		foreach (OpenTDBQuestion? question in response.results)
		{
			// If anything in our question that we need is null, then go to the next question
			if(question == null || question.question == null || question.correct_answer == null
					|| question.incorrect_answers == null)
			{
				Console.WriteLine("Something null in OpenTBDQuestion.");
				continue;
			}

			// Try creating adding the question to our list. If there was an error, write to console and continue
			try
			{
				// Decode the HTML encodings in each answer
				string?[] incorrectAnswers = [.. question.incorrect_answers];
				for (int i = 0; i < incorrectAnswers.Length; i++)
					incorrectAnswers[i] = HttpUtility.HtmlDecode(incorrectAnswers[i]);
                
				// Add a new Question to this Quiz with the given information, decoding all the HTML encodings.
                AddQuestion(HttpUtility.HtmlDecode(question.question),
                    HttpUtility.HtmlDecode(question.correct_answer), incorrectAnswers);

			} catch (Exception ) { Console.WriteLine("We had a bad parameter in creating a Question object."); }
		}

		// We have added the questions we got to our list, now set a valid time limit
		// according to the difficulty and number of questions. 
		int mult = 3;
		if (difficulty != Difficulty.Any) mult = 3 - (int)difficulty;
        TimeLimit = numQuestions * 10 * mult;
    }
}
