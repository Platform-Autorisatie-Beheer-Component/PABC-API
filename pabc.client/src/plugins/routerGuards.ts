import { authService } from "@/services/authService";
import type { App } from "vue";
import type { Router } from "vue-router";
import type { RouteLocationNormalized, NavigationGuardNext } from "vue-router";

const FORBIDDEN = "forbidden";

async function authGuard(
  to: RouteLocationNormalized,
  _from: RouteLocationNormalized,
  next: NavigationGuardNext
) {
  const user = await authService.getCurrentUser();
  if (!user?.isLoggedIn) {
    return authService.login(to.fullPath);
  }
  if (to.name !== FORBIDDEN && !user?.hasFunctioneelBeheerderAccess) {
    return next({ name: FORBIDDEN });
  }
  return next();
}

function titleGuard(
  to: RouteLocationNormalized,
  from: RouteLocationNormalized,
  next: NavigationGuardNext
) {
  const appTitle = document.title.split(" | ").pop() || document.title;
  document.title = `${to.meta?.title ? to.meta.title + " | " : ""}${appTitle}`;

  document.body.setAttribute("tabindex", "-1");
  document.body.focus();
  document.body.removeAttribute("tabindex");

  return next();
}

export default {
  install(app: App, router: Router) {
    router.beforeEach(titleGuard);
    router.beforeEach(authGuard);
  }
};
