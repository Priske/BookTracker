import { useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";

import { useAuth } from "./AuthContext";

export function LogoutButton() {
  const { logout } = useAuth();
  const queryClient = useQueryClient();
  const navigate = useNavigate();

function handleLogout() {
    logout();

    queryClient.removeQueries({
      queryKey: ["current-member"],
    });

    navigate("/login");
  }

  return (
    <button type="button" onClick={handleLogout}>
      Log out
    </button>
  );
}
