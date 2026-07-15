import { Link } from "react-router-dom";
import { useCurrentMember } from "../auth/useCurrentMember";

type EditMemberLinkProps = {
  memberId: number;
  fromMemberList?: boolean;
};

export function EditMemberLink({ memberId, fromMemberList = false }: EditMemberLinkProps) {
  const currentMemberQuery = useCurrentMember();
  const member = currentMemberQuery.data;

  if (!currentMemberQuery.isSuccess || !member) return null;

  const isAuthorized = member.role === "Administrator" || (member.role === "Member" && member.id === memberId);
  if (!isAuthorized) return null;

  return <Link to={`/members/${memberId}/edit`} state={{ fromMemberList }}>Edit Member</Link>;
}