<template>
  <header>
    <span class="heading-1">PABC</span>

    <nav>
      <ul>
        <li>
          <router-link :to="{ name: 'dashboard' }">Dashboard</router-link>
        </li>

        <li>
          <router-link :to="{ name: 'admin' }">Beheer</router-link>
        </li>

        <li>
          <router-link :to="{ name: 'domains' }">Domeinen</router-link>
        </li>

        <li>
          <router-link :to="{ name: 'functionalRoles' }">Functionele rollen</router-link>
        </li>

        <li>
          <router-link :to="{ name: 'entityTypes' }">Entiteit types</router-link>
        </li>
      </ul>
    </nav>
  </header>
</template>

<script lang="ts" setup></script>

<style lang="scss" scoped></style>
