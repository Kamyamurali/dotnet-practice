using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Models;

namespace Portfolio.Api.Data;

// Fills an empty database with the content of the Revature portfolio.
// Runs once at startup: if Profiles already has a row, nothing happens.
public static class SeedData
{
    public static async Task InitializeAsync(PortfolioDbContext db, ILogger logger)
    {
        // Creates the tables if they don't exist yet (Code-First, no migrations folder needed).
        // On Azure SQL: create the EMPTY database in the Portal first (free/Basic tier),
        // otherwise EF would create it on the default, more expensive tier.
        await db.Database.EnsureCreatedAsync();

        if (await db.Profiles.AnyAsync())
        {
            logger.LogInformation("Database already seeded.");
            return;
        }

        db.Profiles.Add(new Profile
        {
            FullName = "Kamya Murali",
            Title = ".NET Full Stack Developer",
            Tagline = "Full-stack .NET developer with a business analyst's eye for what users actually need.",
            CertificationNote = "This portfolio is pending certification from Revature.",
            GitHubUrl = "https://github.com/Kamyamurali",
            Email = null,        // add your public email here (or in appsettings) before going live
            LinkedInUrl = null,  // add your LinkedIn URL here
            AboutParagraphs =
            [
                "I'm currently training with Revature as a .NET Full Stack AI Enabled Developer. Through this program, I'm building a strong foundation in full-stack development with the .NET ecosystem. I'm learning how to design, build, and connect every layer of an application, from the database to the user. I'm also putting that learning into practice through a hands-on capstone project.",
                "Before this, I worked in mobile app development, where I learned to build products with the user in mind. I also have experience in business analysis, working closely with both stakeholders and developers. That taught me how to understand requirements, communicate clearly, and make sure software meets real needs.",
                "I've also been involved in quality assurance and testing, which gave me an eye for detail and reliability. Along the way, I've used AI tools to learn faster and work more efficiently. What I bring is a blend of technical, analytical, and communication skills. I enjoy understanding the bigger picture of a product, not just my part of it.",
                "I'm adaptable, quick to learn, and comfortable working across teams. I'm excited to bring these strengths and contribute to client projects. My goal is to grow into a well-rounded developer who delivers real value."
            ]
        });

        db.Educations.Add(new Education
        {
            Degree = "Bachelor's Degree - Computer Engineering",
            Institution = "SRM Institute of Science and Technology",
            Country = "India",
            GraduationDate = new DateOnly(2026, 5, 1),
            Gpa = 8.75m
        });

        db.SkillCategories.AddRange(
            Category("Programming", 1, "C# with Strong OOP", ".NET Core 10", "SOLID Principles", "Node.js", "Git & GitHub", "Python"),
            Category("Databases", 2, "SQL Server", "Entity Framework Core", "SQL Joins, Stored Procedures & Transactions"),
            Category("Web Technologies", 3, "React", "REST API Development", "HTML 5", "CSS 3", "JavaScript", "TypeScript"),
            Category("AI", 4, "AI Pipelines", "Prompt Engineering", "Generative AI Tools (Claude, ChatGPT)", "AI Code Generation"),
            Category("Agile & Business Analysis", 5, "Agile, Scrum & Sprint Planning", "Jira, Trello & Confluence", "UAT & Test Script Execution", "Business Analysis & Requirements Gathering", "AI Product & Feature Management"),
            Category("Mobile Development", 6, "SwiftUI", "Swift UIKit", "Firebase"),
            Category("DevOps", 7, "Docker", "Kubernetes", "Ansible", "CI/CD Workflows")
        );

        db.Experiences.AddRange(
            new Experience
            {
                Company = "Revature", Role = ".NET Full Stack Developer", Type = ExperienceType.Primary, SortOrder = 1,
                StartDate = new DateOnly(2026, 8, 1), EndDate = new DateOnly(2026, 11, 1),
                Projects =
                [
                    new Project
                    {
                        Title = "Banking & Loan Management WebAPI", Role = "Full-Stack Developer", Featured = true,
                        StartDate = new DateOnly(2026, 9, 1), EndDate = new DateOnly(2026, 10, 1),
                        Description = "A full-stack banking platform with a React frontend, a Node.js middleware layer, and a RESTful ASP.NET Core Web API backend, supporting two roles, admins and customers, with full CRUD operations. Customers can register as members, view available loan and financing products, apply online, track application and payment status, view statements, and see their complete transaction history since registration. Admins can add new products, approve or reject applications, and update disbursement and payment status. Requests flow from React through the Node.js middleware to the Web API, all data is stored in SQL Server, and the API is documented and tested through Swagger.",
                        Responsibilities =
                        [
                            "Analyzed project requirements and researched the client's lending workflow to plan the application's features and structure",
                            "Designed a RESTful ASP.NET Core Web API with CRUD endpoints for customers, loan products, applications, and payments",
                            "Built a React frontend with a customer dashboard and an admin portal",
                            "Built a Node.js middleware layer between the React app and the Web API for request routing and logging",
                            "Designed the SQL Server database schema, including tables, keys, and relationships",
                            "Implemented data access using ADO.NET to connect the API to SQL Server",
                            "Built features for customer registration, product listing, loan applications, status tracking, statements, and transaction history",
                            "Built admin features to add products, approve or reject applications, and update disbursement and payment status",
                            "Implemented input validation, error handling, and appropriate HTTP status codes",
                            "Documented and tested all endpoints using Swagger",
                            "Managed source code and version control using Git and GitHub"
                        ],
                        Technologies = ["React", "Node.js", "ASP.NET Core Web API", "C#", "ADO.NET", "SQL Server", "REST", "Swagger", "Git", "GitHub"]
                    },
                    new Project
                    {
                        Title = "Banking Application", Role = "Full-Stack Developer", Featured = true,
                        StartDate = new DateOnly(2026, 9, 1), EndDate = new DateOnly(2026, 9, 1),
                        Description = "A C# console banking application with two roles, Customer and Admin, backed by SQL Server through Entity Framework Core Code-First. Customers can register or log in, check account details, deposit, withdraw, and transfer funds, view their last 5 transactions, request a cheque book, change their password, and open additional accounts. Admins can create, delete, and edit accounts, view a bank summary, reset customer passwords, and approve cheque book requests. All data persists in SQL Server across sessions, passwords are stored as salted hashes, and all input is validated.",
                        Responsibilities =
                        [
                            "Analyzed project requirements and planned the application's features and structure",
                            "Organized the application into layers: Models, Data access, Security, and the console interface",
                            "Applied object-oriented principles such as encapsulation, inheritance, and abstraction",
                            "Designed the SQL Server database schema, including tables, keys, and relationships",
                            "Implemented data access using Entity Framework Core Code-First with migrations and LINQ",
                            "Secured logins with salted password hashing and strong-password rules",
                            "Built an interactive console interface with user login and menu-driven banking operations",
                            "Implemented input validation and exception handling to keep the application reliable",
                            "Tested and debugged features throughout development to ensure correct behavior",
                            "Managed source code and version control using Git and GitHub"
                        ],
                        Technologies = [".NET 10", "C#", "Entity Framework Core", "SQL Server 2022", "Docker", "LINQ", "Git", "GitHub"]
                    }
                ]
            },
            new Experience
            {
                Company = "ParticleBlack", Role = "Business Analyst Intern", Type = ExperienceType.Other, SortOrder = 2,
                StartDate = new DateOnly(2026, 3, 1), EndDate = new DateOnly(2026, 7, 1),
                Projects =
                [
                    new Project
                    {
                        Title = "Healthcare & Workforce Platforms – Business Analysis and UAT", Role = "Business Analyst Intern",
                        StartDate = new DateOnly(2026, 3, 1), EndDate = new DateOnly(2026, 7, 1),
                        Description = "Supported two client platforms as a Business Analyst: a clinical workforce planning platform in its post-go-live phase and a workforce development platform. Work covered requirements analysis, Jira ticket writing, UAT test planning and execution, product documentation, demo scripts, and competitive analysis, working closely with project managers, reviewers, and the development team in an Agile environment.",
                        Responsibilities =
                        [
                            "Gathered and analyzed business requirements, translating them into clear documentation for project delivery",
                            "Wrote and refined Jira user stories and bug tickets with traceability to the business requirements document",
                            "Created and executed UAT test scripts to validate that delivered features met acceptance criteria",
                            "Mapped business workflows and created process flow diagrams",
                            "Built product documentation, including an executive brief and technical questionnaires for the development team",
                            "Prepared demo scripts and a competitive landscape analysis for a workforce development platform",
                            "Coordinated with stakeholders and technical teams to clarify requirements and keep delivery on track",
                            "Tracked tasks and documentation across Jira, Trello, and Confluence"
                        ],
                        Technologies = ["Jira", "Confluence", "Trello", "Microsoft Excel", "Microsoft Word", "draw.io", "Agile / Scrum"]
                    }
                ]
            },
            new Experience
            {
                Company = "Prodapt", Role = "Generative AI Intern", Type = ExperienceType.Other, SortOrder = 3,
                StartDate = new DateOnly(2025, 12, 1), EndDate = new DateOnly(2026, 3, 1),
                Projects =
                [
                    new Project
                    {
                        Title = "Generative AI Solutions – LLM and RAG Applications", Role = "Generative AI Intern",
                        StartDate = new DateOnly(2025, 12, 1), EndDate = new DateOnly(2026, 3, 1),
                        Description = "Worked on Generative AI initiatives involving large language models, retrieval-augmented generation (RAG), and agentic AI systems. Evaluated LLM use cases, supported the data workflows behind AI solutions, and helped develop, test, and troubleshoot AI-driven applications, documenting results and recommendations.",
                        Responsibilities =
                        [
                            "Analyzed requirements for Generative AI initiatives and evaluated use cases for LLM-based applications",
                            "Worked with RAG-based models, large language models, and agentic AI systems",
                            "Supported data processing workflows underpinning AI model development",
                            "Assisted with developing, testing, and troubleshooting AI-driven solutions",
                            "Applied prompt engineering techniques to improve AI output quality",
                            "Documented findings and recommended improvements"
                        ],
                        Technologies = ["Python", "Large Language Models", "RAG", "Agentic AI", "Prompt Engineering", "Generative AI Tools"]
                    }
                ]
            },
            new Experience
            {
                Company = "Infosys", Role = "iOS App Development Intern", Type = ExperienceType.Other, SortOrder = 4,
                StartDate = new DateOnly(2025, 4, 1), EndDate = new DateOnly(2025, 5, 1),
                Projects =
                [
                    new Project
                    {
                        Title = "iOS Application Development", Role = "iOS App Development Intern",
                        StartDate = new DateOnly(2025, 4, 1), EndDate = new DateOnly(2025, 5, 1),
                        Description = "Developed iOS application features using SwiftUI and Xcode, with a focus on UI development and app functionality. Integrated Apple Maps for location-based features and Supabase as the backend for data storage. Worked in an Agile, sprint-based environment that included sprint planning, testing, debugging, and team collaboration, with tasks tracked in Jira.",
                        Responsibilities =
                        [
                            "Developed iOS application features using SwiftUI",
                            "Integrated Apple Maps (MapKit) for location and map-based features",
                            "Connected the app to Supabase for backend services and data storage",
                            "Translated functional requirements into working UI components",
                            "Participated in sprint planning, testing, and debugging within an Agile team",
                            "Tracked tasks and sprint progress using Jira"
                        ],
                        Technologies = ["SwiftUI", "Xcode", "Apple Maps (MapKit)", "Supabase", "Jira", "Agile / Scrum"]
                    }
                ]
            },
            new Experience
            {
                Company = "Apple - iOS Development Centre", Role = "iOS Student Developer Program", Type = ExperienceType.Other, SortOrder = 5,
                StartDate = new DateOnly(2024, 7, 1), EndDate = new DateOnly(2025, 5, 1),
                Projects =
                [
                    new Project
                    {
                        Title = "iOS App Development Projects", Role = "iOS Student Developer",
                        StartDate = new DateOnly(2024, 7, 1), EndDate = new DateOnly(2025, 5, 1),
                        Description = "Took part in the Apple iOS Student Developer Program, working in a student team to build two App Store-ready applications. Features included Firebase for backend services and data storage, a Core ML recommendation model for personalized content, Razorpay for in-app payments, the Google Books API for book summaries, and a Gemini-powered AI chatbot. The work covered requirements, UI design, feature development, API integration, debugging, and testing within Apple's development ecosystem.",
                        Responsibilities =
                        [
                            "Built two App Store-ready iOS applications as part of a student development team",
                            "Coordinated requirements, design, and development across the team",
                            "Developed user interfaces and app features using Swift, SwiftUI, and UIKit",
                            "Integrated Firebase for authentication, backend services, and data storage",
                            "Built a recommendation model using Core ML to personalize content for users",
                            "Integrated the Razorpay API for in-app payments",
                            "Used the Google Books API to fetch and display book summaries",
                            "Built an AI chatbot using the Gemini API",
                            "Debugged and tested applications to ensure reliable performance"
                        ],
                        Technologies = ["Swift", "SwiftUI", "UIKit", "Xcode", "Firebase", "Core ML", "Razorpay API", "Google Books API", "Gemini API"]
                    }
                ]
            },
            new Experience
            {
                Company = "LTIMindtree", Role = "Database / SQL Intern", Type = ExperienceType.Other, SortOrder = 6,
                StartDate = new DateOnly(2024, 5, 1), EndDate = new DateOnly(2024, 6, 1),
                Projects =
                [
                    new Project
                    {
                        Title = "Database Systems & SQL Analysis", Role = "Database / SQL Intern",
                        StartDate = new DateOnly(2024, 5, 1), EndDate = new DateOnly(2024, 6, 1),
                        Description = "Worked with SQL and database technologies, writing queries, managing data, and performing database-related tasks. Gained practical experience with DBMS concepts, data analysis, troubleshooting, and database tools, and used Excel and Word for reporting and documentation.",
                        Responsibilities =
                        [
                            "Wrote SQL queries to retrieve, manage, and analyze business data",
                            "Applied DBMS concepts, including tables, keys, and relationships",
                            "Performed data analysis and troubleshooting on database tasks",
                            "Used database tools to manage and maintain data",
                            "Prepared reports and documentation using Microsoft Excel and Word"
                        ],
                        Technologies = ["SQL", "Database Management Systems", "Microsoft Excel", "Microsoft Word"]
                    }
                ]
            }
        );

        await db.SaveChangesAsync();
        logger.LogInformation("Seeded portfolio data.");
    }

    private static SkillCategory Category(string name, int order, params string[] skills) => new()
    {
        Name = name,
        SortOrder = order,
        Skills = skills.Select((s, i) => new Skill { Name = s, SortOrder = i + 1 }).ToList()
    };
}
