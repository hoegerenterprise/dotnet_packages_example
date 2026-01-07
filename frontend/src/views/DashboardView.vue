<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { productsApi, customersApi, ordersApi, userGroupsApi } from '@/services/api'
import type { Product, Customer, Order, UserGroup } from '@/types'

const authStore = useAuthStore()

const stats = ref({
  products: 0,
  customers: 0,
  orders: 0,
  userGroups: 0
})

const recentProducts = ref<Product[]>([])
const recentOrders = ref<Order[]>([])
const loading = ref(true)

onMounted(async () => {
  try {
    const [products, customers, orders, groups] = await Promise.all([
      productsApi.getAll(),
      customersApi.getAll(),
      ordersApi.getAll(),
      userGroupsApi.getAll()
    ])

    stats.value = {
      products: products.length,
      customers: customers.length,
      orders: orders.length,
      userGroups: groups.length
    }

    recentProducts.value = products.slice(0, 3)
    recentOrders.value = orders.slice(0, 5)
  } catch (error) {
    console.error('Failed to fetch dashboard data:', error)
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <v-container fluid>
    <v-row class="mb-4">
      <v-col cols="12">
        <h1 class="text-h3 mb-2">
          <v-icon icon="mdi-view-dashboard" class="mr-2"></v-icon>
          Dashboard
        </h1>
        <p class="text-h6 text-medium-emphasis">
          Welcome back, {{ authStore.username }}!
        </p>
      </v-col>
    </v-row>

    <v-row v-if="loading">
      <v-col cols="12" class="text-center">
        <v-progress-circular indeterminate size="64"></v-progress-circular>
      </v-col>
    </v-row>

    <template v-else>
      <!-- Stats Cards -->
      <v-row>
        <v-col cols="12" sm="6" md="3">
          <v-card color="primary" variant="tonal">
            <v-card-text>
              <div class="text-h6 mb-2">
                <v-icon icon="mdi-package-variant"></v-icon>
                Products
              </div>
              <div class="text-h3">{{ stats.products }}</div>
            </v-card-text>
          </v-card>
        </v-col>

        <v-col cols="12" sm="6" md="3">
          <v-card color="success" variant="tonal">
            <v-card-text>
              <div class="text-h6 mb-2">
                <v-icon icon="mdi-account-group"></v-icon>
                Customers
              </div>
              <div class="text-h3">{{ stats.customers }}</div>
            </v-card-text>
          </v-card>
        </v-col>

        <v-col cols="12" sm="6" md="3">
          <v-card color="warning" variant="tonal">
            <v-card-text>
              <div class="text-h6 mb-2">
                <v-icon icon="mdi-cart"></v-icon>
                Orders
              </div>
              <div class="text-h3">{{ stats.orders }}</div>
            </v-card-text>
          </v-card>
        </v-col>

        <v-col cols="12" sm="6" md="3">
          <v-card color="info" variant="tonal">
            <v-card-text>
              <div class="text-h6 mb-2">
                <v-icon icon="mdi-shield-account"></v-icon>
                User Groups
              </div>
              <div class="text-h3">{{ stats.userGroups }}</div>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <!-- User Info -->
      <v-row class="mt-4">
        <v-col cols="12" md="6">
          <v-card>
            <v-card-title>
              <v-icon icon="mdi-account" class="mr-2"></v-icon>
              Your Profile
            </v-card-title>
            <v-card-text>
              <v-list>
                <v-list-item>
                  <v-list-item-title>Username</v-list-item-title>
                  <v-list-item-subtitle>{{ authStore.username }}</v-list-item-subtitle>
                </v-list-item>
                <v-list-item>
                  <v-list-item-title>Email</v-list-item-title>
                  <v-list-item-subtitle>{{ authStore.email }}</v-list-item-subtitle>
                </v-list-item>
                <v-list-item>
                  <v-list-item-title>Groups</v-list-item-title>
                  <v-list-item-subtitle>
                    <v-chip
                      v-for="group in authStore.groups"
                      :key="group"
                      size="small"
                      class="mr-1"
                      color="primary"
                    >
                      {{ group }}
                    </v-chip>
                  </v-list-item-subtitle>
                </v-list-item>
              </v-list>
            </v-card-text>
          </v-card>
        </v-col>

        <v-col cols="12" md="6">
          <v-card>
            <v-card-title>
              <v-icon icon="mdi-package-variant" class="mr-2"></v-icon>
              Recent Products
            </v-card-title>
            <v-card-text>
              <v-list>
                <v-list-item
                  v-for="product in recentProducts"
                  :key="product.id"
                >
                  <v-list-item-title>{{ product.name }}</v-list-item-title>
                  <v-list-item-subtitle>
                    ${{ product.price.toFixed(2) }} - {{ product.category }}
                  </v-list-item-subtitle>
                </v-list-item>
              </v-list>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <!-- Recent Orders -->
      <v-row class="mt-4">
        <v-col cols="12">
          <v-card>
            <v-card-title>
              <v-icon icon="mdi-cart" class="mr-2"></v-icon>
              Recent Orders
            </v-card-title>
            <v-card-text>
              <v-table>
                <thead>
                  <tr>
                    <th>Order ID</th>
                    <th>Customer</th>
                    <th>Product</th>
                    <th>Quantity</th>
                    <th>Total</th>
                    <th>Date</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="order in recentOrders" :key="order.id">
                    <td>{{ order.id }}</td>
                    <td>{{ order.customerName }}</td>
                    <td>{{ order.productName }}</td>
                    <td>{{ order.quantity }}</td>
                    <td>${{ order.totalAmount.toFixed(2) }}</td>
                    <td>{{ new Date(order.orderDate).toLocaleDateString() }}</td>
                  </tr>
                </tbody>
              </v-table>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </template>
  </v-container>
</template>
