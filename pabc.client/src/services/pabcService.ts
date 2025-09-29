import { del, get, post, put } from "@/utils/fetchWrapper";

export type Item = { id?: string; name: string };

export type Domain = Item & {
  description: string;
};

export type FunctionalRole = Item;

const createService = <T extends Item>(endpoint: string) => ({
  getAll: (): Promise<T[]> => get<T[]>(`/api/v1/${endpoint}`),
  getById: (id: string): Promise<T> => get<T>(`/api/v1/${endpoint}/${id}`),
  create: (payload: T): Promise<T> => post<T>(`/api/v1/${endpoint}`, payload),
  update: (payload: T): Promise<T> => put<T>(`/api/v1/${endpoint}/${payload.id}`, payload),
  delete: (id: string): Promise<void> => del(`/api/v1/${endpoint}/${id}`)
});

export type PabcService<T extends Item> = ReturnType<typeof createService<T>>;

export const domainService = createService<Domain>("domains");

export const functionalRoleService = createService<FunctionalRole>("functional-roles");
