// js/scms-menu.js

document.addEventListener("DOMContentLoaded", function () {
    const items = document.querySelectorAll(".scms-item");

    items.forEach(item => {
        const submenu = item.querySelector(".scms-submenu");
        if (submenu) {
            item.addEventListener("mouseenter", () => submenu.style.display = "block");
            item.addEventListener("mouseleave", () => submenu.style.display = "none");

            // Optional: add touch support for mobile
            item.addEventListener("click", e => {
                const isTouch = 'ontouchstart' in window || navigator.maxTouchPoints > 0;
                if (isTouch) {
                    e.stopPropagation();
                    submenu.style.display = submenu.style.display === "block" ? "none" : "block";
                }
            });
        }
    });
});
