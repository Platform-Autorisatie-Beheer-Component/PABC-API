<template>
  <toast-section />

  <the-header />

  <main id="main">
    <router-view />
  </main>
</template>

<script setup lang="ts">
import { RouterView } from "vue-router";
import ToastSection from "./components/toast/ToastSection.vue";
import TheHeader from "./components/TheHeader.vue";
</script>
