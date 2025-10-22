<template>
  <div>
    <h1>403 - Access Denied</h1>
    <p>You don't have permission to access this resource.</p>
    <p>If you believe this is an error, please contact your administrator.</p>
    <button @click="goHome">Go to Home</button>
  </div>
</template>

<script setup lang="ts">
  import { useRouter } from 'vue-router';

  const router = useRouter();

  const goHome = () => {
    router.push('/');
  };
</script>
