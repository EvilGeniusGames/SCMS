# 📁 SCMS Media File Access & Handling

## 📦 Upload Flow

1. Files uploaded via TinyMCE are saved to:  
   `/wwwroot/uploads/temp/`

2. On Save:
   - If `SecurityLevelId == 3` (Anonymous):
     - File is moved to:  
       `/wwwroot/uploads/public/`  
     - Image `src` in HTML becomes:  
       `<img src="/uploads/public/filename.jpg">`  
     - Served directly by web server (no authorization).

   - If `SecurityLevelId < 3` (User/Admin):
     - File is moved to:  
       `/app/uploads/secure/`  
     - Image `src` in HTML becomes:  
       `<img src="/media/secure/filename.jpg">`  
     - Handled via `MediaController` with access validation.

---

## 🧠 Media Request Flow

**Mermaid Diagram:**

```mermaid
flowchart TD  
    A[Browser loads HTML] --> B[Finds <img src="/media/secure/filename.jpg">]  
    B --> C[Browser sends GET to /media/secure/filename.jpg]  
    C --> D[ASP.NET Core matches MediaController.SecureFile]  
    D --> E[Controller checks if user is authenticated]  
    E --> F{Authorized?}  
    F -- Yes --> G[Return PhysicalFile]  
    F -- No --> H[Return 403 Forbidden]
  ```
  🔐 MediaController Responsibilities
- Route `/media/secure/{filename}` maps to files in:
`/app/uploads/secure/`
- Validates user identity or role as needed
- Returns protected media only to authorized users
- Public media (`/uploads/public/`) bypasses controller entirely and is served by the static file middleware.


## ✅ Summary

| Security Level | Description                  | Final Destination            | Access Method       |
|----------------|------------------------------|------------------------------|---------------------|
| Anonymous (3)  | Publicly accessible content  | /wwwroot/uploads/public/     | Static file server  |
| User (2)       | Requires user login          | /app/uploads/secure/         | /media/secure/*     |
| Administrator (1) | Admin-only access         | /app/uploads/secure/         | /media/secure/*     |
