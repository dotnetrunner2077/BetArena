import { useState } from 'react'

const GAMES = ['Roulette', 'Blackjack', 'Slots', 'Poker', 'Baccarat']

const initialForm = { userId: '', gameName: GAMES[0], stake: '', winAmount: '' }

export default function BetForm({ onBetPlaced }) {
  const [form, setForm] = useState(initialForm)
  const [loading, setLoading] = useState(false)
  const [lastBet, setLastBet] = useState(null)

  const set = (field) => (e) => setForm((f) => ({ ...f, [field]: e.target.value }))

  const handleSubmit = async (e) => {
    e.preventDefault()
    setLoading(true)
    try {
      const body = {
        userId: Number(form.userId),
        gameName: form.gameName,
        stake: Number(form.stake),
        winAmount: Number(form.winAmount),
      }
      const res = await fetch('/bets', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body),
      })
      if (!res.ok) {
        const err = await res.json()
        throw new Error(err.error || `Error ${res.status}`)
      }
      const data = await res.json()
      setLastBet(data)
      setForm(initialForm)
      onBetPlaced({ type: 'success', title: 'Bet registered!', message: `Game: ${data.gameName} — Result: ${data.result}` })
    } catch (err) {
      onBetPlaced({ type: 'error', title: 'Failed to register bet', message: err.message })
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="card">
      <div className="card-header">
        <div className="card-title">
          <div className="card-icon purple">🎲</div>
          Register Bet
        </div>
      </div>
      <div className="card-body">
        <form className="form" onSubmit={handleSubmit}>
          <div className="field">
            <label htmlFor="userId">User ID</label>
            <input
              id="userId"
              type="number"
              min="1"
              placeholder="e.g. 1"
              value={form.userId}
              onChange={set('userId')}
              required
            />
          </div>

          <div className="field">
            <label htmlFor="gameName">Game</label>
            <select id="gameName" value={form.gameName} onChange={set('gameName')}>
              {GAMES.map((g) => <option key={g} value={g}>{g}</option>)}
            </select>
          </div>

          <div className="form-row">
            <div className="field">
              <label htmlFor="stake">Stake ($)</label>
              <input
                id="stake"
                type="number"
                min="0.01"
                step="0.01"
                placeholder="100.00"
                value={form.stake}
                onChange={set('stake')}
                required
              />
            </div>
            <div className="field">
              <label htmlFor="winAmount">Win Amount ($)</label>
              <input
                id="winAmount"
                type="number"
                min="0"
                step="0.01"
                placeholder="0.00"
                value={form.winAmount}
                onChange={set('winAmount')}
                required
              />
            </div>
          </div>

          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? <><span className="spinner" /> Registering…</> : '🎯 Register Bet'}
          </button>
        </form>

        {lastBet && (
          <div className="last-bet">
            <div className="last-bet-title">Last Registered Bet</div>
            <div className="last-bet-grid">
              <div className="last-bet-item">
                <span className="lbi-label">User</span>
                <span className="lbi-value">#{lastBet.userId}</span>
              </div>
              <div className="last-bet-item">
                <span className="lbi-label">Game</span>
                <span className="lbi-value">{lastBet.gameName}</span>
              </div>
              <div className="last-bet-item">
                <span className="lbi-label">Stake</span>
                <span className="lbi-value">${lastBet.stake.toFixed(2)}</span>
              </div>
              <div className="last-bet-item">
                <span className="lbi-label">Won</span>
                <span className="lbi-value" style={{ color: lastBet.winAmount > 0 ? 'var(--clr-success)' : 'var(--clr-error)' }}>
                  ${lastBet.winAmount.toFixed(2)}
                </span>
              </div>
              <div className="last-bet-item" style={{ alignItems: 'center', flexDirection: 'row', gap: 8 }}>
                <span className="lbi-label">Result</span>
                <span className={`result-badge ${lastBet.result.toLowerCase()}`}>{lastBet.result}</span>
              </div>
              <div className="rtp-highlight" style={{ gridColumn: '1/-1' }}>
                <span className="rtp-label">Game RTP after this bet</span>
                <span className="rtp-value">{lastBet.gameRtp.toFixed(2)}%</span>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  )
}
