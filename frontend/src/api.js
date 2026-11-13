// All calls to the backend go through this file, so the address is written in one place.
// The path is relative (/api/...): the browser sends it to the same address the page came from,
// and nginx (Docker) or Vite (npm run dev) forwards it to the backend.
export async function getHealth() {
  const response = await fetch('/api/health')

  if (!response.ok) {
    throw new Error(`The backend answered with status ${response.status}`)
  }

  return response.json() // { status: "ok", database: true }
}
