// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function updateClock() {
    fetch('/Home/Clock')
        .then(response => response.text())
        .then(html => {
            const parser = new DOMParser();
            const doc = parser.parseFromString(html, 'text/html');
            const newClockGrid = doc.querySelector('.clock-grid');
            const currentClockGrid = document.querySelector('.clock-grid');
            if (currentClockGrid && newClockGrid) {
                currentClockGrid.innerHTML = newClockGrid.innerHTML;
            }
            console.log("Updated clock")
        })
        .catch(error => console.error('Error updating clock:', error));
}

document.addEventListener('DOMContentLoaded', function () {
    setInterval(updateClock, 10000);
});


