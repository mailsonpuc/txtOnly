import axios from 'axios'
import { useEffect, useState } from 'react'
import type { ReactNode } from 'react'
import { AuthContext } from './AuthContext'
import type { AuthContextValue } from './AuthContext'
import { authApi, usersApi } from '../services/api'
import type { IdentityUser, TokenDTO, UserDTO } from '../types'

const profileRequests = new Map<string, Promise<UserDTO>>()

function readIdentity(token: string): IdentityUser {
  const payload = token.split('.')[1]
  if (!payload) throw new Error('A API retornou um token inválido.')
  const normalized = payload.replace(/-/g, '+').replace(/_/g, '/')
  const binary = atob(normalized.padEnd(Math.ceil(normalized.length / 4) * 4, '='))
  const bytes = Uint8Array.from(binary, (character) => character.charCodeAt(0))
  const claims = JSON.parse(new TextDecoder().decode(bytes)) as Record<string, string>
  const id = claims.sub || claims.nameid || claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']
  if (!id) throw new Error('O token da API não contém o identificador do usuário.')
  return {
    id,
    name: claims.name || claims.unique_name || claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || id,
    email: claims.email || '',
  }
}

function ensureProfile(user: IdentityUser): Promise<UserDTO> {
  const existingRequest = profileRequests.get(user.id)
  if (existingRequest) return existingRequest
  const request = usersApi.get(user.id).then(({ data }) => data).catch((error: unknown) => {
    if (!axios.isAxiosError(error) || error.response?.status !== 404) throw error
    return usersApi.create({ userId: user.id, bio: '' }).then(({ data }) => data)
  })
  profileRequests.set(user.id, request)
  void request.then(() => profileRequests.delete(user.id), () => profileRequests.delete(user.id))
  return request
}

function storeToken(token: TokenDTO) {
  localStorage.setItem('txtonly.accessToken', token.accessToken)
  localStorage.setItem('txtonly.refreshToken', token.refreshToken)
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<IdentityUser | null>(null)
  const [profile, setProfile] = useState<UserDTO | null>(null)
  const [loading, setLoading] = useState(() => Boolean(localStorage.getItem('txtonly.accessToken')))

  useEffect(() => {
    let active = true
    const token = localStorage.getItem('txtonly.accessToken')
    if (!token) return () => { active = false }

    const restore = async () => {
      try {
        const identity = readIdentity(token)
        const userProfile = await ensureProfile(identity)
        if (active) {
          setUser(identity)
          setProfile(userProfile)
        }
      } catch {
        if (active) {
          localStorage.removeItem('txtonly.accessToken')
          localStorage.removeItem('txtonly.refreshToken')
        }
      } finally {
        if (active) setLoading(false)
      }
    }
    void restore()
    return () => { active = false }
  }, [])

  async function signIn(email: string, password: string) {
    const { data } = await authApi.login({ email, password })
    storeToken(data)
    const identity = readIdentity(data.accessToken)
    try {
      const userProfile = await ensureProfile(identity)
      setUser(identity)
      setProfile(userProfile)
    } catch (error) {
      localStorage.removeItem('txtonly.accessToken')
      localStorage.removeItem('txtonly.refreshToken')
      throw error
    }
  }

  async function register(userName: string, email: string, password: string) {
    await authApi.register({ userName, email, password })
    await signIn(email, password)
  }

  function signOut() {
    localStorage.removeItem('txtonly.accessToken')
    localStorage.removeItem('txtonly.refreshToken')
    setUser(null)
    setProfile(null)
  }

  function updateProfile(nextProfile: UserDTO) {
    setProfile(nextProfile)
  }

  const value: AuthContextValue = { user, profile, loading, signIn, register, signOut, updateProfile }
  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}