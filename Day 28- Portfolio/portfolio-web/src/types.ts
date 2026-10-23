// TypeScript interfaces that mirror the C# DTOs in Portfolio.Api/DTOs/Dtos.cs

export interface Profile {
  fullName: string
  title: string
  tagline: string
  certificationNote: string
  about: string[]
  email: string | null
  gitHubUrl: string | null
  linkedInUrl: string | null
}

export interface Education {
  degree: string
  institution: string
  country: string
  graduated: string
  gpa: number
}

export interface Project {
  id: number
  title: string
  role: string
  description: string
  period: string
  featured: boolean
  responsibilities: string[]
  technologies: string[]
}

export type ExperienceType = 'Primary' | 'Other'

export interface Experience {
  id: number
  company: string
  role: string
  type: ExperienceType
  period: string
  projects: Project[]
}

export interface SkillCategory {
  name: string
  skills: string[]
}

export interface Stats {
  companies: number
  projects: number
  technologies: number
  skills: number
}

export interface Portfolio {
  profile: Profile
  education: Education[]
  experiences: Experience[]
  skillCategories: SkillCategory[]
  stats: Stats
}

export interface ContactForm {
  name: string
  email: string
  message: string
}
