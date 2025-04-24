<img src="SCMS_Logo.png" alt="SCMS Logo" style="max-width: 300px; height: auto;" />


# Theming in SCMS

## Overview

The theming system in SCMS allows developers and designers to control the entire visual presentation and layout of a site. Themes are portable, customizable, and follow a structured file layout to support dynamic content rendering using Razor views.

---

## Theme Folder Structure

Themes may also include static image assets needed by the layout, placed in an `images/` folder for use in backgrounds, logos, icons, or other visual elements.
All themes are stored in `/Themes/{ThemeName}/`. A typical theme layout:

```
/Themes/
  └── default/
      ├── layout.cshtml
      ├── theme.config.json
      ├── partials/
      │   ├── header.cshtml
      │   └── footer.cshtml
      ├── templates/
      │   ├── page.cshtml
      │   ├── blog.cshtml
      │   └── post.cshtml
      ├── css/
      │   └── theme.css
      ├── js/
      │   └── theme.js
      └── images/
          ├── logo.png
          └── background.jpg
```

---

## File Descriptions

### theme.config.json schema

```json
{
  "name": "Default Theme",
  "version": "1.0",
  "author": "SCMS Team",
  "description": "A simple starter theme with Bootstrap layout.",
  "previewImage": "/Themes/default/images/preview.png",
  "defaultTemplate": "page.cshtml",
  "layout": "layout.cshtml"
}
```

- `name`: Display name of the theme  
- `version`: Theme version  
- `author`: Theme creator  
- `description`: Optional summary  
- `previewImage`: Screenshot used in theme selector  
- `defaultTemplate`: Default page template  
- `layout`: Main layout file  

Other files:
- **layout.cshtml**: Base layout for the entire site  
- **partials/**: Shared components like headers and footers  
- **templates/**: Content-specific views  
- **css/** and **js/**: Theme-specific styling and scripts  

---

## Theme Selection & Activation

Themes are activated via the Admin UI or configuration and used to resolve view paths at runtime.

---

## Razor View Resolution

Views are resolved from:

```csharp
/Themes/{ActiveTheme}/layout.cshtml
/Themes/{ActiveTheme}/partials/{PartialName}.cshtml
/Themes/{ActiveTheme}/templates/{TemplateName}.cshtml
```

---

## Theme Installation

Upload `.zip` packages to install themes. They extract to `/Themes/{FolderName}` and allow `.cshtml`, `.json`, `.css`, `.js` only.

---

## Using External Frameworks (e.g., Bootstrap or JavaScript Libraries)

Include CSS via CDN or place JS frameworks in `/js/` and reference them in `layout.cshtml`:

```html
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
<script src="/Themes/default/js/bootstrap.bundle.min.js"></script>
```

---

## Asset Handling Notes

- **Images**: Put in `/images/`  
- **Fonts**: Use `/fonts/` and `@font-face`  
- **JavaScript Libraries**: Place in `/js/`  
- **CSS Frameworks**: Use CDN or `/css/`  
- **Icons**: Inline SVG or bundled in `/images/` or `/fonts/`  
- **Code Styling**: Define `.CodeBlock` class for syntax-highlighted code blocks  
- **Markdown/TinyMCE Content**: Themes should render content from these sources cleanly

---

## Best Practices

- Modular layouts  
- Use partials  
- Avoid hardcoded paths  
- Include `theme.config.json`  
- Support tokens and content editors

---

## Designer Tokens

Use in `.cshtml` files to inject dynamic data:

- `<cms:LoginStatus />` – Login/logout button. **Placement determines where login UI appears**  
- `<cms:SiteLogo />`  
- `<cms:Menu orientation="horizontal|vertical" />`  
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
- `<cms:SearchBar />`  
- `<cms:SocialLinks />`  
- `<cms:Breadcrumbs />`  
- `<cms:Privacy />`  
- `<cms:License />`  
- `<cms:Copyright />`

---

## Token Rendering Engine Overview

Tokens like `<cms:SiteLogo />` are rendered after Razor view execution. They are safe, flexible, and allow nested Razor logic.

---

## First Theme Example: Simple One-Page Theme

### partials/header.cshtml

```html
<header>
    <div class="logo">@Html.Raw("<cms:SiteLogo />")</div>
    <nav>@Html.Raw("<cms:Menu orientation=\"horizontal\" />")</nav>
</header>
```

### partials/footer.cshtml

```html
<footer>
    <div>@Html.Raw("<cms:Copyright />")</div>
</footer>
```

### layout.cshtml

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>@RenderSection("Title", false)</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
    <link rel="stylesheet" href="/Themes/default/css/theme.css" />
</head>
<body>
    @await Html.PartialAsync("partials/header")

    <main>
        @Html.Raw("<cms:Content />")
    </main>

    @await Html.PartialAsync("partials/footer")
</body>
</html>
```

---

This model keeps themes clean, portable, and safe for any SCMS deployment.
