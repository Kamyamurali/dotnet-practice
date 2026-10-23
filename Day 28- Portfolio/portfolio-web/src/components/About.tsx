import type { Education } from '../types'

interface Props {
  paragraphs: string[]
  education: Education[]
}

export default function About({ paragraphs, education }: Props) {
  return (
    <section className="section" id="about">
      <div className="wrap two-col">
        <div>
          <h2 className="section-title"><span>01</span> About me</h2>
          <div className="prose">
            {paragraphs.map((p, i) => <p key={i}>{p}</p>)}
          </div>
        </div>

        <aside className="edu-card" aria-labelledby="edu-heading">
          <h3 id="edu-heading">Education &amp; Certification</h3>
          {education.map(e => (
            <div key={e.degree} className="edu-item">
              <p className="edu-degree">{e.degree}</p>
              <p className="muted">{e.institution} ({e.country})</p>
              <div className="edu-meta">
                <span>{e.graduated}</span>
                <span>GPA <strong>{e.gpa.toFixed(2)}</strong></span>
              </div>
            </div>
          ))}
          <div className="edu-item">
            <p className="edu-degree">.NET Full Stack AI Enabled Developer</p>
            <p className="muted">Revature · certification pending</p>
          </div>
        </aside>
      </div>
    </section>
  )
}
