using Microsoft.AspNetCore.Mvc;

namespace QuizKata.Server.Controllers
{
    /// <summary>
    /// This is the Controller that delegates HTTP requests for the Quiz
    /// backend to the QuizManager.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class QuizAPIController : ControllerBase
    {

        /// <summary>
        /// This is copied from the WeatherForecastController 
        /// base code. Not sure exactly how necessary it is, but I'm keeping it for now.
        /// </summary>
        private readonly ILogger<QuizAPIController> _logger;

        /// <summary>
        /// Constructs a Quiz Controller with the given logger. 
        /// This is copied from the WeatherForecastController.
        /// </summary>
        /// <param name="logger"></param>
        public QuizAPIController(ILogger<QuizAPIController> logger)
        {
            _logger = logger;
        }



        /// <summary>
        /// HttpGet method for getting a list of all the categories of available quizzes.
        /// Will return the list of string names.
        /// </summary>
        /// <returns>The list of string names of the categories.</returns>
        [HttpGet("GetCategories", Name = "GetCategories")]
        public IEnumerable<string> GetCategories()
        {
            IEnumerable<string> rtn;
            lock (QuizManager.SharedManager)
            {
                rtn = QuizManager.SharedManager.Categories;
            }
            return rtn;
        }



        /// <summary>
        /// HttpGet method for getting a list of all the quizzes in a certain category.
        /// The category is given as a query (?category=___).
        /// Returns a list of string names of the quizzes.
        /// </summary>
        /// <param name="category">The name of the category to get quizzes from.</param>
        /// <returns>Returns a list of the Quiz names.</returns>
        [HttpGet("GetQuizzesInCategory", Name = "GetQuizzesInCategory")]
        public IEnumerable<string> GetQuizzesInCategory([FromQuery(Name = "category")] string category)
        {
            IEnumerable<string> rtn;
            lock (QuizManager.SharedManager)
            {
                rtn = QuizManager.SharedManager.GetQuizzesInCategory(category);
            }
            return rtn;
        }



        /// <summary>
        /// HttpGet method to get a Quiz from the QuizManager given the category and title of the quiz.
        /// </summary>
        /// <param name="category">The category of the quiz to get.</param>
        /// <param name="title">The title of the quiz to get.</param>
        /// <returns>The Quiz object that was gotten (or null if it wasn't found).</returns>
        [HttpGet("GetQuiz", Name = "GetQuiz")]
        public Quiz? GetQuiz([FromQuery(Name = "category")] string category, [FromQuery(Name = "title")] string title)
        {
            Quiz? rtn;
            lock (QuizManager.SharedManager)
            {
                rtn = QuizManager.SharedManager.GetQuiz(category, title).Result;
            }
            return rtn;
        }



        /// <summary>
        /// HttpPost method to add the given quiz to the QuizManager.
        /// </summary>
        /// <param name="quiz">The quiz to add.</param>
        /// <returns>True if it was added, false if something went wrong and it was not.</returns>
        [HttpPost("AddQuiz", Name = "AddQuiz")]
        public bool AddQuiz([FromBody] Quiz quiz)
        {
            bool rtn;
            lock (QuizManager.SharedManager)
            {
                rtn = QuizManager.SharedManager.AddQuiz(quiz);
            }
            return rtn;
        }



        /// <summary>
        /// HttpDelete method to remove a quiz from the Quiz manager given the 
        /// category and title.
        /// </summary>
        /// <param name="category">The category of the Quiz to remove.</param>
        /// <param name="title">The title of the Quiz to remove.</param>
        /// <returns>True if the quiz was found and removed, false otherwise.</returns>
        [HttpDelete("RemoveQuiz", Name = "RemoveQuiz")]
        public bool RemoveQuiz([FromQuery(Name = "category")] string category, [FromQuery(Name = "title")] string title)
        {
            bool rtn;
            lock (QuizManager.SharedManager)
            {
                rtn = QuizManager.SharedManager.RemoveQuiz(category, title);
            }
            return rtn;
        }



        /// <summary>
        /// Edits the quiz with the given category and title to have the properties
        /// of the Quiz object passed in.
        /// </summary>
        /// <param name="category">The category of the old quiz.</param>
        /// <param name="title">The title of the old quiz.</param>
        /// <param name="quiz">The new quiz.</param>
        /// <returns>True if the change was made succesfully, false if something was wrong and nothing has been changed.</returns>
        [HttpPut("EditQuiz", Name = "EditQuiz")]
        public bool EditQuiz([FromQuery(Name = "category")] string category, [FromQuery(Name = "title")] string title, [FromBody] Quiz quiz)
        {
            bool rtn;
            lock (QuizManager.SharedManager)
            {
                rtn = QuizManager.SharedManager.EditQuiz(category, title, quiz);
            }
            return rtn;
        }
    }
}
