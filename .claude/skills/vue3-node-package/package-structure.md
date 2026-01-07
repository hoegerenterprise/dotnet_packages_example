# NPM Package Structure for Vue 3 Library

This guide details how to configure a Vue 3 application as an installable npm package with proper ESM/UMD exports.

## Package.json Configuration

### Essential Fields

```json
{
  "name": "@yourorg/your-library",
  "version": "1.0.0",
  "description": "Your Vue 3 component library",
  "type": "module",
  "author": "Your Name",
  "license": "MIT",
  "keywords": ["vue", "vuetify", "components"],

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
  },

  "scripts": {
    "dev": "vite",
    "build": "vite build",
    "build:lib": "vite build --mode lib",
    "preview": "vite preview"
  },

  "dependencies": {},

  "peerDependencies": {
    "vue": "^3.5.0",
    "vuetify": "^3.11.0",
    "vue-router": "^4.5.0",
    "pinia": "^3.0.0"
  },

  "devDependencies": {
    "@vitejs/plugin-vue": "^6.0.0",
    "typescript": "^5.7.0",
    "vite": "^7.3.0",
    "vite-plugin-dts": "^4.4.0",
    "vite-plugin-vuetify": "^2.0.0",
    "vue-tsc": "^2.2.0"
  }
}
```

## Field Explanations

### Module Type
```json
"type": "module"
```
- Treats .js files as ES modules
- Required for modern npm packages

### Entry Points
```json
"main": "./dist/your-library.umd.cjs",    // CommonJS entry
"module": "./dist/your-library.js",       // ESM entry
"types": "./dist/index.d.ts"              // TypeScript definitions
```

### Exports Field
```json
"exports": {
  ".": {
    "types": "./dist/index.d.ts",
    "import": "./dist/your-library.js",
    "require": "./dist/your-library.umd.cjs"
  }
}
```
- Modern way to define package entry points
- `types` - TypeScript type definitions (must come first in some bundlers)
- `import` - For ESM imports (`import { Component } from 'package'`)
- `require` - For CommonJS requires (`const { Component } = require('package')`)

### Files Field
```json
"files": ["dist"]
```
- Specifies which files to include when publishing
- Only dist/ folder is published, source files are excluded

### Peer Dependencies
```json
"peerDependencies": {
  "vue": "^3.5.0",
  "vuetify": "^3.11.0",
  "vue-router": "^4.5.0",
  "pinia": "^3.0.0"
}
```
- Packages that consuming projects must install
- Not bundled with library
- Prevents version conflicts and duplication

## Vite Library Build Configuration

### vite.config.ts for Library

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
        insertTypesEntry: true,
      }),
    ].filter(Boolean),

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
        formats: ['es', 'umd'],
        fileName: (format) => {
          return format === 'es'
            ? 'your-library.js'
            : 'your-library.umd.cjs'
        }
      },
      rollupOptions: {
        external: ['vue', 'vue-router', 'pinia', 'vuetify'],
        output: {
          globals: {
            vue: 'Vue',
            'vue-router': 'VueRouter',
            pinia: 'Pinia',
            vuetify: 'Vuetify'
          },
          assetFileNames: (assetInfo) => {
            if (assetInfo.name === 'style.css') {
              return 'your-library.css'
            }
            return assetInfo.name
          }
        }
      },
      sourcemap: true,
      emptyOutDir: true,
    } : {
      outDir: 'dist',
      sourcemap: true,
    }
  }
})
```

### Key Configuration Sections

#### 1. Dual-Mode Configuration
```typescript
const isLib = mode === 'lib'
```
- Supports both app development (`npm run dev`) and library build (`npm run build:lib`)
- Different build configurations for each mode

#### 2. Library Build Options
```typescript
lib: {
  entry: resolve(__dirname, 'src/index.ts'),
  name: 'YourLibrary',
  formats: ['es', 'umd'],
  fileName: (format) => format === 'es' ? 'your-library.js' : 'your-library.umd.cjs'
}
```
- `entry` - Library entry point (src/index.ts)
- `name` - Global variable name for UMD build
- `formats` - Build both ES modules and UMD
- `fileName` - Custom output file names

#### 3. External Dependencies
```typescript
external: ['vue', 'vue-router', 'pinia', 'vuetify']
```
- Dependencies not bundled with library
- Must be provided by consuming application
- Reduces bundle size and prevents version conflicts

#### 4. Globals for UMD
```typescript
globals: {
  vue: 'Vue',
  'vue-router': 'VueRouter',
  pinia: 'Pinia',
  vuetify: 'Vuetify'
}
```
- Maps external dependencies to global variables
- Required for UMD build to work in browser

#### 5. TypeScript Declarations
```typescript
dts({
  tsconfigPath: './tsconfig.json',
  outDir: 'dist',
  insertTypesEntry: true,
})
```
- Generates .d.ts files for TypeScript support
- Places them in dist/ alongside built files

## TypeScript Configuration

### tsconfig.json

```json
{
  "compilerOptions": {
    "target": "ES2020",
    "useDefineForClassFields": true,
    "module": "ESNext",
    "lib": ["ES2020", "DOM", "DOM.Iterable"],
    "skipLibCheck": true,
    "declaration": true,
    "declarationDir": "./dist",
    "emitDeclarationOnly": false,

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
  "exclude": [
    "node_modules",
    "dist",
    "playground"
  ]
}
```

### Key TypeScript Options

- `declaration: true` - Generate .d.ts files
- `declarationDir: "./dist"` - Output declarations to dist
- `paths: {"@/*": ["./src/*"]}` - Path alias support
- `exclude: ["playground"]` - Don't compile playground code

## Library Entry Point (src/index.ts)

### Complete Entry Point Structure

```typescript
import type { App, Plugin } from 'vue'
import { createPinia } from 'pinia'
import { createRouter, createWebHistory } from 'vue-router'

// ===== TYPES =====
export * from './types'

// ===== STORES =====
export { useStore1 } from './stores/store1'
export { useStore2 } from './stores/store2'

// ===== COMPONENTS =====
export { default as Component1 } from './components/Component1.vue'
export { default as Component2 } from './components/Component2.vue'
export { default as Component3 } from './components/Component3.vue'

// ===== VIEWS =====
export { default as View1 } from './views/View1.vue'
export { default as View2 } from './views/View2.vue'
export { default as View3 } from './views/View3.vue'

// ===== ROUTER FACTORY =====
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
      {
        path: '/route3',
        name: 'route3',
        component: () => import('./views/View3.vue'),
      },
    ],
  })
}

// ===== PLUGIN OPTIONS =====
export interface LibraryOptions {
  baseUrl?: string
  apiBaseUrl?: string
  theme?: 'light' | 'dark'
}

// ===== MAIN PLUGIN =====
export const YourLibraryPlugin: Plugin = {
  install(app: App, options: LibraryOptions = {}) {
    // Install Pinia if not already installed
    if (!app.config.globalProperties.$pinia) {
      const pinia = createPinia()
      app.use(pinia)
    }

    // Create and install router if baseUrl is provided
    if (options.baseUrl !== undefined) {
      const router = createLibraryRouter(options.baseUrl)
      app.use(router)
    }

    // Store configuration in global properties
    if (options.apiBaseUrl) {
      app.config.globalProperties.$apiBaseUrl = options.apiBaseUrl
    }

    if (options.theme) {
      app.config.globalProperties.$theme = options.theme
    }
  }
}

// ===== DEFAULT EXPORT =====
export default YourLibraryPlugin
```

## Usage in Consuming Applications

### Installation

```bash
npm install @yourorg/your-library
```

### Basic Usage

```typescript
import { createApp } from 'vue'
import App from './App.vue'
import YourLibraryPlugin from '@yourorg/your-library'
import '@yourorg/your-library/dist/your-library.css'

const app = createApp(App)

app.use(YourLibraryPlugin, {
  baseUrl: '/',
  apiBaseUrl: '/api/v1',
  theme: 'dark'
})

app.mount('#app')
```

### Importing Individual Components

```typescript
import { Component1, Component2, View1 } from '@yourorg/your-library'
```

### Using with Custom Router

```typescript
import { createRouter, createWebHistory } from 'vue-router'
import { View1, View2 } from '@yourorg/your-library'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', component: View1 },
    { path: '/route2', component: View2 },
  ]
})
```

## Publishing to NPM

### Pre-publish Checklist

1. Build library: `npm run build:lib`
2. Check dist/ folder has:
   - `your-library.js` (ESM)
   - `your-library.umd.cjs` (UMD)
   - `index.d.ts` (TypeScript types)
   - `your-library.css` (styles)
3. Test in consuming application
4. Update version in package.json

### Publishing Commands

```bash
# Login to npm
npm login

# Publish public package
npm publish --access public

# Publish scoped package
npm publish
```

### Version Management

```bash
# Patch release (1.0.0 -> 1.0.1)
npm version patch

# Minor release (1.0.0 -> 1.1.0)
npm version minor

# Major release (1.0.0 -> 2.0.0)
npm version major
```

## .npmignore

Create `.npmignore` to exclude files from npm package:

```
# Source files
src/
public/

# Development
*.md
!README.md
.vscode/
.idea/

# Config files
vite.config.ts
tsconfig.json
tsconfig.node.json

# Tests
tests/
*.spec.ts
*.test.ts

# CI/CD
.github/
.gitlab-ci.yml

# Other
playground/
backend/
.env*
```

## Common Issues and Solutions

### Issue: "Cannot find module" errors
**Solution:** Ensure `types` field points to correct .d.ts file and vite-plugin-dts is configured

### Issue: Styles not applied
**Solution:** Consumers must import CSS: `import '@yourorg/your-library/dist/your-library.css'`

### Issue: Large bundle size
**Solution:** Check external dependencies are properly configured in rollupOptions

### Issue: TypeScript errors in consuming app
**Solution:** Ensure `declaration: true` in tsconfig.json and dist/*.d.ts files exist

### Issue: Duplicate Vue instances
**Solution:** Make sure Vue is in both `external` and `peerDependencies`
