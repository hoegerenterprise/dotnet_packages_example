import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi } from '@/services/api'
import type { LoginRequest, RegisterRequest } from '@/types'

export const useAuthStore = defineStore('auth', () => {
  // State
  const token = ref<string | null>(localStorage.getItem('jwt_token'))
  const username = ref<string | null>(localStorage.getItem('username'))
  const email = ref<string | null>(localStorage.getItem('email'))
  const groups = ref<string[]>(JSON.parse(localStorage.getItem('groups') || '[]'))
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const isAuthenticated = computed(() => !!token.value)
  const isAdmin = computed(() => groups.value.includes('Administrators'))
  const isManager = computed(() => groups.value.includes('Managers') || isAdmin.value)

  // Actions
  async function login(credentials: LoginRequest) {
    loading.value = true
    error.value = null
    try {
      const response = await authApi.login(credentials)

      // Store token and user info
      token.value = response.token
      username.value = response.username
      email.value = response.email
      groups.value = response.groups

      // Persist to localStorage
      localStorage.setItem('jwt_token', response.token)
      localStorage.setItem('username', response.username)
      localStorage.setItem('email', response.email)
      localStorage.setItem('groups', JSON.stringify(response.groups))

      return true
    } catch (err: any) {
      error.value = err.message || 'Login failed'
      return false
    } finally {
      loading.value = false
    }
  }

  async function register(userData: RegisterRequest) {
    loading.value = true
    error.value = null
    try {
      await authApi.register(userData)
      return true
    } catch (err: any) {
      error.value = err.message || 'Registration failed'
      return false
    } finally {
      loading.value = false
    }
  }

  function logout() {
    // Clear state
    token.value = null
    username.value = null
    email.value = null
    groups.value = []
    error.value = null

    // Clear localStorage
    localStorage.removeItem('jwt_token')
    localStorage.removeItem('username')
    localStorage.removeItem('email')
    localStorage.removeItem('groups')
  }

  return {
    // State
    token,
    username,
    email,
    groups,
    loading,
    error,

    // Getters
    isAuthenticated,
    isAdmin,
    isManager,

    // Actions
    login,
    register,
    logout
  }
})
