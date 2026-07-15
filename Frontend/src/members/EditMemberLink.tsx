import { Link } from "react-router-dom";
import { useCurrentMember } from "../auth/useCurrentMember";

type EditMemberLinkProps = {
  memberId: number;
};

export function EditMemberLink({ memberId }: EditMemberLinkProps) {
  const currentMemberQuery = useCurrentMember();

  if (!currentMemberQuery.isSuccess) {
    return null;
  }

  if (currentMemberQuery.data.role !== "Administrator") {
    return null;
  }

  return <Link to={`/members/${memberId}/edit`}>Edit Member</Link>;
}