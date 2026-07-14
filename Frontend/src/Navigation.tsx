import { Link } from "react-router-dom";
import { LogoutButton } from "./auth/LogoutButton";
import { useAuth } from "./auth/AuthContext";
import { useCurrentMember } from "./auth/useCurrentMember";

export function Navigation() {
  const { accessToken } = useAuth();
  const currentMemberQuery = useCurrentMember();

  const hasToken = accessToken !== null;

  return (
    <nav>
      <Link to="/">Home</Link>{" "}
      <Link to="/books">Books</Link>{" "}

      {!hasToken && (
        <>
          <Link to="/register">Register</Link>{" "}
          <Link to="/login">Log in</Link>
        </>
      )}

      {hasToken && currentMemberQuery.isPending && (
        <span>Checking account...</span>
      )}

      {hasToken && currentMemberQuery.isSuccess && (
        <>
          <Link to="/account">Account</Link>{" "}
          <LogoutButton />
        </>
      )}

      {hasToken && currentMemberQuery.isError && <LogoutButton />}
    </nav>
  );
}