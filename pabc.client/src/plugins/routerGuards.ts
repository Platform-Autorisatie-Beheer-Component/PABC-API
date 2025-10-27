import { refreshUser, user } from "@/composables/use-user";
import type { App } from "vue";
import type { Router } from "vue-router";
import type { RouteLocationNormalized, NavigationGuardNext } from "vue-router";

const FORBIDDEN = "forbidden";
const LOGIN = "login";

async function refreshUserGuard(
  _to: RouteLocationNormalized,
  _from: RouteLocationNormalized,
  next: NavigationGuardNext
) {
  await refreshUser();
  return next();
}

function loginGuard(
  to: RouteLocationNormalized,
  _from: RouteLocationNormalized,
  next: NavigationGuardNext
) {
  if (to.name === LOGIN && user.value.isLoggedIn) {
    return next({ path: to.query.returnUrl?.toString() || "/" });
  }
  if (to.name !== LOGIN && !user.value.isLoggedIn) {
    return next({ name: LOGIN, query: { returnUrl: to.fullPath } });
  }
  return next();
}

function functioneelBeheerGuard(
  to: RouteLocationNormalized,
  _from: RouteLocationNormalized,
  next: NavigationGuardNext
) {
  if (to.name === FORBIDDEN && user.value.hasFunctioneelBeheerderAccess) {
    return next({ path: "/" });
  }
  if (to.name !== FORBIDDEN && !user.value.hasFunctioneelBeheerderAccess) {
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
    router.beforeEach(refreshUserGuard);
    router.beforeEach(loginGuard);
    router.beforeEach(functioneelBeheerGuard);
    router.beforeEach(titleGuard);
  }
};
