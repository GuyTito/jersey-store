"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";

export function AdminSignOut() {
  const router = useRouter();
  const [error, setError] = useState("");

  async function signOut() {
    setError("");
    try {
      const response = await fetch("/api/admin/session", { method: "DELETE" });
      if (!response.ok) {
        setError("Could not sign out. Please try again.");
        return;
      }
      router.replace("/admin/login");
      router.refresh();
    } catch {
      setError("Could not sign out. Please try again.");
    }
  }

  return (
    <>
      <button className="admin-signout" onClick={signOut}>Sign out</button>
      {error ? <span className="admin-error" role="alert">{error}</span> : null}
    </>
  );
}
