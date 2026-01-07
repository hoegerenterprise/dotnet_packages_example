<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const drawer = ref(true)

function logout() {
  authStore.logout()
  router.push('/login')
}
</script>

<template>
  <v-app>
    <v-app-bar v-if="authStore.isAuthenticated" color="primary" prominent>
      <v-app-bar-nav-icon @click="drawer = !drawer"></v-app-bar-nav-icon>
      <v-toolbar-title>
        <v-icon icon="mdi-package-variant" class="mr-2"></v-icon>
        .NET Packages Demo
      </v-toolbar-title>
      <v-spacer></v-spacer>
      <v-chip class="mr-4" prepend-icon="mdi-account">
        {{ authStore.username }}
      </v-chip>
      <v-btn icon="mdi-logout" @click="logout"></v-btn>
    </v-app-bar>

    <v-navigation-drawer v-if="authStore.isAuthenticated" v-model="drawer" app>
      <v-list nav>
        <v-list-item
          prepend-icon="mdi-view-dashboard"
          title="Dashboard"
          to="/"
        ></v-list-item>

        <v-list-item
          prepend-icon="mdi-account-group"
          title="Users"
          to="/users"
        ></v-list-item>

        <v-divider class="my-2"></v-divider>

        <v-list-item
          prepend-icon="mdi-logout"
          title="Logout"
          @click="logout"
        ></v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-main>
      <router-view></router-view>
    </v-main>
  </v-app>
</template>
