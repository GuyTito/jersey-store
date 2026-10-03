import type { Metadata } from "next";
import Link from "next/link";
import { redirect } from "next/navigation";
import { AdminSignOut } from "@/components/AdminSignOut";
import { getAdminIdentity } from "@/lib/admin-session";

export const metadata: Metadata = {
  title: "Manage products",
  robots: { index: false, follow: false },
};

export default async function AdminLayout({
  children,
}: Readonly<{ children: React.ReactNode }>) {
  const admin = await getAdminIdentity();
  if (!admin) {
    redirect("/admin/login");
  }

  return (
    <div className="admin-page">
      <header className="admin-header">
        <Link href="/admin/products" className="store-brand">
          <span className="store-brand__mark" aria-hidden="true">J</span>
          <span className="store-brand__wordmark">JERSEY STORE <span>ADMIN</span></span>
        </Link>
        <nav className="admin-nav" aria-label="Admin navigation">
          <Link href="/admin/products">Products</Link>
          <Link href="/admin/settings">Settings</Link>
        </nav>
        <div className="admin-header__account">
          <span>{admin.email}</span>
          <AdminSignOut />
        </div>
      </header>
      {children}
    </div>
  );
}
