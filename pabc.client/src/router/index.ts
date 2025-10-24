import { createRouter, createWebHistory } from "vue-router";
import DashboardView from "@/views/DashboardView.vue";
import AdminView from "@/views/AdminView.vue";
import UnauthorizedView from "@/views/UnauthorizedView.vue";

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
    },
    {
      path: "/beheer",
      name: "admin",
      component: AdminView,
      meta: {
        title: "PABC - Beheer"
      }
    },
    {
      path: "/forbidden",
      name: "forbidden",
      component: UnauthorizedView,
      meta: {
        title: "Forbidden"
      }
    }
  ]
});

const title = document.title;

router.beforeEach((to) => {
  document.title = `${to.meta?.title ? to.meta.title + " | " : ""}${title}`;
});

export default router;
