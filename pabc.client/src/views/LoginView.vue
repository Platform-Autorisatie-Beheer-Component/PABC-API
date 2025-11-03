<template>
  <div class="page">
    <h1>U bent niet ingelogd</h1>
    <p>Om toegang tot deze pagina te krijgen, dient u eerst in te loggen.</p>
    <a class="button" :href="`/api/challenge?returnUrl=${$route.query.returnUrl}`">Inloggen</a>
  </div>
</template>
