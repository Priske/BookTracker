import {createContext} from "react";

type AuthContextValue = {
  accessToken: string | null;
  loginWithToken: (token: string) => void;
  logout: () => void;
};

export const AuthContext = createContext<AuthContextValue | null>(null);


