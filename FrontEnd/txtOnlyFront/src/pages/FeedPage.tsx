import { useEffect, useState } from 'react'
import { PostComposer } from '../components/PostComposer'
import { LoadingLine, Notice } from '../components/TerminalShell'
import { PostList } from '../components/PostCard'
import { getApiError, postsApi } from '../services/api'
import type { PostDTO } from '../types'

export function FeedPage() {
  const [posts, setPosts] = useState<PostDTO[]>([])
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  async function loadFeed() {
    setLoading(true)
    setError('')
    try {
      const { data } = await postsApi.list(page, 10)
      setPosts(data.items)
      setTotalPages(data.totalPages)
    } catch (requestError) {
      setError(getApiError(requestError))
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    let active = true
    postsApi.list(page, 10).then(({ data }) => {
      if (!active) return
      setPosts(data.items)
      setTotalPages(data.totalPages)
      setError('')
    }).catch((requestError: unknown) => {
      if (active) setError(getApiError(requestError))
    }).finally(() => {
      if (active) setLoading(false)
    })
    return () => { active = false }
  }, [page])

  return (
    <div className="page-stack">
      <div className="feed-intro"><span>Publicações recentes</span><span>{loading ? 'carregando...' : `${posts.length} nesta página`}</span></div>
      <PostComposer onCreated={async () => { if (page !== 1) { setLoading(true); setPage(1) } else await loadFeed() }} />
      {error && <Notice>{error}</Notice>}
      <PostList posts={posts} loading={loading} error="" onChanged={loadFeed} />
      {!loading && totalPages > 1 && <nav className="pagination" aria-label="Paginação do feed">
        <button className="link-button" onClick={() => { setLoading(true); setPage((current) => Math.max(1, current - 1)) }} disabled={page === 1}>[ anterior ]</button>
        <span>página {page}/{totalPages}</span>
        <button className="link-button" onClick={() => { setLoading(true); setPage((current) => Math.min(totalPages, current + 1)) }} disabled={page === totalPages}>[ próxima ]</button>
      </nav>}
      {loading && posts.length === 0 && <LoadingLine />}
    </div>
  )
}