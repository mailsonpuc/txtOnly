import { useState } from 'react'
import { PenLine } from 'lucide-react'
import { useAuth } from '../auth/AuthContext'
import { getApiError, postsApi } from '../services/api'
import { LoadingLine, Notice } from './TerminalShell'
import { UserAvatar } from './UserAvatar'

export function PostComposer({ onCreated }: { onCreated: () => Promise<void> }) {
  const { user } = useAuth()
  const [content, setContent] = useState('')
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')

  async function submit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const text = content.trim()
    if (!text || !user) return
    setBusy(true)
    setError('')
    try {
      await postsApi.create({ userId: user.id, content: text })
      setContent('')
      await onCreated()
    } catch (requestError) {
      setError(getApiError(requestError))
    } finally {
      setBusy(false)
    }
  }

  return (
    <form className="composer" onSubmit={submit}>
      <div className="composer-header"><span>Compartilhe uma ideia</span><code>somente texto</code></div>
      <div className="composer-input-row"><UserAvatar name={user?.name || 'usuário'} /><label className="visually-hidden" htmlFor="new-post">O que você está pensando?</label>
        <textarea id="new-post" value={content} onChange={(event) => setContent(event.target.value)} maxLength={2000} rows={3} placeholder="O que você está pensando?" required />
      </div>
      <div className="composer-footer"><span aria-label={`${content.length} de 2000 caracteres`}>{content.length} / 2000 caracteres</span><button className="compose-submit" type="submit" disabled={busy || !content.trim()}><PenLine aria-hidden="true" />{busy ? 'Publicando...' : 'Publicar'}</button></div>
      {busy && <LoadingLine>gravando publicação...</LoadingLine>}
      {error && <Notice>{error}</Notice>}
    </form>
  )
}