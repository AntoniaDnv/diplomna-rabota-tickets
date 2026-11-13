import { useEffect, useState } from 'react'
import { getHealth } from './api.js'

// A temporary start page. It only proves that the three containers talk to each other:
// this page (frontend) -> /api/health (backend) -> SQL Server (database).
export default function App() {
  // state: 'loading' at first, then 'ok' or 'error'
  const [state, setState] = useState('loading')
  const [databaseIsUp, setDatabaseIsUp] = useState(false)

  // Runs once, after the page is shown for the first time.
  useEffect(() => {
    getHealth()
      .then((health) => {
        setDatabaseIsUp(health.database)
        setState('ok')
      })
      .catch(() => setState('error'))
  }, [])

  return (
    <main className="page">
      <h1>QueuEase</h1>
      <p>Система за управление на опашки с електронни талони</p>

      <ul className="status">
        <li>Интерфейс: работи</li>
        <li>Сървър: {state === 'loading' ? 'проверка…' : state === 'ok' ? 'работи' : 'няма връзка'}</li>
        <li>База данни: {state === 'loading' ? 'проверка…' : databaseIsUp ? 'работи' : 'няма връзка'}</li>
      </ul>
    </main>
  )
}
