import type { ReactNode } from 'react'
import { ArrowUpRight } from 'lucide-react'
import { Link, Outlet, useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'
import { Sidebar } from './Sidebar'
import { UserAvatar } from './UserAvatar'

export function TerminalShell() {
  const { user, profile, signOut } = useAuth()
  const navigate = useNavigate()

  function exitSession() {
    signOut()
    navigate('/login', { replace: true })
  }

  return (
    <main className="app-shell">
      <Sidebar name={user?.name || 'usuário'} onSignOut={exitSession} />
      <div className="social-columns">
        <section className="feed-column">
          <header className="feed-header"><span className="feed-kicker">txtOnly <span>/</span> texto puro</span></header>
          <div className="route-content"><Outlet /></div>
        </section>
        <aside className="right-rail" aria-label="Seu perfil e informações">
          <section className="right-profile">
            <span className="rail-label">SUA CONTA</span>
            <div className="right-profile-user"><UserAvatar name={user?.name || 'usuário'} size="large" /><div><strong>{user?.name}</strong><span>@{user?.name}</span></div></div>
            <p>{profile?.bio || 'Uma rede feita para ideias em texto.'}</p>
            <Link to="/profiles">Ver perfil <ArrowUpRight aria-hidden="true" /></Link>
          </section>
          <section className="right-note"><span className="rail-label">TXT / ONLY</span><p>Menos ruído. Mais espaço para ideias.</p><span>Somente texto, sempre.</span></section>
        </aside>
      </div>
    </main>
  )
}

export function Notice({ children, tone = 'error' }: { children: ReactNode; tone?: 'error' | 'success' | 'info' }) {
  return <p className={`notice notice-${tone}`} role={tone === 'error' ? 'alert' : 'status'}>{children}</p>
}

export function LoadingLine({ children = 'consultando a API...' }: { children?: ReactNode }) {
  return <p className="loading-line" role="status"><span aria-hidden="true">·</span> {children}</p>
}