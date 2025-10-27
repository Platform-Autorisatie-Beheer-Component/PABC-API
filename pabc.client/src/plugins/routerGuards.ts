import toast from "@/components/toast/toast";
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
  await refreshUser().catch((reason) => {
    toast.add({
      text: "Fout bij verversen gebruikersgegevens. Neem contact op met een Beheerder.",
      type: "error"
    });
    return Promise.reject(reason);
  });

  const isLoginPage = to.name === LOGIN;
  const isForbiddenPage = to.name === FORBIDDEN;

  // Not logged in: redirect to login page
  if (!user.value.isLoggedIn)
    return isLoginPage ? next() : next({ name: LOGIN, query: { returnUrl: to.fullPath } });

  // Logged in: redirect away from login page
  if (isLoginPage) return next({ path: to.query.returnUrl?.toString() || "/" });

  // No access: redirect to forbidden page
  if (!user.value.hasFunctioneelBeheerderAccess)
    return isForbiddenPage ? next() : next({ name: FORBIDDEN });

  // Has access: redirect away from forbidden page
  if (isForbiddenPage) return next("/");

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
    router.beforeEach(authGuard);
    router.beforeEach(titleGuard);
  }
};
