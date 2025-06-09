# 🖼️ SCMS TinyMCE Image Upload & Normalization Workflow

## ✅ Summary

1. User uploads an image via TinyMCE in the admin interface.
2. The image is stored in `/uploads/temp/`.
3. TinyMCE inserts an `<img src="/uploads/temp/filename.jpg">` tag into the content.
4. On form submission, the server:
   - Parses the HTML for temp image references
   - Moves each image to either `/uploads/public/` or `/uploads/protected/`
     based on the selected `SecurityLevelId`
   - Updates each `<img src="...">` path in the HTML
5. The normalized HTML is stored in the database and rendered securely.

During normalization, if a file with the same name already exists in the destination folder, the system checks for collisions. It may prompt the user to overwrite, or automatically resolve the conflict by appending a numeric suffix to the filename (e.g., banner-1.jpg, banner-2.jpg, etc.). This ensures existing files are preserved unless explicitly replaced.

---

## 🔁 Flowchart

```mermaid
flowchart TD
    A[User selects content in SCMS admin editor] --> B[TinyMCE editor initialized]
    B --> C[User uploads image]
    C --> D[/admin/upload/image stores file in /uploads/temp/]
    D --> E[TinyMCE inserts <img src="/uploads/temp/filename.jpg"> into HTML]
    E --> F[User submits form]
    F --> G[Server parses htmlContent]
    G --> H{SecurityLevelId}
    H -- Anonymous --> I[Move to /uploads/public/]
    H -- User/Admin --> J[Move to /uploads/protected/]
    I --> K[Rewrite <img src> to new path]
    J --> K
    K --> L[Save normalized htmlContent to database]
