# Simple Content Management System Planning

## Overview
A lightweight, modular, and Dockerized Content Management System (CMS) built on .NET Core 8 with SQLite, Entity Framework Core, and ASP.NET Core Identity. The system is designed for full portability, media support, dynamic layout theming, insertable content modules, and an extensible token engine.

---

## 🧱 Core Platform Stack
- **Framework:** .NET Core 8
- **Database:** SQLite (portable, file-based)
- **ORM:** Entity Framework Core
- **Authentication:** ASP.NET Core Identity (role-based)
- **Deployment:** Dockerized, portable (media + DB = deployable package)
- **Frontend:** Bootstrap + TinyMCE + Responsive FileManager

---

## 📄 Content Management
- Rich page editor with TinyMCE
- Supports HTML content and insertable tokens/modules
- Page visibility options: Public / Members Only
- Pages use slug-based routing for clean URLs
- Page templates handled via the theming system

---

## 🧬 Token System
Tokens dynamically inject CMS values into theme files and structured content.

### Designer Tokens
Used in themes/layouts only:
- `<cms:Content />`
- `<cms:PageTitle />`
- `<cms:CreatedDate />`
- `<cms:LastUpdated />`
- `<cms:UpdatedBy />`
- `<cms:Username />`
- `<cms:UserEmail />`
- `<cms:UserRole />`
- `<cms:IsAuthenticated />`
- `<cms:SiteTitle />`
- `<cms:ThemeName />`
- `<cms:CurrentUrl />`
- `<cms:Year />`
- `<cms:Menu orientation="horizontal" />`
- `<cms:SearchBar />`
- `<cms:SocialLinks />`
- `<cms:Breadcrumbs />`
- `<cms:Privacy />`
- `<cms:License />`

### Insertable Module Tokens (via TinyMCE)
- `<cms:Image src="id,id,..." mode="random|sequence" />`
- `<cms:Audio src="id" />`
- `<cms:Video src="id" />`
- `<cms:Slideshow images="id,id,..." layout="full|poster" transition="fade|slide" delay="3000" />`
- `<cms:Gallery />`
- `<cms:Blog />`
- `<cms:BlogContent />`
- `<cms:BlogCitation />`
- `<cms:TagCloud />`
- `<cms:Store />`
- `<cms:Store:OnSaleItems />`
- `<cms:Store:Category name="X" />`

---

## 🎨 Theming System (Summary)
Themes in SCMS live under `/Themes/{ThemeName}` and control the layout and presentation of the site. Each theme includes:

- `layout.cshtml`: Root layout structure
- `partials/`: Shared view components like `header.cshtml`, `footer.cshtml`
- `templates/`: Content-specific templates (e.g., `page.cshtml`, `blog.cshtml`)
- `theme.config.json`: Metadata and default template reference
- Optional `css/` and `js/` folders for theme-specific styling

Themes can be uploaded as ZIPs and are extracted safely, enabling visual customization without touching backend code.

For full details, see the [Theming.md](./Theming.md) document.

---

## 📷 Media Management
- Media types: images, video, audio, documents (e.g., zip/pdf)
- Stored in `wwwroot/media/`
- Managed through Responsive FileManager
- Metadata saved in `MediaFile` table (type, size, duration, dimensions, etc.)
- Gallery module reads from a configured media folder
- File picker integrated with TinyMCE for uploads/links

---

## 🧭 Navigation & Menus
- Menu structure stored in `NavigationLink` table
- Initial implementation renders entire menu
- Future support for menu groups (Main, Footer, Sidebar, etc.)
- Drag-and-drop admin menu organizer planned
- Breadcrumbs token (`<cms:Breadcrumbs />`) tracks location

---

## 📝 Blog System
- Supports blog posts with title, content, tags, categories
- Inline numbered citations (e.g., [1]) stored per post
- Citation list rendered with `<cms:BlogCitation />`
- Blog rendered via `<cms:Blog />`, `<cms:BlogContent />`
- Filterable tag cloud with `<cms:TagCloud />`

---

## 🛍️ Future Store Module
- Insertable store tokens like:
  - `<cms:Store />`
  - `<cms:Store:OnSaleItems />`
  - `<cms:Store:Category name="X" />`
- Future support for shopping cart, product pages

---

## 🔎 Search
- Full-text search using SQLite FTS5
- `<cms:SearchBar />` token injects search UI
- Search results use relevance scoring (bm25)

---

## ⚙️ Admin Tools
- Page manager
- Media manager
- Blog manager
- Navigation/menu editor (drag-and-drop)
- Social media settings
- General system settings

---

## 🔒 Security & Access
- Role-based access control via Identity
- Pages can be marked Public or Members Only
- Editor/admin privileges scoped to roles

---

## 🧰 Portable & Local Dev
- Fully Dockerized
- Runs locally with Docker Compose
- Deploy by copying SQLite DB and `/media/` folder
- Suitable for hobbyists and small orgs

---

This document serves as the foundation for CMS development and will evolve as features are implemented.
