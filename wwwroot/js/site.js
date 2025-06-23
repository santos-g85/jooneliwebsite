document.addEventListener('DOMContentLoaded', function () {
    const navbarContainer = document.querySelector('.navbar-container');
    const solutionsDropdown = document.querySelector('.solutions-dropdown');
    const megaDropdownContainer = document.querySelector('.mega-dropdown-container');
    const navLinks = document.querySelectorAll('.nav-link-custom');

    let isHoveringNavLink = false;
    let isHoveringDropdown = false;
    let timeout;

    // Function to show navbar background and change colors
    function showNavbarBackground() {
        navbarContainer.classList.add('animate-bg');
    }

    // Function to hide navbar background and reset colors
    function hideNavbarBackground() {
        if (!isHoveringNavLink && !isHoveringDropdown) {
            navbarContainer.classList.remove('animate-bg');
        }
    }

    // Function to show mega dropdown
    function showMegaDropdown() {
        clearTimeout(timeout);
        megaDropdownContainer.classList.add('show');
        showNavbarBackground();
        isHoveringDropdown = true;
    }

    // Function to hide mega dropdown
    function hideMegaDropdown() {
        timeout = setTimeout(() => {
            if (!isHoveringDropdown && !isHoveringNavLink) {
                megaDropdownContainer.classList.remove('show');
                hideNavbarBackground();
            }
        }, 200);
    }

    // Solutions dropdown hover events
    if (solutionsDropdown && megaDropdownContainer) {
        solutionsDropdown.addEventListener('mouseenter', function () {
            showMegaDropdown();
            solutionsDropdown.classList.add('show');
        });

        solutionsDropdown.addEventListener('mouseleave', function () {
            isHoveringDropdown = false;
            solutionsDropdown.classList.remove('show');
            hideMegaDropdown();
        });

        // Mega dropdown hover events
        megaDropdownContainer.addEventListener('mouseenter', function () {
            clearTimeout(timeout);
            isHoveringDropdown = true;
            showNavbarBackground();
        });

        megaDropdownContainer.addEventListener('mouseleave', function () {
            isHoveringDropdown = false;
            hideMegaDropdown();
        });
    }

    // Regular nav links hover events
    navLinks.forEach(link => {
        // Skip the solutions dropdown link as it's handled separately
        if (link.closest('.solutions-dropdown')) {
            return;
        }

        link.addEventListener('mouseenter', function () {
            isHoveringNavLink = true;
            showNavbarBackground();

            setTimeout(() => {
                // Hide mega dropdown when hovering over other nav links
                megaDropdownContainer.classList.remove('show');
                solutionsDropdown.classList.remove('show');
                isHoveringDropdown = false;
            }, 200);
        });

        link.addEventListener('mouseleave', function () {
            isHoveringNavLink = false;
            setTimeout(() => {
                if (!isHoveringDropdown && !isHoveringNavLink) {
                    hideNavbarBackground();
                }
            }, 200);
        });
    });

    // Handle solution cards hover effects
    const solutionCards = document.querySelectorAll('.solution-card');
    solutionCards.forEach(card => {
        card.addEventListener('mouseenter', function () {
            this.style.transform = 'translateY(0) scale(1.02)';
        });

        card.addEventListener('mouseleave', function () {
            this.style.transform = 'translateY(0) scale(1)';
        });
    });

    // Smooth scroll for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });

    // Close mobile menu when clicking on a link
    const mobileMenuLinks = document.querySelectorAll('#mobileMenu .nav-link');
    const mobileMenu = document.getElementById('mobileMenu');

    mobileMenuLinks.forEach(link => {
        link.addEventListener('click', function () {
            if (mobileMenu) {
                const bsOffcanvas = bootstrap.Offcanvas.getInstance(mobileMenu);
                if (bsOffcanvas) {
                    bsOffcanvas.hide();
                }
            }
        });
    });

    // Handle window resize
    window.addEventListener('resize', function () {
        if (window.innerWidth >= 992) {
            // Close mobile menu if open on desktop
            if (mobileMenu) {
                const bsOffcanvas = bootstrap.Offcanvas.getInstance(mobileMenu);
                if (bsOffcanvas) {
                    bsOffcanvas.hide();
                }
            }
        }
    });

    // Intersection Observer for scroll effects (optional)
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-in');
            }
        });
    }, observerOptions);

    // Observe elements for scroll animations
    document.querySelectorAll('.solution-card, .cta-section').forEach(el => {
        observer.observe(el);
    });
});



