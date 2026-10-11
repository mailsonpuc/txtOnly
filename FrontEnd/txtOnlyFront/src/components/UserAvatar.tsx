export function UserAvatar({ name, size = 'regular' }: { name: string; size?: 'regular' | 'large' }) {
  const initial = Array.from(name.trim())[0]?.toLocaleUpperCase('pt-BR') || '?'
  return <span className={`user-avatar user-avatar-${size}`} aria-hidden="true">{initial}</span>
}