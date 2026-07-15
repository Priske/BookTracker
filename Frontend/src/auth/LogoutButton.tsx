import { useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { removeAccessToken } from "./tokenStorage";

export function LogoutButton() {
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  function handleLogout() {
    removeAccessToken();

    queryClient.removeQueries({
      queryKey: ["current-member"],
    });

    navigate("/login", { replace: true });
  }

  return (
    <button type="button" onClick={handleLogout}>
      Log out
    </button>
  );
}