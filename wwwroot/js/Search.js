// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const searchFocus = document.getElementById('search-focus');
const keys = [
    { keyCode: 'AltLeft', isTriggered: false },
    { keyCode: 'ControlLeft', isTriggered: false },
];

window.addEventListener('keydown', (e) => {
    keys.forEach((obj) => {
        if (obj.keyCode === e.code) {
            obj.isTriggered = true;
        }
    });

    const shortcutTriggered = keys.filter((obj) => obj.isTriggered).length === keys.length;

    if (shortcutTriggered) {
        searchFocus.focus();
    }
});

window.addEventListener('keyup', (e) => {
    keys.forEach((obj) => {
        if (obj.keyCode === e.code) {
            obj.isTriggered = false;
        }
    });
});