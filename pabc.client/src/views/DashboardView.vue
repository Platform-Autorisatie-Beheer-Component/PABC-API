<template>
  <div class="page">
    <h1>Dashboard</h1>

    <menu class="reset simple-grid">
      <li>
        <router-link :to="{ name: 'domains' }">Domeinen</router-link>
      </li>
    </menu>
  </div>
</template>

<script setup lang="ts"></script>

<style lang="scss" scoped>
a {
  display: block;
  text-align: center;
  text-decoration: none;
  padding: var(--spacing-extralarge);
  border: 1px solid var(--border);
  border-block-end-color: var(--secondary);
  background-color: var(--secondary-light);

  &:hover,
  &:active,
  &:focus-visible {
    text-decoration: underline;
  }
}
</style>
