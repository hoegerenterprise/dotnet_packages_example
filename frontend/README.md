# .NET Packages Demo - Frontend

Vue 3 + Vuetify frontend application with JWT authentication for the .NET 8 backend API.

## Features

- **JWT Authentication**: Secure login and registration with token-based authentication
- **User Management**: View and manage users (admin-only features)
- **Dashboard**: Overview of products, customers, orders, and user groups
- **Protected Routes**: Route guards ensure only authenticated users can access protected pages
- **Role-Based UI**: Different UI elements shown based on user roles (Admin, Manager, User)
- **Vuetify Material Design**: Modern, responsive UI components
- **TypeScript**: Full type safety throughout the application

## Tech Stack

- **Vue 3**: Progressive JavaScript framework with Composition API
- **Vuetify 3**: Material Design component library
- **TypeScript**: Type-safe development
- **Pinia**: State management
- **Vue Router**: Client-side routing with navigation guards
- **Vite**: Fast build tool and dev server

## Setup

### Install Dependencies

```bash
npm install
```

### Development Server

```bash
npm run dev
```

The frontend will be available at `http://localhost:5173`

**Important**: Make sure the .NET backend is running at `http://localhost:5000` before starting the frontend.

### Build for Production

```bash
npm run build
```

### Preview Production Build

```bash
npm run preview
```

## Project Structure

```
frontend/
├── src/
│   ├── components/      # Reusable Vue components
│   ├── views/           # Page components
│   │   ├── LoginView.vue
│   │   ├── RegisterView.vue
│   │   ├── DashboardView.vue
│   │   └── UsersView.vue
│   ├── stores/          # Pinia stores
│   │   ├── auth.ts      # Authentication state
│   │   └── users.ts     # Users management state
│   ├── router/          # Vue Router configuration
│   │   └── index.ts     # Routes and navigation guards
│   ├── services/        # API service layer
│   │   └── api.ts       # Backend API calls
│   ├── types/           # TypeScript type definitions
│   │   └── index.ts     # API response types
│   ├── plugins/         # Vue plugins
│   │   └── vuetify.ts   # Vuetify configuration
│   ├── App.vue          # Root component
│   └── main.ts          # Application entry point
├── index.html
├── package.json
├── vite.config.ts
└── tsconfig.json
```

## Usage

### Login

1. Navigate to `http://localhost:5173/login`
2. Use the test credentials or register a new account
3. After successful login, you'll be redirected to the dashboard

### Register

1. Click "Create Account" on the login page
2. Fill in the registration form:
   - Username (min 3 characters)
   - Email (valid email format)
   - Password (min 8 characters)
   - First Name
   - Last Name
3. After registration, you'll be redirected to login

### Dashboard

The dashboard shows:
- Statistics: Total products, customers, orders, and user groups
- Your profile information
- Recent products
- Recent orders

### User Management

Navigate to "Users" in the sidebar to:
- View all users in the system
- See user details (username, email, groups, status)
- Delete users (admin only)

## API Integration

The frontend connects to the .NET backend API at `http://localhost:5000/api/v1`.

### API Proxy

Vite proxy configuration forwards `/api` requests to the backend:

```typescript
proxy: {
  '/api': {
    target: 'http://localhost:5000',
    changeOrigin: true,
  }
}
```

### Authentication Flow

1. User submits login credentials
2. Frontend calls `POST /api/v1/auth/login`
3. Backend returns JWT token
4. Token is stored in localStorage
5. Subsequent API calls include `Authorization: Bearer <token>` header
6. Protected routes check for valid token before allowing access

### State Management

- **Auth Store** (`stores/auth.ts`): Manages authentication state, login/logout, JWT token
- **Users Store** (`stores/users.ts`): Manages user list and CRUD operations

## Configuration

### Theme

Edit `src/plugins/vuetify.ts` to customize colors and themes:

```typescript
theme: {
  defaultTheme: 'light',  // or 'dark'
  themes: {
    light: {
      colors: {
        primary: '#1976D2',
        // ... other colors
      }
    }
  }
}
```

### API Base URL

The API base URL is configured in `src/services/api.ts`:

```typescript
const API_BASE_URL = '/api/v1'
```

For production, update this to point to your deployed backend.

## Protected Routes

Routes are protected using Vue Router navigation guards:

```typescript
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next({ name: 'login' })
  } else {
    next()
  }
})
```

## Role-Based Features

Different features are shown based on user roles:

- **Administrators**: Full access, can delete users
- **Managers**: Can view and update users
- **Users**: Can view their own data and dashboard

Check roles in components:

```vue
<v-btn v-if="authStore.isAdmin">Admin Only Button</v-btn>
```

## Development Notes

- Hot Module Replacement (HMR) is enabled for fast development
- TypeScript type checking is performed during build
- All API calls are typed using TypeScript interfaces
- State is persisted in localStorage for JWT tokens

## Troubleshooting

### CORS Errors

If you see CORS errors, make sure:
1. The backend is running on `http://localhost:5000`
2. The backend has CORS configured to allow requests from `http://localhost:5173`

### 401 Unauthorized

If you get 401 errors:
1. Check that your JWT token is valid (not expired)
2. Try logging out and logging back in
3. Check browser console for token presence in localStorage

### Cannot Connect to Backend

1. Verify the backend is running: `curl http://localhost:5000/api/v1/products`
2. Check the Vite proxy configuration in `vite.config.ts`
3. Ensure no firewall is blocking ports 5000 or 5173
