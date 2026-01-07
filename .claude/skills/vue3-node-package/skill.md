# Vue 3 + Vuetify Installable Package Skill

This skill provides comprehensive knowledge about creating and maintaining a Vue 3 component library with Vuetify that is structured as an installable npm package, including a playground demonstration app and Flask backend.

## Overview

This skill covers the architecture of a Vue 3 application built as a reusable component library that can be installed via npm. The project consists of:

1. **Main Package** (`/`) - Installable npm package with Vue components, stores, and router
2. **Playground** (`/playground`) - Demonstration app showing library usage
3. **Backend** (`/backend`) - Flask API server (if applicable)

## Project Structure

```
project-root/
├── src/                          # Main library source
│   ├── components/               # Reusable Vue components
│   ├── views/                    # Route view components
│   ├── stores/                   # Pinia stores
│   ├── types/                    # TypeScript definitions
│   ├── router/                   # Vue Router config
│   └── index.ts                  # Library entry point
├── playground/                   # Demo application
│   ├── src/
│   │   ├── views/               # Playground-specific views
│   │   ├── router/              # Custom router config
│   │   ├── plugins/
│   │   │   └── vuetify.ts       # Vuetify configuration
│   │   ├── App.vue
│   │   └── main.ts
│   ├── package.json
│   └── vite.config.ts
├── backend/                      # Flask API (optional)
│   ├── app.py
│   ├── models.py
│   ├── requirements.txt
│   └── .gitignore
├── package.json                  # Library package config
├── vite.config.ts               # Build configuration
└── tsconfig.json                # TypeScript config
```

## Technology Stack

### Frontend
- **Vue 3** - Progressive JavaScript framework with Composition API
- **Vuetify 3** - Material Design component framework
- **Pinia** - State management
- **Vue Router** - Client-side routing
- **TypeScript** - Type safety
- **Vite** - Build tool and dev server
- **vite-plugin-dts** - TypeScript declaration generation

### Backend (Optional)
- **Flask** - Python web framework
- **Flask-CORS** - Cross-origin resource sharing

## Key Concepts

### 1. Installable NPM Package Structure

Structure package.json for dual ESM/UMD export:

```json
{
  "name": "@yourorg/your-library",
  "version": "1.0.0",
  "type": "module",
  "main": "./dist/your-library.umd.cjs",
  "module": "./dist/your-library.js",
  "types": "./dist/index.d.ts",
  "files": ["dist"],
  "exports": {
    ".": {
      "types": "./dist/index.d.ts",
      "import": "./dist/your-library.js",
      "require": "./dist/your-library.umd.cjs"
    }
  }
}
```

### 2. Library Entry Point (src/index.ts)

Export all components, views, stores, and create a Vue plugin:

```typescript
import type { App, Plugin } from 'vue'
import { createPinia } from 'pinia'
import { createRouter, createWebHistory } from 'vue-router'

// Export types
export * from './types'

// Export stores
export { useYourStore } from './stores/yourStore'

// Export components
export { default as Component1 } from './components/Component1.vue'
export { default as Component2 } from './components/Component2.vue'

// Export views
export { default as View1 } from './views/View1.vue'
export { default as View2 } from './views/View2.vue'

// Router factory
export function createLibraryRouter(baseUrl = '/') {
  return createRouter({
    history: createWebHistory(baseUrl),
    routes: [
      {
        path: '/',
        name: 'home',
        component: () => import('./views/View1.vue'),
      },
      {
        path: '/route2',
        name: 'route2',
        component: () => import('./views/View2.vue'),
      },
    ],
  })
}

// Plugin options interface
export interface LibraryOptions {
  baseUrl?: string
  apiBaseUrl?: string
}

// Main plugin
export const YourLibraryPlugin: Plugin = {
  install(app: App, options: LibraryOptions = {}) {
    // Install Pinia if not already installed
    if (!app.config.globalProperties.$pinia) {
      const pinia = createPinia()
      app.use(pinia)
    }

    // Create and install router if baseUrl is provided
    if (options.baseUrl) {
      const router = createLibraryRouter(options.baseUrl)
      app.use(router)
    }

    // Store API base URL if provided
    if (options.apiBaseUrl) {
      app.config.globalProperties.$apiBaseUrl = options.apiBaseUrl
    }
  }
}

// Default export
export default YourLibraryPlugin
```

### 3. Vite Configuration for Library Build

Configure Vite for dual-mode (app development and library build):

```typescript
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vuetify from 'vite-plugin-vuetify'
import dts from 'vite-plugin-dts'
import { fileURLToPath, URL } from 'node:url'
import { resolve } from 'path'

export default defineConfig(({ mode }) => {
  const isLib = mode === 'lib'

  return {
    plugins: [
      vue(),
      vuetify({ autoImport: true }),
      isLib && dts({
        tsconfigPath: './tsconfig.json',
        outDir: 'dist',
      }),
    ],
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url))
      }
    },
    server: {
      port: 5173,
      proxy: {
        '/api': {
          target: 'http://localhost:5000',
          changeOrigin: true,
        }
      }
    },
    build: isLib ? {
      lib: {
        entry: resolve(__dirname, 'src/index.ts'),
        name: 'YourLibrary',
        fileName: (format) => `your-library.${format === 'es' ? 'js' : 'umd.cjs'}`
      },
      rollupOptions: {
        external: ['vue', 'vue-router', 'pinia', 'vuetify'],
        output: {
          globals: {
            vue: 'Vue',
            'vue-router': 'VueRouter',
            pinia: 'Pinia',
            vuetify: 'Vuetify'
          }
        }
      }
    } : undefined
  }
})
```

### 4. Playground Structure

The playground is a separate Vue application that demonstrates library usage.

**playground/package.json:**
```json
{
  "name": "library-playground",
  "version": "1.0.0",
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "vite build"
  },
  "dependencies": {
    "vue": "^3.5.24",
    "vuetify": "^3.11.5",
    "pinia": "^3.0.4",
    "vue-router": "^4.5.0"
  }
}
```

**playground/src/main.ts:**
```typescript
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import vuetify from './plugins/vuetify'
import router from './router'

const app = createApp(App)

// Install plugins
app.use(vuetify)
app.use(createPinia())
app.use(router)

app.mount('#app')
```

**playground/src/router/index.ts:**
```typescript
import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'

// Import views from the library (parent directory)
import { View1, View2 } from '../../../src/index'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/home',
      name: 'home',
      component: HomeView,
    },
    {
      path: '/',
      name: 'library-view1',
      component: View1,
    },
    {
      path: '/view2',
      name: 'library-view2',
      component: View2,
    },
    {
      path: '/about',
      name: 'about',
      component: () => import('../views/AboutView.vue'),
    },
  ],
})

export default router
```

**Key principle:** Playground imports from source (`../../../src/index`) during development, not from built dist.

### 5. Vuetify Configuration

**playground/src/plugins/vuetify.ts:**
```typescript
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
      dark: {
        colors: {
          primary: '#2196F3',
          secondary: '#424242',
          success: '#4CAF50',
          error: '#f44336',
          warning: '#ff9800',
          info: '#2196F3',
        }
      }
    }
  }
})
```

### 6. Component Patterns with Vuetify

Always use Vuetify components for theme consistency:

```vue
<script setup lang="ts">
import { computed } from 'vue'
import { useYourStore } from '@/stores/yourStore'
import type { YourType } from '@/types'

const store = useYourStore()
const items = computed(() => store.items)

const handleAction = async () => {
  await store.performAction()
}
</script>

<template>
  <v-card>
    <v-card-title>Component Title</v-card-title>
    <v-card-text>
      <v-table>
        <thead>
          <tr>
            <th>Column 1</th>
            <th>Column 2</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in items" :key="item.id">
            <td>{{ item.name }}</td>
            <td>{{ item.value }}</td>
            <td>
              <v-btn color="primary" size="small" @click="handleAction">
                Action
              </v-btn>
            </td>
          </tr>
        </tbody>
      </v-table>
    </v-card-text>
  </v-card>
</template>
```

### 7. Pinia Store Pattern

```typescript
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { YourType } from '@/types'

export const useYourStore = defineStore('your-store', () => {
  // State
  const items = ref<YourType[]>([])
  const loading = ref(false)

  // Getters
  const itemCount = computed(() => items.value.length)

  // Actions
  const fetchItems = async () => {
    loading.value = true
    try {
      const response = await fetch('/api/v1/items')
      items.value = await response.json()
    } catch (error) {
      console.error('Failed to fetch items:', error)
    } finally {
      loading.value = false
    }
  }

  const addItem = async (item: YourType) => {
    const response = await fetch('/api/v1/items', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(item)
    })
    const newItem = await response.json()
    items.value.push(newItem)
  }

  return {
    items,
    loading,
    itemCount,
    fetchItems,
    addItem,
  }
})
```

### 8. TypeScript Type Definitions

**src/types/index.ts:**
```typescript
export interface YourMainType {
  id: string
  name: string
  description: string
  items: YourItemType[]
  createdAt: string
  updatedAt: string
}

export interface YourItemType {
  id: string
  name: string
  quantity: number
  status: ItemStatus
}

export type ItemStatus = 'pending' | 'active' | 'completed'

export interface ApiResponse<T> {
  data: T
  message?: string
  error?: string
}
```

### 9. Backend Flask Structure (Optional)

**backend/app.py:**
```python
from flask import Flask, jsonify, request
from flask_cors import CORS

app = Flask(__name__)
CORS(app)

@app.route('/api/v1/items', methods=['GET'])
def get_items():
    # Your logic here
    return jsonify([])

@app.route('/api/v1/items', methods=['POST'])
def create_item():
    data = request.json
    # Your logic here
    return jsonify(data), 201

if __name__ == '__main__':
    app.run(debug=True, port=5000)
```

**backend/.gitignore:**
```
__pycache__/
*.py[cod]
*$py.class
*.so
.Python
venv/
ENV/
env/
.venv
*.db
*.sqlite3
.env
.flaskenv
instance/
.pytest_cache/
.coverage
htmlcov/
```

## Build and Development Workflow

### Main Library
```bash
npm install
npm run dev          # Development with HMR
npm run build        # Build for production (app)
npm run build:lib    # Build as library
```

### Playground
```bash
cd playground
npm install
npm run dev          # Run playground dev server
npm run build        # Build playground for production
```

### Backend
```bash
cd backend
pip install -r requirements.txt
python app.py        # Run Flask server on port 5000
```

## Important Rules and Best Practices

1. **Always use Vuetify components** - Never use plain HTML elements like `<button>`, `<table>`, `<input>` when Vuetify equivalents exist (`<v-btn>`, `<v-table>`, `<v-text-field>`)

2. **Import from source in playground** - Use `import { Component } from '../../../src/index'` not from dist during development

3. **Theme consistency** - Use Vuetify theme colors, avoid hardcoded colors except for specific design requirements

4. **Composition API with `<script setup>`** - Always use modern Vue 3 patterns

5. **TypeScript everywhere** - Define types in src/types/index.ts and use them consistently

6. **Proxy API calls** - Configure Vite proxy to forward `/api` to backend port 5000

7. **External dependencies in library build** - Always externalize vue, vue-router, pinia, vuetify in rollupOptions

8. **Separate playground package.json** - Playground has its own dependencies, doesn't depend on the library package

9. **Router factory pattern** - Export a function that creates the router, don't export router instance directly

10. **Plugin options** - Make baseUrl and apiBaseUrl configurable via plugin options

## Related Documentation

See the following files for detailed information:

- [package-structure.md](./package-structure.md) - NPM package configuration details
- [vue-stack.md](./vue-stack.md) - Vue, Vuetify, Pinia, Router patterns
- [playground-setup.md](./playground-setup.md) - Playground app configuration
- [backend-structure.md](./backend-structure.md) - Flask backend API patterns
- [components-guide.md](./components-guide.md) - Component patterns and theming

## Common Tasks

### Adding a New Component
1. Create component in `src/components/YourComponent.vue`
2. Export it in `src/index.ts`: `export { default as YourComponent } from './components/YourComponent.vue'`
3. Use Vuetify components for UI
4. Add TypeScript types to `src/types/index.ts` if needed

### Adding a New View
1. Create view in `src/views/YourView.vue`
2. Export it in `src/index.ts`
3. Add route to router in `src/router/index.ts`
4. Import and use in playground router

### Adding Playground-Specific Pages
1. Create view in `playground/src/views/YourView.vue`
2. Add route in `playground/src/router/index.ts`
3. Add menu item in `playground/src/App.vue`

### Modifying Theme Colors
1. Edit `playground/src/plugins/vuetify.ts`
2. Update theme colors in the `themes.dark.colors` object
3. Restart dev server for changes to take effect
