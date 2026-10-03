import type { Metadata } from "next";
import { AdminSettingsForm } from "@/components/AdminSettingsForm";

export const metadata: Metadata = { title: "Store settings" };

export default function AdminSettingsPage() {
  return (
    <main className="admin-main">
      <div className="admin-page-title">
        <div>
          <span className="section-eyebrow">Store configuration</span>
          <h1>Settings</h1>
          <p>Control how customers reach you.</p>
        </div>
      </div>
      <AdminSettingsForm />
    </main>
  );
}
