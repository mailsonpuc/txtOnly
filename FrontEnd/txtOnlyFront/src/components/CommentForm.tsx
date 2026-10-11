import { Send } from 'lucide-react'

export function CommentForm({ inputId, value, disabled, onChange, onSubmit }: {
  inputId: string
  value: string
  disabled: boolean
  onChange: (value: string) => void
  onSubmit: (event: React.FormEvent<HTMLFormElement>) => void
}) {
  return (
    <form className="comment-form" onSubmit={onSubmit}>
      <label className="visually-hidden" htmlFor={inputId}>Escreva um comentário</label>
      <input id={inputId} value={value} onChange={(event) => onChange(event.target.value)} placeholder="Escreva uma resposta..." maxLength={500} />
      <button className="comment-submit" disabled={disabled || !value.trim()} aria-label="Enviar comentário" title="Enviar"><Send aria-hidden="true" /></button>
    </form>
  )
}