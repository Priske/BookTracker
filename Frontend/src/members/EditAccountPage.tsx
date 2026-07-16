import { useState, type FormEvent } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Link, Navigate, useNavigate } from "react-router-dom";
import { ApiError } from "../api";
import { useCurrentMember } from "../auth/useCurrentMember";
import type { UpdateMemberRequest } from "./types";
import { updateMember } from "./membersApi";
import { DeleteMemberButton } from "./DeleteBookButton";

export function EditAccountPage() {
  const currentMemberQuery = useCurrentMember();
  const [formError, setFormError] = useState<string | null>(null);
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  const updateMutation = useMutation({
    mutationFn: (request: UpdateMemberRequest) => {
      const member = currentMemberQuery.data;

      if (!member) {
        throw new Error("Current member is unavailable.");
      }

      return updateMember(member.id, request);
    },

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["current-member"],
        exact: true,
      });

      navigate("/account", { replace: true });
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

    updateMutation.mutate({ name, email });
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

  if (!member) {
    return <p>Could not load the account.</p>;
  }

  const mutationStatus =
    updateMutation.error instanceof ApiError
      ? updateMutation.error.status
      : null;

  return (
    <main>
      <Link to="/account">Cancel</Link>

      <h1>Edit account</h1>

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
            type="email"
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
      <DeleteMemberButton memberId={member.id} />
      
      {formError && <p>{formError}</p>}
      {mutationStatus === 400 && <p>The API rejected the account data.</p>}
      {mutationStatus === 401 && <p>Your login is missing or expired.</p>}
      {mutationStatus === 403 && <p>You cannot edit this account.</p>}
      {mutationStatus === 404 && <p>This account no longer exists.</p>}
      {mutationStatus === 409 && <p>That email address is already in use.</p>}

      {updateMutation.isError && mutationStatus === null && (
        <p>Could not update the account.</p>
      )}
    </main>
  );
}