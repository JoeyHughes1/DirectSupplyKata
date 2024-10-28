using System;

/// <summary>
/// This manages references to and organizes all of the Quizzes that can be accessed.
/// Acts as the API between the Client code and the Quizzes, giving out quizzes and
/// allowing editing of Quizzes.
/// </summary>
public class QuizManager
{
    /// <summary>
    /// The QuizManager instance the entire backend will be using.
    /// </summary>
    public static readonly QuizManager SharedManager = new QuizManager();

    /// <summary>
    /// This class is really a container for a list of Quizzes, with another property to hold
    /// the category name that all of those Quizzes share. This is useful for being able to display Quizzes by their categories.
    /// </summary>
    private class CategoryList
	{
		/// <summary>
		/// The string of the Category for this list of Quizzes.
		/// </summary>
		public string Category { get; private set; }

		/// <summary>
		/// The list of Quizzes that share this Category.
		/// </summary>
		public Dictionary<string, Quiz> Quizzes { get; private set; }

		/// <summary>
		/// Constructs a new CategoryList with the given category.
		/// Initializes the list to be empty. If the category is null or empty,
		/// throws an ArgumentNullException
		/// </summary>
		/// <param name="category">The name of the category for this list.</param>
		/// <exception cref="ArgumentNullException">If the category name is null or empty.</exception>
		public CategoryList(string category)
		{
			// Make sure the category isn't null
			// (I'm getting a compiler message that ArgumentNullException usually takes
			// just the name of the parameter that was null. I'm used to putting a message
			// there. For now I will keep it like this, but I'm curious if that is actually the norm)
			if (String.IsNullOrEmpty(category))
				throw new ArgumentNullException("Category cannot be null.");

			// Set the category
			Category = category;

			// Initialize the list of Quizzes.
			Quizzes = [];
		}
	}

	/// <summary>
	/// The dictionary of CategoryLists. The key is the category name, the value
	/// is the CategoryList object.
	/// </summary>
	private Dictionary<string, CategoryList> MapCategoryLists = [];



    /// <summary>
    /// This will return true if a quiz with the given
    /// category and title already exists in the data
    /// structure.
    /// </summary>
    /// <param name="category">The category of the quiz.</param>
    /// <param name="title">The title of the quiz.</param>
    /// <returns>True if it already exists, false if it does not.</returns>
    /// <exception cref="ArgumentNullException">If either the category or the title given are null or empty.</exception>
    private bool QuizExists(string category, string title)
	{
        // Make sure the category and title are not null or empty.
        if (String.IsNullOrEmpty(category) || String.IsNullOrEmpty(title)) return false;

        // Try to get the category list for the category. If doesn't exist, return false.
        if (!MapCategoryLists.TryGetValue(category, out CategoryList? listWithQuiz)) return false;

		// Return if the category list has an entry for that title. If it does, it exists.
		return listWithQuiz.Quizzes.ContainsKey(title);
    }



	/// <summary>
	/// A property for grabbing a list of all of the categories.
	/// </summary>
	public List<string> Categories {
		get => [.. MapCategoryLists.Keys];
	}



    /// <summary>
    /// Gets all the quizzes in the given category.
    /// </summary>
    /// <param name="category">The category to get the quizzes from.</param>
    /// <returns>A List with all of the Quiz objects.</returns>
	/// <exception cref="ArgumentNullException">If either the category or the title given are null or empty.</exception>
    public List<string> GetQuizzesInCategory(string category) 
	{
        // If the category is null or empty, return an empty list.
        if (String.IsNullOrEmpty(category)) return [];

        // If the category cannot be found, then it has no quizzes, so return an empty list.
        if (!MapCategoryLists.TryGetValue(category, out CategoryList? listWithQuizzes)) return [];

		// Otherwise, return a list with all the quiz objects
		return [.. listWithQuizzes.Quizzes.Keys];
	}



    /// <summary>
    /// Gets the Quiz in the given category with the given title.
    /// If there is no such quiz, returns null.
    /// </summary>
    /// <param name="category">The category the quiz has.</param>
    /// <param name="title">The title of the quiz.</param>
    /// <returns>The Quiz object, or null if it wasn't found.</returns>
    public async Task<Quiz?> GetQuiz(string category, string title)
	{
        // Make sure the category and title are not null or empty.
        if (String.IsNullOrEmpty(category) || String.IsNullOrEmpty(title)) return null;

        // Try to get the CategoryList. If there is none, return null.
        if (!MapCategoryLists.TryGetValue(category, out CategoryList? listWithQuiz)) return null;

		// Otherwise, we have the list.
		// Now check if the quiz is in the list. If not, return null
		if (!listWithQuiz.Quizzes.TryGetValue(title, out Quiz? quiz)) return null;

        // It's found. Now check if it's a random quiz. If so, randomize it first before returning
        if (quiz is RandomQuiz randQuiz)
            await randQuiz.RandomizeQuestions();

		return quiz;
    }



	/// <summary>
	/// This will attempt to add a new Quiz to the manager. This will fail
	/// if there already is a quiz with the same category and title.
	/// </summary>
	/// <param name="quiz">The Quiz object to add.</param>
	/// <returns>
	/// True if it was added successfully. 
	/// False if a quiz with the same category and title already existed.
	/// </returns>
	public bool AddQuiz(Quiz quiz)
	{
		// If the quiz is null, we ain't adding that.
		if(quiz == null) return false;

		// The CategoryList we are adding to
		CategoryList listToAddTo;

		// If a category list for this category list doesn't already exist, make one and add it to the dictionary.
		if (!MapCategoryLists.ContainsKey(quiz.Category))
		{
			// Create the new CategoryList, add it to the map.
			listToAddTo = new CategoryList(quiz.Category);
			MapCategoryLists[quiz.Category] = listToAddTo;
		} else
		{
            // If list exists already, just grab it from the dictionary
            listToAddTo = MapCategoryLists[quiz.Category];

			// Since the list already exists, we do a check if a quiz with this title already exists.
			// If one does, we throw an exception.
			if (listToAddTo.Quizzes.ContainsKey(quiz.Title))
				return false;
        }

		// Then, since this is a unique new quiz, we add it to the Dictionary and we're done.
		listToAddTo.Quizzes[quiz.Title] = quiz;
		return true;
	}



    /// <summary>
    /// Tries to remove the quiz with the given category and title from the manager.
    /// If there was no quiz of the sort, returns false. Otherwise, returns true.
    /// </summary>
    /// <param name="category">The category of the quiz to remove.</param>
    /// <param name="title">The title of the quiz to remove.</param>
    /// <returns>True if the quiz was found and removed. False if it was not found.</returns>
    public bool RemoveQuiz(string category, string title) => RemoveQuiz(category, title, true);



    /// <summary>
    /// Tries to remove the quiz with the given category and title from the manager.
    /// If there was no quiz of the sort, returns false. Otherwise, returns true.
    /// The last argument determines whether we remove the CategoryList on deleting its
    /// last quiz or not.
    /// </summary>
    /// <param name="category">The category of the quiz to remove.</param>
    /// <param name="title">The title of the quiz to remove.</param>
    /// <param name="removeEmptyCategoryList">If true, then will remove empty lists. If false, we won't.</param>
    /// <returns>True if the quiz was found and removed. False if it was not found.</returns>
    private bool RemoveQuiz(string category, string title, bool removeEmptyCategoryList)
	{
		// Make sure the category and title are not null or empty.
		if(String.IsNullOrEmpty(category) || String.IsNullOrEmpty(title)) return false;

		// If there is no list for that category, can't find it.
		if (!MapCategoryLists.TryGetValue(category, out CategoryList? listWithQuiz)) return false;

		// Otherwise, we have the list.
		// Now check if the quiz is in the list.
		if (listWithQuiz.Quizzes.TryGetValue(title, out Quiz? quizToRemove))
		{
			// Check the quiz that we want to remove. If it is a random quiz, we can't remove it.
			if (quizToRemove is RandomQuiz) return false;
		}
		else return false; // If it wasn't in the list, return false

		// Try to Remove it. We should be able to. If we can't, return false.
		if (!listWithQuiz.Quizzes.Remove(title)) return false;

		// If we got here, then we removed it from the category.
		// If we are removing empty category lists, then we make the check
		if(removeEmptyCategoryList && listWithQuiz.Quizzes.Count == 0)
		{
			// Once I add file support, this is where I would delete the folder.
			// Now, we remove the empty category list from the map
			MapCategoryLists.Remove(category);
		}

		// We succesfully removed the quiz, so we return true.
		return true;
    }



	/// <summary>
	/// Do an "Edit" of a quiz, which means removing the quiz as it was before and adding a new Quiz.
	/// This is because we are, for now, treating quizzes as immutable from the Client's perspective,
	/// so to "Edit" a quiz, you remove the quiz as it was before and add a new one.
	/// </summary>
	/// <param name="category">The category of the old Quiz.</param>
	/// <param name="title">The title of the old Quiz.</param>
	/// <param name="newQuiz">The Quiz after being edited.</param>
	/// <returns>
	/// True if it was removed and added successfully. 
	/// False if the operation couldn't go through, and nothing was changed.
	/// </returns>
	public bool EditQuiz(string category, string title, Quiz newQuiz)
	{
		if (String.IsNullOrEmpty(category) || String.IsNullOrEmpty(title)) return false;

		// If the title and category before and after the edit are the same, then
		// add will always succeed, since we just removed what would've made it fail
		if(category.Equals(newQuiz.Category) && title.Equals(newQuiz.Title))
		{
            // Try to remove the old Quiz. If failed, then no harm no foul, return false.
			// This marks to not go through the trouble of removing the categoryList, since
			// we know we are about to add right back to it.
            if (!RemoveQuiz(category, title, false)) return false;

            // This should always return true at this point.
            return AddQuiz(newQuiz);
        }

        // We need to avoid removing a quiz and then failing to add.
        // AddQuiz only fails when a quiz with the new quiz's information already
        // exists. So, if we have changed a title or a category, we make sure that add
        // will not fail beforehand.
        // If there already is a quiz with the information of the new quiz,
        // we would've failed the AddQuiz after already removing, which is not good. Return false now.
        if (QuizExists(newQuiz.Category, newQuiz.Title)) return false;

        // Try to remove the old Quiz. If failed, then no harm no foul, return false.
        if (!RemoveQuiz(category, title)) return false;

        // This should always return true at this point.
        return AddQuiz(newQuiz);

		// It is probably possible to condense this function into just one big boolean return, but 
		// that would be unreadable, so I'm leaving it like this.
    }


	
	/// <summary>
	/// This generates a new QuizManager. Initially, it populates it's category lists and quizzes with the
	/// random quizzes that OpenTriviaDB offers. TODO: Add file backups of custom quizzes.
	/// </summary>
    public QuizManager()
	{

		// We want to initialize the Random Quizzes for all the categories from the OpenTDB
		foreach (string category in RandomQuiz.OpenTriviaDB_CategoryToIDMap.Keys)
		{
			// For every category that can have a random quiz
			// Create the CategoryList, add it to the map
			CategoryList list = new CategoryList(category);
			MapCategoryLists.Add(category, list);

			// Create a RandomQuiz and add it to the list
			RandomQuiz quiz = new RandomQuiz(category);
			list.Quizzes.Add(quiz.Title, quiz);
		}
	}
}
