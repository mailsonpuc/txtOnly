import { useState } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'
import { getApiError } from '../services/api'
import { Notice } from '../components/TerminalShell'

export function AuthPage({ mode }: { mode: 'login' | 'register' }) {
  const { user, signIn, register } = useAuth()
  const navigate = useNavigate()
  const [userName, setUserName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')
  const creatingAccount = mode === 'register'

  if (user) return <Navigate to="/feed" replace />

  async function submit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setBusy(true)
    setError('')
    try {
      if (creatingAccount) await register(userName.trim(), email.trim(), password)
      else await signIn(email.trim(), password)
      navigate('/feed', { replace: true })
    } catch (requestError) {
      setError(getApiError(requestError))
    } finally {
      setBusy(false)
    }
  }

  return (
    <main className="auth-screen">
      <section className="auth-terminal">
        <p className="auth-kicker">GNU/Linux · sessão segura</p>
        <h1>txt<span>Only</span></h1>
        <p className="auth-prompt"><span>guest@txtOnly</span>:~$ {creatingAccount ? 'useradd' : 'login'}</p>
        <form className="auth-form" onSubmit={(event) => void submit(event)}>
          {creatingAccount && <label>username<input autoComplete="username" value={userName} onChange={(event) => setUserName(event.target.value)} required maxLength={50} /></label>}
          <label>email<input type="email" autoComplete="email" value={email} onChange={(event) => setEmail(event.target.value)} required /></label>
          <label>password<input type="password" autoComplete={creatingAccount ? 'new-password' : 'current-password'} value={password} onChange={(event) => setPassword(event.target.value)} required minLength={creatingAccount ? 6 : undefined} /></label>
          {error && <Notice>{error}</Notice>}
          <button className="terminal-button auth-submit" disabled={busy}>{busy ? '[ conectando... ]' : creatingAccount ? '[ criar conta ]' : '[ entrar ]'}</button>
        </form>
        <p className="auth-switch">{creatingAccount ? 'já tem usuário?' : 'novo por aqui?'} <Link to={creatingAccount ? '/login' : '/register'}>{creatingAccount ? 'login' : 'criar conta'}</Link></p>
      </section>
    </main>
  )
}