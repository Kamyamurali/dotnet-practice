import { useMemo, useState } from 'react'
import type { SkillCategory } from '../types'

export default function Skills({ categories }: { categories: SkillCategory[] }) {
  const [search, setSearch] = useState('')

  // useMemo: only re-filter when the search text or data changes
  const visible = useMemo(() => {
    const term = search.trim().toLowerCase()
    if (!term) return categories
    return categories
      .map(c => ({ ...c, skills: c.skills.filter(s => s.toLowerCase().includes(term)) }))
      .filter(c => c.skills.length > 0)
  }, [categories, search])

  return (
    <section className="section alt" id="skills">
      <div className="wrap">
        <div className="section-head">
          <h2 className="section-title"><span>02</span> Skill matrix</h2>
          <label className="search">
            <span className="sr-only">Filter skills</span>
            <input
              type="search"
              placeholder="Filter skills, e.g. SQL"
              value={search}
              onChange={e => setSearch(e.target.value)}
            />
          </label>
        </div>

        {visible.length === 0 ? (
          <p className="muted">No skills match “{search}”.</p>
        ) : (
          <div className="skill-grid">
            {visible.map(c => (
              <article key={c.name} className="skill-card">
                <header>
                  <h3>{c.name}</h3>
                  <span className="count">{c.skills.length}</span>
                </header>
                <ul>
                  {c.skills.map(s => <li key={s}>{s}</li>)}
                </ul>
              </article>
            ))}
          </div>
        )}
      </div>
    </section>
  )
}
