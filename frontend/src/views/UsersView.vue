<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useUsersStore } from '@/stores/users'
import type { User } from '@/types'

const authStore = useAuthStore()
const usersStore = useUsersStore()

const dialog = ref(false)
const selectedUser = ref<User | null>(null)

onMounted(async () => {
  await usersStore.fetchUsers()
})

async function confirmDelete(user: User) {
  selectedUser.value = user
  dialog.value = true
}

async function deleteUser() {
  if (!selectedUser.value) return

  try {
    await usersStore.deleteUser(selectedUser.value.id)
    dialog.value = false
    selectedUser.value = null
  } catch (error) {
    console.error('Failed to delete user:', error)
  }
}
</script>

<template>
  <v-container fluid>
    <v-row class="mb-4">
      <v-col cols="12">
        <h1 class="text-h3">
          <v-icon icon="mdi-account-group" class="mr-2"></v-icon>
          User Management
        </h1>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <v-card>
          <v-card-title>
            <v-row align="center">
              <v-col>
                Users
              </v-col>
              <v-col cols="auto">
                <v-btn color="primary" prepend-icon="mdi-refresh" @click="usersStore.fetchUsers">
                  Refresh
                </v-btn>
              </v-col>
            </v-row>
          </v-card-title>

          <v-card-text>
            <v-alert
              v-if="usersStore.error"
              type="error"
              variant="tonal"
              class="mb-4"
            >
              {{ usersStore.error }}
            </v-alert>

            <v-progress-linear
              v-if="usersStore.loading"
              indeterminate
              color="primary"
              class="mb-4"
            ></v-progress-linear>

            <v-table v-else>
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Username</th>
                  <th>Email</th>
                  <th>Name</th>
                  <th>Groups</th>
                  <th>Status</th>
                  <th>Last Login</th>
                  <th v-if="authStore.isAdmin">Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="user in usersStore.users" :key="user.id">
                  <td>{{ user.id }}</td>
                  <td>
                    <v-chip size="small" color="primary">
                      {{ user.username }}
                    </v-chip>
                  </td>
                  <td>{{ user.email }}</td>
                  <td>{{ user.firstName }} {{ user.lastName }}</td>
                  <td>
                    <v-chip
                      v-for="group in user.groups"
                      :key="group"
                      size="small"
                      class="mr-1"
                      :color="group === 'Administrators' ? 'error' : group === 'Managers' ? 'warning' : 'default'"
                    >
                      {{ group }}
                    </v-chip>
                  </td>
                  <td>
                    <v-chip
                      size="small"
                      :color="user.isActive ? 'success' : 'error'"
                    >
                      {{ user.isActive ? 'Active' : 'Inactive' }}
                    </v-chip>
                  </td>
                  <td>
                    {{ user.lastLoginAt ? new Date(user.lastLoginAt).toLocaleString() : 'Never' }}
                  </td>
                  <td v-if="authStore.isAdmin">
                    <v-btn
                      size="small"
                      color="error"
                      variant="text"
                      icon="mdi-delete"
                      @click="confirmDelete(user)"
                    ></v-btn>
                  </td>
                </tr>
              </tbody>
            </v-table>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Delete Confirmation Dialog -->
    <v-dialog v-model="dialog" max-width="500">
      <v-card>
        <v-card-title class="text-h5">
          Confirm Delete
        </v-card-title>
        <v-card-text>
          Are you sure you want to delete user <strong>{{ selectedUser?.username }}</strong>?
          This action cannot be undone.
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn
            color="grey"
            variant="text"
            @click="dialog = false"
          >
            Cancel
          </v-btn>
          <v-btn
            color="error"
            variant="flat"
            @click="deleteUser"
            :loading="usersStore.loading"
          >
            Delete
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>
