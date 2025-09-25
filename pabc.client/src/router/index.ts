import { createRouter, createWebHistory } from "vue-router";
import DashboardView from "@/views/DashboardView.vue";
import DomainsView from "@/views/DomainsView.vue";
import DomainView from "@/views/DomainView.vue";
import FunctionalRolesView from "@/views/FunctionalRolesView.vue";
import FunctionalRoleView from "@/views/FunctionalRoleView.vue";

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
      path: "/domeinen",
      name: "domains",
      component: DomainsView,
      meta: {
        title: "Domeinen"
      }
    },
    {
      path: "/domein/:id?",
      name: "domain",
      component: DomainView,
      props: true,
      meta: {
        title: "Domein"
      }
    },
    {
      path: "/functionele-rollen",
      name: "functionalRoles",
      component: FunctionalRolesView,
      meta: {
        title: "Functionele rollen"
      }
    },
    {
      path: "/functionele-rol/:id?",
      name: "functionalRole",
      component: FunctionalRoleView,
      props: true,
      meta: {
        title: "Functionele rol"
      }
    }
  ]
});

const title = document.title;

router.beforeEach((to) => {
  document.title = `${to.meta?.title ? to.meta.title + " | " : ""}${title}`;
});

export default router;
