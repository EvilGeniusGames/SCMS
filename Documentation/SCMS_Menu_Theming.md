## Token-Based Menu Rendering Plan (No Razor, Theme-Friendly)

### 1. Define Menu Template System

- Each theme contains a static file:
/Themes/{Theme}/partials/menu.template.html

- Example template format (Mustache-style):
```html
<ul class="navbar-nav">
  {{#each Items}}
    {{#if Children.length}}
      <li class="nav-item dropdown">
        <a class="nav-link dropdown-toggle" href="#" data-bs-toggle="dropdown">{{Text}}</a>
        <ul class="dropdown-menu">
          {{#each Children}}
            <li><a class="dropdown-item" href="{{Url}}">{{Text}}</a></li>
          {{/each}}
        </ul>
      </li>
    {{else}}
      <li class="nav-item">
        <a class="nav-link" href="{{Url}}">{{Text}}</a>
      </li>
    {{/if}}
  {{/each}}
</ul>
```

### 2. Create a Lightweight Template Parser
Implement a C# parser to support:

{{Variable}}

{{#each Items}} ... {{/each}}

{{#if Condition}} ... {{else}} ... {{/if}}

Recursive parsing to support nested items (e.g., dropdowns)

### 3. Modify MenuBuilder.GenerateMenuHtml()
Load the template from theme's folder

Use the parser to inject values from a MenuRenderModel

Return the fully processed HTML

### 4. Fallback Strategy
If no theme template exists, fallback to a default hardcoded structure

Log a warning for easier troubleshooting

### 5. Future Extension
Allow themes to define custom menu styles or classes via theme.config.json

Consider shared macros or reusable block templates later