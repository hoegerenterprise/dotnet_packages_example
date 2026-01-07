import type { LoginRequest, LoginResponse, RegisterRequest, User, UserGroup, Product, Customer, Order } from '@/types'

const API_BASE_URL = '/api/v1'

class ApiError extends Error {
  constructor(public status: number, message: string) {
    super(message)
    this.name = 'ApiError'
  }
}

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: 'An error occurred' }))
    throw new ApiError(response.status, error.message || `HTTP ${response.status}`)
  }
  return response.json()
}

function getAuthHeaders(): HeadersInit {
  const token = localStorage.getItem('jwt_token')
  return {
    'Content-Type': 'application/json',
    ...(token && { 'Authorization': `Bearer ${token}` })
  }
}

export const authApi = {
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    const response = await fetch(`${API_BASE_URL}/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(credentials)
    })
    return handleResponse<LoginResponse>(response)
  },

  async register(userData: RegisterRequest): Promise<User> {
    const response = await fetch(`${API_BASE_URL}/auth/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(userData)
    })
    return handleResponse<User>(response)
  }
}

export const usersApi = {
  async getAll(): Promise<User[]> {
    const response = await fetch(`${API_BASE_URL}/users`, {
      headers: getAuthHeaders()
    })
    return handleResponse<User[]>(response)
  },

  async getById(id: number): Promise<User> {
    const response = await fetch(`${API_BASE_URL}/users/${id}`, {
      headers: getAuthHeaders()
    })
    return handleResponse<User>(response)
  },

  async update(id: number, data: Partial<User>): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/users/${id}`, {
      method: 'PUT',
      headers: getAuthHeaders(),
      body: JSON.stringify(data)
    })
    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: 'An error occurred' }))
      throw new ApiError(response.status, error.message || `HTTP ${response.status}`)
    }
  },

  async delete(id: number): Promise<void> {
    const response = await fetch(`${API_BASE_URL}/users/${id}`, {
      method: 'DELETE',
      headers: getAuthHeaders()
    })
    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: 'An error occurred' }))
      throw new ApiError(response.status, error.message || `HTTP ${response.status}`)
    }
  }
}

export const userGroupsApi = {
  async getAll(): Promise<UserGroup[]> {
    const response = await fetch(`${API_BASE_URL}/usergroups`, {
      headers: getAuthHeaders()
    })
    return handleResponse<UserGroup[]>(response)
  }
}

export const productsApi = {
  async getAll(): Promise<Product[]> {
    const response = await fetch(`${API_BASE_URL}/products`)
    return handleResponse<Product[]>(response)
  }
}

export const customersApi = {
  async getAll(): Promise<Customer[]> {
    const response = await fetch(`${API_BASE_URL}/customers`)
    return handleResponse<Customer[]>(response)
  }
}

export const ordersApi = {
  async getAll(): Promise<Order[]> {
    const response = await fetch(`${API_BASE_URL}/orders`)
    return handleResponse<Order[]>(response)
  }
}
