import { createRouter, createWebHistory } from "vue-router";
import DashboardView from "@/views/DashboardView.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "dashboard",
      component: DashboardView,
      meta: {
        title: "PABC"
      }
    }
  ]
});

const title = document.title;

router.beforeEach((to) => {
  document.title = `${to.meta?.title ? to.meta.title + " | " : ""}${title}`;
});

export default router;
