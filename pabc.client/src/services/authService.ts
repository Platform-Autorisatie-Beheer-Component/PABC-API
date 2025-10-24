import type { User } from "@/types/user";
import { get } from "@/utils/fetchWrapper";

class AuthService {
  async getCurrentUser(): Promise<User | null> {
    try {
      const data = await get<User>(`/api/me`);

      if (data) {
        const userData: User = {
          isLoggedIn: data.isLoggedIn,
          email: data.email || "",
          name: data.name || "",
          roles: data.roles || [],
          hasFunctioneelBeheerderAccess: data.hasFunctioneelBeheerderAccess || false
        };
        return userData;
      }
      return null;
    } catch (error) {
      throw error;
    }
  }

  login(returnUrl?: string) {
    const currentUrl = window.location.href;

    const encodedReturnUrl = encodeURIComponent(returnUrl || currentUrl);

    window.location.href = `/api/challenge?returnUrl=${encodedReturnUrl}`;
  }
}

export const authService = new AuthService();
