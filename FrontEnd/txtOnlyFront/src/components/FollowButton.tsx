import { UserRoundCheck, UserRoundPlus } from 'lucide-react'

export function FollowButton({ following, busy = false, onClick }: {
  following: boolean
  busy?: boolean
  onClick: () => void
}) {
  const LabelIcon = following ? UserRoundCheck : UserRoundPlus
  const label = following ? 'Deixar de seguir' : 'Seguir'
  return <button className={`follow-button${following ? ' is-following' : ''}`} onClick={onClick} disabled={busy} aria-pressed={following}>
    <LabelIcon aria-hidden="true" />{busy ? 'Aguarde' : label}
  </button>
}