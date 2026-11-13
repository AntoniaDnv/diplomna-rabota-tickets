import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './App.jsx'
import './index.css'

// The entry point: finds <div id="root"> in index.html and renders the App component in it.
// StrictMode only adds extra checks and warnings during development.
createRoot(document.getElementById('root')).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
