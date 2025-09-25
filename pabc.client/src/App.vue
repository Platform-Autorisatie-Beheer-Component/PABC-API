<template>
  <toast-section />

  <the-header />

  <main id="main">
    <router-view />
  </main>

  <the-footer />
</template>

<script setup lang="ts">
import { RouterView } from "vue-router";
import ToastSection from "./components/toast/ToastSection.vue";
import TheHeader from "./components/TheHeader.vue";
import TheFooter from "./components/TheFooter.vue";
</script>
