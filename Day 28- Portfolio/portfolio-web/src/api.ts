import type { ContactForm, Portfolio } from './types'

// All calls are relative (/api/...): same origin in production, Vite proxy in development.

export async function getPortfolio(): Promise<Portfolio> {
  const res = await fetch('/api/portfolio')
  if (!res.ok) throw new Error(`API returned ${res.status}`)
  return res.json()
}

export interface ContactResult {
  ok: boolean
  message: string
  fieldErrors: Record<string, string[]>
}

export async function sendContact(form: ContactForm): Promise<ContactResult> {
  const res = await fetch('/api/contact', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(form)
  })

  const body = await res.json().catch(() => ({}))

  if (res.status === 201) {
    return { ok: true, message: body.message ?? 'Message sent.', fieldErrors: {} }
  }
  if (res.status === 400 && body.errors) {
    // ASP.NET Core validation ProblemDetails: { errors: { Email: ["..."] } }
    return { ok: false, message: 'Please fix the highlighted fields.', fieldErrors: body.errors }
  }
  return { ok: false, message: 'Something went wrong. Please try again later.', fieldErrors: {} }
}
