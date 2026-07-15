import { useQuery } from "@tanstack/react-query";
import { Link, useParams, useLocation } from "react-router-dom";
import { ApiError } from "../api";
import { getMember } from "../members/membersApi";
import { EditMemberLink } from "../members/EditMemberLink";
import { useCurrentMember } from "../auth/useCurrentMember";

type AccountLocationState = {
  fromMemberList?: boolean;
};

function readMemberId(value: string | undefined) {
  if (value === undefined) {
    return null;
  }

  const memberId = Number(value);

  return Number.isInteger(memberId) && memberId > 0
    ? memberId
    : null;
}


export function AccountPage() {
  const location = useLocation();
  const { memberId: memberIdParameter } = useParams();

  const currentMemberQuery = useCurrentMember();
  const fromMemberList = (location.state as AccountLocationState | null)?.fromMemberList === true;

  const hasMemberParameter = memberIdParameter !== undefined;
  const memberId = readMemberId(memberIdParameter);


  const selectedMemberQuery = useQuery({
    queryKey: ["members", "detail", memberId],
    queryFn: () => {
      if (memberId === null) {
        throw new Error("Invalid member id");
      }

      return getMember(memberId);
    },
    enabled: hasMemberParameter && memberId !== null,
    retry: false,
  });

  if (hasMemberParameter && memberId === null) {
    return (
      <main>
        <h1>Invalid member id</h1>
        <Link to="/account">Back to account</Link>
      </main>
    );
  }

  if (
    currentMemberQuery.isPending ||
    (hasMemberParameter && selectedMemberQuery.isPending)
  ) {
    return <p>Loading account...</p>;
  }

  if (currentMemberQuery.isError) {
    return <p>Could not load your account.</p>;
  }


  const selectedMemberNotFound =
    selectedMemberQuery.error instanceof ApiError &&
    selectedMemberQuery.error.status === 404;

  if (hasMemberParameter && selectedMemberNotFound) {
    return (
      <main>
        <h1>Member not found</h1>
        <Link to="/members">Back to members</Link>
      </main>
    );
  }

  if (hasMemberParameter && selectedMemberQuery.isError) {
    return <p>Could not load the member.</p>;
  }

  const member = hasMemberParameter
    ? selectedMemberQuery.data
    : currentMemberQuery.data;

  if (!member) {
     return <p>Could not load the member.</p>;
  }
  return (
    <main>
      {fromMemberList && (
        <Link to="/members">Return to members</Link>
      )}
      <h1>{member.name}</h1>
      <p>{member.email}</p>
      <p>Role: {member.role}</p>

      <EditMemberLink memberId={member.id} fromMemberList={fromMemberList} />
    </main>
  );
}