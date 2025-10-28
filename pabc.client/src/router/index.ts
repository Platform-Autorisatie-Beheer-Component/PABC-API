import { createRouter, createWebHistory } from "vue-router";
import DashboardView from "@/views/DashboardView.vue";
import DomainsEntityTypesView from "@/views/DomainsEntityTypesView.vue";
import AdminView from "@/views/AdminView.vue";
import UnauthorizedView from "@/views/UnauthorizedView.vue";
import LoginView from "@/views/LoginView.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "dashboard",
      component: DashboardView,
      meta: {
        title: "Dashboard"
      }
    },
    {
      path: "/domeinen",
      name: "domains",
      component: DomainsEntityTypesView,
      meta: {
        title: "Domeinen met Entiteitstypes"
      }
    },
    {
      path: "/beheer",
      name: "admin",
      component: AdminView,
      meta: {
        title: "Beheer"
      }
    },
    {
      path: "/forbidden",
      name: "forbidden",
      component: UnauthorizedView,
      meta: {
        title: "Forbidden"
      }
    },
    {
      path: "/login",
      name: "login",
      component: LoginView,
      meta: {
        title: "Login"
      }
    }
  ]
});

export default router;
