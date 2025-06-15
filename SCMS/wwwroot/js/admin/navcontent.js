
document.addEventListener("DOMContentLoaded", function () {
    // Ensure DOM is fully loaded before running scripts

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
   

    // Load the menu tree when group changes in dropdown
    document.getElementById("menuGroupDropdown").addEventListener("change", function () {
        updateRenameVisibility(this.value);
        loadMenuTree(this.value);
    });

    // wire up the clicks for the menu items
    async function loadMenuTree(groupName) {
        const container = document.getElementById("menuTreeView");
        container.innerHTML = `<li class="list-group-item text-muted">Loading...</li>`;

        const response = await fetch(`/admin/navcontent/group/items/${groupName}`);
        if (response.ok) {
            const html = await response.text();
            container.innerHTML = html;

            wireMenuItemClicks(); // <-- needs to run before restoring selection

            const savedId = localStorage.getItem("navcontent.selectedId");
            if (savedId) {
                const match = document.querySelector(`#menuTreeView li[data-id="${savedId}"]`);
                if (match) {
                    match.classList.add("active");
                    match.scrollIntoView({ behavior: "smooth", block: "nearest" });

                    const reload = await fetch(`/admin/navcontent/load/${savedId}`);
                    if (reload.ok) {
                        const data = await reload.json();
                        renderEditor(data);
                    }
                }
            }
        } else {
            container.innerHTML = `<li class="list-group-item text-danger">Failed to load menu items.</li>`;
        }
    }

    // render the editor pane
    function renderEditor(data) {
        const editorPane = document.getElementById("menuItemEditor");

        editorPane.innerHTML = `
            <div class="card-body">
                <form id="menuEditorForm" data-id="${data.id}">
                    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem;">
                        <h5 class="mb-0">Editing: ${data.title}</h5>
                        <button type="submit" class="btn btn-sm btn-primary">Save</button>
                    </div>
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
        // Initialize TinyMCE editor
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

    // outdent menu item
    document.getElementById('outdentBtn').addEventListener('click', async () => {
        // Check for selected item
        const selected = document.querySelector("#menuTreeView li.active");
        if (!selected) {
            alert("Select a menu item first.");
            return;
        }

        // Get the selected item's ID and current group
        const selectedId = parseInt(selected.getAttribute("data-id"));
        const currentGroup = document.getElementById("menuGroupDropdown").value;

        // Fetch the current menu structure from controller
        const response = await fetch(`/admin/navcontent/group/structure/${currentGroup}`);
        if (!response.ok) {
            alert("Failed to retrieve structure.");
            return;
        }

        // Serialize the menu structure
        const menu = await response.json();

        // searches the menu array for the item whose id matches the selected item's ID.
        const current = menu.find(i => i.id === selectedId);
        if (!current) {
            alert("Current item not found.");
            return;
        }

        // Check if the item is already at root level
        if (!current.parentId) {
            alert("Item is already at root level.");
            return;
        }

        // Locate the index of the current item in the menu array
        const index = menu.findIndex(i => i.id === selectedId);

        // Attempt to find the first item above whose ID matches the current item's parentId
        const currentParentId = current.parentId;
        const parent = menu.find(i => i.id === currentParentId);

        // Determine the grandparentId of the current item
        const grandParentId = parent?.parentId ?? null;

        // If no parent found, we can't outdent
        const newParentId = parent?.parentId ?? null;

        // Set new parent
        const parentResult = await fetch(`/admin/navcontent/item/set-parent`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                id: selectedId,
                parentId: newParentId
            })
        });
        // Check if the parent update was successful
        if (!parentResult.ok) {
            alert("Outdent failed.");
            return;
        }
        // Fetch structure again to reorder siblings
        const reorderResponse = await fetch(`/admin/navcontent/group/structure/${currentGroup}`);
        if (reorderResponse.ok) {
            const refreshed = await reorderResponse.json();
            const siblings = refreshed.filter(i => i.parentId === newParentId);

            // Find index of previous parent in sibling list
            let insertIndex = siblings.findIndex(i => i.id === parent?.id);

            // If no parent found, insert at the end of the list
            if (insertIndex === -1) {
                // fallback: place after the first root-level item
                insertIndex = 0;
            }

            // If the current item is already at the end, no need to reorder
            siblings.splice(insertIndex + 1, 0, current);

            // Reorder siblings based on their new order
            const reorderPayload = siblings.map((item, i) => ({ id: item.id, order: i }));
            // Send reorder request
            await fetch("/admin/navcontent/item/reorder", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(reorderPayload)
            });
        }
        // Reload the menu tree to reflect changes
        await loadMenuTree(currentGroup);
    });


        // Function to calculate depth of an item in the tree
    function getDepth(tree, item) {
        let depth = 0;
        let cursor = item;
        while (cursor?.parentId) {
            cursor = tree.find(i => i.id === cursor.parentId);
            if (cursor) depth++;
        }
        return depth;
    }

    // Indent menu item
    document.getElementById('indentBtn').addEventListener('click', async () => {
        //cehck for selected item
        const selected = document.querySelector("#menuTreeView li.active");
        if (!selected) {
            alert("Select a menu item first.");
            return;
        }
        // Get the selected item's ID and current group
        const selectedId = parseInt(selected.getAttribute("data-id"));
        const currentGroup = document.getElementById("menuGroupDropdown").value;
        // Fetch the current menu structure from controller
        const response = await fetch(`/admin/navcontent/group/structure/${currentGroup}`);
        if (!response.ok) {
            alert("Failed to retrieve structure.");
            return;
        }
        // seralize the menu structure
        const menu = await response.json();
        //check if there is an item aboe it in the hierarchy
        const index = menu.findIndex(i => i.id === selectedId);
        if (index <= 0) {
            alert("Cannot indent — no item above.");
            return;
        }
        // Get the item above the selected one
        const above = menu[index - 1];
        const current = menu[index];

        // Find nearest shallower item to act as new parent
        let newParentId = null;

        for (let i = index - 1; i >= 0; i--) {
            const candidate = menu[i];

            // Find the first item above at the same level
            if ((candidate.parentId ?? null) === (current.parentId ?? null)) {
                newParentId = candidate.id;
                break;
            }
        }

        // if no item above has a parentid that matches the current item's parentId, we can't indent
        if (!newParentId) {
            alert("No suitable parent found for indent.");
            return;
        }

        // Set new parent
        const parentResult = await fetch(`/admin/navcontent/item/set-parent`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                id: selectedId,
                parentId: newParentId
            })
        });
        // Check if the parent update was successful
        if (!parentResult.ok) {
            alert("Parent update failed.");
            return;
        }

        // Fetch structure again to reorder siblings
        const reorderResponse = await fetch(`/admin/navcontent/group/structure/${currentGroup}`);
        if (reorderResponse.ok) {
            const refreshed = await reorderResponse.json();
            const siblings = refreshed.filter(i => i.parentId === newParentId);
            const reorderPayload = siblings.map((item, i) => ({ id: item.id, order: i }));

            await fetch("/admin/navcontent/item/reorder", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(reorderPayload)
            });
        }
        // Reload the menu tree to reflect changes
        await loadMenuTree(currentGroup);
    });

    // Move down menu item
    document.getElementById('moveDownBtn').addEventListener('click', async () => {
        const selected = document.querySelector("#menuTreeView li.active");
        if (!selected) {
            alert("Select a menu item first.");
            return;
        }

        const selectedId = parseInt(selected.getAttribute("data-id"));
        const currentGroup = document.getElementById("menuGroupDropdown").value;

        const response = await fetch(`/admin/navcontent/group/structure/${currentGroup}`);
        if (!response.ok) {
            alert("Failed to retrieve structure.");
            return;
        }

        const menu = await response.json();
        const index = menu.findIndex(i => i.id === selectedId);
        const current = menu[index];

        // Find the next sibling with the same parent
        for (let i = index + 1; i < menu.length; i++) {
            const candidate = menu[i];
            if (candidate.parentId === current.parentId) {
                const result = await fetch("/admin/navcontent/item/reorder", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify([
                        { id: current.id, order: candidate.order },
                        { id: candidate.id, order: current.order }
                    ])
                });

                if (result.ok) {
                    await loadMenuTree(currentGroup);
                } else {
                    alert("Move down failed.");
                }

                return;
            }
        }

        alert("No sibling below to move down with.");
    });

    // Move up menu item
    document.getElementById('moveUpBtn').addEventListener('click', async () => {
        const selected = document.querySelector("#menuTreeView li.active");
        if (!selected) {
            alert("Select a menu item first.");
            return;
        }

        const selectedId = parseInt(selected.getAttribute("data-id"));
        const currentGroup = document.getElementById("menuGroupDropdown").value;

        const response = await fetch(`/admin/navcontent/group/structure/${currentGroup}`);
        if (!response.ok) {
            alert("Failed to retrieve structure.");
            return;
        }

        const menu = await response.json();
        const index = menu.findIndex(i => i.id === selectedId);
        const current = menu[index];

        // Find previous sibling in same parent scope
        for (let i = index - 1; i >= 0; i--) {
            const prev = menu[i];
            if (prev.parentId === current.parentId) {
                const result = await fetch("/admin/navcontent/item/reorder", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify([
                        { id: current.id, order: prev.order },
                        { id: prev.id, order: current.order }
                    ])
                });

                if (result.ok) {
                    await loadMenuTree(currentGroup);
                } else {
                    alert("Move up failed.");
                }
                return;
            }
        }

        alert("Cannot move up — no sibling above.");
    });

    // add new menu
    const addItemBtn = document.getElementById('addItemBtn');
    if (addItemBtn) {
        addItemBtn.addEventListener('click', async () => {
            const selected = document.querySelector("#menuTreeView li.active");
            const currentGroup = document.getElementById("menuGroupDropdown").value;

            const payload = {
                title: "New Item",
                group: currentGroup,
                parentId: selected?.getAttribute("data-parent-id") || null,
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

    //delete selected menu item
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

    //submit the form
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

    // Update rename button visibility based on group
    function updateRenameVisibility(groupName) {
        document.getElementById('renameGroupBtn').classList.toggle('d-none', groupName === "Main");
    }

    // wire up the menu item clicks
    function wireMenuItemClicks() {
        document.querySelectorAll("#menuTreeView li").forEach(li => {
            li.addEventListener("click", async function (e) {
                // Highlight selected
                document.querySelectorAll("#menuTreeView li").forEach(el => el.classList.remove("active"));
                this.classList.add("active");
                e.preventDefault();

                // Store selected ID
                localStorage.setItem("navcontent.selectedId", this.getAttribute("data-id"));

                // Load editor pane
                const itemId = this.getAttribute("data-id");
                const response = await fetch(`/admin/navcontent/load/${itemId}`);
                if (!response.ok) return;
                const data = await response.json();
                renderEditor(data);
            });
        });
    }


    // Initial load of menu tree
    const initialGroup = document.getElementById("menuGroupDropdown")?.value;
    if (initialGroup) {
        loadMenuTree(initialGroup);
        updateRenameVisibility(initialGroup)
    }

});
