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
      </ul>
    </nav>
  </header>
</template>
