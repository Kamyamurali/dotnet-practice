import type { Profile, Stats } from '../types'

interface Props {
  profile: Profile
  stats: Stats
}

export default function Hero({ profile, stats }: Props) {
  const statItems = [
    { value: stats.companies, label: 'organisations' },
    { value: stats.projects, label: 'projects' },
    { value: stats.technologies, label: 'technologies used' },
    { value: stats.skills, label: 'skills' }
  ]

  return (
    <section className="hero" id="top">
      <div className="wrap">
        <p className="badge"><span className="dot" aria-hidden /> Revature · .NET Full Stack AI Enabled Developer</p>
        <h1>{profile.fullName}</h1>
        <p className="hero-title">{profile.title}</p>
        <p className="hero-tagline">{profile.tagline}</p>

        <div className="hero-actions">
          <a className="btn primary" href="#experience">See my projects</a>
          <a className="btn" href="#contact">Get in touch</a>
          {profile.gitHubUrl && (
            <a className="btn ghost" href={profile.gitHubUrl} target="_blank" rel="noreferrer">GitHub ↗</a>
          )}
        </div>

        <dl className="stats">
          {statItems.map(s => (
            <div key={s.label}>
              <dt>{s.label}</dt>
              <dd>{s.value}</dd>
            </div>
          ))}
        </dl>
      </div>
    </section>
  )
}
