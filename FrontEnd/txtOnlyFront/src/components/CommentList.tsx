import type { CommentDTO } from '../types'
import { ExpandableText } from './ExpandableText'

export function CommentList({ comments, currentUserId, currentUserName }: {
  comments: CommentDTO[]
  currentUserId: string
  currentUserName: string
}) {
  if (!comments.length) return <p className="empty-line">Ainda não há respostas. Comece a conversa.</p>
  return <ul className="comment-list">
    {comments.map((comment) => <li key={comment.commentId}>
      <span className="comment-author">{comment.userId === currentUserId ? currentUserName : comment.userName || `id:${comment.userId.slice(0, 8)}`}</span>
      <ExpandableText className="comment-text" text={comment.content} limit={160} lines={3} />
    </li>)}
  </ul>
}