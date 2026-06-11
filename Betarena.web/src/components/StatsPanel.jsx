export default function StatsPanel({ stats, loading, onRefresh }) {
  return (
    <div className="card">
      <div className="card-header">
        <div className="card-title">
          <div className="card-icon blue">📊</div>
          Statistics
        </div>
        <button className="btn btn-secondary" onClick={onRefresh} disabled={loading}>
          {loading ? <><span className="spinner" style={{ borderTopColor: 'var(--clr-text)' }} /> Loading…</> : '↻ Refresh'}
        </button>
      </div>

      <div className="card-body">
        {loading && !stats ? (
          <div className="empty-state">
            <div className="empty-icon">⏳</div>
            Loading statistics…
          </div>
        ) : (
          <div className="stats-sections">
            {/* Games section */}
            <section>
              <div className="section-label">🎮 Games — RTP</div>
              <div className="table-wrapper">
                {stats?.games?.length > 0 ? (
                  <table className="table">
                    <thead>
                      <tr>
                        <th>Game</th>
                        <th>RTP</th>
                        <th>Performance</th>
                      </tr>
                    </thead>
                    <tbody>
                      {stats.games.map((g, i) => (
                        <tr key={i}>
                          <td>{g.game}</td>
                          <td className="val-rtp">{Number(g.rtp).toFixed(2)}%</td>
                          <td>
                            <RtpBar value={g.rtp} />
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                ) : (
                  <div className="empty-state">
                    <div className="empty-icon">🎮</div>
                    No game data yet. Register some bets first.
                  </div>
                )}
              </div>
            </section>

            {/* Users section */}
            <section>
              <div className="section-label">👤 Players — Totals</div>
              <div className="table-wrapper">
                {stats?.users?.length > 0 ? (
                  <table className="table">
                    <thead>
                      <tr>
                        <th>User ID</th>
                        <th>Total Stake</th>
                        <th>Total Win</th>
                        <th>Net P&L</th>
                      </tr>
                    </thead>
                    <tbody>
                      {stats.users.map((u, i) => {
                        const pnl = u.totalWin - u.totalStake
                        return (
                          <tr key={i}>
                            <td className="val-number">#{u.userId}</td>
                            <td className="val-number">${Number(u.totalStake).toFixed(2)}</td>
                            <td className="val-win">${Number(u.totalWin).toFixed(2)}</td>
                            <td style={{ color: pnl >= 0 ? 'var(--clr-success)' : 'var(--clr-error)', fontWeight: 600 }}>
                              {pnl >= 0 ? '+' : ''}${pnl.toFixed(2)}
                            </td>
                          </tr>
                        )
                      })}
                    </tbody>
                  </table>
                ) : (
                  <div className="empty-state">
                    <div className="empty-icon">👤</div>
                    No user data yet.
                  </div>
                )}
              </div>
            </section>
          </div>
        )}
      </div>
    </div>
  )
}

function RtpBar({ value }) {
  const pct = Math.min(100, Math.max(0, Number(value)))
  const color = pct >= 95 ? 'var(--clr-success)' : pct >= 50 ? 'var(--clr-primary)' : 'var(--clr-error)'
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
      <div style={{ flex: 1, height: 6, background: 'var(--clr-border)', borderRadius: 4, overflow: 'hidden' }}>
        <div style={{ width: `${pct}%`, height: '100%', background: color, borderRadius: 4, transition: 'width 0.4s ease' }} />
      </div>
    </div>
  )
}
