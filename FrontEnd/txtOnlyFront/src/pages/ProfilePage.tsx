import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'
import { LoadingLine, Notice } from '../components/TerminalShell'
import { PostList } from '../components/PostCard'
import { FollowButton } from '../components/FollowButton'
import { UserProfile } from '../components/UserProfile'
import { followsApi, getApiError, listAllFollowing, postsApi, usersApi } from '../services/api'
import type { PostDTO, UserDTO } from '../types'

export function ProfilePage({ own = false }: { own?: boolean }) {
  const { user, profile: ownProfile, updateProfile } = useAuth()
  const { userId } = useParams()
  const targetId = own ? user?.id : userId
  const isOwnProfile = targetId === user?.id
  const [profile, setProfile] = useState<UserDTO | null>(null)
  const [posts, setPosts] = useState<PostDTO[]>([])
  const [postCount, setPostCount] = useState(0)
  const [followers, setFollowers] = useState(0)
  const [followingCount, setFollowingCount] = useState(0)
  const [isFollowing, setIsFollowing] = useState(false)
  const [bioDraft, setBioDraft] = useState('')
  const [editingBio, setEditingBio] = useState(false)
  const [loading, setLoading] = useState(true)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')

  async function loadProfile() {
    if (!targetId || !user) return
    setLoading(true)
    setError('')
    try {
      const [profileResponse, postResponse, followerResponse, followingResponse, myFollowingResponse] = await Promise.all([
        isOwnProfile && ownProfile ? Promise.resolve({ data: ownProfile }) : usersApi.get(targetId),
        postsApi.byUser(targetId, 1, 10),
        followsApi.followers(targetId, 1, 100),
        followsApi.following(targetId, 1, 100),
        listAllFollowing(user.id),
      ])
      setProfile(profileResponse.data)
      setBioDraft(profileResponse.data.bio)
      setPosts(postResponse.data.items)
      setPostCount(postResponse.data.totalCount)
      setFollowers(followerResponse.data.totalCount)
      setFollowingCount(followingResponse.data.totalCount)
      setIsFollowing(myFollowingResponse.some((follow) => follow.followingId === targetId))
    } catch (requestError) {
      setError(getApiError(requestError))
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    let active = true
    if (!targetId || !user) return () => { active = false }
    Promise.all([
      isOwnProfile && ownProfile ? Promise.resolve({ data: ownProfile }) : usersApi.get(targetId),
      postsApi.byUser(targetId, 1, 10),
      followsApi.followers(targetId, 1, 100),
      followsApi.following(targetId, 1, 100),
      listAllFollowing(user.id),
    ]).then(([profileResponse, postResponse, followerResponse, followingResponse, myFollowingResponse]) => {
      if (!active) return
      setProfile(profileResponse.data)
      setBioDraft(profileResponse.data.bio)
      setPosts(postResponse.data.items)
      setPostCount(postResponse.data.totalCount)
      setFollowers(followerResponse.data.totalCount)
      setFollowingCount(followingResponse.data.totalCount)
      setIsFollowing(myFollowingResponse.some((follow) => follow.followingId === targetId))
      setError('')
    }).catch((requestError: unknown) => {
      if (active) {
        setProfile(null)
        setError(getApiError(requestError))
      }
    }).finally(() => {
      if (active) setLoading(false)
    })
    return () => { active = false }
  }, [targetId, user, ownProfile, isOwnProfile])

  async function saveBio(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!profile) return
    setBusy(true)
    setError('')
    try {
      const { data } = await usersApi.update({ ...profile, bio: bioDraft.trim() })
      setProfile(data)
      updateProfile(data)
      setEditingBio(false)
    } catch (requestError) {
      setError(getApiError(requestError))
    } finally {
      setBusy(false)
    }
  }

  async function toggleFollow() {
    if (!user || !targetId) return
    setBusy(true)
    setError('')
    try {
      if (isFollowing) await followsApi.remove(user.id, targetId)
      else await followsApi.add(user.id, targetId)
      await loadProfile()
    } catch (requestError) {
      setError(getApiError(requestError))
    } finally {
      setBusy(false)
    }
  }

  if (loading || (profile && profile.userId !== targetId)) return <LoadingLine>abrindo perfil...</LoadingLine>
  if (error && !profile) return <div className="page-stack"><Notice>{error}</Notice><p className="empty-line">// perfil indisponível</p></div>
  if (!profile || !targetId) return <p className="empty-line">// perfil não encontrado</p>
  const displayName = isOwnProfile ? user?.name || profile.userName : profile.userName || 'Perfil de usuário'

  return (
    <div className="page-stack">
      <UserProfile name={displayName} bio={profile.bio} postCount={postCount} followers={followers} following={followingCount}>
        {editingBio ? <form className="bio-form" onSubmit={(event) => void saveBio(event)}>
          <label htmlFor="profile-bio">Editar bio</label><textarea id="profile-bio" value={bioDraft} onChange={(event) => setBioDraft(event.target.value)} maxLength={200} rows={3} />
          <div className="inline-actions"><button className="terminal-button" disabled={busy}>Salvar bio</button><button type="button" className="link-button" onClick={() => { setBioDraft(profile.bio); setEditingBio(false) }}>Cancelar</button><span>{bioDraft.length}/200</span></div>
        </form> : isOwnProfile ? <button className="link-button profile-edit-button" onClick={() => setEditingBio(true)}>Editar perfil</button> : <FollowButton following={isFollowing} busy={busy} onClick={() => void toggleFollow()} />}
      </UserProfile>
      {error && <Notice>{error}</Notice>}
      <div className="subsection-heading"><h3>publicações</h3><span>{postCount} total</span></div>
      <PostList posts={posts} loading={false} error="" onChanged={loadProfile} />
    </div>
  )
}