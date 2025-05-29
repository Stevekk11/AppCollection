document.addEventListener("DOMContentLoaded", function() {
    const colorMap = {
        colorPrimary: "bg-primary navbar-dark",
        colorSecondary: "bg-secondary navbar-dark",
        colorSuccess: "bg-success navbar-dark",
        colorDanger: "bg-danger navbar-dark",
        colorWarning: "bg-warning navbar-dark",
        colorInfo: "bg-info navbar-dark",
        colorDark: "bg-dark navbar-dark",
        colorWhite: "bg-white navbar-light",
    };

    const navbar = document.getElementById('mainNavbar');
    const ribbonColorRadios = document.getElementById('ribbonColorRadios');


    const savedColor = localStorage.getItem('navbarRibbonColor');
    if (navbar && savedColor && colorMap[savedColor]) {
        navbar.className = navbar.className.replace(/\bbg-\w+\b/g, '');
        navbar.className = navbar.className.replace(/\bnavbar-(light|dark)\b/g, '');
        navbar.classList.add(...colorMap[savedColor].split(' '));

        if (ribbonColorRadios && document.getElementById(savedColor)) {
            document.getElementById(savedColor).checked = true;
        }
    }

    if (ribbonColorRadios) {
        ribbonColorRadios.addEventListener('change', function(e) {
            if (e.target.name === "ribbonColor") {
                // Remove old classes
                navbar.className = navbar.className.replace(/\bbg-\w+\b/g, '');
                navbar.className = navbar.className.replace(/\bnavbar-(light|dark)\b/g, '');
                // Add new classes
                navbar.classList.add(...colorMap[e.target.id].split(' '));
                localStorage.setItem('navbarRibbonColor', e.target.id);
            }
        });
    }
});
