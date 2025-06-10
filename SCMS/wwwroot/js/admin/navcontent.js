
document.addEventListener("DOMContentLoaded", function () {
    console.log("DOM fully loaded");
    const editorPane = document.getElementById("menuItemEditor");
      
    let groupEditMode = null; // 'add' or 'rename'

    // Show input and set mode
    function showGroupEditor(mode, initialValue = '') {
        groupEditMode = mode;
        document.getElementById('groupNameInput').value = initialValue;
        document.getElementById('groupNameEditor').classList.remove('d-none');
        document.getElementById('menuGroupDropdown').classList.add('d-none');
    }

    // Hide input and reset state
    function cancelGroupEdit() {
        groupEditMode = null;
        document.getElementById('groupNameInput').value = '';
        document.getElementById('groupNameEditor').classList.add('d-none');
        document.getElementById('menuGroupDropdown').classList.remove('d-none');
    }

    // Confirm add or rename
    async function confirmGroupEdit() {

        const currentGroup = document.getElementById('menuGroupDropdown').value;
        if (groupEditMode === 'rename' && currentGroup === "Main") {
            alert("The 'Main' group cannot be renamed.");
            return;
        }
        const input = document.getElementById('groupNameInput').value.trim();
        if (input.length < 1) {
            alert("Group name must be at least 1 character.");
            return;
        }

        let response;
        if (groupEditMode === 'add') {
            response = await fetch('/admin/navcontent/group/add', {
                method: 'POST',
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ name: input })
            });
        } else if (groupEditMode === 'rename') {
            const currentGroup = document.getElementById('menuGroupDropdown').value;
            response = await fetch('/admin/navcontent/group/rename', {
                method: 'POST',
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ oldName: currentGroup, newName: input })
            });
        }

        if (response.ok) {
            cancelGroupEdit();

            // Add to dropdown if not present
            const dropdown = document.getElementById('menuGroupDropdown');
            let exists = Array.from(dropdown.options).some(o => o.value === input);
            if (!exists) {
                const option = document.createElement("option");
                option.value = input;
                option.text = input;
                dropdown.appendChild(option);
            }

            dropdown.value = input;
            await loadMenuTree(input); // ✅ Direct call instead of relying on event
        }
    }

    // Delete group
    document.getElementById('deleteGroupBtn').addEventListener('click', async () => {
        const group = document.getElementById('menuGroupDropdown').value;
        
        if (group === "Main") {
            alert("The 'Main' group cannot be deleted.");
            return;
        }

        if (!confirm(`Delete group '${group}' and all its menu items?`)) return;

        const response = await fetch('/admin/navcontent/group/delete', {
            method: 'POST',
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ name: group })
        });

        if (response.ok) location.reload();
        else alert("Delete failed.");
    });

    // Add/Rename actions
    document.getElementById('addGroupBtn').addEventListener('click', () => showGroupEditor('add'));
    document.getElementById('renameGroupBtn').addEventListener('click', () => {
        const current = document.getElementById('menuGroupDropdown').value;
        showGroupEditor('rename', current);
    });
    document.getElementById('cancelGroupBtn').addEventListener('click', cancelGroupEdit);
    document.getElementById('confirmGroupBtn').addEventListener('click', confirmGroupEdit);

    // load the menu list
    async function loadMenuTree(groupName) {
        const container = document.getElementById("menuTreeView");
        container.innerHTML = `<li class="list-group-item text-muted">Loading...</li>`;

        const response = await fetch(`/admin/navcontent/group/items/${groupName}`);
        if (response.ok) {
            const html = await response.text();
            container.innerHTML = html;
            wireMenuItemClicks();
        } else {
            container.innerHTML = `<li class="list-group-item text-danger">Failed to load menu items.</li>`;
        }
    }

    // Load the menu tree when group changes in dropdown
    document.getElementById("menuGroupDropdown").addEventListener("change", function () {
        updateRenameVisibility(this.value);
        loadMenuTree(this.value);
    });

    //wire up the clicks for the menu items
    function wireMenuItemClicks() {
        document.querySelectorAll("#menuTreeView li").forEach(li => {
            li.addEventListener("click", async function (e) {
                // Highlight selected
                document.querySelectorAll("#menuTreeView li").forEach(el => el.classList.remove("active"));
                this.classList.add("active");
                e.preventDefault();
                const itemId = this.getAttribute("data-id");
                const response = await fetch(`/admin/navcontent/load/${itemId}`);
                if (!response.ok) return;
                const data = await response.json();
                renderEditor(data);
            });
        });
    }
    function renderEditor(data) {
        const editorPane = document.getElementById("menuItemEditor");

        editorPane.innerHTML = `
        <div class="card-body">
            <h5 class="mb-3">Editing: ${data.title}</h5>
            <form id="menuEditorForm" data-id="${data.id}">
                <div class="mb-3">
                    <label class="form-label">Menu label/name</label>
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

                <div class="mb-3">
                    <label class="form-label">Page Title (used in browser tab)</label>
                    <input type="text" class="form-control" name="pageTitle" value="${data.pageTitle || ''}" />
                </div>


                <div class="mb-3">
                    <label class="form-label">Meta Description</label>
                    <textarea class="form-control" name="metaDescription" rows="2">${data.metaDescription || ''}</textarea>
                </div>

                <div class="mb-3">
                    <label class="form-label">Meta Keywords (comma-separated)</label>
                    <textarea class="form-control" name="metaKeywords" rows="2">${(data.metaKeywords || []).join(', ')}</textarea>
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
                        plugins: 'code link image lists table',
                        toolbar: 'fullscreenToggle | undo redo | blocks fontfamily fontsize | bold italic underline | alignleft aligncenter alignright | bullist numlist outdent indent | link image table code removeformat',
                        menubar: false,
                        branding: false,
                        license_key: 'gpl',
                        height: 400,
                        paste_as_text: true,
                        images_upload_url: '/admin/upload/image',
                        file_picker_types: 'image',
                        file_picker_callback: function (cb, value, meta) {
                            if (meta.filetype !== 'image') return;
                            const input = document.createElement('input');
                            input.setAttribute('type', 'file');
                            input.setAttribute('accept', 'image/*');
                            input.onchange = function () {
                                const file = this.files[0];
                                const formData = new FormData();
                                formData.append('file', file, file.name);
                                fetch('/admin/upload/image', {
                                    method: 'POST',
                                    body: formData
                                })
                                    .then(response => response.json())
                                    .then(data => {
                                        if (data && data.url) {
                                            cb(data.url, { alt: file.name });
                                        } else {
                                            alert("Upload failed: Invalid response.");
                                        }
                                    })
                                    .catch(err => {
                                        alert("Upload error: " + err);
                                    });
                            };
                            input.click();
                        },
                        setup: function (editor) {
                            editor.on('init', () => {
                                if (data.htmlContent) {
                                    editor.setContent(data.htmlContent);
                                }
                            });
                            editor.ui.registry.addButton('fullscreenToggle', {
                                icon: 'fullscreen',
                                tooltip: 'Toggle Full Window',
                                onAction: function () {
                                    document.body.classList.toggle('tinymce-fullscreen');
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
    }

    const addItemBtn = document.getElementById('addItemBtn');
    if (addItemBtn) {
        addItemBtn.addEventListener('click', async () => {
            const selected = document.querySelector("#menuTreeView li.active");
            const currentGroup = document.getElementById("menuGroupDropdown").value;

            const payload = {
                title: "New Item",
                group: currentGroup,
                parentId: selected?.getAttribute("data-id") || null,
                insertAfterId: selected?.getAttribute("data-id") || null
            };

            const response = await fetch("/admin/navcontent/item/create", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload)
            });

            if (response.ok) {
                await loadMenuTree(currentGroup);
            } else {
                alert("Failed to create item.");
            }
        });
    }

    document.getElementById('deleteItemBtn').addEventListener('click', async () => {
        const selected = document.querySelector("#menuTreeView li.active");
        if (!selected) {
            alert("Please select a menu item to delete.");
            return;
        }

        const id = selected.getAttribute("data-id");
        const confirmed = confirm("Are you sure you want to delete this menu item and its page?");
        if (!confirmed) return;

        const response = await fetch(`/admin/navcontent/item/delete/${id}`, {
            method: "POST"
        });

        if (response.ok) {
            const group = document.getElementById("menuGroupDropdown").value;
            await loadMenuTree(group);
            document.getElementById("menuItemEditor").innerHTML =
                `<div class="card-body text-muted">Select a menu item to begin editing.</div>`;
        } else {
            alert("Delete failed.");
        }
    });


    document.addEventListener("submit", async function (e) {
        if (e.target.id === "menuEditorForm") {
            e.preventDefault();

            const form = e.target;
            const id = form.getAttribute("data-id");
            const originalTitle = document.querySelector(`#menuTreeView li[data-id="${id}"]`)?.innerText.trim();
            const isExternal = form.isExternal.checked;
            const editorContent = isExternal ? null : tinymce.activeEditor.getContent();

            const payload = {
                id: parseInt(id),
                title: form.title.value,
                pageTitle: form.pageTitle.value,
                url: form.url?.value || null,
                isExternal: isExternal,
                isVisible: form.isVisible.checked,
                securityLevelId: parseInt(form.securityLevelId.value),
                htmlContent: editorContent,
                metaDescription: form.metaDescription.value,
                metaKeywords: form.metaKeywords.value.split(',').map(k => k.trim()).filter(k => k.length > 0)
            };


            const response = await fetch("/admin/navcontent/save", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload)
            });

            if (response.ok) {
                const toast = new bootstrap.Toast(document.getElementById("saveToast"));
                toast.show();

                const currentGroup = document.getElementById("menuGroupDropdown").value;
                const updatedTitle = form.title.value.trim();

                if (originalTitle && updatedTitle && originalTitle !== updatedTitle) {
                    await loadMenuTree(currentGroup); // Rebuild menu tree if title changed
                }


                if (!isExternal) {
                    const reload = await fetch(`/admin/navcontent/load/${id}`);
                    if (reload.ok) {
                        const newData = await reload.json();
                        tinymce.activeEditor.setContent(newData.htmlContent || "");
                    }
                }
            } else {
                alert("Save failed.");
            }
        }
    });

    function updateRenameVisibility(groupName) {
        document.getElementById('renameGroupBtn').classList.toggle('d-none', groupName === "Main");
    }

    const initialGroup = document.getElementById("menuGroupDropdown")?.value;
    if (initialGroup) {
        loadMenuTree(initialGroup);
        updateRenameVisibility(initialGroup)
    }

});
