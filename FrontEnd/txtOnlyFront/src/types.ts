export interface TokenDTO {
  accessToken: string
  refreshToken: string
  expiration: string
}

export interface UserDTO {
  userId: string
  userName: string
  bio: string
  createdAt: string
}

export interface PostDTO {
  postId: string
  content: string
  createdAt: string
  updatedAt: string | null
  isDeleted: boolean
  userId: string
  userName: string
}

export interface CommentDTO {
  commentId: string
  content: string
  createdAt: string
  updatedAt: string | null
  postId: string
  userId: string
  userName: string
}

export interface LikeDTO {
  likeId: string
  userId: string
  postId: string
  createdAt: string
}

export interface FollowDTO {
  followId: string
  followerId: string
  followingId: string
  createdAt: string
}

export interface PageDTO<T> {
  items: T[]
  currentPage: number
  pageSize: number
  totalPages: number
  totalCount: number
}

export interface IdentityUser {
  id: string
  name: string
  email: string
}