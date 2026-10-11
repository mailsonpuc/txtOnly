import { Heart, MessageCircle } from 'lucide-react'

export function PostActions({ liked, likeCount, commentCount, busy, onLike, onComments, commentsExpanded }: {
  liked: boolean
  likeCount: number
  commentCount: number
  busy: boolean
  onLike: () => void
  onComments: () => void
  commentsExpanded: boolean
}) {
  return (
    <div className="post-actions" aria-label="Ações da publicação">
      <button className={`post-action${liked ? ' is-active' : ''}`} onClick={onLike} disabled={busy} aria-label={liked ? `Descurtir publicação, ${likeCount} curtidas` : `Curtir publicação, ${likeCount} curtidas`} aria-pressed={liked} title={liked ? 'Descurtir' : 'Curtir'}><Heart aria-hidden="true" /><span>{likeCount}</span></button>
      <button className="post-action" onClick={onComments} aria-expanded={commentsExpanded} aria-label={`Comentar, ${commentCount} comentários`} title="Comentar"><MessageCircle aria-hidden="true" /><span>{commentCount}</span></button>
    </div>
  )
}