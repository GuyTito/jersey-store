"use client";

import { FormEvent, useEffect, useState } from "react";

async function readError(response: Response, fallback: string) {
  const text = await response.text();
  try {
    const body = JSON.parse(text) as { error?: string; detail?: string; title?: string };
    return body.error ?? body.detail ?? body.title ?? fallback;
  } catch {
    return text || fallback;
  }
}

export function AdminSettingsForm() {
  const [whatsAppNumber, setWhatsAppNumber] = useState("");
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState("");
  const [notice, setNotice] = useState("");

  useEffect(() => {
    let active = true;
    fetch("/api/admin/proxy/settings", { cache: "no-store" })
      .then(async (response) => {
        if (!response.ok) throw new Error(await readError(response, "Could not load settings."));
        const data = (await response.json()) as { whatsAppNumber: string };
        if (active) setWhatsAppNumber(data.whatsAppNumber);
      })
      .catch((loadError: unknown) => {
        if (active) setError(loadError instanceof Error ? loadError.message : "Could not load settings.");
      })
      .finally(() => {
        if (active) setLoading(false);
      });
    return () => {
      active = false;
    };
  }, []);

  async function save(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setBusy(true);
    setError("");
    setNotice("");
    try {
      const response = await fetch("/api/admin/proxy/settings", {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ whatsAppNumber }),
      });
      if (!response.ok) throw new Error(await readError(response, "Could not save settings."));
      const data = (await response.json()) as { whatsAppNumber: string };
      setWhatsAppNumber(data.whatsAppNumber);
      setNotice("WhatsApp number saved. It is now used for orders and the support button.");
    } catch (saveError) {
      setError(saveError instanceof Error ? saveError.message : "Could not save settings.");
    } finally {
      setBusy(false);
    }
  }

  return (
    <section className="admin-panel admin-settings">
      <form className="admin-form" onSubmit={save}>
        <label>
          WhatsApp number
          <input
            type="tel"
            inputMode="tel"
            required
            maxLength={40}
            placeholder="+233501234567"
            disabled={loading}
            value={whatsAppNumber}
            onChange={(event) => setWhatsAppNumber(event.target.value)}
          />
        </label>
        <p className="admin-help">
          Use international format with the country code. Customers are sent here when they order a
          jersey or tap the floating support button.
        </p>
        {error ? <p className="admin-error" role="alert">{error}</p> : null}
        {notice ? <p className="admin-notice" role="status">{notice}</p> : null}
        <button className="button button--lime" disabled={busy || loading}>
          {busy ? "Saving…" : "Save settings"}
        </button>
      </form>
    </section>
  );
}
