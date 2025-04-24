
# Simple Content Management System Planning

## Overview
A lightweight, modular, and Dockerized Content Management System (CMS) built on .NET Core 8 with SQLite, Entity Framework Core, and ASP.NET Core Identity. The system is designed for full portability, media support, dynamic layout theming, insertable content modules, and an extensible token engine.

_Last Updated: 2025-04-24 14:23:45_

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

### Designer Tokens
Used in themes/layouts only:
### Designer Tokens (Used in themes/layouts only)

- `<scms:SiteLogo />`
- `<scms:SiteTitle />`
- `<scms:ThemeName />`
- `<scms:PageTitle />`
- `<scms:CreatedDate />`
- `<scms:LastUpdated />`
- `<scms:UpdatedBy />`
- `<scms:Username />`
- `<scms:UserEmail />`
- `<scms:UserRole />`
- `<scms:IsAuthenticated />`
- `<scms:CurrentUrl />`
- `<scms:Year />`
- `<scms:Menu orientation="horizontal|vertical" />`
- `<scms:Content />`
- `<scms:Content name="..." />`
- `<scms:Breadcrumb />`
- `<scms:LoginStatus />`
- `<scms:TagCloud />`
- `<scms:Search />`
- `<scms:SocialLinks />`
- `<scms:Privacy />`
- `<scms:License />`


### Module Tokens
Inserted via layout or templates for dynamic features:
- `<scms:BlogContent />`
- `<scms:BlogCitation />`
- `<scms:Slideshow layout="poster" />`
- `<scms:Store:OnSaleItems />`
- `<scms:Survey id="..." />`
- `<scms:Comments pageId="..." />`
- `<scms:Captcha />`
- `<scms:ExternalLoginButtons />`
- `<scms:Donate />`
- `<scms:Map lat="..." lon="..." zoom="..." />`

### Inline Tokens (via TinyMCE)
- `<scms:Image src="..." mode="random|sequence" width="..." height="..." />`
- `<scms:Audio src="..." autoplay="..." controls="..." />`
- `<scms:Video src="..." autoplay="..." controls="..." width="..." height="..." />`
- `<scms:Download file="..." label="..." />`

---

## 🎨 Theming System
- Themes live under `/Themes/{ThemeName}`
- Responsive design is required
- Theme controls full vs fixed width layout
- `theme.config.json` contains metadata (description, preview, responsive flag)
- CDN/local toggle for TinyMCE via admin setting
- Admin theming support is deferred

---

## 📷 Media Management
- Media types: image, video, audio, document
- Metadata: filename, type, size, width/height, duration, tags
- Media grouped via `MediaGroup` entity (with image preview)
- Full media manager with file picker for TinyMCE
- File uploads stored in `/wwwroot/media/`

---

## 🧭 Navigation & Menus
- Structured menus from database
- Drag-and-drop menu manager
- Menu tokens for theme integration
- Multiple groups (Main, Footer, etc.) planned

---

## 📝 Blog System
- Blog posts with title, content, categories, tags, and citations
- Citation rendering: `<scms:BlogCitation />`
- Display: `<scms:BlogContent />`
- Tag cloud: `<scms:TagCloud />`

---

## 🔍 Search
- Full-text search (SQLite FTS5)
- Token: `<scms:Search />`
- Results scored for relevance

---

## 🛒 Store Module (Planned)
- Sell digital or physical items
- Tokens for embedding featured items or categories

---

## 🧩 Modules Overview

### Core (Built-In)
- **Media Module** — Powers inline media tokens and gallery features

### Planned Modules
- **Maps Module** — Interactive map pins + `<scms:Map />` token
- **Survey Module** — Poll and survey embedding + analytics
- **External Authentication** — OAuth, SAML, OpenID + login tokens
- **Comment Module** — Threaded comments, votes, emoji reactions
- **Captcha Module** — Required for guest content; supports reCAPTCHA, hCaptcha
- **Storefront Module** — Embeddable store listings and cart (future)
- **Messages Module** — Direct messages and @username mentions
- **Notices Module** — Sticky notices, auto-expire or user-dismissable
- **Donations Module** — Ko-fi, Patreon, PayPal support
- **Forums Module** — Threaded discussion with voting, sticky support (potential)
- **2FA Module** — Two-Factor Auth via TOTP or Passkeys
- **Forms Module** — Custom form builder and submissions
- **Feedback Module** — Lightweight feedback collection
- **Ratings Module** — Integrated content ratings and voting
- **Bookmarks/Links Module** — Public/private links, categories, and browser import

---

## ⚙️ Admin Tools
- Page manager
- Media manager
- Blog manager
- Navigation/menu editor
- Social media settings
- System settings (signup toggle, TinyMCE license key, TinyMCE source)

---

## 🔐 Security & Access
- ASP.NET Identity-based role management
- Private vs Public page visibility
- 2FA support module with passkeys
- External login support via module

---

## 🔄 Portability & Deployment
- Dockerized from day one
- Easily movable via DB + media folders
- Lightweight and portable
- Suitable for hobbyists and small orgs
---

## 🔧 Extensibility & Modularity
- Module system with self-registration and EF migrations
- Future roadmap includes module store/registry for versioning and upgrades

---

This document serves as the foundation for CMS development and will evolve as features are implemented.
