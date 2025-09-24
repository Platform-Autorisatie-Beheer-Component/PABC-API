import { del, get, post, put } from "@/utils/fetchWrapper";

export type Domain = {
  id?: string;
  name: string;
  description: string;
};

export type FunctionalRole = {
  id: string;
  name: string;
};

const createService = <T extends { id?: string }>(endpoint: string) => ({
  getAll: (): Promise<T[]> => {
    return get<T[]>(`/api/v1/${endpoint}`);
  },
  getById: (id: string): Promise<T> => {
    return get<T>(`/api/v1/${endpoint}/${id}`);
  },
  create: (payload: T): Promise<T> => {
    return post<T>(`/api/v1/${endpoint}`, payload);
  },
  update: (payload: T): Promise<T> => {
    return put<T>(`/api/v1/${endpoint}/${payload.id}`, payload);
  },
  delete: (id: string): Promise<void> => {
    return del(`/api/v1/${endpoint}/${id}`);
  }
});

export const domainService = createService<Domain>("domains");

export const functionalRoleService = createService<FunctionalRole>("functionalroles");
