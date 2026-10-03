import { NextResponse, type NextRequest } from "next/server";

const ADMIN_COOKIE = "jersey_admin_token";

export function proxy(request: NextRequest) {
  const { pathname } = request.nextUrl;
  const hasSession = request.cookies.has(ADMIN_COOKIE);

  if (pathname === "/admin/login") {
    return NextResponse.next();
  }

  if (!hasSession) {
    return NextResponse.redirect(new URL("/admin/login", request.url));
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/admin", "/admin/:path*"],
};
