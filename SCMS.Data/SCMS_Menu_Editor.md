### 🎛 Menu Group Controls

- **Dropdown List**
  - `[Main Menu] ▼`
  - Lists all existing menu groups.

- **Buttons/Icons beside Dropdown**
  - ➕ **Add Group** – Opens modal to name new group.
  - ✏️ **Rename Group** – Renames selected group.
  - 🗑 **Delete Group** – Confirms and deletes the selected group.

### ✏️ Rename Group – Implementation Flow

1. **User Action**  
   Clicks the ✏️ *Rename Group* button next to the dropdown.

2. **UI Response**  
   - Inline text input replaces the dropdown temporarily, pre-filled with current name.  
   - Two buttons: ✅ *Save* | ❌ *Cancel*

3. **On Save**  
   - Validate: non-empty, unique.  
   - Call `MenuGroupService.RenameGroup(groupId, newName)`  
   - Reload group list and update dropdown.

4. **On Cancel**  
   - Restore original dropdown state.

### 🛠️ Toolbar (above the group editor)

- ➕ Add Item  
- ➕ Add Submenu *(disabled unless parent selected)*  
- 🗑 Delete Item  
- ☰ Move Up  
- ☰ Move Down  
- 💾 Save Menu  

### 🖱 Context Menu (on menu item right-click)

- ✏️ Edit  
- ➕ Add Submenu  
- ➕ Add Sibling Item  
- ☰ Move Up / Move Down  
- 🗑 Delete  
- 👁 Go to Page *(opens in new tab if linked to internal page)*

### 📐 Final Layout Order (Top to Bottom)

1. **Menu Group Selector Row**  
   `[Dropdown]` ➕ ✏️ 🗑

2. **Main Toolbar (Menu Controls)**  
   ➕ Add Item | ➕ Add Submenu | 🗑 Delete | ☰ Move | 💾 Save

3. **Menu Item Tree/List View** *(30% of screen width)*  
   - Drag-and-drop support  
   - Right-click actions

4. **Right Pane Editor**  
   - Displays menu item editing fields when item is selected.
   - At the bottom: **Edit Content** button  
     - If the item is not an external link  
     - Slides out from right to left  
     - Contains a **TinyMCE editor** for editing page HTML content
