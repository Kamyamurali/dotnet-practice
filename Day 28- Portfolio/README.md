# Kamya Murali · Portfolio

Personal portfolio site built from my Revature portfolio, using the stack from the
.NET Full Stack AI Enabled Developer program, hosted on an **Azure Windows VM (IIS)**
with an **Azure SQL** database.

```
React + TypeScript (Vite)        portfolio-web/
        │  fetch /api/...
ASP.NET Core 10 Web API          Portfolio.Api/Controllers
  └ Middleware (logging, errors)  Portfolio.Api/Middleware
  └ Services  (LINQ, DTOs)        Portfolio.Api/Services
  └ Repository (DI, SOLID)        Portfolio.Api/Repositories
EF Core Code-First               Portfolio.Api/Data, Models
        │
Azure SQL Database               database/portfolio-queries.sql
```

## What each course topic looks like in this project

| Course topic | Where it is |
|---|---|
| C# / OOP (abstraction, inheritance, encapsulation) | `Models/Entities.cs` — `BaseEntity`, enums, properties |
| LINQ | `Services/PortfolioService.cs` — `SelectMany`, `GroupBy`, `Distinct`, `OrderBy` |
| DI / SOLID / Repository pattern | `IPortfolioRepository`, `IPortfolioService`, registrations in `Program.cs` |
| SQL Server + T-SQL | `database/portfolio-queries.sql` — JOIN, GROUP BY, OPENJSON, stored procedure, transaction |
| EF Core Code-First | `Data/PortfolioDbContext.cs` (Fluent API, one-to-many), `Data/SeedData.cs` |
| JSON serialization | Enum-as-string converter, JSON primitive collections |
| ASP.NET Core Web API | `Controllers/`, DTOs, data-annotation validation, status codes (200/201/400/404/500) |
| Middleware | `Middleware/Middleware.cs` — request logging + global exception handler |
| Swagger | `/swagger` |
| React + TypeScript | `portfolio-web/src` — components, `useState`/`useEffect`/`useMemo`, typed API |
| Docker | `Dockerfile` (multi-stage: Node → .NET SDK → runtime) |
| Azure (Day 27) | Resource group, Windows VM over RDP, Azure SQL — steps below |

---

## 1. Run it on your Mac

No database needed: with an empty connection string the API uses EF Core's in-memory database.

```bash
# frontend → builds into Portfolio.Api/wwwroot
cd portfolio-web
npm install
npm run build

# API (serves the site too)
cd ../Portfolio.Api
dotnet run
```

Open **http://localhost:5080** (site) and **http://localhost:5080/swagger** (API).

While editing React, run `npm run dev` in `portfolio-web` as well and use http://localhost:5173 —
it hot-reloads and forwards `/api` calls to the running .NET app.

**To use your local Docker SQL Server instead** (Day 7 setup), put this in
`Portfolio.Api/appsettings.Development.json`:

```json
{ "ConnectionStrings": { "PortfolioDb": "Server=localhost,1433;Database=PortfolioDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True" } }
```

### Before going live
Open `Portfolio.Api/Data/SeedData.cs` and fill in `Email` and `LinkedInUrl` (they're `null` now,
so the page hides them).

---

## 2. Host it on Azure (Windows VM + Azure SQL)

Same Portal flow as Day 27. Use one resource group so cleanup is one click.

### A. Resource group
Portal → **Resource groups** → **+ Create** → name `rg-portfolio`, region **East US**.

### B. Azure SQL (database first, so EF Core doesn't pick an expensive tier)
1. **SQL databases** → **+ Create**.
2. Resource group `rg-portfolio`. Database name **`PortfolioDb`**.
3. Server: **Create new** → e.g. `kamya-portfolio-sql`, East US, **Use SQL authentication**,
   admin login `sqladmin` + a password you write down.
4. If you see **"Apply offer"** for the free Azure SQL Database offer, apply it. Otherwise
   **Configure database** → **Basic**.
5. **Networking** tab: Connectivity method **Public endpoint**, then turn on
   **Allow Azure services and resources to access this server** (lets the VM in) and
   **Add current client IP address** (lets your Mac / VS Code in).
6. **Review + create** → **Create**.

Leave the database empty — the app creates the tables and fills them on first start.

### C. Windows VM
1. **Virtual machines** → **+ Create** → **Azure virtual machine**.
2. Resource group `rg-portfolio`, name `vm-portfolio`, region East US.
3. Image **Windows Server 2022 Datacenter: Azure Edition**, size **B2s** (B1s is too small for Windows).
4. Admin username + password (write them down).
5. **Inbound port rules → Allow selected ports → HTTP (80)** and **RDP (3389)**.
6. **Review + create** → **Create**.

If you forgot port 80: VM → **Networking / Network settings** → **Create port rule → Inbound**,
destination port `80`, protocol TCP, Allow.

### D. Publish the app on your Mac

```bash
cd portfolio-web && npm install && npm run build
cd ../Portfolio.Api
dotnet publish -c Release -o ../publish
```

Then zip the `publish` folder (right-click → Compress in Finder).

### E. Copy it to the VM
Connect with **Windows App** exactly like Day 27 (VM → Connect → Native RDP → Download RDP file).
To get the zip across, either:
- copy the zip in Finder and paste it on the VM desktop, or
- in Windows App: edit the PC → **Folders** → add your Mac folder that contains the zip;
  it shows up on the VM under *This PC → Redirected drives*.

### F. Set up IIS on the VM
Inside the VM, open **PowerShell as Administrator**:

```powershell
Install-WindowsFeature -Name Web-Server -IncludeManagementTools
```

Then in the VM's Edge browser, go to **dotnet.microsoft.com/download/dotnet/10.0**, and under
*ASP.NET Core Runtime → Windows* download and install the **Hosting Bundle**. Afterwards:

```powershell
net stop was /y
net start w3svc
```

### G. Put the site in IIS
1. Extract the zip to `C:\inetpub\portfolio` (so `Portfolio.Api.dll` and `web.config` sit directly inside it).
2. In that folder create **`appsettings.Production.json`** (keeps the password out of GitHub):

   ```json
   {
     "ConnectionStrings": {
       "PortfolioDb": "Server=tcp:kamya-portfolio-sql.database.windows.net,1433;Initial Catalog=PortfolioDb;User ID=sqladmin;Password=YOUR_PASSWORD;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
     }
   }
   ```
3. Open **IIS Manager** → *Sites* → **Default Web Site** → **Basic Settings…** →
   Physical path `C:\inetpub\portfolio` → OK.
4. *Application Pools* → **DefaultAppPool** → **Basic Settings…** →
   .NET CLR version **No Managed Code** → OK, then **Recycle**.

### H. Test it
From your Mac browser: **http://&lt;VM public IP&gt;/** and **http://&lt;VM public IP&gt;/swagger**.
Send yourself a message through the contact form, then in VS Code (mssql extension, same
settings as Day 27, database `PortfolioDb`) run `database/portfolio-queries.sql`.

### Troubleshooting
| You see | Cause / fix |
|---|---|
| Browser times out | Port 80 rule missing on the VM (step C), or VM is stopped |
| HTTP Error 500.19 | Hosting Bundle not installed, or IIS not restarted after installing it |
| HTTP Error 500.30 | App crashed on start. In `web.config` set `stdoutLogEnabled="true"`, create `C:\inetpub\portfolio\logs`, reload, read the log |
| Page says "Couldn't reach the API" | API errored — usually the connection string or the SQL firewall ("Allow Azure services" off) |
| IIS default page instead of the site | Physical path not changed in step G.3 |

### Keep the free credit
- Stop the VM (**Stop**, so it shows *Stopped (deallocated)*) when you're not demoing. The public IP
  may change after a restart unless you set it to **Static** (VM → Networking → public IP → Configuration).
- When you're done with it: delete **rg-portfolio** (and `NetworkWatcherRG` if it was auto-created).

---

## 3. Optional: Docker

```bash
docker build -t kamya-portfolio .
docker run -p 8080:8080 kamya-portfolio
```
