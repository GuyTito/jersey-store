"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { FormEvent, useEffect, useState } from "react";

export default function AdminLoginPage() {
  const router = useRouter();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [busy, setBusy] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  useEffect(() => {
    let active = true;
    fetch("/api/admin/session", { cache: "no-store" })
      .then((response) => {
        if (active && response.ok) {
          router.replace("/admin/products");
          router.refresh();
        } else if (active && response.status !== 401) {
          setError("Could not check the current admin session. You can still try to sign in.");
        }
      })
      .catch(() => {
        if (active) {
          setError("Could not check the current admin session. You can still try to sign in.");
        }
      });
    return () => {
      active = false;
    };
  }, [router]);

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError("");
    setBusy(true);

    try {
      const response = await fetch("/api/admin/session", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
      });
      if (!response.ok) {
        const result = (await response.json()) as { error?: string };
        setError(result.error ?? "Unable to sign in.");
        return;
      }

      router.replace("/admin/products");
      router.refresh();
    } catch {
      setError("The sign-in service is unavailable. Try again.");
    } finally {
      setBusy(false);
    }
  }

  return (
    <main className="admin-login">
      <div className="admin-login__card">
        <Link href="/" className="store-brand">
          <span className="store-brand__mark" aria-hidden="true">J</span>
          <span className="store-brand__wordmark">JERSEY STORE</span>
        </Link>
        <span className="section-eyebrow">Administration</span>
        <h1>Sign in</h1>
        <p>Manage the catalog, stock, and product imagery.</p>
        <form className="admin-form" onSubmit={submit}>
          <label>
            Email
            <input
              type="email"
              autoComplete="username"
              required
              maxLength={256}
              value={email}
              onChange={(event) => setEmail(event.target.value)}
            />
          </label>
          <label>
            Password
            <span className="password-field">
              <input
                type={showPassword ? "text" : "password"}
                autoComplete="current-password"
                required
                maxLength={256}
                value={password}
                onChange={(event) => setPassword(event.target.value)}
              />
              <button
                type="button"
                className="password-field__toggle"
                aria-pressed={showPassword}
                onClick={() => setShowPassword((visible) => !visible)}
              >
                {showPassword ? "Hide" : "Show"}
              </button>
            </span>
          </label>
          {error ? <p className="admin-error" role="alert">{error}</p> : null}
          <button className="button button--lime" disabled={busy}>
            {busy ? "Signing in…" : "Sign in"}
          </button>
        </form>
        <Link href="/" className="admin-login__back">Return to storefront</Link>
      </div>
    </main>
  );
}
