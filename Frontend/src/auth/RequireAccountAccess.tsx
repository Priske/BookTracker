import { Navigate, Outlet, useParams } from "react-router-dom";
import { useCurrentMember } from "./useCurrentMember";

export function RequireAccountAccess() {
  const { memberId } = useParams();
  const currentMemberQuery = useCurrentMember();

  if (currentMemberQuery.isPending) {
    return <p>Checking permissions...</p>;
  }

  if (currentMemberQuery.isError) {
    return <Navigate to="/login" replace />;
  }

  const currentMember = currentMemberQuery.data;

  // /account has no memberId, so any authenticated user may view it.
  if (memberId === undefined) {
    return <Outlet />;
  }

  const requestedMemberId = Number(memberId);

  if (!Number.isInteger(requestedMemberId) || requestedMemberId <= 0) {
    return (
      <main>
        <h1>Invalid member id</h1>
      </main>
    );
  }

  const isAdministrator =
    currentMember.role === "Administrator";

  const isOwnAccount =
    currentMember.id === requestedMemberId;

  if (!isAdministrator && !isOwnAccount) {
    return (
      <main>
        <h1>Forbidden</h1>
        <p>You cannot view this account.</p>
      </main>
    );
  }

  return <Outlet />;
}