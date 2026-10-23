const links = [
  { href: '#about', label: 'About' },
  { href: '#skills', label: 'Skills' },
  { href: '#experience', label: 'Experience' },
  { href: '#contact', label: 'Contact' }
]

export default function Header({ name }: { name: string }) {
  const initials = name.split(' ').map(part => part[0]).join('')

  return (
    <header className="site-header">
      <div className="wrap header-row">
        <a href="#top" className="logo" aria-label="Back to top">{initials}</a>
        <nav aria-label="Sections">
          {links.map(link => (
            <a key={link.href} href={link.href}>{link.label}</a>
          ))}
        </nav>
      </div>
    </header>
  )
}
