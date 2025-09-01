document.addEventListener('DOMContentLoaded', function () {
    // Initialize CoreUI sidebar
    const sidebarElement = document.querySelector('.sidebar-nav[data-coreui="navigation"]');
    if (sidebarElement) {
        new CoreUI.Sidebar(sidebarElement);
    } else {
        console.error('Sidebar element with data-coreui="navigation" not found.');
    }
});