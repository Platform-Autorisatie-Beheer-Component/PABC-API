import { del, get, post, put } from "@/utils/fetchWrapper";

export type Item = { id?: string; name: string };

export type Domain = Item & {
  description: string;
};

export type FunctionalRole = Item;

export type EntityType = Item & {
  entityTypeId: string;
  type: string;
  uri: string;
};

const createPabcService = <T extends Item>(endpoint: string) => ({
  getAll: (): Promise<T[]> =>
    get<T[]>(`/api/v1/${endpoint}`).then((items) =>
      items.sort((a, b) => a.name.localeCompare(b.name))
    ),
  getById: (id: string): Promise<T> => get<T>(`/api/v1/${endpoint}/${id}`),
  create: (payload: T): Promise<T> => post<T>(`/api/v1/${endpoint}`, payload),
  update: (payload: T): Promise<T> => put<T>(`/api/v1/${endpoint}/${payload.id}`, payload),
  delete: (id: string): Promise<void> => del(`/api/v1/${endpoint}/${id}`)
});

export type PabcService<T extends Item> = ReturnType<typeof createPabcService<T>>;

export const domainService = createPabcService<Domain>("domains");

export const functionalRoleService = createPabcService<FunctionalRole>("functional-roles");

export const entityTypeService = createPabcService<EntityType>("entity-types");
