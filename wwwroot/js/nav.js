document.addEventListener('DOMContentLoaded', function () {
    const navbarContainer = document.querySelector('.navbar-container');
    const navbarBgOverlay = document.querySelector('.navbar-bg-overlay');
    const solutionsDropdowns = document.querySelectorAll('.solutions-dropdown');
    const megaDropdownContainer = document.querySelector('.mega-dropdown-container');
    const navLinks = document.querySelectorAll('.nav-link-custom');

    let isHoveringNavLink = false;
    let isHoveringDropdown = false;
    let activeDropdown = null;
    let timeout;

    // Heights for different states
    const NAVBAR_HEIGHT = 80; // Height of navbar in pixels
    const MEGADROPDOWN_HEIGHT = 400; // Height of mega dropdown in pixels

  
    // Function to set background height
    function setBackgroundHeight(height) {
        navbarBgOverlay.style.height = `${height}px`;
    }

    // Function to activate navbar background
    function activateNavbarBackground() {
        navbarContainer.classList.add('animate-bg');
        setBackgroundHeight(NAVBAR_HEIGHT);
    }

    // Function to extend background for mega dropdown
    function extendBackgroundForDropdown() {
        setBackgroundHeight(NAVBAR_HEIGHT + MEGADROPDOWN_HEIGHT);
    }

    // Function to deactivate navbar background
    function deactivateNavbarBackground() {
        if (!isHoveringNavLink && !isHoveringDropdown) {
            navbarContainer.classList.remove('animate-bg');
            setBackgroundHeight(0);
        }
    }

    // Function to show mega dropdown
    function showMegaDropdown(dropdown) {
        clearTimeout(timeout);
        activateNavbarBackground();
        isHoveringDropdown = true;
        activeDropdown = dropdown;
        setTimeout(() => {
            if (isHoveringDropdown) {
                extendBackgroundForDropdown();
                megaDropdownContainer.classList.add('show');
            }
        }, 50);
    }

    // Function to hide mega dropdown
    //function hideMegaDropdown() {
    //    isHoveringDropdown = false;
    //    activeDropdown = null;
    //    megaDropdownContainer.classList.remove('show');

    //    // Return to normal navbar height before hiding completely
    //    setBackgroundHeight(NAVBAR_HEIGHT);

    //    timeout = setTimeout(() => {
    //        if (!isHoveringDropdown && !isHoveringNavLink) {
    //            deactivateNavbarBackground();
    //        }
    //    }, 200);
    //}
    function hideMegaDropdown() {
        isHoveringDropdown = false;
        activeDropdown = null;

        timeout = setTimeout(() => {
            if (!isHoveringDropdown && !isHoveringNavLink) {
                navbarContainer.classList.remove('animate-bg');
                megaDropdownContainer.classList.remove('show');
                setBackgroundHeight(0);
            }
        }, 100); // Short delay to allow for fast hover re-entries
    }

    // Setup dropdown hover events
    solutionsDropdowns.forEach(dropdown => {
        dropdown.addEventListener('mouseenter', function () {
            isHoveringDropdown = true;
            showMegaDropdown(this);
            this.classList.add('active');
        });

        dropdown.addEventListener('mouseleave', function () {
            this.classList.remove('active');

            // Delay setting flag false to allow hover into megaDropdown
            setTimeout(() => {
                if (!megaDropdownContainer.matches(':hover')) {
                    isHoveringDropdown = false;
                    if (activeDropdown === this) {
                        hideMegaDropdown();
                    }
                }
            }, 100);
        });
    });

    // Mega dropdown container events
    if (megaDropdownContainer) {
        megaDropdownContainer.addEventListener('mouseenter', function () {
            clearTimeout(timeout);
            isHoveringDropdown = true;
            activateNavbarBackground();
            extendBackgroundForDropdown();
        });

        megaDropdownContainer.addEventListener('mouseleave', function () {
            // Delay helps prevent flicker when moving quickly between elements
            setTimeout(() => {
                if (!document.querySelector('.solutions-dropdown:hover')) {
                    isHoveringDropdown = false;
                    hideMegaDropdown();
                }
            }, 100);
        });
    }

    // Regular nav links hover events
    navLinks.forEach(link => {
        // Skip dropdown links
        if (link.closest('.solutions-dropdown')) return;

        link.addEventListener('mouseenter', function () {
            isHoveringNavLink = true;
            activateNavbarBackground();

            // Hide mega dropdown when hovering over other nav links
            megaDropdownContainer.classList.remove('show');
            solutionsDropdowns.forEach(d => d.classList.remove('active'));
            isHoveringDropdown = false;
        });

        link.addEventListener('mouseleave', function () {
            isHoveringNavLink = false;
            setTimeout(() => {
                if (!isHoveringDropdown && !isHoveringNavLink) {
                    deactivateNavbarBackground();
                }
            }, 200);
        });
    });

    // Handle window resize
    window.addEventListener('resize', function () {
        // Close mega dropdown on mobile
        if (window.innerWidth < 992) {
            megaDropdownContainer.classList.remove('show');
            solutionsDropdowns.forEach(d => d.classList.remove('active'));
            isHoveringDropdown = false;
            deactivateNavbarBackground();
        }
    });
});