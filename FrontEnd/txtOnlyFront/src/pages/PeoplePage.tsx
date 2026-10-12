import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { FollowButton } from '../components/FollowButton'
import { useAuth } from '../auth/AuthContext'
import { LoadingLine, Notice } from '../components/TerminalShell'
import { followsApi, getApiError, listAllFollowing, usersApi } from '../services/api'
import type { UserDTO } from '../types'

export function PeoplePage() {
  const { user } = useAuth()
  const [people, setPeople] = useState<UserDTO[]>([])
  const [following, setFollowing] = useState<Set<string>>(new Set())
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [loading, setLoading] = useState(true)
  const [busyId, setBusyId] = useState('')
  const [error, setError] = useState('')
  const [query, setQuery] = useState('')

  useEffect(() => {
    let active = true
    if (!user) return () => { active = false }
    setLoading(true)
    const request = query.trim()
      ? usersApi.filterByName(query, page, 20)
      : usersApi.list(page, 20)
    Promise.all([
        request,
        listAllFollowing(user.id),
    ]).then(([usersResponse, followingList]) => {
      if (!active) return
      setPeople(usersResponse.data.items.filter((person) => person.userId !== user.id))
      setTotalPages(usersResponse.data.totalPages)
      setFollowing(new Set(followingList.map((follow) => follow.followingId)))
      setError('')
    }).catch((requestError: unknown) => {
      if (active) setError(getApiError(requestError))
    }).finally(() => {
      if (active) setLoading(false)
    })
    return () => { active = false }
  }, [page, user, query])

  async function toggleFollow(targetId: string) {
    if (!user) return
    setBusyId(targetId)
    setError('')
    try {
      if (following.has(targetId)) await followsApi.remove(user.id, targetId)
      else await followsApi.add(user.id, targetId)
      setFollowing((current) => {
        const next = new Set(current)
        if (next.has(targetId)) next.delete(targetId)
        else next.add(targetId)
        return next
      })
    } catch (requestError) {
      setError(getApiError(requestError))
    } finally {
      setBusyId('')
    }
  }

  return (
    <div className="page-stack">
      <p className="directory-intro">Conheça as pessoas que escrevem por aqui.</p>
      <form className="search-bar" onSubmit={(event) => { event.preventDefault(); setPage(1) }}>
        <input
          type="text"
          placeholder="pesquisar por nome de usuário"
          value={query}
          onChange={(event) => { setQuery(event.target.value); setPage(1) }}
        />
        <button type="submit">[ buscar ]</button>
      </form>

      {error && <Notice>{error}</Notice>}
      {loading ? <LoadingLine>listando usuários...</LoadingLine> : people.length === 0 ? <p className="empty-line">// nenhum outro perfil encontrado</p> : <ul className="people-list">
        {people.map((person) => <li className="person-row" key={person.userId}>
          <div className="person-info"><Link to={`/profiles/${person.userId}`} className="person-id">{person.userName || person.userId}</Link><p>{person.bio || 'Ainda sem bio.'}</p></div>
          <FollowButton following={following.has(person.userId)} busy={busyId === person.userId} onClick={() => void toggleFollow(person.userId)} />
        </li>)}
      </ul>}
      {!loading && totalPages > 1 && <nav className="pagination" aria-label="Paginação de usuários">
        <button className="link-button" onClick={() => { setLoading(true); setPage((current) => Math.max(1, current - 1)) }} disabled={page === 1}>[ anterior ]</button>
        <span>página {page}/{totalPages}</span>
        <button className="link-button" onClick={() => { setLoading(true); setPage((current) => Math.min(totalPages, current + 1)) }} disabled={page === totalPages}>[ próxima ]</button>
      </nav>}
    </div>
  )
}