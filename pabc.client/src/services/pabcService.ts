import { del, get, post, put } from "@/utils/fetchWrapper";

export type Item = { id?: string; name: string };

export type Domain = Item & {
  description: string;
};

export type FunctionalRole = Item;

export type EntityType = Item & {
  type: string;
  entityTypeId: string;
  uri: string;
};

export type ApplicationRole = Item & {
  applicationId: string;
  application: string;
};

export type Application = Item;

export type Mapping = {
  functionalRoleId: string;
  applicationRoleId: string;
  domainId: string | null;
  isAllEntityTypes: boolean;
};

export type DomainEntityTypes = Domain & {
  entityTypes: string[];
};

export type FunctionalRoleMappings = Required<FunctionalRole> & {
  mappings: MappingResponse[];
};

export type MappingResponse = Required<Item> & {
  applicationRoleId: string;
  domain: string | null;
  isAllEntityTypes: boolean;
};

const createPabcService = <T extends Item>(endpoint: string, createEmpty: () => T) => ({
  getAll: (): Promise<T[]> => get<T[]>(`/api/v1/${endpoint}`),
  getById: (id: string): Promise<T> => get<T>(`/api/v1/${endpoint}/${id}`),
  create: (payload: T): Promise<T> => post<T>(`/api/v1/${endpoint}`, payload),
  update: (payload: T): Promise<T> => put<T>(`/api/v1/${endpoint}/${payload.id}`, payload),
  delete: (id: string): Promise<void> => del(`/api/v1/${endpoint}/${id}`),
  createEmpty
});

export type PabcService<T extends Item> = ReturnType<typeof createPabcService<T>>;

export const domainService = createPabcService<Domain>("domains", () => ({
  name: "",
  description: ""
}));

export const functionalRoleService = createPabcService<FunctionalRole>("functional-roles", () => ({
  name: ""
}));

export const entityTypeService = createPabcService<EntityType>("entity-types", () => ({
  type: "",
  entityTypeId: "",
  name: "",
  uri: ""
}));

export const applicationRoleService = createPabcService<ApplicationRole>(
  "application-roles",
  () => ({
    name: "",
    applicationId: "",
    application: ""
  })
);

export const applicationService = createPabcService<Application>("applications", () => ({
  name: ""
}));

export const domainEntityTypesService = {
  getAll: (): Promise<DomainEntityTypes[]> => get<DomainEntityTypes[]>("/api/v1/domains"),
  add: (domainId: string, entityTypeId: string): Promise<void> =>
    post<void>(`/api/v1/domains/${domainId}/entity-types/${entityTypeId}`, undefined),
  remove: (domainId: string, entityTypeId: string): Promise<void> =>
    del(`/api/v1/domains/${domainId}/entity-types/${entityTypeId}`)
};

export const functionalRoleMappingsService = {
  getAll: (): Promise<FunctionalRoleMappings[]> =>
    get<FunctionalRoleMappings[]>("/api/v1/functional-roles"),
  add: (payload: Mapping): Promise<void> =>
    post<void>(`/api/v1/functional-roles/${payload.functionalRoleId}/mappings`, payload),
  remove: (functionalRoleId: string, mappingId: string): Promise<void> =>
    del(`/api/v1/functional-roles/${functionalRoleId}/mappings/${mappingId}`)
};
