<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const username = ref('')
const password = ref('')
const showPassword = ref(false)

const rules = {
  required: (v: string) => !!v || 'This field is required',
  min: (v: string) => v.length >= 3 || 'Minimum 3 characters'
}

async function handleLogin() {
  if (!username.value || !password.value) return

  const success = await authStore.login({
    username: username.value,
    password: password.value
  })

  if (success) {
    router.push('/')
  }
}
</script>

<template>
  <v-container class="fill-height" fluid>
    <v-row align="center" justify="center">
      <v-col cols="12" sm="8" md="6" lg="4">
        <v-card elevation="8" class="pa-4">
          <v-card-title class="text-h4 text-center mb-4">
            <v-icon icon="mdi-lock" size="large" class="mr-2"></v-icon>
            Login
          </v-card-title>

          <v-card-text>
            <v-form @submit.prevent="handleLogin">
              <v-text-field
                v-model="username"
                label="Username"
                prepend-inner-icon="mdi-account"
                variant="outlined"
                :rules="[rules.required, rules.min]"
                class="mb-2"
              ></v-text-field>

              <v-text-field
                v-model="password"
                label="Password"
                prepend-inner-icon="mdi-lock"
                :append-inner-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
                :type="showPassword ? 'text' : 'password'"
                variant="outlined"
                :rules="[rules.required]"
                @click:append-inner="showPassword = !showPassword"
                class="mb-2"
              ></v-text-field>

              <v-alert
                v-if="authStore.error"
                type="error"
                variant="tonal"
                class="mb-4"
              >
                {{ authStore.error }}
              </v-alert>

              <v-btn
                type="submit"
                color="primary"
                size="large"
                block
                :loading="authStore.loading"
                class="mb-4"
              >
                Login
              </v-btn>

              <v-divider class="my-4"></v-divider>

              <v-btn
                color="secondary"
                variant="outlined"
                size="large"
                block
                @click="router.push('/register')"
              >
                Create Account
              </v-btn>
            </v-form>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>
