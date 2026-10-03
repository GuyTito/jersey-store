import { cookies } from "next/headers";
import { NextResponse } from "next/server";
import {
  ADMIN_COOKIE,
  isSameOriginRequest,
  requireApiUrl,
} from "@/lib/admin-session";

type RouteContext = { params: Promise<{ path: string[] }> };
type ProxyMethod = "GET" | "POST" | "PUT" | "PATCH" | "DELETE";

const idPattern = /^[1-9]\d*$/;

function isSupportedRoute(path: string[], method: ProxyMethod) {
  if (path.length === 2 && path[0] === "auth" && path[1] === "me") {
    return method === "GET";
  }

  if (path.length === 1 && path[0] === "settings") {
    return method === "GET" || method === "PUT";
  }

  if (path.length === 1 && path[0] === "products") {
    return method === "GET" || method === "POST";
  }

  if (path.length === 2 && path[0] === "products" && idPattern.test(path[1])) {
    return method === "GET" || method === "PUT" || method === "DELETE";
  }

  if (
    path.length === 3 &&
    path[0] === "products" &&
    idPattern.test(path[1]) &&
    path[2] === "images"
  ) {
    return method === "POST";
  }

  if (
    path.length === 4 &&
    path[0] === "products" &&
    idPattern.test(path[1]) &&
    path[2] === "images" &&
    idPattern.test(path[3])
  ) {
    return method === "DELETE";
  }

  if (
    path.length === 5 &&
    path[0] === "products" &&
    idPattern.test(path[1]) &&
    path[2] === "images" &&
    idPattern.test(path[3]) &&
    path[4] === "primary"
  ) {
    return method === "PUT";
  }

  return (
    path.length === 5 &&
    path[0] === "products" &&
    idPattern.test(path[1]) &&
    path[2] === "variants" &&
    idPattern.test(path[3]) &&
    path[4] === "stock" &&
    method === "PATCH"
  );
}

async function proxy(request: Request, context: RouteContext, method: ProxyMethod) {
  if (method !== "GET" && !isSameOriginRequest(request)) {
    return NextResponse.json({ error: "Forbidden origin." }, { status: 403 });
  }

  const { path } = await context.params;
  if (!isSupportedRoute(path, method)) {
    return NextResponse.json({ error: "Unsupported admin API route." }, { status: 404 });
  }

  const token = (await cookies()).get(ADMIN_COOKIE)?.value;
  if (!token) {
    return NextResponse.json({ error: "Authentication required." }, { status: 401 });
  }

  const target = `${requireApiUrl()}/api/admin/${path.map(encodeURIComponent).join("/")}${new URL(request.url).search}`;
  const headers = new Headers({ Authorization: `Bearer ${token}` });
  const contentType = request.headers.get("content-type");
  if (contentType) {
    headers.set("Content-Type", contentType);
  }

  let upstream: Response;
  try {
    upstream = await fetch(target, {
      method,
      headers,
      body: method === "GET" || method === "DELETE" ? undefined : await request.arrayBuffer(),
      cache: "no-store",
      redirect: "manual",
    });
  } catch (error) {
    console.error("Admin API proxy request failed.", error);
    return NextResponse.json({ error: "The administration service is unavailable." }, { status: 502 });
  }

  const responseHeaders = new Headers({ "Cache-Control": "no-store" });
  const responseContentType = upstream.headers.get("content-type");
  if (responseContentType) {
    responseHeaders.set("Content-Type", responseContentType);
  }
  const response = new NextResponse(upstream.body, {
    status: upstream.status,
    headers: responseHeaders,
  });
  if (upstream.status === 401) {
    response.cookies.set(ADMIN_COOKIE, "", {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "lax",
      path: "/",
      expires: new Date(0),
      maxAge: 0,
    });
  }
  return response;
}

export const GET = (request: Request, context: RouteContext) => proxy(request, context, "GET");
export const POST = (request: Request, context: RouteContext) => proxy(request, context, "POST");
export const PUT = (request: Request, context: RouteContext) => proxy(request, context, "PUT");
export const PATCH = (request: Request, context: RouteContext) => proxy(request, context, "PATCH");
export const DELETE = (request: Request, context: RouteContext) => proxy(request, context, "DELETE");
