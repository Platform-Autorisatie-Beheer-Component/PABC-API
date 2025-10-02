import { createRouter, createWebHistory } from "vue-router";
import DashboardView from "@/views/DashboardView.vue";
import AdminView from "@/views/AdminView.vue";
import DomainsView from "@/views/DomainsView.vue";
import DomainView from "@/views/DomainView.vue";
import FunctionalRolesView from "@/views/FunctionalRolesView.vue";
import FunctionalRoleView from "@/views/FunctionalRoleView.vue";
import EntityTypesView from "@/views/EntityTypesView.vue";

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
    },
    {
      path: "/entiteit-types",
      name: "entityTypes",
      component: EntityTypesView,
      meta: {
        title: "Entiteit types"
      }
    }
  ]
});

const title = document.title;

router.beforeEach((to) => {
  document.title = `${to.meta?.title ? to.meta.title + " | " : ""}${title}`;
});

export default router;
