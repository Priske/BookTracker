import { useState, type FormEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Link, useNavigate, useParams } from "react-router-dom";
import { ApiError } from "../api";
import type { UpdateMemberRequest } from "./types";
import { getMember, updateMember } from "./membersApi";

function readMemberId(value: string | undefined) {
  const memberId = Number(value);
  return Number.isInteger(memberId) && memberId > 0 ? memberId : null;
}

export function EditMemberPage() {
  const { memberId: memberIdParameter } = useParams();
  const memberId = readMemberId(memberIdParameter);

  const [formError, setFormError] = useState<string | null>(null);

  const queryClient = useQueryClient();
  const navigate = useNavigate();

  const memberQuery = useQuery({
    queryKey: ["members", "detail", memberId],
    queryFn: () => {
      if (memberId === null) {
        throw new Error("Invalid member id");
      }

      return getMember(memberId);
    },
    enabled: memberId !== null,
    retry: false,
  });

  const updateMutation = useMutation({
    mutationFn: (request: UpdateMemberRequest) => {
      if (memberId === null) {
        throw new Error("Invalid member id");
      }

      return updateMember(memberId, request);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["members"],
      });

      navigate(`/members/${memberId}`);
    },
  });

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError(null);

    const formData = new FormData(event.currentTarget);
    const name = formData.get("name")?.toString().trim() ?? "";
    const email = formData.get("email")?.toString().trim() ?? "";

    if (!name || !email) {
      setFormError("Enter a valid name and email.");
      return;
    }

    updateMutation.mutate({
      name,
      email,
    });
  }

  if (memberId === null) {
    return (
      <main>
        <h1>Invalid member id</h1>
        <Link to="/members">Back to members</Link>
      </main>
    );
  }

  if (memberQuery.isPending) {
    return <p>Loading member...</p>;
  }

  const queryNotFound =
    memberQuery.error instanceof ApiError &&
    memberQuery.error.status === 404;

  if (queryNotFound) {
    return (
      <main>
        <h1>Member not found</h1>
        <Link to="/members">Back to members</Link>
      </main>
    );
  }

  if (memberQuery.isError) {
    return <p>Could not load the member.</p>;
  }

  const member = memberQuery.data;

  const mutationStatus =
    updateMutation.error instanceof ApiError
      ? updateMutation.error.status
      : null;

  return (
    <main>
      <Link to={`/members/${memberId}`}>Cancel</Link>

      <h1>Edit {member.name}</h1>

      <form key={member.id} onSubmit={handleSubmit}>
        <label>
          Name
          <input
            name="name"
            defaultValue={member.name}
            maxLength={100}
            required
          />
        </label>

        <label>
          Email
          <input
            name="email"
            defaultValue={member.email}
            maxLength={100}
            required
          />
        </label>

        <button type="submit" disabled={updateMutation.isPending}>
          {updateMutation.isPending ? "Saving..." : "Save changes"}
        </button>
      </form>

      {formError && <p>{formError}</p>}
      {mutationStatus === 400 && <p>The API rejected the member data.</p>}
      {mutationStatus === 401 && <p>Your login is missing or expired.</p>}
      {mutationStatus === 403 && <p>Only administrators can edit this member.</p>}
      {mutationStatus === 404 && <p>This member no longer exists.</p>}
      {updateMutation.isError && mutationStatus === null && (
        <p>Could not update the member.</p>
      )}
    </main>
  );
}