import { NextResponse } from "next/server";
import { cookies } from "next/headers";
import { ADMIN_COOKIE, isSameOriginRequest, requireApiUrl } from "@/lib/admin-session";

type LoginResponse = {
  accessToken?: string;
  expiresAt?: string;
};

export async function GET() {
  const token = (await cookies()).get(ADMIN_COOKIE)?.value;
  if (!token) {
    return NextResponse.json({ signedIn: false }, { status: 401 });
  }

  let upstream: Response;
  try {
    upstream = await fetch(`${requireApiUrl()}/api/admin/auth/me`, {
      headers: { Authorization: `Bearer ${token}` },
      cache: "no-store",
    });
  } catch (error) {
    console.error("Admin session check failed.", error);
    return NextResponse.json({ error: "The sign-in service is unavailable." }, { status: 502 });
  }

  if (upstream.status === 401 || upstream.status === 403) {
    const response = NextResponse.json({ signedIn: false }, { status: 401 });
    response.cookies.set(ADMIN_COOKIE, "", {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "lax",
      path: "/",
      expires: new Date(0),
      maxAge: 0,
    });
    return response;
  }
  if (!upstream.ok) {
    return NextResponse.json({ error: "Unable to validate the admin session." }, { status: upstream.status });
  }

  const identity = (await upstream.json()) as { email: string };
  return NextResponse.json({ signedIn: true, email: identity.email });
}

export async function POST(request: Request) {
  if (!isSameOriginRequest(request)) {
    return NextResponse.json({ error: "Forbidden origin." }, { status: 403 });
  }

  let credentials: unknown;
  try {
    credentials = await request.json();
  } catch {
    return NextResponse.json({ error: "Invalid login request." }, { status: 400 });
  }

  let upstream: Response;
  try {
    upstream = await fetch(`${requireApiUrl()}/api/admin/auth/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(credentials),
      cache: "no-store",
    });
  } catch (error) {
    console.error("Admin login API request failed.", error);
    return NextResponse.json({ error: "The sign-in service is unavailable." }, { status: 502 });
  }

  if (upstream.status === 401) {
    return NextResponse.json({ error: "Invalid email or password." }, { status: 401 });
  }
  if (!upstream.ok) {
    return NextResponse.json({ error: "Unable to sign in." }, { status: upstream.status });
  }

  const login = (await upstream.json()) as LoginResponse;
  const expiresAt = login.expiresAt ? Date.parse(login.expiresAt) : Number.NaN;
  if (!login.accessToken || !Number.isFinite(expiresAt) || expiresAt <= Date.now()) {
    return NextResponse.json({ error: "The sign-in service returned an invalid session." }, { status: 502 });
  }

  const response = NextResponse.json({ signedIn: true });
  response.cookies.set(ADMIN_COOKIE, login.accessToken, {
    httpOnly: true,
    secure: process.env.NODE_ENV === "production",
    sameSite: "lax",
    path: "/",
    expires: new Date(expiresAt),
    maxAge: Math.min(1800, Math.floor((expiresAt - Date.now()) / 1000)),
  });
  return response;
}

export async function DELETE(request: Request) {
  if (!isSameOriginRequest(request)) {
    return NextResponse.json({ error: "Forbidden origin." }, { status: 403 });
  }

  const response = NextResponse.json({ signedOut: true });
  response.cookies.set(ADMIN_COOKIE, "", {
    httpOnly: true,
    secure: process.env.NODE_ENV === "production",
    sameSite: "lax",
    path: "/",
    expires: new Date(0),
    maxAge: 0,
  });
  return response;
}
