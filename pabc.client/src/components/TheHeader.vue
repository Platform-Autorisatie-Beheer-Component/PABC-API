<template>
  <header v-if="user.isLoggedIn">
    <nav>
      <ul>
        <template v-if="user.hasFunctioneelBeheerderAccess">
          <li>
            <router-link :to="{ name: 'dashboard' }">Dashboard</router-link>
          </li>
          <li>
            <router-link :to="{ name: 'admin' }">Beheer</router-link>
          </li>
        </template>
        <li class="user-name">
          {{ user.name }}
        </li>
        <li>
          <a href="/api/logoff">Uitloggen</a>
        </li>
      </ul>
    </nav>
  </header>
</template>

<script setup lang="ts">
import { user } from "@/composables/use-user";
</script>

<style lang="scss" scoped>
.user-name {
  margin-inline-start: auto;
}
</style>
