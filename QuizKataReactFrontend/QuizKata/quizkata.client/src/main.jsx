import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.jsx'

// FIXME: Note, I got rid of the StrictMode wrapping, since it was causing an issue with the open trivia API with requesting too fast.
// That should be added back in in a full implementation.
createRoot(document.getElementById('root')).render(
  <App />,
)
