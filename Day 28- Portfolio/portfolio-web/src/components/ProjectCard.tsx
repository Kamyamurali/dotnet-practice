import type { Project } from '../types'

export default function ProjectCard({ project }: { project: Project }) {
  return (
    <article className={project.featured ? 'project featured' : 'project'}>
      <div className="project-top">
        <h3>{project.title}</h3>
        <span className="period">{project.period}</span>
      </div>
      <p className="project-role">Role: {project.role}</p>
      <p>{project.description}</p>

      <details>
        <summary>Responsibilities ({project.responsibilities.length})</summary>
        <ul className="ticks">
          {project.responsibilities.map((r, i) => <li key={i}>{r}</li>)}
        </ul>
      </details>

      <ul className="tech-list" aria-label="Tools and technologies">
        {project.technologies.map(t => <li key={t}>{t}</li>)}
      </ul>
    </article>
  )
}
