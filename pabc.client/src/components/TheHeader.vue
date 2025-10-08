<template>
  <header>
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
