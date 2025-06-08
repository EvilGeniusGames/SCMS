document.addEventListener("DOMContentLoaded", function () {
    const editorPane = document.getElementById("menuItemEditor");

    document.querySelectorAll("#menuTreeView li").forEach(li => {
        li.addEventListener("click", async function (e) {
            e.preventDefault();
            const itemId = this.getAttribute("data-id");

            const response = await fetch(`/admin/navcontent/load/${itemId}`);
            if (!response.ok) return;

            const data = await response.json();
            editorPane.innerHTML = `
                <div class="card-body">
                    <h5 class="mb-3">Editing: ${data.title}</h5>
                    <form id="menuEditorForm" data-id="${data.id}">
                        <div class="mb-3">
                            <label class="form-label">Title</label>
                            <input type="text" class="form-control" name="title" value="${data.title}" />
                        </div>

                        <div class="mb-3 form-check">
                            <input type="checkbox" class="form-check-input" name="isExternal" id="isExternalCheck" ${data.url ? 'checked' : ''} />
                            <label class="form-check-label" for="isExternalCheck">External Link</label>
                        </div>

                        <div id="urlGroup" class="mb-3 ${data.url ? '' : 'd-none'}">
                            <label class="form-label">URL</label>
                            <input type="text" class="form-control" name="url" value="${data.url || ''}" />
                        </div>

                        <div class="mb-3 form-check">
                            <input type="checkbox" class="form-check-input" name="isVisible" id="isVisibleCheck" ${data.isVisible ? 'checked' : ''} />
                            <label class="form-check-label" for="isVisibleCheck">Visible</label>
                        </div>

                        <div class="mb-3">
                            <label class="form-label">Security Level</label>
                            <select class="form-select" name="securityLevelId">
                                <option value="3" ${data.securityLevelId == 3 ? 'selected' : ''}>Anonymous</option>
                                <option value="2" ${data.securityLevelId == 2 ? 'selected' : ''}>User</option>
                                <option value="1" ${data.securityLevelId == 1 ? 'selected' : ''}>Administrator</option>
                            </select>
                        </div>

                        <div id="pageEditorGroup" class="mb-3 ${data.url ? 'd-none' : ''}">
                            <label class="form-label">Page Content</label>
                            <textarea id="tinymceEditor"></textarea>
                        </div>

                        <button type="submit" class="btn btn-primary">Save</button>
                    </form>
                </div>
            `;

            const isExternalCheck = document.getElementById("isExternalCheck");
            const urlGroup = document.getElementById("urlGroup");
            const pageEditorGroup = document.getElementById("pageEditorGroup");

            isExternalCheck.addEventListener("change", function () {
                const external = this.checked;
                urlGroup.classList.toggle("d-none", !external);
                pageEditorGroup.classList.toggle("d-none", external);

                if (!external) {
                    setTimeout(() => {
                        tinymce.remove();
                        tinymce.init({
                            selector: '#tinymceEditor',
                            plugins: 'code link image lists table hr paste',
                            toolbar: 'undo redo | blocks fontfamily fontsize | bold italic underline | alignleft aligncenter alignright | bullist numlist outdent indent | link image table hr | code removeformat',
                            menubar: false,
                            branding: false,
                            height: 400,
                            paste_as_text: true,
                            setup: function (editor) {
                                editor.on('init', () => {
                                    if (data.htmlContent) {
                                        editor.setContent(data.htmlContent);
                                    }
                                });
                            }
                        });
                    }, 10);
                } else {
                    tinymce.remove();
                }
            });

            isExternalCheck.dispatchEvent(new Event("change"));
        });

        document.addEventListener("submit", async function (e) {
            if (e.target.id === "menuEditorForm") {
                e.preventDefault();

                const form = e.target;
                const id = form.getAttribute("data-id");
                const isExternal = form.isExternal.checked;
                const editorContent = isExternal ? null : tinymce.activeEditor.getContent();

                const payload = {
                    id: parseInt(id),
                    title: form.title.value,
                    url: form.url?.value || null,
                    isExternal: isExternal,
                    isVisible: form.isVisible.checked,
                    securityLevelId: parseInt(form.securityLevelId.value),
                    htmlContent: editorContent
                };

                const response = await fetch("/admin/navcontent/save", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(payload)
                });

                if (response.ok) {
                    const toast = new bootstrap.Toast(document.getElementById("saveToast"));
                    toast.show();
                } else {
                    alert("Save failed.");
                }
            }
        });
    });
});
