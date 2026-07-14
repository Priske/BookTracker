import {createContext,useContext,useState, type ReactNode,} from "react";
import {  getAccessToken, setAccessToken as saveAccessToken,  removeAccessToken,} from "./tokenStorage";

type AuthContextValue = {
  accessToken: string | null;
  loginWithToken: (token: string) => void;
  logout: () => void;
};

const AuthContext = createContext<AuthContextValue | null>(null);

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

export function useAuth() {
  const context = useContext(AuthContext);

  if (context === null) {
    throw new Error("useAuth must be used inside AuthProvider");
  }

  return context;
}
