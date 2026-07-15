import { useState, type ReactNode,} from "react";
import {  getAccessToken, setAccessToken as saveAccessToken,  removeAccessToken,} from "./tokenStorage";
import { AuthContext } from "./AuthContext";

export function AuthProvider({ children }: { children: ReactNode }) {
  const [accessToken, setAccessTokenState] = useState<string | null>(() =>
    getAccessToken(),
  );

  function loginWithToken(token: string) {
    saveAccessToken(token);
    setAccessTokenState(token);
  }

  function logout() {
    removeAccessToken();
    setAccessTokenState(null);
  }

  return (
    <AuthContext.Provider
      value={{
        accessToken,
        loginWithToken,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}