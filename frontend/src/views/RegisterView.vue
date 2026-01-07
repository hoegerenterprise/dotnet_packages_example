<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const username = ref('')
const email = ref('')
const password = ref('')
const firstName = ref('')
const lastName = ref('')
const showPassword = ref(false)
const successMessage = ref(false)

const rules = {
  required: (v: string) => !!v || 'This field is required',
  min: (v: string) => v.length >= 3 || 'Minimum 3 characters',
  minPassword: (v: string) => v.length >= 8 || 'Password must be at least 8 characters',
  email: (v: string) => /.+@.+\..+/.test(v) || 'Must be a valid email'
}

async function handleRegister() {
  if (!username.value || !email.value || !password.value || !firstName.value || !lastName.value) return

  const success = await authStore.register({
    username: username.value,
    email: email.value,
    password: password.value,
    firstName: firstName.value,
    lastName: lastName.value
  })

  if (success) {
    successMessage.value = true
    setTimeout(() => {
      router.push('/login')
    }, 2000)
  }
}
</script>

<template>
  <v-container class="fill-height" fluid>
    <v-row align="center" justify="center">
      <v-col cols="12" sm="10" md="8" lg="6">
        <v-card elevation="8" class="pa-4">
          <v-card-title class="text-h4 text-center mb-4">
            <v-icon icon="mdi-account-plus" size="large" class="mr-2"></v-icon>
            Create Account
          </v-card-title>

          <v-card-text>
            <v-form @submit.prevent="handleRegister">
              <v-row>
                <v-col cols="12" md="6">
                  <v-text-field
                    v-model="firstName"
                    label="First Name"
                    prepend-inner-icon="mdi-account"
                    variant="outlined"
                    :rules="[rules.required]"
                  ></v-text-field>
                </v-col>
                <v-col cols="12" md="6">
                  <v-text-field
                    v-model="lastName"
                    label="Last Name"
                    prepend-inner-icon="mdi-account"
                    variant="outlined"
                    :rules="[rules.required]"
                  ></v-text-field>
                </v-col>
              </v-row>

              <v-text-field
                v-model="username"
                label="Username"
                prepend-inner-icon="mdi-account-circle"
                variant="outlined"
                :rules="[rules.required, rules.min]"
                class="mb-2"
              ></v-text-field>

              <v-text-field
                v-model="email"
                label="Email"
                type="email"
                prepend-inner-icon="mdi-email"
                variant="outlined"
                :rules="[rules.required, rules.email]"
                class="mb-2"
              ></v-text-field>

              <v-text-field
                v-model="password"
                label="Password"
                prepend-inner-icon="mdi-lock"
                :append-inner-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
                :type="showPassword ? 'text' : 'password'"
                variant="outlined"
                :rules="[rules.required, rules.minPassword]"
                @click:append-inner="showPassword = !showPassword"
                hint="Minimum 8 characters"
                class="mb-2"
              ></v-text-field>

              <v-alert
                v-if="successMessage"
                type="success"
                variant="tonal"
                class="mb-4"
              >
                Account created successfully! Redirecting to login...
              </v-alert>

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
                Register
              </v-btn>

              <v-divider class="my-4"></v-divider>

              <v-btn
                color="secondary"
                variant="outlined"
                size="large"
                block
                @click="router.push('/login')"
              >
                Already have an account? Login
              </v-btn>
            </v-form>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>
