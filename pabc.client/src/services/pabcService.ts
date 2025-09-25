import { del, get, post, put } from "@/utils/fetchWrapper";

export type Domain = {
  id?: string;
  name: string;
  description: string;
};

export const pabcService = {
  getAllDomains: (): Promise<Domain[]> => {
    return get<Domain[]>(`/api/v1/domains`);
  },
  getDomainById: (id: string): Promise<Domain> => {
    return get<Domain>(`/api/v1/domains/${id}`);
  },
  createDomain: (payload: Domain): Promise<Domain> => {
    return post<Domain>(`/api/v1/domains`, payload);
  },
  updateDomain: (payload: Domain): Promise<Domain> => {
    return put<Domain>(`/api/v1/domains/${payload.id}`, payload);
  },
  deleteDomain: (id: string): Promise<void> => {
    return del(`/api/v1/domains/${id}`);
  }
};
