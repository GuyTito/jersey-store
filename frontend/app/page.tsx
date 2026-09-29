"use client";

import { useEffect, useState } from "react";

type HealthResponse = {
  status: string;
  message: string;
  timestamp: string;
};

type HealthState =
  | { status: "loading" }
  | { status: "success"; data: HealthResponse }
  | { status: "error"; message: string };

export default function Home() {
  const [healthState, setHealthState] = useState<HealthState>({
    status: "loading",
  });

  useEffect(() => {
    const controller = new AbortController();

    async function loadHealth() {
      try {
        const apiUrl = process.env.NEXT_PUBLIC_API_URL;

        if (!apiUrl) {
          throw new Error("NEXT_PUBLIC_API_URL is not configured.");
        }

        const response = await fetch(`${apiUrl}/api/health`, {
          signal: controller.signal,
        });

        if (!response.ok) {
          throw new Error(`API responded with HTTP ${response.status}.`);
        }

        const data = (await response.json()) as HealthResponse;
        setHealthState({ status: "success", data });
      } catch (error) {
        if (controller.signal.aborted) {
          return;
        }

        setHealthState({
          status: "error",
          message: error instanceof Error ? error.message : "Request failed.",
        });
      }
    }

    void loadHealth();

    return () => controller.abort();
  }, []);

  return (
    <main className="mx-auto max-w-2xl p-8">
      <h1 className="text-3xl font-bold">Jersey Store</h1>
      <p className="mt-2 text-gray-600">Backend connection check</p>

      <section className="mt-8 rounded border p-6" aria-live="polite">
        {healthState.status === "loading" && <p>Contacting the API...</p>}

        {healthState.status === "success" && (
          <div>
            <p className="font-semibold">{healthState.data.message}</p>
            <p className="mt-2">Status: {healthState.data.status}</p>
            <p className="mt-1 text-sm text-gray-600">
              Timestamp: {new Date(healthState.data.timestamp).toLocaleString()}
            </p>
          </div>
        )}

        {healthState.status === "error" && (
          <p role="alert" className="text-red-700">
            Could not reach the API: {healthState.message}
          </p>
        )}
      </section>
    </main>
  );
}
