# Component Patterns and Theming Guide

This guide covers best practices for creating Vue 3 components with Vuetify, including theming, component patterns, and styling conventions.

## Component Architecture Principles

1. **Always use Vuetify components** - Never use plain HTML elements when Vuetify equivalents exist
2. **Composition API with `<script setup>`** - Modern Vue 3 pattern
3. **TypeScript for type safety** - Define props and emits with interfaces
4. **Pinia for state management** - Don't duplicate state in components
5. **Props down, events up** - Standard Vue communication pattern

## Vuetify Component Mapping

### Replace HTML with Vuetify Components

| HTML Element | Vuetify Component | Notes |
|-------------|-------------------|-------|
| `<button>` | `<v-btn>` | Use color, variant, size props |
| `<table>` | `<v-table>` | Automatic theming |
| `<input type="text">` | `<v-text-field>` | Use variant="outlined" |
| `<input type="checkbox">` | `<v-checkbox>` | Better theme support |
| `<select>` | `<v-select>` | Dropdown with search |
| `<input type="radio">` | `<v-radio-group>` | Radio button group |
| `<textarea>` | `<v-textarea>` | Multi-line input |
| `<div class="card">` | `<v-card>` | Material design card |
| `<span class="badge">` | `<v-chip>` | Labeled badges |
| `<nav>` | `<v-navigation-drawer>` | Side navigation |
| `<header>` | `<v-app-bar>` | Top app bar |
| `<dialog>` | `<v-dialog>` | Modal dialogs |
| `<ul>` | `<v-list>` | List with items |

## Component Pattern Templates

### Data Display Component

```vue
<script setup lang="ts">
import { computed } from 'vue'
import { useYourStore } from '@/stores/yourStore'
import type { YourType } from '@/types'

interface Props {
  title?: string
  showActions?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  title: 'Default Title',
  showActions: true
})

interface Emits {
  (e: 'item-click', item: YourType): void
  (e: 'action', id: string, action: string): void
}

const emit = defineEmits<Emits>()

const store = useYourStore()
const items = computed(() => store.items)

const handleItemClick = (item: YourType) => {
  emit('item-click', item)
}

const handleAction = (id: string, action: string) => {
  emit('action', id, action)
}

const getStatusColor = (status: string): string => {
  const colors: Record<string, string> = {
    'pending': 'warning',
    'active': 'primary',
    'completed': 'success',
    'error': 'error',
  }
  return colors[status] || 'default'
}
</script>

<template>
  <v-card>
    <v-card-title>{{ title }}</v-card-title>

    <v-card-text>
      <v-table>
        <thead>
          <tr>
            <th>Name</th>
            <th>Status</th>
            <th v-if="showActions">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="item in items"
            :key="item.id"
            @click="handleItemClick(item)"
            class="cursor-pointer"
          >
            <td>{{ item.name }}</td>
            <td>
              <v-chip
                :color="getStatusColor(item.status)"
                size="small"
              >
                {{ item.status }}
              </v-chip>
            </td>
            <td v-if="showActions">
              <v-btn
                icon="mdi-pencil"
                size="small"
                variant="text"
                @click.stop="handleAction(item.id, 'edit')"
              />
              <v-btn
                icon="mdi-delete"
                size="small"
                variant="text"
                color="error"
                @click.stop="handleAction(item.id, 'delete')"
              />
            </td>
          </tr>
        </tbody>
      </v-table>
    </v-card-text>
  </v-card>
</template>

<style scoped>
.cursor-pointer {
  cursor: pointer;
}

.cursor-pointer:hover {
  background-color: rgba(0, 0, 0, 0.05);
}
</style>
```

### Form Component

```vue
<script setup lang="ts">
import { ref, computed } from 'vue'
import type { YourType } from '@/types'

interface Props {
  initialData?: YourType
  mode?: 'create' | 'edit'
}

const props = withDefaults(defineProps<Props>(), {
  mode: 'create'
})

interface Emits {
  (e: 'submit', data: YourType): void
  (e: 'cancel'): void
}

const emit = defineEmits<Emits>()

const formRef = ref()
const valid = ref(false)

const formData = ref({
  name: props.initialData?.name || '',
  description: props.initialData?.description || '',
  status: props.initialData?.status || 'pending',
  quantity: props.initialData?.quantity || 0,
})

const statusOptions = [
  { title: 'Pending', value: 'pending' },
  { title: 'Active', value: 'active' },
  { title: 'Completed', value: 'completed' },
]

const rules = {
  required: (v: any) => !!v || 'Field is required',
  minLength: (min: number) => (v: string) =>
    v.length >= min || `Minimum ${min} characters`,
  number: (v: any) => !isNaN(Number(v)) || 'Must be a number',
  positive: (v: number) => v >= 0 || 'Must be positive',
}

const handleSubmit = async () => {
  const { valid } = await formRef.value.validate()
  if (valid) {
    emit('submit', { ...formData.value } as YourType)
  }
}

const handleCancel = () => {
  formRef.value.reset()
  emit('cancel')
}

const isEditMode = computed(() => props.mode === 'edit')
</script>

<template>
  <v-card>
    <v-card-title>
      {{ isEditMode ? 'Edit Item' : 'Create New Item' }}
    </v-card-title>

    <v-card-text>
      <v-form ref="formRef" v-model="valid" @submit.prevent="handleSubmit">
        <v-text-field
          v-model="formData.name"
          label="Name"
          variant="outlined"
          :rules="[rules.required, rules.minLength(3)]"
          required
        />

        <v-textarea
          v-model="formData.description"
          label="Description"
          variant="outlined"
          rows="3"
        />

        <v-select
          v-model="formData.status"
          :items="statusOptions"
          label="Status"
          variant="outlined"
          :rules="[rules.required]"
        />

        <v-text-field
          v-model.number="formData.quantity"
          label="Quantity"
          variant="outlined"
          type="number"
          :rules="[rules.required, rules.number, rules.positive]"
        />
      </v-form>
    </v-card-text>

    <v-card-actions>
      <v-spacer />
      <v-btn @click="handleCancel">
        Cancel
      </v-btn>
      <v-btn
        color="primary"
        :disabled="!valid"
        @click="handleSubmit"
      >
        {{ isEditMode ? 'Update' : 'Create' }}
      </v-btn>
    </v-card-actions>
  </v-card>
</template>
```

### Dialog Component

```vue
<script setup lang="ts">
import { ref, watch } from 'vue'
import type { YourType } from '@/types'

interface Props {
  modelValue: boolean
  item?: YourType
  title?: string
}

const props = withDefaults(defineProps<Props>(), {
  title: 'Dialog'
})

interface Emits {
  (e: 'update:modelValue', value: boolean): void
  (e: 'confirm', item: YourType): void
}

const emit = defineEmits<Emits>()

const dialog = ref(props.modelValue)

// Sync with v-model
watch(() => props.modelValue, (newVal) => {
  dialog.value = newVal
})

watch(dialog, (newVal) => {
  emit('update:modelValue', newVal)
})

const handleConfirm = () => {
  if (props.item) {
    emit('confirm', props.item)
  }
  dialog.value = false
}

const handleCancel = () => {
  dialog.value = false
}
</script>

<template>
  <v-dialog v-model="dialog" max-width="600">
    <v-card>
      <v-card-title>{{ title }}</v-card-title>

      <v-card-text>
        <div v-if="item">
          <p><strong>Name:</strong> {{ item.name }}</p>
          <p><strong>Status:</strong> {{ item.status }}</p>
          <p><strong>Description:</strong> {{ item.description }}</p>
        </div>
        <slot v-else />
      </v-card-text>

      <v-card-actions>
        <v-spacer />
        <v-btn @click="handleCancel">
          Cancel
        </v-btn>
        <v-btn color="primary" @click="handleConfirm">
          Confirm
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
```

### Statistics/Dashboard Component

```vue
<script setup lang="ts">
import { computed } from 'vue'
import { useYourStore } from '@/stores/yourStore'

const store = useYourStore()

const stats = computed(() => {
  const items = store.items
  return {
    total: items.length,
    pending: items.filter(i => i.status === 'pending').length,
    active: items.filter(i => i.status === 'active').length,
    completed: items.filter(i => i.status === 'completed').length,
  }
})

const statCards = computed(() => [
  {
    title: 'Total Items',
    value: stats.value.total,
    icon: 'mdi-view-dashboard',
    color: 'primary',
  },
  {
    title: 'Pending',
    value: stats.value.pending,
    icon: 'mdi-clock-outline',
    color: 'warning',
  },
  {
    title: 'Active',
    value: stats.value.active,
    icon: 'mdi-play-circle',
    color: 'info',
  },
  {
    title: 'Completed',
    value: stats.value.completed,
    icon: 'mdi-check-circle',
    color: 'success',
  },
])
</script>

<template>
  <v-row>
    <v-col
      v-for="stat in statCards"
      :key="stat.title"
      cols="12"
      sm="6"
      md="3"
    >
      <v-card :color="stat.color" variant="tonal">
        <v-card-text>
          <div class="d-flex align-center justify-space-between">
            <div>
              <div class="text-h4 mb-1">{{ stat.value }}</div>
              <div class="text-subtitle-1">{{ stat.title }}</div>
            </div>
            <v-icon :icon="stat.icon" size="48" />
          </div>
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>
</template>
```

## Theming Best Practices

### Using Theme Colors

```vue
<template>
  <!-- Good: Use theme color names -->
  <v-btn color="primary">Primary Button</v-btn>
  <v-btn color="secondary">Secondary Button</v-btn>
  <v-btn color="success">Success Button</v-btn>
  <v-btn color="error">Error Button</v-btn>
  <v-btn color="warning">Warning Button</v-btn>
  <v-btn color="info">Info Button</v-btn>

  <!-- Good: Custom colors for specific design requirements -->
  <v-chip color="#667eea">Custom Color</v-chip>

  <!-- Bad: Hardcoded colors that don't respect theme -->
  <button style="background: blue">Don't do this</button>
</template>
```

### Color Selection Helper

```typescript
// Define in component or composable
const getStatusColor = (status: string): string => {
  const colorMap: Record<string, string> = {
    'pending': 'warning',
    'planning': 'info',
    'parts_ordered': 'primary',
    'production': 'secondary',
    'assembly': 'info',
    'completed': 'success',
    'cancelled': 'error',
  }
  return colorMap[status] || 'default'
}

// Usage
<v-chip :color="getStatusColor(item.status)">
  {{ item.status }}
</v-chip>
```

### Variant Styles

```vue
<template>
  <!-- Button variants -->
  <v-btn variant="flat">Flat (default)</v-btn>
  <v-btn variant="elevated">Elevated</v-btn>
  <v-btn variant="tonal">Tonal</v-btn>
  <v-btn variant="outlined">Outlined</v-btn>
  <v-btn variant="text">Text</v-btn>
  <v-btn variant="plain">Plain</v-btn>

  <!-- Card variants -->
  <v-card variant="flat">Flat Card</v-card>
  <v-card variant="elevated">Elevated Card</v-card>
  <v-card variant="tonal">Tonal Card</v-card>
  <v-card variant="outlined">Outlined Card</v-card>

  <!-- Chip variants -->
  <v-chip variant="flat">Flat Chip</v-chip>
  <v-chip variant="elevated">Elevated Chip</v-chip>
  <v-chip variant="tonal">Tonal Chip</v-chip>
  <v-chip variant="outlined">Outlined Chip</v-chip>
</template>
```

### Size Options

```vue
<template>
  <!-- Buttons -->
  <v-btn size="x-small">Extra Small</v-btn>
  <v-btn size="small">Small</v-btn>
  <v-btn size="default">Default</v-btn>
  <v-btn size="large">Large</v-btn>
  <v-btn size="x-large">Extra Large</v-btn>

  <!-- Chips -->
  <v-chip size="x-small">XS</v-chip>
  <v-chip size="small">Small</v-chip>
  <v-chip size="default">Default</v-chip>
  <v-chip size="large">Large</v-chip>

  <!-- Icons -->
  <v-icon size="small">mdi-home</v-icon>
  <v-icon size="default">mdi-home</v-icon>
  <v-icon size="large">mdi-home</v-icon>
  <v-icon size="x-large">mdi-home</v-icon>
</template>
```

## Component Communication Patterns

### Props Down

```vue
<!-- Parent Component -->
<script setup lang="ts">
import { ref } from 'vue'
import ChildComponent from './ChildComponent.vue'

const items = ref([...])
const showActions = ref(true)
</script>

<template>
  <ChildComponent
    :items="items"
    :show-actions="showActions"
    title="My Items"
  />
</template>
```

```vue
<!-- Child Component -->
<script setup lang="ts">
import type { YourType } from '@/types'

interface Props {
  items: YourType[]
  showActions?: boolean
  title?: string
}

const props = withDefaults(defineProps<Props>(), {
  showActions: true,
  title: 'Items'
})
</script>
```

### Events Up

```vue
<!-- Child Component -->
<script setup lang="ts">
interface Emits {
  (e: 'item-selected', id: string): void
  (e: 'action-triggered', action: string, data: any): void
}

const emit = defineEmits<Emits>()

const handleSelect = (id: string) => {
  emit('item-selected', id)
}

const handleAction = (action: string, data: any) => {
  emit('action-triggered', action, data)
}
</script>

<template>
  <v-btn @click="handleSelect('123')">Select Item</v-btn>
</template>
```

```vue
<!-- Parent Component -->
<script setup lang="ts">
const handleItemSelected = (id: string) => {
  console.log('Item selected:', id)
}

const handleActionTriggered = (action: string, data: any) => {
  console.log('Action:', action, data)
}
</script>

<template>
  <ChildComponent
    @item-selected="handleItemSelected"
    @action-triggered="handleActionTriggered"
  />
</template>
```

### v-model Pattern

```vue
<!-- Child Component with v-model -->
<script setup lang="ts">
interface Props {
  modelValue: string
}

const props = defineProps<Props>()

interface Emits {
  (e: 'update:modelValue', value: string): void
}

const emit = defineEmits<Emits>()

const updateValue = (value: string) => {
  emit('update:modelValue', value)
}
</script>

<template>
  <v-text-field
    :model-value="modelValue"
    @update:model-value="updateValue"
  />
</template>
```

```vue
<!-- Parent Component -->
<script setup lang="ts">
import { ref } from 'vue'

const searchQuery = ref('')
</script>

<template>
  <ChildComponent v-model="searchQuery" />
</template>
```

## Loading States

```vue
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useYourStore } from '@/stores/yourStore'

const store = useYourStore()
const loading = ref(true)

onMounted(async () => {
  loading.value = true
  await store.fetchItems()
  loading.value = false
})
</script>

<template>
  <v-card>
    <v-card-title>Items</v-card-title>
    <v-card-text>
      <!-- Loading State -->
      <div v-if="loading" class="text-center pa-4">
        <v-progress-circular indeterminate color="primary" />
        <p class="mt-2">Loading items...</p>
      </div>

      <!-- Empty State -->
      <div v-else-if="!store.items.length" class="text-center pa-8">
        <v-icon icon="mdi-inbox" size="64" color="grey" />
        <p class="text-h6 mt-4">No items found</p>
        <p class="text-body-2">Create your first item to get started</p>
        <v-btn color="primary" class="mt-4">
          Create Item
        </v-btn>
      </div>

      <!-- Content -->
      <v-table v-else>
        <!-- Table content -->
      </v-table>
    </v-card-text>
  </v-card>
</template>
```

## Error Handling

```vue
<script setup lang="ts">
import { ref } from 'vue'
import { useYourStore } from '@/stores/yourStore'

const store = useYourStore()
const error = ref<string | null>(null)

const handleSubmit = async (data: any) => {
  error.value = null
  try {
    await store.createItem(data)
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'An error occurred'
  }
}
</script>

<template>
  <v-card>
    <v-alert
      v-if="error"
      type="error"
      closable
      @click:close="error = null"
    >
      {{ error }}
    </v-alert>

    <!-- Rest of component -->
  </v-card>
</template>
```

## Responsive Design

```vue
<template>
  <v-row>
    <!-- Full width on mobile, half on tablet, quarter on desktop -->
    <v-col
      v-for="item in items"
      :key="item.id"
      cols="12"
      sm="6"
      md="3"
    >
      <v-card>{{ item.name }}</v-card>
    </v-col>
  </v-row>

  <!-- Hide on mobile -->
  <v-btn class="d-none d-md-flex">Desktop Only</v-btn>

  <!-- Show only on mobile -->
  <v-btn class="d-flex d-md-none">Mobile Only</v-btn>
</template>
```

## Best Practices Checklist

- [ ] Use Vuetify components instead of plain HTML
- [ ] Use `<script setup lang="ts">` with TypeScript
- [ ] Define props and emits with interfaces
- [ ] Use theme colors (primary, secondary, etc.)
- [ ] Handle loading states with v-progress-circular
- [ ] Handle empty states with icons and messages
- [ ] Handle error states with v-alert
- [ ] Use proper variants (flat, tonal, outlined, etc.)
- [ ] Use appropriate sizes (small, default, large)
- [ ] Make components responsive with v-col breakpoints
- [ ] Use Pinia stores for state management
- [ ] Emit events for parent communication
- [ ] Use v-model for two-way binding
- [ ] Add scoped styles when needed
- [ ] Use composables for reusable logic
