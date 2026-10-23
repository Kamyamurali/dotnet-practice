import { useEffect, useState } from 'react'
import { getPortfolio } from './api'
import type { Portfolio } from './types'
import Header from './components/Header'
import Hero from './components/Hero'
import About from './components/About'
import Skills from './components/Skills'
import ExperienceSection from './components/ExperienceSection'
import Contact from './components/Contact'
import BuiltWith from './components/BuiltWith'

type LoadState =
  | { status: 'loading' }
  | { status: 'error'; message: string }
  | { status: 'ready'; data: Portfolio }

export default function App() {
  const [state, setState] = useState<LoadState>({ status: 'loading' })

  useEffect(() => {
    let cancelled = false
    getPortfolio()
      .then(data => { if (!cancelled) setState({ status: 'ready', data }) })
      .catch((err: Error) => { if (!cancelled) setState({ status: 'error', message: err.message }) })
    return () => { cancelled = true }   // cleanup if the component unmounts
  }, [])

  if (state.status === 'loading') {
    return <div className="center-msg"><span className="spinner" aria-hidden /> Loading portfolio…</div>
  }

  if (state.status === 'error') {
    return (
      <div className="center-msg">
        <p><strong>Couldn't reach the API.</strong></p>
        <p className="muted">{state.message}. Is Portfolio.Api running?</p>
      </div>
    )
  }

  const { profile, education, experiences, skillCategories, stats } = state.data

  return (
    <>
      <Header name={profile.fullName} />
      <main>
        <Hero profile={profile} stats={stats} />
        <About paragraphs={profile.about} education={education} />
        <Skills categories={skillCategories} />
        <ExperienceSection experiences={experiences} />
        <Contact profile={profile} />
        <BuiltWith />
      </main>
      <footer className="site-footer">
        <div className="wrap">
          © {new Date().getFullYear()} {profile.fullName} · {profile.certificationNote}
        </div>
      </footer>
    </>
  )
}
