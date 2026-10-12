import axios from 'axios'
import type { InternalAxiosRequestConfig } from 'axios'
import type {
  CommentDTO,
  FollowDTO,
  LikeDTO,
  PageDTO,
  PostDTO,
  TokenDTO,
  UserDTO,
} from '../types'

const apiUrl = (import.meta.env.VITE_API_URL || 'http://localhost:5011/api').replace(/\/+$/, '')

export const apiClient = axios.create({
  baseURL: apiUrl,
  headers: { 'Content-Type': 'application/json' },
})

let refreshInProgress: Promise<TokenDTO> | null = null

interface RetryableRequest extends InternalAxiosRequestConfig {
  _retried?: boolean
}

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('txtonly.accessToken')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

apiClient.interceptors.response.use(undefined, async (error: unknown) => {
  if (!axios.isAxiosError(error) || error.response?.status !== 401 || !error.config) throw error
  const request = error.config as RetryableRequest
  const isAuthRequest = /\/Auth\/(login|register|refresh-token)/i.test(request.url || '')
  const refreshToken = localStorage.getItem('txtonly.refreshToken')
  if (isAuthRequest || request._retried || !refreshToken) throw error

  request._retried = true
  try {
    refreshInProgress ??= axios.post<TokenDTO>(`${apiUrl}/Auth/refresh-token`, { refreshToken })
      .then((response) => response.data)
      .finally(() => { refreshInProgress = null })
    const token = await refreshInProgress
    localStorage.setItem('txtonly.accessToken', token.accessToken)
    localStorage.setItem('txtonly.refreshToken', token.refreshToken)
    request.headers.Authorization = `Bearer ${token.accessToken}`
    return apiClient(request)
  } catch (refreshError) {
    localStorage.removeItem('txtonly.accessToken')
    localStorage.removeItem('txtonly.refreshToken')
    throw refreshError
  }
})

export function getApiError(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as { message?: string; title?: string; detail?: string } | string | undefined
    if (typeof data === 'string' && data.trim()) return data
    if (data && typeof data === 'object' && (data.message || data.title || data.detail)) return data.message || data.detail || data.title || 'Falha na solicitação.'
    if (error.response?.status === 401) return 'Sessão expirada ou credenciais inválidas.'
    if (error.response?.status === 0 || !error.response) return 'Não foi possível conectar à API.'
  }
  return 'Ocorreu um erro inesperado. Tente novamente.'
}

export const authApi = {
  register: (payload: { userName: string; email: string; password: string }) =>
    apiClient.post<{ status: string; message: string }>('/Auth/register', payload),
  login: (payload: { email: string; password: string }) =>
    apiClient.post<TokenDTO>('/Auth/login', payload),
  refresh: (refreshToken: string) =>
    apiClient.post<TokenDTO>('/Auth/refresh-token', { refreshToken }),
}

export const usersApi = {
  get: (id: string) => apiClient.get<UserDTO>(`/Users/${id}`),
  list: (pageNumber = 1, pageSize = 50) =>
    apiClient.get<PageDTO<UserDTO>>('/Users/pagination', { params: { pageNumber, pageSize } }),
  filterByName: (userName: string, pageNumber = 1, pageSize = 50) =>
    apiClient.get<PageDTO<UserDTO>>('/Users/filter-by-name', { params: { userName, pageNumber, pageSize } }),
  create: (payload: { userId: string; bio: string }) => apiClient.post<UserDTO>('/Users', payload),
  update: (user: UserDTO) => apiClient.put<UserDTO>(`/Users/${user.userId}`, user),
}

export const postsApi = {
  list: (pageNumber = 1, pageSize = 10) =>
    apiClient.get<PageDTO<PostDTO>>('/Posts/pagination', { params: { pageNumber, pageSize } }),
  byUser: (userId: string, pageNumber = 1, pageSize = 10) =>
    apiClient.get<PageDTO<PostDTO>>(`/Posts/user/${userId}/pagination`, { params: { pageNumber, pageSize } }),
  create: (payload: { userId: string; content: string }) => apiClient.post<PostDTO>('/Posts', payload),
  update: (post: PostDTO) => apiClient.put<PostDTO>(`/Posts/${post.postId}`, post),
  remove: (postId: string) => apiClient.delete(`/Posts/${postId}`),
}

export const commentsApi = {
  list: (postId: string, pageNumber = 1, pageSize = 10) =>
    apiClient.get<PageDTO<CommentDTO>>(`/Comments/post/${postId}/pagination`, { params: { pageNumber, pageSize } }),
  create: (payload: { userId: string; postId: string; content: string }) => apiClient.post<CommentDTO>('/Comments', payload),
}

export const likesApi = {
  list: (postId: string, pageNumber = 1, pageSize = 50) =>
    apiClient.get<PageDTO<LikeDTO>>(`/Likes/post/${postId}/pagination`, { params: { pageNumber, pageSize } }),
  add: (payload: { userId: string; postId: string }) => apiClient.post<LikeDTO>('/Likes', payload),
  remove: (userId: string, postId: string) => apiClient.delete(`/Likes/user/${userId}/post/${postId}`),
}

export const followsApi = {
  followers: (userId: string, pageNumber = 1, pageSize = 50) =>
    apiClient.get<PageDTO<FollowDTO>>(`/Follows/user/${userId}/followers`, { params: { pageNumber, pageSize } }),
  following: (userId: string, pageNumber = 1, pageSize = 50) =>
    apiClient.get<PageDTO<FollowDTO>>(`/Follows/user/${userId}/following`, { params: { pageNumber, pageSize } }),
  add: (followerId: string, followingId: string) =>
    apiClient.post<FollowDTO>('/Follows', { followerId, followingId }),
  remove: (followerId: string, followingId: string) =>
    apiClient.delete(`/Follows/follower/${followerId}/following/${followingId}`),
}

export async function listAllFollowing(userId: string) {
  const firstPage = await followsApi.following(userId, 1, 100)
  if (firstPage.data.totalPages <= 1) return firstPage.data.items
  const remainingPages = await Promise.all(Array.from(
    { length: firstPage.data.totalPages - 1 },
    (_, index) => followsApi.following(userId, index + 2, 100),
  ))
  return [...firstPage.data.items, ...remainingPages.flatMap((page) => page.data.items)]
}