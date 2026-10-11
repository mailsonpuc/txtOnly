import { UserAvatar } from './UserAvatar'

export function UserProfile({ name, bio, postCount, followers, following, children }: {
  name: string
  bio: string
  postCount: number
  followers: number
  following: number
  children: React.ReactNode
}) {
  return (
    <section className="profile-panel ascii-profile">
      <div className="profile-frame-label" aria-hidden="true"><span>┌─</span><span>PERFIL</span><span className="profile-frame-rule" /><span>┐</span></div>
      <header className="profile-identity"><UserAvatar name={name} size="large" /><div><h2>{name}</h2><span>@{name}</span></div></header>
      <p className="profile-bio-text">{bio || 'Ainda sem bio.'}</p>
      <div className="profile-stats"><span><strong>{postCount}</strong> publicações</span><span><strong>{followers}</strong> seguidores</span><span><strong>{following}</strong> seguindo</span></div>
      <div className="user-profile-actions">{children}</div>
    </section>
  )
}