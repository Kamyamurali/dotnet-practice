import { useState, type FormEvent } from 'react'
import { sendContact } from '../api'
import type { ContactForm, Profile } from '../types'

const empty: ContactForm = { name: '', email: '', message: '' }

export default function Contact({ profile }: { profile: Profile }) {
  const [form, setForm] = useState<ContactForm>(empty)
  const [sending, setSending] = useState(false)
  const [status, setStatus] = useState<{ ok: boolean; text: string } | null>(null)
  const [errors, setErrors] = useState<Record<string, string[]>>({})

  function update(field: keyof ContactForm, value: string) {
    setForm(prev => ({ ...prev, [field]: value }))
  }

  // ASP.NET Core returns error keys like "Email" or "$.email" — match either
  const errorFor = (field: string) =>
    Object.entries(errors).find(([k]) => k.toLowerCase().endsWith(field))?.[1]?.[0]

  async function onSubmit(e: FormEvent) {
    e.preventDefault()
    setSending(true)
    setStatus(null)
    try {
      const result = await sendContact(form)
      setErrors(result.fieldErrors)
      setStatus({ ok: result.ok, text: result.message })
      if (result.ok) setForm(empty)
    } catch {
      setStatus({ ok: false, text: 'Network error. Please try again.' })
    } finally {
      setSending(false)
    }
  }

  return (
    <section className="section alt" id="contact">
      <div className="wrap two-col">
        <div>
          <h2 className="section-title"><span>04</span> Contact</h2>
          <p className="prose">
            I'm excited to contribute to client projects. Send a message and it goes straight into
            this site's Azure SQL database.
          </p>
          <ul className="contact-links">
            {profile.email && <li><a href={`mailto:${profile.email}`}>{profile.email}</a></li>}
            {profile.linkedInUrl && <li><a href={profile.linkedInUrl} target="_blank" rel="noreferrer">LinkedIn ↗</a></li>}
            {profile.gitHubUrl && <li><a href={profile.gitHubUrl} target="_blank" rel="noreferrer">GitHub ↗</a></li>}
          </ul>
        </div>

        <form className="contact-form" onSubmit={onSubmit} noValidate>
          <label>
            Name
            <input value={form.name} onChange={e => update('name', e.target.value)} aria-invalid={!!errorFor('name')} />
            {errorFor('name') && <span className="field-error">{errorFor('name')}</span>}
          </label>
          <label>
            Email
            <input type="email" value={form.email} onChange={e => update('email', e.target.value)} aria-invalid={!!errorFor('email')} />
            {errorFor('email') && <span className="field-error">{errorFor('email')}</span>}
          </label>
          <label>
            Message
            <textarea rows={5} value={form.message} onChange={e => update('message', e.target.value)} aria-invalid={!!errorFor('message')} />
            {errorFor('message') && <span className="field-error">{errorFor('message')}</span>}
          </label>
          <button className="btn primary" type="submit" disabled={sending}>
            {sending ? 'Sending…' : 'Send message'}
          </button>
          {status && <p className={status.ok ? 'form-status ok' : 'form-status err'} role="status">{status.text}</p>}
        </form>
      </div>
    </section>
  )
}
