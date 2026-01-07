import { defineStore } from 'pinia'
import { ref } from 'vue'
import { usersApi } from '@/services/api'
import type { User } from '@/types'

export const useUsersStore = defineStore('users', () => {
  // State
  const users = ref<User[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Actions
  async function fetchUsers() {
    loading.value = true
    error.value = null
    try {
      users.value = await usersApi.getAll()
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch users'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function deleteUser(id: number) {
    loading.value = true
    error.value = null
    try {
      await usersApi.delete(id)
      users.value = users.value.filter(u => u.id !== id)
    } catch (err: any) {
      error.value = err.message || 'Failed to delete user'
      throw err
    } finally {
      loading.value = false
    }
  }

  return {
    users,
    loading,
    error,
    fetchUsers,
    deleteUser
  }
})
