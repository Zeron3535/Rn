// Doğrudan sayfa yönlendirmesi yapan yardımcı fonksiyon
function redirectTo(url) {
    window.location.href = url;
}

// Tüm butonların tıklanabilir olmasını sağlayacak fonksiyon
function fixAllButtons() {
    // Tüm butonları ve bağlantıları seçelim
    const buttons = document.querySelectorAll('button, .btn, a.btn, .btn-glass');
    
    buttons.forEach(button => {
        // Önce var olan event listener'ları temizleyelim (optimization için)
        const newButton = button.cloneNode(true);
        if (button.parentNode) {
            button.parentNode.replaceChild(newButton, button);
        }
        
        // Butonun href özelliği var mı kontrol edelim
        const href = newButton.getAttribute('href');
        if (href) {
            // Eğer href varsa, tıklandığında yönlendirme işlemi yapalım
            newButton.addEventListener('click', function(e) {
                e.preventDefault();
                window.location.href = href;
            });
        }
        
        // onclick özelliği var mı kontrol edelim
        const onclickAttr = newButton.getAttribute('onclick');
        if (!onclickAttr && href) {
            // Eğer onclick özelliği yoksa ve href varsa, onclick özelliği ekleyelim
            newButton.setAttribute('onclick', `window.location.href='${href}'`);
        }
        
        // Tüm butonlara curser:pointer stilini ekleyelim
        newButton.style.cursor = 'pointer';
    });
}

// Main site JavaScript functionality

// Document ready function
document.addEventListener('DOMContentLoaded', function() {
    // Yeni özel butonları aktif et
    initHeroCustomButtons();
    
    // Tüm butonların çalışmasını sağla
    fixAllButtons();
    
    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Initialize popovers
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });

    // Add animated-link class to nav links
    document.querySelectorAll('.nav-link').forEach(link => {
        link.classList.add('animated-link');
    });

    // Navbar scroll effect
    initNavbarScroll();

    // Animate elements on scroll
    initScrollAnimations();

    // Appointment page functionality
    initAppointmentPage();
    
    // Improve mobile touch interactions
    improveMobileTouchInteractions();
    
    // Ensure hero section links work properly
    makeHeroLinksMoreResponsive();
    
    // Fix hero button click functionality
    fixHeroButtonClicks();
    
    // Add extra button listeners for the new hero buttons
    initHeroButtons();

    // Super simple direct link handling for critical buttons
    document.querySelectorAll('.btn').forEach(btn => {
        btn.addEventListener('click', function(e) {
            const href = this.getAttribute('href');
            if (href && href.startsWith('/')) {
                window.location.href = href;
            }
        });
    });
});

// Function to handle navigation directly
function navigateTo(url) {
    window.location.href = url;
    return false;
}

// Initialize the new hero buttons
function initHeroButtons() {
    // Get the buttons by ID
    const randevuButton = document.getElementById('randevuButton');
    const hizmetlerimButton = document.getElementById('hizmetlerimButton');
    const servicesRandevuButton = document.getElementById('servicesRandevuButton');
    const footerRandevuButton = document.getElementById('footerRandevuButton');
    const whatsappButton = document.getElementById('whatsappButton');
    const phoneButton = document.getElementById('phoneButton');
    
    // List of all important buttons
    const allImportantButtons = [
        randevuButton, 
        hizmetlerimButton, 
        servicesRandevuButton, 
        footerRandevuButton,
        whatsappButton,
        phoneButton
    ];
    
    // Add multiple event listeners to all important buttons
    allImportantButtons.forEach(button => {
        if (button) {
            // Standard click
            button.addEventListener('click', function(e) {
                const url = this.getAttribute('href');
                if (url) {
                    e.preventDefault();
                    navigateTo(url);
                }
            });
            
            // Touch events for mobile
            button.addEventListener('touchend', function(e) {
                e.preventDefault();
                const url = this.getAttribute('href');
                if (url) {
                    // Add a visible feedback that the button was pressed
                    this.style.opacity = '0.7';
                    setTimeout(() => {
                        this.style.opacity = '1';
                        navigateTo(url);
                    }, 150);
                }
            });
            
            // Mouse events for desktop hover effect
            button.addEventListener('mouseenter', function() {
                this.style.transform = 'translateY(-3px)';
                this.style.boxShadow = '0 6px 15px rgba(0, 0, 0, 0.2)';
            });
            
            button.addEventListener('mouseleave', function() {
                this.style.transform = '';
                this.style.boxShadow = '';
            });
        }
    });
    
    // Apply touch feedback effect
    document.querySelectorAll('.hero-button, .btn').forEach(btn => {
        btn.addEventListener('touchstart', function() {
            this.style.transform = 'translateY(1px)';
            this.style.boxShadow = '0 2px 5px rgba(0, 0, 0, 0.2)';
        }, { passive: true });
        
        btn.addEventListener('touchend', function() {
            this.style.transform = '';
            this.style.boxShadow = '';
        }, { passive: true });
    });
}

// Function to fix hero button click functionality
function fixHeroButtonClicks() {
    const heroButtons = document.querySelectorAll('.hero-section .btn');
    
    heroButtons.forEach(button => {
        // Clear existing event listeners
        const newButton = button.cloneNode(true);
        button.parentNode.replaceChild(newButton, button);
        
        // Add direct click handler
        newButton.addEventListener('click', function(e) {
            const href = this.getAttribute('href');
            if (href) {
                window.location.href = href;
            }
        });
        
        // Disable any default touch handlers that might be interfering
        newButton.addEventListener('touchstart', function(e) {
            // Mark as being touched
            this.classList.add('button-touched');
        }, { passive: true });
        
        newButton.addEventListener('touchend', function(e) {
            // If the button was touched, navigate
            if (this.classList.contains('button-touched')) {
                const href = this.getAttribute('href');
                if (href) {
                    e.preventDefault();
                    window.location.href = href;
                }
                this.classList.remove('button-touched');
            }
        });
    });
}

// Make hero section links more responsive to touch
function makeHeroLinksMoreResponsive() {
    // Get all hero section links
    const heroLinks = document.querySelectorAll('.hero-section a.btn');
    
    // Add touch events for better mobile responsiveness
    heroLinks.forEach(link => {
        // Prevent default on touchstart to avoid double tap issues
        link.addEventListener('touchstart', function(e) {
            this.classList.add('touched');
            this.style.transform = 'scale(0.98)';
        }, { passive: true });
        
        link.addEventListener('touchend', function(e) {
            this.classList.remove('touched');
            this.style.transform = 'scale(1)';
            
            // Navigate to the href after a small delay
            const href = this.getAttribute('href');
            if (href) {
                setTimeout(() => {
                    window.location.href = href;
                }, 50);
            }
        }, { passive: false });
        
        // Add hover effect using pointer-events for desktop
        link.addEventListener('mouseenter', function() {
            this.style.boxShadow = '0 0 15px rgba(255, 255, 255, 0.3)';
            this.style.transform = 'translateY(-2px)';
        });
        
        link.addEventListener('mouseleave', function() {
            this.style.boxShadow = '';
            this.style.transform = '';
        });
    });
    
    // Make navbar links more responsive too
    const navLinks = document.querySelectorAll('.navbar-nav .nav-link');
    navLinks.forEach(link => {
        link.style.cursor = 'pointer';
        
        // Add touch events
        link.addEventListener('touchstart', function() {
            this.style.opacity = '0.8';
        }, { passive: true });
        
        link.addEventListener('touchend', function() {
            this.style.opacity = '1';
        }, { passive: true });
    });
}

// Initialize navbar scroll effect
function initNavbarScroll() {
    const navbar = document.querySelector('.navbar');
    if (!navbar) return;

    window.addEventListener('scroll', function() {
        if (window.scrollY > 50) {
            navbar.classList.add('scrolled');
        } else {
            navbar.classList.remove('scrolled');
        }
    });
}

// Initialize scroll animations
function initScrollAnimations() {
    // Add fade-in animation to elements with data-animate attribute
    const animatedElements = document.querySelectorAll('[data-animate]');

    if (animatedElements.length === 0) return;

    // Create intersection observer
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const el = entry.target;
                const animation = el.dataset.animate || 'fadeIn';
                const delay = el.dataset.delay || 0;

                setTimeout(() => {
                    el.classList.add(animation);
                    el.classList.add('animated');
                }, delay);

                // Unobserve after animation
                observer.unobserve(el);
            }
        });
    }, {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    });

    // Observe all animated elements
    animatedElements.forEach(el => {
        observer.observe(el);
    });

    // Add animation classes to service cards if not already animated with Animate.css
    document.querySelectorAll('.service-card:not(.animate__animated)').forEach((card, index) => {
        card.dataset.animate = 'fadeInUp';
        card.dataset.delay = 100 * index;
        observer.observe(card);
    });

    // Add animation classes to testimonial cards if not already animated with Animate.css
    document.querySelectorAll('.testimonial-card:not(.animate__animated)').forEach((card, index) => {
        card.dataset.animate = 'fadeInUp';
        card.dataset.delay = 100 * index;
        observer.observe(card);
    });

    // Add parallax effect to elements with parallax class
    window.addEventListener('scroll', function() {
        document.querySelectorAll('.parallax').forEach(element => {
            const speed = element.dataset.speed || 0.5;
            const yPos = -(window.scrollY * speed);
            element.style.transform = `translateY(${yPos}px)`;
        });
    });

    // Add tilt effect to cards with tilt class
    document.querySelectorAll('.tilt').forEach(card => {
        card.addEventListener('mousemove', function(e) {
            const cardRect = card.getBoundingClientRect();
            const cardCenterX = cardRect.left + cardRect.width / 2;
            const cardCenterY = cardRect.top + cardRect.height / 2;
            const angleY = (e.clientX - cardCenterX) / 10;
            const angleX = (cardCenterY - e.clientY) / 10;

            card.style.transform = `perspective(1000px) rotateX(${angleX}deg) rotateY(${angleY}deg)`;
        });

        card.addEventListener('mouseleave', function() {
            card.style.transform = 'perspective(1000px) rotateX(0) rotateY(0)';
        });
    });

    // Add glass effect to elements when they come into view
    const glassObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('glass-visible');
                glassObserver.unobserve(entry.target);
            }
        });
    }, {
        threshold: 0.3
    });

    document.querySelectorAll('.card-glass, .btn-glass').forEach(element => {
        glassObserver.observe(element);
    });
}

// Initialize appointment page functionality
function initAppointmentPage() {
    const appointmentForm = document.getElementById('appointmentForm');
    if (!appointmentForm) return;

    const dateInput = document.getElementById('appointmentDate');
    const serviceCheckboxes = document.querySelectorAll('.service-checkbox');
    const timeSlotsContainer = document.getElementById('timeSlots');
    const selectedTimeInput = document.getElementById('startTime');

    // Date change handler
    if (dateInput) {
        dateInput.addEventListener('change', function() {
            window.updateTimeSlots();
        });
    }

    // Service selection handler
    if (serviceCheckboxes.length > 0) {
        serviceCheckboxes.forEach(checkbox => {
            checkbox.addEventListener('change', function() {
                window.updateTimeSlots();
            });
        });
    }

    // Update time slots based on selected date and services
    // Make it globally accessible
    window.updateTimeSlots = function() {
        // Re-get elements to ensure we have the latest references
        const dateInput = document.getElementById('appointmentDate');
        const serviceCheckboxes = document.querySelectorAll('.service-checkbox');
        const timeSlotsContainer = document.getElementById('timeSlots');
        const selectedTimeInput = document.getElementById('startTime');

        if (!dateInput || !timeSlotsContainer) return;

        // Clear any previously selected time
        if (selectedTimeInput) {
            selectedTimeInput.value = '';
        }

        const selectedDate = dateInput.value;
        if (!selectedDate) {
            timeSlotsContainer.innerHTML = '<div class="alert alert-info">Lütfen önce bir tarih seçin.</div>';
            return;
        }

        // Get selected services
        const selectedServices = [];
        serviceCheckboxes.forEach(checkbox => {
            if (checkbox.checked) {
                selectedServices.push(parseInt(checkbox.value));
            }
        });

        if (selectedServices.length === 0) {
            timeSlotsContainer.innerHTML = '<div class="alert alert-info">Lütfen en az bir hizmet seçin.</div>';
            return;
        }

        // Check if the selected date is available for booking
        fetch(`/Appointments/IsDayAvailable?date=${selectedDate}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Sunucu yanıt vermiyor. Lütfen daha sonra tekrar deneyin.');
                }
                return response.json();
            })
            .then(data => {
                if (!data.isAvailable) {
                    timeSlotsContainer.innerHTML = '<div class="alert alert-warning">Seçilen tarihte salon kapalıdır. Lütfen başka bir tarih seçin.</div>';
                    return;
                }

                // Fetch available time slots
                timeSlotsContainer.innerHTML = '<div class="text-center p-4"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Yükleniyor...</span></div><p class="mt-2">Randevu saatleri yükleniyor...</p></div>';

                const params = new URLSearchParams();
                params.append('date', selectedDate);
                selectedServices.forEach(id => params.append('serviceIds', id));

                fetch(`/Appointments/GetAvailableTimeSlots?${params.toString()}`)
                    .then(response => {
                        if (!response.ok) {
                            throw new Error('Randevu saatleri yüklenirken bir hata oluştu.');
                        }
                        return response.json();
                    })
                    .then(timeSlots => {
                        if (!timeSlots || timeSlots.length === 0) {
                            timeSlotsContainer.innerHTML = '<div class="alert alert-warning">Seçilen tarihte uygun randevu saati bulunmamaktadır. Lütfen başka bir tarih seçin.</div>';
                            return;
                        }

                        let html = '<div class="mb-3"><label class="form-label fw-bold">Randevu Saati</label>' +
                                  '<p class="text-muted small mb-3">Lütfen size uygun bir randevu saati seçin. (Seçilen hizmetlere göre uygun saatler gösterilmektedir)</p>' +
                                  '<div class="time-slots-wrapper d-flex flex-wrap gap-2">';
                        
                        // Group time slots by availability
                        const availableSlots = timeSlots.filter(slot => slot.isAvailable);
                        const unavailableSlots = timeSlots.filter(slot => !slot.isAvailable);

                        if (availableSlots.length === 0) {
                            timeSlotsContainer.innerHTML = '<div class="alert alert-warning">Seçilen tarihte uygun randevu saati bulunmamaktadır. Lütfen başka bir tarih seçin.</div>';
                            return;
                        }

                        availableSlots.forEach(slot => {
                            const startTime = slot.startTime.substring(0, 5);
                            const endTime = slot.endTime.substring(0, 5);
                            
                            html += `<div class="time-slot available" data-time="${startTime}" title="Bitiş: ${endTime}">${startTime}</div>`;
                        });

                        if (unavailableSlots.length > 0) {
                            unavailableSlots.forEach(slot => {
                                const startTime = slot.startTime.substring(0, 5);
                                const endTime = slot.endTime.substring(0, 5);
                                
                                html += `<div class="time-slot unavailable" title="Dolu: ${startTime} - ${endTime}">${startTime}</div>`;
                            });
                        }

                        html += '</div></div>';
                        
                        html += `
                        <div class="time-slot-details mt-3 mb-3 d-none" id="timeSlotDetails">
                            <div class="card bg-light">
                                <div class="card-body">
                                    <h6 class="card-title">Randevu Bilgileri</h6>
                                    <div class="row">
                                        <div class="col-md-6">
                                            <p class="mb-1"><strong>Başlangıç:</strong> <span id="displayStartTime"></span></p>
                                        </div>
                                        <div class="col-md-6">
                                            <p class="mb-1"><strong>Bitiş:</strong> <span id="displayEndTime"></span></p>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        `;

                        timeSlotsContainer.innerHTML = html;

                        // Add click event to time slots
                        document.querySelectorAll('.time-slot.available').forEach(slot => {
                            slot.addEventListener('click', function() {
                                document.querySelectorAll('.time-slot').forEach(s => s.classList.remove('selected'));
                                this.classList.add('selected');
                                
                                const startTime = this.getAttribute('data-time');
                                selectedTimeInput.value = startTime;
                                
                                // Find the corresponding time slot to get the end time
                                const selectedSlot = availableSlots.find(s => s.startTime.substring(0, 5) === startTime);
                                if (selectedSlot) {
                                    const endTime = selectedSlot.endTime.substring(0, 5);
                                    document.getElementById('displayStartTime').textContent = startTime;
                                    document.getElementById('displayEndTime').textContent = endTime;
                                    document.getElementById('timeSlotDetails').classList.remove('d-none');
                                }
                            });
                        });
                    })
                    .catch(error => {
                        console.error('Error fetching time slots:', error);
                        timeSlotsContainer.innerHTML = `<div class="alert alert-danger">Randevu saatleri yüklenirken bir hata oluştu: ${error.message}</div>`;
                    });
            })
            .catch(error => {
                console.error('Error checking day availability:', error);
                timeSlotsContainer.innerHTML = `<div class="alert alert-danger">Tarih kontrolü yapılırken bir hata oluştu: ${error.message}</div>`;
            });
    }

    // Form validation
    if (appointmentForm) {
        appointmentForm.addEventListener('submit', function(event) {
            if (!validateAppointmentForm()) {
                event.preventDefault();
            }
        });
    }

    // Validate appointment form
    function validateAppointmentForm() {
        let isValid = true;

        // Re-get elements to ensure we have the latest references
        const serviceCheckboxes = document.querySelectorAll('.service-checkbox');
        const selectedTimeInput = document.getElementById('startTime');

        // Check if at least one service is selected
        let serviceSelected = false;
        serviceCheckboxes.forEach(checkbox => {
            if (checkbox.checked) {
                serviceSelected = true;
            }
        });

        if (!serviceSelected) {
            isValid = false;
            alert('Lütfen en az bir hizmet seçin.');
        }

        // Check if time is selected
        if (selectedTimeInput && !selectedTimeInput.value) {
            isValid = false;
            alert('Lütfen bir randevu saati seçin.');
        }

        return isValid;
    }
}

// Improve mobile touch interactions
function improveMobileTouchInteractions() {
    // Add active class on touch for all buttons
    const buttons = document.querySelectorAll('.btn, .btn-glass, .btn-primary, .btn-outline-light');
    
    buttons.forEach(button => {
        button.addEventListener('touchstart', function() {
            this.classList.add('active');
        }, { passive: true });
        
        button.addEventListener('touchend', function() {
            this.classList.remove('active');
        }, { passive: true });
        
        button.addEventListener('touchcancel', function() {
            this.classList.remove('active');
        }, { passive: true });
    });
}

// Add CSS for touched buttons to make them visually responsive
document.head.insertAdjacentHTML('beforeend', `
<style>
.btn.touched, .btn.active {
    opacity: 0.8;
    transform: scale(0.98);
}
.hero-section .btn {
    transition: all 0.2s ease;
    border-radius: 8px;
}
</style>
`);

// Ana sayfa butonları yeniden oluşturuluyor
function initHeroCustomButtons() {
    const allCustomButtons = document.querySelectorAll('.hero-purple-button, .hero-white-button, .service-button, .detail-button');
    
    allCustomButtons.forEach(button => {
        // Standart tıklama
        button.addEventListener('click', function(e) {
            e.preventDefault();
            const href = this.getAttribute('href');
            if (href) {
                // Tıklama animasyonu
                this.style.transform = 'scale(0.98)';
                this.style.boxShadow = '0 2px 5px rgba(0, 0, 0, 0.15)';
                
                // Kısa gecikme sonrası yönlendirme
                setTimeout(() => {
                    window.location.href = href;
                }, 150);
            }
        });
        
        // Mobil dokunma olayları
        button.addEventListener('touchstart', function() {
            this.style.transform = 'scale(0.98)';
            this.style.boxShadow = '0 2px 5px rgba(0, 0, 0, 0.15)';
        }, { passive: true });
        
        button.addEventListener('touchend', function() {
            const href = this.getAttribute('href');
            if (href) {
                setTimeout(() => {
                    window.location.href = href;
                }, 150);
            }
        }, { passive: false });
    });
}
