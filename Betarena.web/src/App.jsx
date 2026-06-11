import { useState, useEffect, useCallback } from 'react'
import BetForm from './components/BetForm'
import StatsPanel from './components/StatsPanel'
import './App.css'

function Toast({ toasts }) {
  return (
    <div className="toast-container">
      {toasts.map((t) => (
        <div key={t.id} className={`toast ${t.type}`}>
          <span className="toast-icon">{t.type === 'success' ? '✅' : '❌'}</span>
          <div className="toast-text">
            <div className="toast-title">{t.title}</div>
            <div className="toast-message">{t.message}</div>
          </div>
        </div>
      ))}
    </div>
  )
}

export default function App() {
  const [stats, setStats] = useState(null)
  const [statsLoading, setStatsLoading] = useState(false)
  const [toasts, setToasts] = useState([])

  const addToast = useCallback((toast) => {
    const id = Date.now()
    setToasts((prev) => [...prev, { ...toast, id }])
    setTimeout(() => setToasts((prev) => prev.filter((t) => t.id !== id)), 4000)
  }, [])

  const fetchStats = useCallback(async () => {
    setStatsLoading(true)
    try {
      const res = await fetch('/stats')
      if (!res.ok) throw new Error(`Error ${res.status}`)
      setStats(await res.json())
    } catch (err) {
      addToast({ type: 'error', title: 'Failed to load stats', message: err.message })
    } finally {
      setStatsLoading(false)
    }
  }, [addToast])

  useEffect(() => { fetchStats() }, [fetchStats])

  const handleBetPlaced = useCallback((toast) => {
    addToast(toast)
    if (toast.type === 'success') fetchStats()
  }, [addToast, fetchStats])

  return (
    <div className="app-shell">
      <header className="header">
        <div className="header-brand">
          <div className="header-logo">🎰</div>
          <span className="header-title">Bet<span>Arena</span></span>
        </div>
        <span className="header-badge">Dashboard</span>
      </header>

      <main className="main-content">
        <BetForm onBetPlaced={handleBetPlaced} />
        <StatsPanel stats={stats} loading={statsLoading} onRefresh={fetchStats} />
      </main>

      <Toast toasts={toasts} />
    </div>
  )
}

