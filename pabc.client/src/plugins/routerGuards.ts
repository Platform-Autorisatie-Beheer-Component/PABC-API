import { refreshUser, user } from "@/composables/use-user";
import type { App } from "vue";
import type { Router } from "vue-router";
import type { RouteLocationNormalized, NavigationGuardNext } from "vue-router";

const FORBIDDEN = "forbidden";
const LOGIN = "login";

async function authGuard(
  to: RouteLocationNormalized,
  _from: RouteLocationNormalized,
  next: NavigationGuardNext
) {
  await refreshUser();
  if (to.name === LOGIN) {
    return user.value.isLoggedIn ? next({ path: to.query.returnUrl?.toString() || "/" }) : next();
  }
  if (to.name === FORBIDDEN) {
    return user.value.hasFunctioneelBeheerderAccess ? next({ path: "/" }) : next();
  }
  if (!user.value.isLoggedIn) {
    return next({ name: LOGIN, query: { returnUrl: to.fullPath } });
  }
  if (!user.value.hasFunctioneelBeheerderAccess) {
    return next({ name: FORBIDDEN });
  }
  return next();
}

function titleGuard(
  to: RouteLocationNormalized,
  _from: RouteLocationNormalized,
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
  install(_app: App, router: Router) {
    router.beforeEach(titleGuard);
    router.beforeEach(authGuard);
  }
};
