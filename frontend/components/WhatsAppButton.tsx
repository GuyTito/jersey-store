import { getStorefront } from "@/lib/api";

export async function WhatsAppButton() {
  let number: string | null = null;
  try {
    number = (await getStorefront())?.whatsAppNumber ?? null;
  } catch {
    return null;
  }

  const digits = number?.replace(/[^\d]/g, "");
  if (!digits) return null;

  const message = encodeURIComponent("Hi, I have a question about Jersey Store.");
  return (
    <a
      className="whatsapp-fab"
      href={`https://wa.me/${digits}?text=${message}`}
      target="_blank"
      rel="noopener noreferrer"
      aria-label="Chat with us on WhatsApp"
    >
      <svg viewBox="0 0 32 32" width="28" height="28" aria-hidden="true" fill="currentColor">
        <path d="M16.04 3C9.4 3 4 8.38 4 15.01c0 2.12.55 4.19 1.6 6.01L4 29l8.17-1.57a12.03 12.03 0 0 0 3.87.63C22.68 28.06 28 22.68 28 16.05 28 9.4 22.68 3 16.04 3Zm0 22.03c-1.2 0-2.38-.32-3.4-.93l-.5-.29-4.85.93.97-4.72-.32-.51a9.9 9.9 0 0 1-1.52-5.27c0-5.48 4.47-9.94 9.97-9.94 5.5 0 9.97 4.46 9.97 9.94 0 5.5-4.47 9.79-10.32 10.79Zm5.47-7.43c-.3-.15-1.77-.87-2.04-.97-.27-.1-.47-.15-.67.15-.2.3-.77.97-.94 1.17-.17.2-.35.22-.65.07-.3-.15-1.26-.46-2.4-1.48-.89-.79-1.48-1.76-1.66-2.06-.17-.3-.02-.46.13-.6.13-.13.3-.35.45-.52.15-.17.2-.3.3-.5.1-.2.05-.37-.02-.52-.07-.15-.67-1.61-.92-2.2-.24-.58-.49-.5-.67-.51h-.57c-.2 0-.52.07-.8.37-.27.3-1.04 1.02-1.04 2.48 0 1.46 1.07 2.88 1.22 3.08.15.2 2.1 3.2 5.08 4.49.71.31 1.27.49 1.7.63.71.23 1.36.2 1.88.12.57-.09 1.77-.72 2.02-1.42.25-.7.25-1.3.17-1.42-.07-.12-.27-.2-.57-.35Z" />
      </svg>
    </a>
  );
}
