# Playground Setup for Library Demonstration

This guide covers how to set up a playground application that demonstrates your Vue 3 library package usage.

## Playground Purpose

The playground is a separate Vue application that:
- Demonstrates library features in action
- Serves as living documentation
- Tests library integration during development
- Imports from library source (not built dist) for faster development

## Directory Structure

```
playground/
├── public/
│   └── favicon.ico
├── src/
│   ├── assets/
│   ├── views/
│   │   ├── HomeView.vue           # Playground-specific home page
│   │   └── AboutView.vue          # About/documentation page
│   ├── router/
│   │   └── index.ts               # Custom router with library routes
│   ├── plugins/
│   │   └── vuetify.ts             # Vuetify configuration
│   ├── App.vue                    # Main app component
│   └── main.ts                    # Application entry point
├── index.html
├── package.json                   # Separate package config
├── tsconfig.json                  # TypeScript configuration
├── tsconfig.node.json
└── vite.config.ts                 # Vite dev server config
```

## Package Configuration

### playground/package.json

```json
{
  "name": "library-playground",
  "version": "1.0.0",
  "private": true,
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "vue-tsc && vite build",
    "preview": "vite preview"
  },
  "dependencies": {
    "vue": "^3.5.24",
    "vuetify": "^3.11.5",
    "pinia": "^3.0.4",
    "vue-router": "^4.5.0",
    "@mdi/font": "^7.4.47"
  },
  "devDependencies": {
    "@vitejs/plugin-vue": "^6.0.0",
    "@vue/tsconfig": "^0.7.0",
    "typescript": "^5.7.0",
    "vite": "^7.3.0",
    "vite-plugin-vuetify": "^2.0.0",
    "vue-tsc": "^2.2.0"
  }
}
```

**Key Points:**
- `"private": true` - Not meant to be published
- No dependency on the parent library package
- Same versions of Vue, Vuetify, Pinia, Vue Router as parent

## Vite Configuration

### playground/vite.config.ts

```typescript
import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vuetify from 'vite-plugin-vuetify'

export default defineConfig({
  plugins: [
    vue(),
    vuetify({ autoImport: true }),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  server: {
    port: 5174,
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        rewrite: (path) => path
      }
    }
  }
})
```

**Configuration Details:**
- Port 5174 (different from main library's 5173)
- API proxy forwards to backend
- Path alias `@` points to `playground/src`

## TypeScript Configuration

### playground/tsconfig.json

```json
{
  "compilerOptions": {
    "target": "ES2020",
    "useDefineForClassFields": true,
    "module": "ESNext",
    "lib": ["ES2020", "DOM", "DOM.Iterable"],
    "skipLibCheck": true,

    "moduleResolution": "bundler",
    "allowImportingTsExtensions": true,
    "resolveJsonModule": true,
    "isolatedModules": true,
    "moduleDetection": "force",
    "noEmit": true,
    "jsx": "preserve",

    "strict": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "noFallthroughCasesInSwitch": true,

    "paths": {
      "@/*": ["./src/*"]
    }
  },
  "include": [
    "src/**/*.ts",
    "src/**/*.tsx",
    "src/**/*.vue"
  ],
  "references": [
    { "path": "./tsconfig.node.json" }
  ]
}
```

### playground/tsconfig.node.json

```json
{
  "compilerOptions": {
    "target": "ES2022",
    "lib": ["ES2023"],
    "module": "ESNext",
    "skipLibCheck": true,
    "composite": true,
    "moduleResolution": "bundler",
    "allowSyntheticDefaultImports": true,
    "strict": true,
    "noEmit": true
  },
  "include": [
    "vite.config.ts"
  ]
}
```

**Important:** Self-contained configs without external `extends` to avoid resolution issues.

## Application Entry Point

### playground/src/main.ts

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

**Key Points:**
- Does NOT use the library plugin
- Manually installs Vuetify, Pinia, and router
- Imports router from local configuration

## Vuetify Configuration

### playground/src/plugins/vuetify.ts

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
    },
    VCard: {
      elevation: 2,
    },
  }
})
```

**Theme Colors:**
- Should match or be consistent with the library's expected theme
- Can be customized for playground demonstration

## Router Configuration

### playground/src/router/index.ts

```typescript
import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import HomeView from '../views/HomeView.vue'

// Import views from parent library source
import {
  View1,
  View2,
  View3
} from '../../../src/index'

const routes: RouteRecordRaw[] = [
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
    path: '/view3',
    name: 'library-view3',
    component: View3,
  },
  {
    path: '/about',
    name: 'about',
    component: () => import('../views/AboutView.vue'),
  },
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

export default router
```

**Critical Points:**
- Import library views from `../../../src/index` (source, not dist)
- Combines library routes with playground-specific routes
- Lazy loads playground-specific views

## App Component

### playground/src/App.vue

```vue
<script setup lang="ts">
import { ref } from 'vue'

const drawer = ref(false)

interface MenuItem {
  title: string
  icon: string
  to: string
}

const menuItems: MenuItem[] = [
  { title: 'Home', icon: 'mdi-home', to: '/home' },
  { title: 'View 1', icon: 'mdi-view-dashboard', to: '/' },
  { title: 'View 2', icon: 'mdi-package-variant', to: '/view2' },
  { title: 'View 3', icon: 'mdi-factory', to: '/view3' },
  { title: 'About', icon: 'mdi-information', to: '/about' },
]
</script>

<template>
  <v-app>
    <v-app-bar color="secondary" prominent>
      <v-app-bar-nav-icon @click="drawer = !drawer" />
      <v-toolbar-title>Library Playground</v-toolbar-title>
    </v-app-bar>

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

    <v-main>
      <v-container fluid>
        <router-view />
      </v-container>
    </v-main>
  </v-app>
</template>

<style scoped>
.v-container {
  max-width: 1400px;
}
</style>
```

## Playground Views

### HomeView.vue - Landing Page

```vue
<script setup lang="ts">
const features = [
  {
    title: 'Feature 1',
    description: 'Description of first feature',
    icon: 'mdi-view-dashboard',
    to: '/',
    color: 'primary'
  },
  {
    title: 'Feature 2',
    description: 'Description of second feature',
    icon: 'mdi-package-variant',
    to: '/view2',
    color: 'success'
  },
  {
    title: 'Feature 3',
    description: 'Description of third feature',
    icon: 'mdi-factory',
    to: '/view3',
    color: 'info'
  },
]
</script>

<template>
  <div>
    <v-row>
      <v-col cols="12">
        <v-card>
          <v-card-title class="text-h4">
            Welcome to Library Playground
          </v-card-title>
          <v-card-text>
            <p class="text-h6">
              This playground demonstrates the features of the component library.
            </p>
            <p>
              Explore the different sections using the navigation menu or
              click on the cards below.
            </p>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <v-row>
      <v-col
        v-for="feature in features"
        :key="feature.title"
        cols="12"
        md="4"
      >
        <v-card :color="feature.color" variant="tonal" height="100%">
          <v-card-title>
            <v-icon :icon="feature.icon" size="large" class="mr-2" />
            {{ feature.title }}
          </v-card-title>
          <v-card-text>
            {{ feature.description }}
          </v-card-text>
          <v-card-actions>
            <v-spacer />
            <v-btn :to="feature.to" variant="text">
              Explore
              <v-icon end icon="mdi-arrow-right" />
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <v-card>
          <v-card-title>Library Features</v-card-title>
          <v-card-text>
            <v-list>
              <v-list-item>
                <template #prepend>
                  <v-icon icon="mdi-check-circle" color="success" />
                </template>
                <v-list-item-title>Vue 3 Composition API</v-list-item-title>
              </v-list-item>
              <v-list-item>
                <template #prepend>
                  <v-icon icon="mdi-check-circle" color="success" />
                </template>
                <v-list-item-title>Vuetify 3 Components</v-list-item-title>
              </v-list-item>
              <v-list-item>
                <template #prepend>
                  <v-icon icon="mdi-check-circle" color="success" />
                </template>
                <v-list-item-title>Pinia State Management</v-list-item-title>
              </v-list-item>
              <v-list-item>
                <template #prepend>
                  <v-icon icon="mdi-check-circle" color="success" />
                </template>
                <v-list-item-title>TypeScript Support</v-list-item-title>
              </v-list-item>
            </v-list>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>
```

### AboutView.vue - Documentation Page

```vue
<script setup lang="ts">
const version = '1.0.0'
</script>

<template>
  <div>
    <v-card>
      <v-card-title class="text-h4">
        About This Library
      </v-card-title>
      <v-card-text>
        <p class="text-body-1 mb-4">
          A Vue 3 component library built with Vuetify 3, providing
          reusable components for building modern web applications.
        </p>
        <v-chip color="primary">Version {{ version }}</v-chip>
      </v-card-text>
    </v-card>

    <v-card class="mt-4">
      <v-card-title>Installation</v-card-title>
      <v-card-text>
        <p class="mb-2">Install the library using npm:</p>
        <v-code class="bg-grey-darken-3 pa-4">
          npm install @yourorg/your-library
        </v-code>
      </v-card-text>
    </v-card>

    <v-card class="mt-4">
      <v-card-title>Usage</v-card-title>
      <v-card-text>
        <p class="mb-2">Import and use the library in your Vue 3 application:</p>
        <v-code class="bg-grey-darken-3 pa-4">
          import { createApp } from 'vue'<br>
          import YourLibraryPlugin from '@yourorg/your-library'<br>
          import '@yourorg/your-library/dist/your-library.css'<br>
          <br>
          const app = createApp(App)<br>
          app.use(YourLibraryPlugin, {<br>
          &nbsp;&nbsp;baseUrl: '/',<br>
          &nbsp;&nbsp;apiBaseUrl: '/api/v1'<br>
          })<br>
        </v-code>
      </v-card-text>
    </v-card>

    <v-card class="mt-4">
      <v-card-title>Components</v-card-title>
      <v-card-text>
        <v-list>
          <v-list-subheader>Available Components</v-list-subheader>
          <v-list-item
            v-for="comp in ['Component1', 'Component2', 'Component3']"
            :key="comp"
          >
            <template #prepend>
              <v-icon icon="mdi-puzzle" />
            </template>
            <v-list-item-title>{{ comp }}</v-list-item-title>
          </v-list-item>
        </v-list>
      </v-card-text>
    </v-card>

    <v-card class="mt-4">
      <v-card-title>Resources</v-card-title>
      <v-card-text>
        <v-list>
          <v-list-item
            href="https://github.com/yourorg/your-library"
            target="_blank"
          >
            <template #prepend>
              <v-icon icon="mdi-github" />
            </template>
            <v-list-item-title>GitHub Repository</v-list-item-title>
          </v-list-item>
          <v-list-item
            href="https://your-docs-site.com"
            target="_blank"
          >
            <template #prepend>
              <v-icon icon="mdi-file-document" />
            </template>
            <v-list-item-title>Documentation</v-list-item-title>
          </v-list-item>
        </v-list>
      </v-card-text>
    </v-card>
  </div>
</template>

<style scoped>
.v-code {
  display: block;
  border-radius: 4px;
  font-family: 'Courier New', monospace;
  white-space: pre-wrap;
  line-height: 1.5;
}
</style>
```

## HTML Entry Point

### playground/index.html

```html
<!DOCTYPE html>
<html lang="en">
  <head>
    <meta charset="UTF-8">
    <link rel="icon" href="/favicon.ico">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Library Playground</title>
  </head>
  <body>
    <div id="app"></div>
    <script type="module" src="/src/main.ts"></script>
  </body>
</html>
```

## Development Workflow

### Running the Playground

```bash
cd playground
npm install
npm run dev
```

The playground will be available at `http://localhost:5174`

### Building the Playground

```bash
cd playground
npm run build
```

Build output goes to `playground/dist/`

### Hot Module Replacement

When you edit files in the parent library (`../src/`), the playground will automatically reload since it imports from source.

## Best Practices

1. **Import from source during development**
   ```typescript
   import { Component } from '../../../src/index'  // ✓ Good
   import { Component } from '@yourorg/your-library'  // ✗ Bad in playground
   ```

2. **Keep playground package.json separate**
   - Don't add the library as a dependency
   - Use same versions of peer dependencies

3. **Create meaningful demo pages**
   - Show all major features
   - Include usage examples
   - Document configuration options

4. **Use consistent theming**
   - Match library's expected theme
   - Demonstrate theme customization

5. **Add both declarative and programmatic examples**
   - Show component usage in templates
   - Show API usage in scripts

6. **Test library integration**
   - Ensure imports work correctly
   - Test store integration
   - Test router integration

7. **Document edge cases**
   - Show error handling
   - Demonstrate loading states
   - Show empty states

8. **Keep it simple**
   - Don't add unnecessary dependencies
   - Focus on library demonstration
   - Avoid complex playground-specific features

## Common Issues

### Issue: "Cannot resolve module" errors
**Solution:** Check that imports use correct relative path: `../../../src/index`

### Issue: Playground and library use different Vue instances
**Solution:** Ensure Vue version matches in both package.json files

### Issue: Styles not applying
**Solution:** Verify Vuetify is properly configured in playground/src/plugins/vuetify.ts

### Issue: Router conflicts
**Solution:** Playground should create its own router, not use library's plugin

### Issue: Store not persisting
**Solution:** Playground must manually install Pinia, not rely on library plugin
