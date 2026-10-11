import { useEffect, useState } from 'react'
import { Pencil, Trash2 } from 'lucide-react'
import { Link } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'
import { commentsApi, getApiError, likesApi, postsApi } from '../services/api'
import type { CommentDTO, LikeDTO, PostDTO } from '../types'
import { LoadingLine, Notice } from './TerminalShell'
import { CommentForm } from './CommentForm'
import { CommentList } from './CommentList'
import { PostActions } from './PostActions'
import { UserAvatar } from './UserAvatar'
import { ExpandableText } from './ExpandableText'

function formatDate(value: string) {
  return new Intl.DateTimeFormat('pt-BR', { dateStyle: 'medium', timeStyle: 'short' }).format(new Date(value))
}

function wasEdited(post: PostDTO) {
  if (!post.updatedAt) return false
  const updatedAt = Date.parse(post.updatedAt)
  const createdAt = Date.parse(post.createdAt)
  return Number.isFinite(updatedAt) && Number.isFinite(createdAt) && updatedAt - createdAt > 1000
}

export function PostCard({ post, author, isOwnPost, onChanged }: {
  post: PostDTO
  author: string
  isOwnPost: boolean
  onChanged: () => Promise<void>
}) {
  const { user } = useAuth()
  const [comments, setComments] = useState<CommentDTO[]>([])
  const [commentCount, setCommentCount] = useState(0)
  const [liked, setLiked] = useState(false)
  const [likeCount, setLikeCount] = useState(0)
  const [commentText, setCommentText] = useState('')
  const [editing, setEditing] = useState(false)
  const [editText, setEditText] = useState(post.content)
  const [showComments, setShowComments] = useState(false)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')
  const commentInputId = `comment-${post.postId}`

  useEffect(() => {
    let active = true
    Promise.all([
      commentsApi.list(post.postId),
      likesApi.list(post.postId),
    ]).then(([commentResponse, likeResponse]) => {
      if (!active) return
      setComments(commentResponse.data.items)
      setCommentCount(commentResponse.data.totalCount)
      setLiked(likeResponse.data.items.some((like: LikeDTO) => like.userId === user?.id))
      setLikeCount(likeResponse.data.totalCount)
    }).catch((requestError: unknown) => {
      if (active) setError(getApiError(requestError))
    })
    return () => { active = false }
  }, [post.postId, user?.id])

  async function reloadInteractions() {
    const [commentResponse, likeResponse] = await Promise.all([
      commentsApi.list(post.postId),
      likesApi.list(post.postId),
    ])
    setComments(commentResponse.data.items)
    setCommentCount(commentResponse.data.totalCount)
    setLiked(likeResponse.data.items.some((like) => like.userId === user?.id))
    setLikeCount(likeResponse.data.totalCount)
  }

  async function toggleLike() {
    if (!user) return
    setBusy(true)
    setError('')
    try {
      if (liked) await likesApi.remove(user.id, post.postId)
      else await likesApi.add({ userId: user.id, postId: post.postId })
      await reloadInteractions()
    } catch (requestError) {
      setError(getApiError(requestError))
    } finally {
      setBusy(false)
    }
  }

  async function addComment(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const content = commentText.trim()
    if (!content || !user) return
    setBusy(true)
    setError('')
    try {
      await commentsApi.create({ userId: user.id, postId: post.postId, content })
      setCommentText('')
      setShowComments(true)
      await reloadInteractions()
    } catch (requestError) {
      setError(getApiError(requestError))
    } finally {
      setBusy(false)
    }
  }

  async function saveEdit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const content = editText.trim()
    if (!content) return
    setBusy(true)
    setError('')
    try {
      await postsApi.update({ ...post, content })
      setEditing(false)
      await onChanged()
    } catch (requestError) {
      setError(getApiError(requestError))
    } finally {
      setBusy(false)
    }
  }

  async function deletePost() {
    if (!window.confirm('Excluir esta publicação?')) return
    setBusy(true)
    setError('')
    try {
      await postsApi.remove(post.postId)
      await onChanged()
    } catch (requestError) {
      setError(getApiError(requestError))
    } finally {
      setBusy(false)
    }
  }

  return (
    <article className="timeline-post" data-post-id={post.postId}>
      <UserAvatar name={author} />
      <div className="timeline-post-main">
        <header className="post-header">
          <div className="post-identity">
            <Link to={`/profiles/${post.userId}`} className="post-author">{author}</Link>
            <span className="post-meta">@{author} · <time dateTime={post.createdAt}>{formatDate(post.createdAt)}</time>{wasEdited(post) && <span> · editado</span>}</span>
          </div>
          {isOwnPost && <div className="post-owner-actions">
            <button className="icon-button" onClick={() => { setEditText(post.content); setEditing((value) => !value) }} aria-label="Editar publicação" title="Editar"><Pencil aria-hidden="true" /></button>
            <button className="icon-button danger-link" onClick={() => void deletePost()} disabled={busy} aria-label="Excluir publicação" title="Excluir"><Trash2 aria-hidden="true" /></button>
          </div>}
        </header>

        {editing ? <form className="edit-form" onSubmit={(event) => void saveEdit(event)}>
        <label className="visually-hidden" htmlFor={`edit-${post.postId}`}>Editar texto da publicação</label>
          <textarea id={`edit-${post.postId}`} value={editText} onChange={(event) => setEditText(event.target.value)} rows={4} required />
          <div className="inline-actions"><button className="terminal-button" disabled={busy}>Salvar</button><button type="button" className="link-button" onClick={() => setEditing(false)}>Cancelar</button></div>
        </form> : <ExpandableText key={post.content} className="post-content" text={post.content} />}

        <PostActions liked={liked} likeCount={likeCount} commentCount={commentCount} busy={busy} onLike={() => void toggleLike()} onComments={() => setShowComments((value) => !value)} commentsExpanded={showComments} />

        {showComments && <section className="comments-area" aria-label={`Comentários de ${author}`}>
          <CommentForm inputId={commentInputId} value={commentText} disabled={busy} onChange={setCommentText} onSubmit={(event) => { void addComment(event) }} />
          <CommentList comments={comments} currentUserId={user?.id || ''} currentUserName={user?.name || 'Você'} />
        </section>}
        {error && <Notice>{error}</Notice>}
      </div>
    </article>
  )
}

export function PostList({ posts, loading, error, onChanged }: {
  posts: PostDTO[]
  loading: boolean
  error: string
  onChanged: () => Promise<void>
}) {
  const { user } = useAuth()
  if (loading) return <LoadingLine>carregando publicações...</LoadingLine>
  if (error) return <Notice>{error}</Notice>
  if (!posts.length) return <p className="empty-line">// ainda não há publicações por aqui</p>
  return <div className="post-list">{posts.map((post) => <PostCard
    key={post.postId}
    post={post}
    author={post.userId === user?.id ? user.name : post.userName || `id:${post.userId.slice(0, 8)}`}
    isOwnPost={post.userId === user?.id}
    onChanged={onChanged}
  />)}</div>
}