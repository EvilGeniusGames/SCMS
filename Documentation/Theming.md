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
      ├── layout.cshtml            # Main layout template
      ├── theme.config.json        # Theme metadata (name, description, version, etc.)
      ├── partials/                # Partial views (e.g., header, footer, sidebar)
      │   ├── header.cshtml
      │   └── footer.cshtml
      ├── templates/               # Templates for specific content types
      │   ├── page.cshtml
      │   ├── blog.cshtml
      │   └── post.cshtml
      ├── css/                     # Theme-specific styles
      │   └── theme.css
      ├── js/                      # Theme-specific scripts
      │   └── theme.js
      └── images/                  # Static image assets for layout
          ├── logo.png
          └── background.jpg
```

---

## File Descriptions

### theme.config.json schema

The theme configuration file should define the following properties:

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
- `author`: Theme creator name or organization
- `description`: Optional summary shown in the admin panel
- `previewImage`: Path to a screenshot/thumbnail used in the theme selector
- `defaultTemplate`: Fallback template used for pages
- `layout`: Main layout file to wrap all rendered content
- **layout.cshtml**: Acts as the base layout for all pages rendered with this theme. Typically includes references to partials and defines the main layout grid.
- **theme.config.json**: Contains metadata for the theme such as:
  ```json
  {
    "name": "Default Theme",
    "version": "1.0",
    "author": "SCMS Team",
    "defaultTemplate": "page.cshtml"
  }
  ```
- **partials/**: Contains reusable visual components that appear on multiple pages.
- **templates/**: Includes the core content-rendering templates for pages, blogs, posts, etc.
- **css/** and **js/**: Contain theme-specific styling and behavior.

---

## Theme Selection & Activation

- The active theme is configured via the Admin UI or stored in the CMS configuration.
- On runtime, SCMS dynamically selects the correct layout and template files based on the selected theme.

---

## Razor View Resolution

The CMS overrides default Razor view locations to support:

```csharp
/Themes/{ActiveTheme}/layout.cshtml
/Themes/{ActiveTheme}/partials/{PartialName}.cshtml
/Themes/{ActiveTheme}/templates/{TemplateName}.cshtml
```

Views are rendered dynamically by injecting content into `@RenderBody()` or named `@RenderSection()` blocks.

---

## Theme Installation

- Themes can be uploaded as `.zip` packages containing the folder structure above.
- Upon upload, the system extracts the package into `/Themes/{FolderName}`.
- Only safe assets (cshtml, json, css, js) are permitted—no binaries or executables.

---

## Using External Frameworks (e.g., Bootstrap or JavaScript Libraries)

Designers can include external CSS frameworks such as Bootstrap by referencing them in `layout.cshtml` via CDN or by bundling them into the theme's `css/` directory.

For example, to include Bootstrap via CDN:

```html
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-..." crossorigin="anonymous">
```

This allows use of Bootstrap's grid system, components, and utility classes directly in theme files and templates.

Designers may also include static versions of JavaScript frameworks by copying their `.js` files into the theme's `/js/` directory and referencing them in `layout.cshtml` like so:
```html
<script src="/Themes/default/js/bootstrap.bundle.min.js"></script>
```

## Asset Handling Notes

- **Images**: Place decorative or structural images (e.g., backgrounds, logos) in the `/images/` directory. Reference them in CSS or HTML using relative theme paths.
- **Fonts**: Store custom web fonts in a `/fonts/` folder within the theme. Use `@font-face` rules in your CSS to load them.
- **JavaScript Libraries**: Place local versions of JavaScript libraries in the `/js/` directory and reference them in `layout.cshtml`.
- **CSS Frameworks**: Include via CDN or store in `/css/` for portability.
- **Icons**: Use inline SVGs or icon libraries; bundle local assets in `/images/` or `/fonts/`.
- **Avoid Hardcoded Paths**: Always use `/Themes/{Theme}/...` when referencing assets to ensure compatibility with theme switching.

## Best Practices

- Keep layouts minimal and modular
- Use partials for all shared components
- Avoid hardcoded paths—use token rendering where appropriate
- Include a clear `theme.config.json` file

---

## Designer Tokens

Tokens that can be used inside theme files to inject dynamic CMS data:
- `<cms:LoginStatus />` – Renders the login/logout button or user link depending on authentication state.
- `<cms:SiteLogo />` – Displays the logo configured in the CMS settings.
- `<cms:Menu orientation="horizontal|vertical" />` – Renders the menu in the specified layout.
- `<cms:Content />` – Injects the main body content of the page.
- `<cms:PageTitle />` – Outputs the current page title.
- `<cms:CreatedDate />` – Displays the date the page was created.
- `<cms:LastUpdated />` – Shows the date the page was last updated.
- `<cms:UpdatedBy />` – Shows the name of the user who last edited the page.
- `<cms:Username />` – Renders the currently logged-in user's name.
- `<cms:UserEmail />` – Displays the logged-in user’s email.
- `<cms:UserRole />` – Outputs the role of the logged-in user.
- `<cms:IsAuthenticated />` – Outputs true/false based on user login state.
- `<cms:SiteTitle />` – Displays the site title from CMS settings.
- `<cms:ThemeName />` – Shows the name of the active theme.
- `<cms:CurrentUrl />` – Outputs the current page’s URL.
- `<cms:Year />` – Displays the current year.
- `<cms:SearchBar />` – Injects a search input.
- `<cms:SocialLinks />` – Renders social icons/links defined in admin.
- `<cms:Breadcrumbs />` – Renders the navigation trail based on menu structure.
- `<cms:Privacy />` – Outputs the privacy policy snippet.
- `<cms:License />` – Renders the license text or link.
- `<cms:Copyright />` – Renders the copyright line from admin settings.

## Token Rendering Engine Overview

SCMS uses a lightweight token parsing engine to dynamically inject values into theme files at runtime. Tokens are written in the format `<cms:TokenName />` or `<cms:Namespace:Token />`.

### How It Works

- When rendering Razor views (layout, partials, templates), the system passes HTML content through a token renderer.
- The engine scans for known tokens and replaces them with rendered HTML or plain text.
- Token rendering occurs **after view rendering**, allowing for nested Razor logic and conditional display to work seamlessly.

### Safe Rendering

- Tokens are HTML-safe by default.
- Complex tokens (like menus or forms) are rendered server-side with full Razor support before being injected into the layout.

This approach ensures that tokens are cleanly separated from business logic while remaining flexible and safe to use anywhere within a theme file.

## First Theme Example: Simple One-Page Theme

A basic theme layout using:

- Logo token in the top left corner
- Horizontal menu token beneath the header
- Content section in the center
- Footer with copyright line token

### partials/header.cshtml

```html
<header>
    <div class="logo">@Html.Raw("<cms:SiteLogo />")</div>
    <nav>@Html.Raw("<cms:Menu orientation="horizontal" />")</nav>
</header>
```

### partials/footer.cshtml

```html
<footer>
    <div>@Html.Raw("<cms:Copyright />")</div>
</footer>
```

A basic theme layout using:

- Logo token in the top left corner
- Horizontal menu token beneath the header
- Content section in the center
- Footer with copyright line token

### layout.cshtml

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>@RenderSection("Title", false)</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-..." crossorigin="anonymous">
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

This theming model ensures that SCMS is visually flexible, portable, and safe for user-contributed themes.
