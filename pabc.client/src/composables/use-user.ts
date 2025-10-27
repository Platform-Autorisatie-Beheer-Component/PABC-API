import type { User } from "@/types/user";
import { get } from "@/utils/fetchWrapper";
import { ref, type DeepReadonly } from "vue";

const _user = ref<User>({
  email: "",
  isLoggedIn: false,
  name: "",
  roles: [],
  hasFunctioneelBeheerderAccess: false
});

export const user = _user as DeepReadonly<typeof _user>;
export const refreshUser = async () => (_user.value = await get<User>("/api/me"));
