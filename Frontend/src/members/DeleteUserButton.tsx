import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { ApiError } from "../api";
import { useCurrentMember } from "../auth/useCurrentMember";
import { removeAccessToken } from "../auth/tokenStorage";
import { deleteMember } from "./membersApi";

type DeleteMemberButtonProps = {
  memberId: number;
};

export function DeleteMemberButton({
  memberId,
}: DeleteMemberButtonProps) {
  const [confirming, setConfirming] = useState(false);
  const currentMemberQuery = useCurrentMember();
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  const deletingCurrentMember =
    currentMemberQuery.data?.id === memberId;

  function clearDeletedMemberQueries() {
    queryClient.invalidateQueries({
      queryKey: ["members"],
      refetchType: "none",
    });

    queryClient.removeQueries({
      queryKey: ["members", "detail", memberId],
      exact: true,
    });
  }

  function leaveDeletedMember() {
    clearDeletedMemberQueries();

    if (deletingCurrentMember) {
      removeAccessToken();

      queryClient.removeQueries({
        queryKey: ["current-member"],
        exact: true,
      });

      navigate("/login", {
        replace: true,
        state: {
          accountDeleted: true,
        },
      });

      return;
    }

    navigate("/members", { replace: true });
  }

  const deleteMutation = useMutation({
    mutationFn: () => deleteMember(memberId),
    onSuccess: leaveDeletedMember,
  });

const canDelete =
  currentMemberQuery.isSuccess &&
  (
    currentMemberQuery.data.role === "Administrator" ||
    currentMemberQuery.data.id === memberId
  );

if (!canDelete) {
  return null;
}


  if (!confirming) {
    return (
      <button
        type="button"
        onClick={() => setConfirming(true)}
      >
        Delete member
      </button>
    );
  }

  const mutationStatus =
    deleteMutation.error instanceof ApiError
      ? deleteMutation.error.status
      : null;

  return (
    <section aria-labelledby="delete-member-heading">
      <h2 id="delete-member-heading">
        {deletingCurrentMember
          ? "Delete your account?"
          : "Delete this member?"}
      </h2>

      <p>This action cannot be undone.</p>

      <button
        type="button"
        onClick={() => deleteMutation.mutate()}
        disabled={deleteMutation.isPending}
      >
        {deleteMutation.isPending
          ? "Deleting..."
          : deletingCurrentMember
            ? "Yes, delete my account"
            : "Yes, delete member"}
      </button>{" "}

      <button
        type="button"
        onClick={() => {
          deleteMutation.reset();
          setConfirming(false);
        }}
        disabled={deleteMutation.isPending}
      >
        Cancel
      </button>

      {mutationStatus === 401 && (
        <p>Your login is missing or expired.</p>
      )}

      {mutationStatus === 403 && (
        <p>You are not allowed to delete this member.</p>
      )}

      {mutationStatus === 404 && (
        <div>
          <p>
            This member no longer exists. It may already have been deleted.
          </p>

          <button
            type="button"
            onClick={leaveDeletedMember}
          >
            {deletingCurrentMember
              ? "Return to login"
              : "Back to members"}
          </button>
        </div>
      )}

      {deleteMutation.isError &&
        mutationStatus !== 401 &&
        mutationStatus !== 403 &&
        mutationStatus !== 404 && (
          <p>Could not delete the member.</p>
        )}
    </section>
  );
}