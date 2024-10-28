
import { useEffect, useState } from 'react';
import './App.css';

function App() {
    // Our quiz state to hold our quiz.
    const [quiz, setQuiz] = useState();

    // The effect to run at startup to do some tests and to grab a random quiz.
    useEffect(() => {
        populateQuizData();
    }, []);

    // If the quiz didn't load, something went wrong, otherwise set the contents to our quiz.
    const contents = quiz === undefined
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
        :
        // The contents of the page with our quiz
        <div>
            <h2>{quiz.title}</h2>
            <p>Category: {quiz.category}</p>
            <table className="table table-striped" aria-labelledby="tableLabel">
                <thead>
                    <tr>
                        <th>Question</th>
                        <th>Correct Answer</th>
                        <th>Incorrect Answers</th>
                    </tr>
                </thead>
                <tbody>
                    {quiz.questions.map(question =>
                        <tr>
                            <td>{question.prompt}</td>
                            <td>{question.correctAnswer}</td>
                            {question.incorrectAnswers.map(choice => <td>{choice}</td>)}
                        </tr>
                    )}
                </tbody>
            </table>
        </div>;

    // Return the page with a label and our Quiz contents.
    return (
        <div>
            <h1 id="tableLabel">Quiz Program</h1>
            <p>This component demonstrates fetching data from the server with the API and getting a random Quiz.</p>
            {contents}
        </div>
    );

    // This function runs some tests to show off the API, then grabs a random quiz.
    async function populateQuizData() {

        // Add an initial Custom Quiz to show AddQuiz
        let response = await fetch("quizapi/AddQuiz", {
            method: "POST",
            body: JSON.stringify({
                questions: [{
                    prompt: "What is the antimatter equivalent of an electron?",
                    correctAnswer: "Positron",
                    incorrectAnswers: ["Antitron", "Megatron"]
                }],
                category: "Science",
                Title: "Epic Quiz",
                timeLimit: 300
            }),
            headers: {
                "Content-Type": "application/json",
            },
        });
        let data = await response.json();
        if (data == false) { // Make sure we added successfully
            console.log("Something went wrong, Couldn't add custom quiz.");
        }

        // Get the custom quiz we just made to show GetQuiz
        response = await fetch("quizapi/GetQuiz?category=Science&Title=Epic%20Quiz");
        data = await response.json();
        if (data.questions[0].correctAnswer != "Positron") { // Make sure we got the right quiz.
            console.log("The getting went wrong. It's not right.");
        }

        // Edit the custom quiz to show EditQuiz
        response = await fetch("quizapi/EditQuiz?category=Science&Title=Epic%20Quiz", {
            method: "PUT",
            body: JSON.stringify({
                questions: [{
                    prompt: "What is the antimatter equivalent of an electron?",
                    correctAnswer: "Positron",
                    incorrectAnswers: ["Antitron", "Megatron", "Countertron"]
                }],
                category: "Physics",
                Title: "Epic Quiz",
                timeLimit: 20
            }),
            headers: {
                "Content-Type": "application/json",
            },
        });
        data = await response.json();
        if (data == false) { // Make sure we edited successfully
            console.log("Something went wrong with editing the quiz.");
        }

        // Remove the quiz to show RemoveQuiz
        response = await fetch("quizapi/RemoveQuiz?category=Physics&Title=Epic%20Quiz", {
            method: "DELETE",
        });
        data = await response.json();
        if (data == false) { // Make sure we deleted successfully
            console.log("Something went wrong with deleting the quiz."); 
        }

        // Get a random quiz to show off
        response = await fetch("quizapi/GetQuiz?category=Mythology&Title=Random%20Quiz");
        data = await response.json();
        setQuiz(data); 
        console.log(data); // log the quiz to look at.
    }
}

export default App;