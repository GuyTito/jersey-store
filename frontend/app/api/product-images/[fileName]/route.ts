import { NextResponse } from "next/server";
import { requireApiUrl } from "@/lib/admin-session";

const imageNamePattern = /^[a-f0-9]{32}\.(?:png|jpg|webp)$/;

export async function GET(
  _request: Request,
  context: { params: Promise<{ fileName: string }> },
) {
  const { fileName } = await context.params;
  if (!imageNamePattern.test(fileName)) {
    return NextResponse.json({ error: "Image not found." }, { status: 404 });
  }

  let upstream: Response;
  try {
    upstream = await fetch(`${requireApiUrl()}/uploads/${fileName}`, { cache: "no-store" });
  } catch (error) {
    console.error("Product image request failed.", error);
    return NextResponse.json({ error: "Image service unavailable." }, { status: 502 });
  }

  if (!upstream.ok || !upstream.headers.get("content-type")?.startsWith("image/")) {
    return NextResponse.json({ error: "Image not found." }, { status: upstream.ok ? 502 : upstream.status });
  }

  return new Response(upstream.body, {
    headers: {
      "Content-Type": upstream.headers.get("content-type")!,
      "Cache-Control": "public, max-age=3600, stale-while-revalidate=86400",
      "X-Content-Type-Options": "nosniff",
    },
  });
}
