import "server-only";
import { cookies } from "next/headers";

export const ADMIN_COOKIE = "jersey_admin_token";

export type AdminIdentity = { email: string };

function getApiUrl() {
  const apiUrl = process.env.API_URL;
  if (!apiUrl) {
    throw new Error("API_URL is not configured.");
  }

  return apiUrl;
}

export function isSameOriginRequest(request: Request) {
  const origin = request.headers.get("origin");
  return origin !== null && origin === new URL(request.url).origin;
}

export function requireApiUrl() {
  return getApiUrl();
}

export async function getAdminIdentity(): Promise<AdminIdentity | null> {
  const token = (await cookies()).get(ADMIN_COOKIE)?.value;
  if (!token) {
    return null;
  }

  const response = await fetch(`${getApiUrl()}/api/admin/auth/me`, {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  });

  if (response.status === 401 || response.status === 403) {
    return null;
  }

  if (!response.ok) {
    throw new Error(`Admin session check failed with HTTP ${response.status}.`);
  }

  return (await response.json()) as AdminIdentity;
}
