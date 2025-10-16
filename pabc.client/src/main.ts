import "./assets/base.css";
import "./assets/main.scss";

import { createApp } from "vue";
import App from "./App.vue";
import { pinia } from "./stores";

const app = createApp(App);


app.use(pinia);

(async () => {

  // Load router after theme, to be able to use theme settings
  const { default: router } = await import("./router");
  const { default: routerGuardsPlugin } = await import("./plugins/routerGuards");

  app.use(router);
  app.use(routerGuardsPlugin, router);

  app.mount("#app");
})();
