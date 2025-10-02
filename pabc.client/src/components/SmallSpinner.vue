<template>
  <span class="spinner" role="presentation" aria-hidden="true"></span>
</template>

<style lang="scss" scoped>
.spinner {
  --_spinner-size: 2rem;

  display: block;
  block-size: var(--_spinner-size);
  inline-size: var(--_spinner-size);
  border: 0.2rem solid var(--disabled);
  border-top-color: var(--accent);
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}
</style>
