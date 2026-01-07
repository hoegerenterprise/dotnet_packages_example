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
    defaultTheme: 'light',
    themes: {
      light: {
        colors: {
          primary: '#1976D2',
          secondary: '#424242',
          success: '#4CAF50',
          error: '#F44336',
          warning: '#FF9800',
          info: '#2196F3',
        }
      },
      dark: {
        colors: {
          primary: '#2196F3',
          secondary: '#424242',
          success: '#4CAF50',
          error: '#F44336',
          warning: '#FF9800',
          info: '#2196F3',
        }
      }
    }
  }
})
