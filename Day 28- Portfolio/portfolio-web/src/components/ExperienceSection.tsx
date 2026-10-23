import { useMemo, useState } from 'react'
import type { Experience, ExperienceType } from '../types'
import ProjectCard from './ProjectCard'

const tabs: { type: ExperienceType; label: string }[] = [
  { type: 'Primary', label: 'Work experience' },
  { type: 'Other', label: 'Other experience' }
]

export default function ExperienceSection({ experiences }: { experiences: Experience[] }) {
  const [tab, setTab] = useState<ExperienceType>('Primary')
  const [tech, setTech] = useState<string | null>(null)

  const inTab = useMemo(() => experiences.filter(e => e.type === tab), [experiences, tab])

  // Every technology used in this tab, most common first
  const technologies = useMemo(() => {
    const counts = new Map<string, number>()
    inTab.flatMap(e => e.projects).flatMap(p => p.technologies)
      .forEach(t => counts.set(t, (counts.get(t) ?? 0) + 1))
    return [...counts.entries()].sort((a, b) => b[1] - a[1] || a[0].localeCompare(b[0])).map(([t]) => t)
  }, [inTab])

  const shown = inTab
    .map(e => ({ ...e, projects: tech ? e.projects.filter(p => p.technologies.includes(tech)) : e.projects }))
    .filter(e => e.projects.length > 0)

  function switchTab(next: ExperienceType) {
    setTab(next)
    setTech(null)
  }

  return (
    <section className="section" id="experience">
      <div className="wrap">
        <h2 className="section-title"><span>03</span> Experience &amp; projects</h2>

        <div className="tabs" role="tablist">
          {tabs.map(t => (
            <button
              key={t.type}
              role="tab"
              aria-selected={tab === t.type}
              className={tab === t.type ? 'tab active' : 'tab'}
              onClick={() => switchTab(t.type)}
            >
              {t.label}
            </button>
          ))}
        </div>

        <div className="tech-filter" aria-label="Filter by technology">
          <button className={tech === null ? 'chip active' : 'chip'} onClick={() => setTech(null)}>All</button>
          {technologies.map(t => (
            <button key={t} className={tech === t ? 'chip active' : 'chip'} onClick={() => setTech(t === tech ? null : t)}>
              {t}
            </button>
          ))}
        </div>

        <ol className="timeline">
          {shown.map(e => (
            <li key={e.id} className="timeline-item">
              <div className="timeline-meta">
                <p className="company">{e.company}</p>
                <p className="muted">{e.role}</p>
                <p className="period">{e.period}</p>
              </div>
              <div className="timeline-projects">
                {e.projects.map(p => <ProjectCard key={p.id} project={p} />)}
              </div>
            </li>
          ))}
        </ol>
      </div>
    </section>
  )
}
