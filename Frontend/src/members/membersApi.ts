import { apiRequest, apiRequestWithoutResponse } from "../api";
import type { PagedResult } from "../types";
import type {
  GetMembersRequest,
  MemberDetails,
  MemberSummary,
  RegisterMemberRequest,
  RegisterMemberResponse,
  UpdateMemberRequest,
} from "./types";

export function registerMember(request: RegisterMemberRequest) {
  return apiRequest<RegisterMemberResponse>("/members", {
    method: "POST",
    body: JSON.stringify(request),
  });
}

export function getMember(memberId: number) {
  return apiRequest<MemberDetails>(`/members/${memberId}`);
}

export function updateMember(id: number, request: UpdateMemberRequest) {
  return apiRequestWithoutResponse(`/members/${id}`, {
    method: "PUT",
    body: JSON.stringify(request),
  });
}
export function updateCurrentMember(request: UpdateMemberRequest) {
  return apiRequest<void>("/auth/me", {
    method: "PUT",
    body: JSON.stringify(request),
  });
}

export function getMembers(request: GetMembersRequest) {
  const parameters = new URLSearchParams({
    page: request.page.toString(),
    pageSize: request.pageSize.toString(),
  });

  if (request.search) {
    parameters.set("search", request.search);
  }

  return apiRequest<PagedResult<MemberSummary>>(
    `/members?${parameters.toString()}`,
  );

}

  