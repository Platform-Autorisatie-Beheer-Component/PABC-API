export type User = {
  isLoggedIn: boolean;
  email: string;
  name: string;
  roles: string[];
  hasFunctioneelBeheerderAccess: boolean;
};
