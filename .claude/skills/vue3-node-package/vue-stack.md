# Vue 3 Stack: Vue + Vuetify + Pinia + Router

This guide covers the complete Vue 3 technology stack including Vuetify for UI, Pinia for state management, and Vue Router for navigation.

## Vue 3 Composition API Patterns

### Component Structure with `<script setup>`

```vue
<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useYourStore } from '@/stores/yourStore'
import type { YourType } from '@/types'

// Props
interface Props {
  id: string
  disabled?: boolean
}
const props = withDefaults(defineProps<Props>(), {
  disabled: false
})

// Emits
interface Emits {
  (e: 'update', value: YourType): void
  (e: 'delete', id: string): void
}
const emit = defineEmits<Emits>()

// Composables
const router = useRouter()
const store = useYourStore()

// Reactive state
const loading = ref(false)
const searchQuery = ref('')
const items = ref<YourType[]>([])

// Computed properties
const filteredItems = computed(() => {
  if (!searchQuery.value) return items.value
  return items.value.filter(item =>
    item.name.toLowerCase().includes(searchQuery.value.toLowerCase())
  )
})

// Methods
const handleUpdate = (item: YourType) => {
  emit('update', item)
}

const navigateToDetail = (id: string) => {
  router.push(`/detail/${id}`)
}

// Lifecycle
onMounted(async () => {
  loading.value = true
  await store.fetchItems()
  items.value = store.items
  loading.value = false
})

// Watchers
watch(() => props.id, (newId) => {
  console.log('ID changed:', newId)
})
</script>

<template>
  <div class="component-wrapper">
    <v-text-field
      v-model="searchQuery"
      label="Search"
      prepend-icon="mdi-magnify"
    />

    <v-progress-linear v-if="loading" indeterminate />

    <v-list v-else>
      <v-list-item
        v-for="item in filteredItems"
        :key="item.id"
        @click="navigateToDetail(item.id)"
      >
        <v-list-item-title>{{ item.name }}</v-list-item-title>
      </v-list-item>
    </v-list>
  </div>
</template>

<style scoped>
.component-wrapper {
  padding: 16px;
}
</style>
```

### Key Composition API Concepts

#### Reactivity
```typescript
import { ref, reactive, computed, readonly } from 'vue'

// ref - for primitive values
const count = ref(0)
const name = ref('John')

// reactive - for objects
const state = reactive({
  user: null,
  loading: false
})

// computed - derived state
const doubleCount = computed(() => count.value * 2)

// readonly - prevent mutations
const readonlyState = readonly(state)
```

#### Lifecycle Hooks
```typescript
import { onMounted, onBeforeMount, onUpdated, onBeforeUnmount, onUnmounted } from 'vue'

onBeforeMount(() => {
  console.log('Before mount')
})

onMounted(() => {
  console.log('Mounted - fetch data here')
})

onUpdated(() => {
  console.log('Updated')
})

onBeforeUnmount(() => {
  console.log('Before unmount - cleanup here')
})

onUnmounted(() => {
  console.log('Unmounted')
})
```

#### Watchers
```typescript
import { watch, watchEffect } from 'vue'

// Watch single ref
watch(count, (newValue, oldValue) => {
  console.log(`Count changed from ${oldValue} to ${newValue}`)
})

// Watch multiple sources
watch([count, name], ([newCount, newName], [oldCount, oldName]) => {
  console.log('Multiple values changed')
})

// Watch reactive object property
watch(() => state.user, (newUser) => {
  console.log('User changed:', newUser)
})

// Deep watch
watch(state, (newState) => {
  console.log('State changed deeply:', newState)
}, { deep: true })

// Immediate watch
watch(count, (value) => {
  console.log('Runs immediately:', value)
}, { immediate: true })

// watchEffect - automatically tracks dependencies
watchEffect(() => {
  console.log(`Count is ${count.value}, name is ${name.value}`)
})
```

## Vuetify 3 Component Library

### Installation and Setup

```bash
npm install vuetify @mdi/font
```

### Vuetify Plugin Configuration

```typescript
// src/plugins/vuetify.ts
import 'vuetify/styles'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import { aliases, mdi } from 'vuetify/iconsets/mdi'
import '@mdi/font/css/materialdesignicons.css'

export default createVuetify({
  components,
  directives,
  icons: {
    defaultSet: 'mdi',
    aliases,
    sets: { mdi },
  },
  theme: {
    defaultTheme: 'dark',
    themes: {
      light: {
        colors: {
          primary: '#1976D2',
          secondary: '#424242',
          accent: '#82B1FF',
          error: '#FF5252',
          info: '#2196F3',
          success: '#4CAF50',
          warning: '#FFC107',
        }
      },
      dark: {
        colors: {
          primary: '#2196F3',
          secondary: '#424242',
          accent: '#FF4081',
          error: '#f44336',
          info: '#2196F3',
          success: '#4CAF50',
          warning: '#ff9800',
        }
      }
    }
  },
  defaults: {
    VBtn: {
      variant: 'flat',
      color: 'primary',
    },
    VCard: {
      elevation: 2,
    },
    VTextField: {
      variant: 'outlined',
      density: 'comfortable',
    },
  }
})
```

### Essential Vuetify Components

#### Layout Components
```vue
<template>
  <!-- App Bar -->
  <v-app-bar color="secondary" prominent>
    <v-app-bar-nav-icon @click="drawer = !drawer" />
    <v-toolbar-title>Application Title</v-toolbar-title>
    <v-spacer />
    <v-btn icon="mdi-magnify" />
  </v-app-bar>

  <!-- Navigation Drawer -->
  <v-navigation-drawer v-model="drawer" temporary>
    <v-list>
      <v-list-item
        v-for="item in menuItems"
        :key="item.title"
        :to="item.to"
        :prepend-icon="item.icon"
        :title="item.title"
      />
    </v-list>
  </v-navigation-drawer>

  <!-- Main Content -->
  <v-main>
    <v-container fluid>
      <router-view />
    </v-container>
  </v-main>
</template>
```

#### Card Components
```vue
<template>
  <v-card>
    <v-card-title>Card Title</v-card-title>
    <v-card-subtitle>Subtitle text</v-card-subtitle>
    <v-card-text>
      Main content goes here
    </v-card-text>
    <v-card-actions>
      <v-btn>Cancel</v-btn>
      <v-spacer />
      <v-btn color="primary">Confirm</v-btn>
    </v-card-actions>
  </v-card>
</template>
```

#### Data Tables
```vue
<template>
  <v-card>
    <v-card-title>
      <span>Data Table</span>
      <v-spacer />
      <v-text-field
        v-model="search"
        prepend-inner-icon="mdi-magnify"
        label="Search"
        single-line
        hide-details
      />
    </v-card-title>

    <v-card-text>
      <v-table>
        <thead>
          <tr>
            <th>Name</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in items" :key="item.id">
            <td>{{ item.name }}</td>
            <td>
              <v-chip :color="getStatusColor(item.status)" size="small">
                {{ item.status }}
              </v-chip>
            </td>
            <td>
              <v-btn icon="mdi-pencil" size="small" variant="text" />
              <v-btn icon="mdi-delete" size="small" variant="text" color="error" />
            </td>
          </tr>
        </tbody>
      </v-table>
    </v-card-text>
  </v-card>
</template>
```

#### Forms
```vue
<template>
  <v-form ref="formRef" v-model="valid" @submit.prevent="handleSubmit">
    <v-text-field
      v-model="formData.name"
      label="Name"
      :rules="[rules.required]"
      required
    />

    <v-text-field
      v-model="formData.email"
      label="Email"
      :rules="[rules.required, rules.email]"
      type="email"
    />

    <v-select
      v-model="formData.status"
      :items="statusOptions"
      label="Status"
      :rules="[rules.required]"
    />

    <v-textarea
      v-model="formData.description"
      label="Description"
      rows="3"
    />

    <v-checkbox
      v-model="formData.agree"
      label="I agree to terms"
      :rules="[rules.required]"
    />

    <v-btn type="submit" color="primary" :disabled="!valid">
      Submit
    </v-btn>
  </v-form>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const valid = ref(false)
const formRef = ref()

const formData = ref({
  name: '',
  email: '',
  status: '',
  description: '',
  agree: false,
})

const rules = {
  required: (v: any) => !!v || 'Field is required',
  email: (v: string) => /.+@.+\..+/.test(v) || 'Email must be valid',
}

const handleSubmit = async () => {
  const { valid } = await formRef.value.validate()
  if (valid) {
    console.log('Form submitted:', formData.value)
  }
}
</script>
```

#### Dialogs
```vue
<template>
  <v-dialog v-model="dialog" max-width="500">
    <template #activator="{ props }">
      <v-btn v-bind="props">Open Dialog</v-btn>
    </template>

    <v-card>
      <v-card-title>Dialog Title</v-card-title>
      <v-card-text>
        Dialog content goes here
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn @click="dialog = false">Cancel</v-btn>
        <v-btn color="primary" @click="handleConfirm">Confirm</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const dialog = ref(false)

const handleConfirm = () => {
  console.log('Confirmed')
  dialog.value = false
}
</script>
```

#### Chips and Badges
```vue
<template>
  <!-- Chips -->
  <v-chip color="primary">Primary Chip</v-chip>
  <v-chip color="success" size="small">Small Success</v-chip>
  <v-chip color="#667eea" variant="tonal">Custom Color</v-chip>
  <v-chip closable @click:close="handleClose">Closable</v-chip>

  <!-- Chip Group -->
  <v-chip-group v-model="selected" column>
    <v-chip v-for="tag in tags" :key="tag">{{ tag }}</v-chip>
  </v-chip-group>

  <!-- Badges -->
  <v-badge content="6" color="error">
    <v-icon icon="mdi-bell" />
  </v-badge>
</template>
```

#### Loading States
```vue
<template>
  <!-- Progress Linear -->
  <v-progress-linear indeterminate color="primary" />
  <v-progress-linear :model-value="progress" color="success" />

  <!-- Progress Circular -->
  <v-progress-circular indeterminate color="primary" />
  <v-progress-circular :model-value="progress" color="success" />

  <!-- Skeleton Loaders -->
  <v-skeleton-loader type="card" />
  <v-skeleton-loader type="table" />
</template>
```

## Pinia State Management

### Store Setup (Composition API Style)

```typescript
// src/stores/yourStore.ts
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { YourType, ItemStatus } from '@/types'

export const useYourStore = defineStore('your-store', () => {
  // ===== STATE =====
  const items = ref<YourType[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const selectedId = ref<string | null>(null)

  // ===== GETTERS =====
  const itemCount = computed(() => items.value.length)

  const selectedItem = computed(() =>
    items.value.find(item => item.id === selectedId.value)
  )

  const itemsByStatus = computed(() => {
    return (status: ItemStatus) =>
      items.value.filter(item => item.status === status)
  })

  const hasItems = computed(() => items.value.length > 0)

  // ===== ACTIONS =====
  const fetchItems = async () => {
    loading.value = true
    error.value = null
    try {
      const response = await fetch('/api/v1/items')
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }
      items.value = await response.json()
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Failed to fetch items'
      console.error('Failed to fetch items:', e)
    } finally {
      loading.value = false
    }
  }

  const fetchItemById = async (id: string) => {
    loading.value = true
    error.value = null
    try {
      const response = await fetch(`/api/v1/items/${id}`)
      if (!response.ok) throw new Error('Item not found')
      const item = await response.json()

      // Update existing item or add new one
      const index = items.value.findIndex(i => i.id === id)
      if (index !== -1) {
        items.value[index] = item
      } else {
        items.value.push(item)
      }
      return item
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Failed to fetch item'
      throw e
    } finally {
      loading.value = false
    }
  }

  const createItem = async (data: Omit<YourType, 'id'>) => {
    loading.value = true
    error.value = null
    try {
      const response = await fetch('/api/v1/items', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
      })
      if (!response.ok) throw new Error('Failed to create item')
      const newItem = await response.json()
      items.value.push(newItem)
      return newItem
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Failed to create item'
      throw e
    } finally {
      loading.value = false
    }
  }

  const updateItem = async (id: string, data: Partial<YourType>) => {
    loading.value = true
    error.value = null
    try {
      const response = await fetch(`/api/v1/items/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
      })
      if (!response.ok) throw new Error('Failed to update item')
      const updatedItem = await response.json()

      const index = items.value.findIndex(item => item.id === id)
      if (index !== -1) {
        items.value[index] = updatedItem
      }
      return updatedItem
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Failed to update item'
      throw e
    } finally {
      loading.value = false
    }
  }

  const deleteItem = async (id: string) => {
    loading.value = true
    error.value = null
    try {
      const response = await fetch(`/api/v1/items/${id}`, {
        method: 'DELETE'
      })
      if (!response.ok) throw new Error('Failed to delete item')

      const index = items.value.findIndex(item => item.id === id)
      if (index !== -1) {
        items.value.splice(index, 1)
      }
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Failed to delete item'
      throw e
    } finally {
      loading.value = false
    }
  }

  const selectItem = (id: string | null) => {
    selectedId.value = id
  }

  const clearError = () => {
    error.value = null
  }

  const reset = () => {
    items.value = []
    loading.value = false
    error.value = null
    selectedId.value = null
  }

  // ===== RETURN =====
  return {
    // State
    items,
    loading,
    error,
    selectedId,

    // Getters
    itemCount,
    selectedItem,
    itemsByStatus,
    hasItems,

    // Actions
    fetchItems,
    fetchItemById,
    createItem,
    updateItem,
    deleteItem,
    selectItem,
    clearError,
    reset,
  }
})
```

### Using Stores in Components

```vue
<script setup lang="ts">
import { onMounted, computed } from 'vue'
import { useYourStore } from '@/stores/yourStore'

const store = useYourStore()

// Access state
const items = computed(() => store.items)
const loading = computed(() => store.loading)

// Access getters
const count = computed(() => store.itemCount)

// Call actions
onMounted(async () => {
  await store.fetchItems()
})

const handleCreate = async () => {
  await store.createItem({
    name: 'New Item',
    status: 'pending'
  })
}

const handleDelete = async (id: string) => {
  await store.deleteItem(id)
}
</script>
```

### Multiple Stores

```typescript
// Use multiple stores in one component
import { useYourStore } from '@/stores/yourStore'
import { useAuthStore } from '@/stores/authStore'
import { useSettingsStore } from '@/stores/settingsStore'

const yourStore = useYourStore()
const authStore = useAuthStore()
const settingsStore = useSettingsStore()
```

## Vue Router

### Router Configuration

```typescript
// src/router/index.ts
import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'home',
    component: () => import('../views/HomeView.vue'),
    meta: { requiresAuth: false }
  },
  {
    path: '/items',
    name: 'items',
    component: () => import('../views/ItemsView.vue'),
    meta: { requiresAuth: true }
  },
  {
    path: '/items/:id',
    name: 'item-detail',
    component: () => import('../views/ItemDetailView.vue'),
    props: true, // Pass route params as props
    meta: { requiresAuth: true }
  },
  {
    path: '/about',
    name: 'about',
    component: () => import('../views/AboutView.vue')
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'not-found',
    component: () => import('../views/NotFoundView.vue')
  }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition
    } else {
      return { top: 0 }
    }
  }
})

// Navigation guards
router.beforeEach((to, from, next) => {
  const requiresAuth = to.meta.requiresAuth
  const isAuthenticated = checkAuth() // Your auth check

  if (requiresAuth && !isAuthenticated) {
    next({ name: 'login', query: { redirect: to.fullPath } })
  } else {
    next()
  }
})

export default router
```

### Using Router in Components

```vue
<script setup lang="ts">
import { useRouter, useRoute } from 'vue-router'
import { computed } from 'vue'

const router = useRouter()
const route = useRoute()

// Access route params
const id = computed(() => route.params.id as string)

// Access query params
const page = computed(() => route.query.page as string)

// Navigate programmatically
const navigateToDetail = (itemId: string) => {
  router.push({ name: 'item-detail', params: { id: itemId } })
}

const navigateWithQuery = () => {
  router.push({ path: '/items', query: { page: '2', filter: 'active' } })
}

const goBack = () => {
  router.back()
}

const replace = () => {
  router.replace({ name: 'home' })
}
</script>

<template>
  <div>
    <!-- Declarative navigation -->
    <router-link to="/">Home</router-link>
    <router-link :to="{ name: 'items' }">Items</router-link>
    <router-link :to="{ name: 'item-detail', params: { id: '123' } }">
      Item 123
    </router-link>

    <!-- Programmatic navigation -->
    <v-btn @click="navigateToDetail('456')">Go to Item 456</v-btn>
    <v-btn @click="goBack">Go Back</v-btn>
  </div>
</template>
```

### Router with Props

```typescript
// Route definition
{
  path: '/items/:id',
  name: 'item-detail',
  component: ItemDetailView,
  props: true  // Pass params as props
}
```

```vue
<!-- ItemDetailView.vue -->
<script setup lang="ts">
interface Props {
  id: string
}
const props = defineProps<Props>()

// id is now available as a prop
console.log('Item ID:', props.id)
</script>
```

### Nested Routes

```typescript
{
  path: '/dashboard',
  component: DashboardLayout,
  children: [
    {
      path: '',
      name: 'dashboard-home',
      component: DashboardHome
    },
    {
      path: 'stats',
      name: 'dashboard-stats',
      component: DashboardStats
    }
  ]
}
```

## Best Practices

1. **Always use TypeScript** - Type everything for better DX
2. **Use Composition API** - More flexible and testable
3. **Use `<script setup>`** - More concise and better performance
4. **Use Vuetify components** - Don't mix with plain HTML
5. **Centralize state in Pinia** - Don't use component state for shared data
6. **Use computed for derived state** - More efficient than methods
7. **Lazy load routes** - Use dynamic imports for better code splitting
8. **Use route meta** - For permissions, titles, breadcrumbs
9. **Follow naming conventions** - Views end with `View.vue`, stores with `Store.ts`
10. **Keep stores focused** - One store per domain/feature
