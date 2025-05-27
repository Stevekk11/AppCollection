// Map radio button IDs to Bootstrap navbar color classes

document.addEventListener("DOMContentLoaded", function() {
    const colorMap = {
        colorPrimary: "bg-primary navbar-primary",
        colorSecondary: "bg-secondary navbar-secondary",
        colorSuccess: "bg-success navbar-success",
        colorDanger: "bg-danger navbar-danger",
        colorWarning: "bg-warning navbar-warning",
        colorInfo: "bg-info navbar-info",
        colorDark: "bg-dark navbar-dark",
        colorWhite: "bg-white",
    };

// Find the navbar
    const navbar = document.getElementById('mainNavbar');

// Listen for radio button changes
    document.getElementById('ribbonColorRadios').addEventListener('change', function (e) {
        if (e.target.name === "ribbonColor") {
            // Remove all bg-* classes
            navbar.className = navbar.className.replace(/\bbg-\w+\b/g, '');
            // Add the selected color class
            navbar.classList.add(...colorMap[e.target.id].split(' '));
        }
    });

    const savedColor = localStorage.getItem('navbarRibbonColor');
    if (savedColor && colorMap[savedColor]) {
        navbar.className = navbar.className.replace(/\bbg-\w+\b/g, '');
        navbar.classList.add(...colorMap[savedColor].split(' '));
        document.getElementById(savedColor).checked = true;
    }

    document.getElementById('ribbonColorRadios').addEventListener('change', function(e) {
        if (e.target.name === "ribbonColor") {
            navbar.className = navbar.className.replace(/\bbg-\w+\b/g, '');
            navbar.classList.add(...colorMap[e.target.id].split(' '));
            localStorage.setItem('navbarRibbonColor', e.target.id);
        }
    });
})

