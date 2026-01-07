export interface User {
  id: number
  username: string
  email: string
  firstName: string
  lastName: string
  isActive: boolean
  createdAt: string
  lastLoginAt: string | null
  groups: string[]
}

export interface LoginRequest {
  username: string
  password: string
}

export interface RegisterRequest {
  username: string
  email: string
  password: string
  firstName: string
  lastName: string
}

export interface LoginResponse {
  token: string
  username: string
  email: string
  groups: string[]
  expiresAt: string
}

export interface UserGroup {
  id: number
  name: string
  description: string
  createdAt: string
  memberCount: number
}

export interface Product {
  id: number
  name: string
  description: string
  price: number
  category: string
}

export interface Customer {
  id: number
  name: string
  email: string
  registeredDate: string
}

export interface Order {
  id: number
  customerId: number
  customerName: string
  productId: number
  productName: string
  quantity: number
  orderDate: string
  totalAmount: number
}
