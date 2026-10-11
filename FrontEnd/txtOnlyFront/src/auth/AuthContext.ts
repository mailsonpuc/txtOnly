import { createContext, useContext } from 'react'
import type { IdentityUser, UserDTO } from '../types'

export interface AuthContextValue {
  user: IdentityUser | null
  profile: UserDTO | null
  loading: boolean
  signIn: (email: string, password: string) => Promise<void>
  register: (userName: string, email: string, password: string) => Promise<void>
  signOut: () => void
  updateProfile: (profile: UserDTO) => void
}

export const AuthContext = createContext<AuthContextValue | null>(null)

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) throw new Error('useAuth precisa estar dentro de AuthProvider.')
  return context
}