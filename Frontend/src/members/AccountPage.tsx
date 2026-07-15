import { Link, Navigate } from "react-router-dom";
import { ApiError } from "../api";
import { useAccessToken } from "../auth/tokenStorage";
import { useCurrentMember } from "../auth/useCurrentMember";

export function AccountPage() {
  const accessToken = useAccessToken();
  const currentMemberQuery = useCurrentMember();

  if (accessToken === null) {
    return <Navigate to="/login" replace />;
  }

  if (currentMemberQuery.isPending) {
    return <p>Loading account...</p>;
  }

  const unauthorized =
    currentMemberQuery.error instanceof ApiError &&
    currentMemberQuery.error.status === 401;

  if (unauthorized) {
    return <Navigate to="/login" replace />;
  }

  if (currentMemberQuery.isError) {
    return <p>Could not load the account.</p>;
  }

  const member = currentMemberQuery.data;

  return (
    <main>
      <h1>{member.name}</h1>
      <p>{member.email}</p>
      <p>Role: {member.role}</p>

      <Link to="/account/edit">Edit account</Link>
    </main>
  );
}