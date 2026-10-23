// "How this site is built" — the architecture, layer by layer
const layers = [
  { name: 'React + TypeScript', detail: 'Components, hooks, typed API models (Vite build)' },
  { name: 'ASP.NET Core 10 Web API', detail: 'REST controllers, DTOs, validation, middleware, Swagger at /swagger' },
  { name: 'Services & Repository', detail: 'Dependency injection, SOLID, LINQ' },
  { name: 'Entity Framework Core', detail: 'Code-First models, relationships, seeding' },
  { name: 'Azure SQL Database', detail: 'Portfolio content and contact messages' }
]

export default function BuiltWith() {
  return (
    <section className="section" id="built">
      <div className="wrap">
        <h2 className="section-title"><span>05</span> How this site is built</h2>
        <ol className="stack">
          {layers.map(l => (
            <li key={l.name}>
              <strong>{l.name}</strong>
              <span>{l.detail}</span>
            </li>
          ))}
        </ol>
        <p className="muted host-note">Hosted on an Azure Windows Virtual Machine running IIS.</p>
      </div>
    </section>
  )
}
