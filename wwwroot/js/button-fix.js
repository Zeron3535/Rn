// Görseldeki gibi butonu çalışır hale getirmek için özel işlev
function fixPurpleRoundedButtons() {
    console.log("Fixing purple rounded buttons...");
    
    // Görseldeki butonları seç
    const roundedButtons = document.querySelectorAll('.purple-button-rounded');
    
    roundedButtons.forEach(button => {
        // Mevcut onclick işlevini koruyarak yeni dinleyiciler ekle
        const existingOnClick = button.getAttribute('onclick');
        
        // onclick olayını kaldır ve yerine addEventListener kullan
        button.removeAttribute('onclick');
        
        // Tıklama olayı
        button.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            
            // CSS efekti
            this.style.transform = 'scale(0.95)';
            this.style.boxShadow = '0 2px 5px rgba(0, 0, 0, 0.15)';
            
            // URL'yi al veya önceki onclick'ten çıkar
            let targetUrl = '/randevu';
            
            if (existingOnClick) {
                // onclick="window.location.href='/randevu'" türünden URL'yi çıkar
                const match = existingOnClick.match(/window\.location\.href\s*=\s*['"]([^'"]+)['"]/);
                if (match && match[1]) {
                    targetUrl = match[1];
                }
            }
            
            console.log("Navigating to:", targetUrl);
            
            // Gecikme ile yönlendir
            setTimeout(() => {
                window.location.href = targetUrl;
            }, 150);
        });
        
        // Dokunmatik olaylar
        button.addEventListener('touchend', function(e) {
            e.preventDefault();
            e.stopPropagation();
            
            // URL'yi al veya önceki onclick'ten çıkar
            let targetUrl = '/randevu';
            
            if (existingOnClick) {
                // onclick="window.location.href='/randevu'" türünden URL'yi çıkar
                const match = existingOnClick.match(/window\.location\.href\s*=\s*['"]([^'"]+)['"]/);
                if (match && match[1]) {
                    targetUrl = match[1];
                }
            }
            
            console.log("Touch navigating to:", targetUrl);
            
            // Gecikme ile yönlendir
            setTimeout(() => {
                window.location.href = targetUrl;
            }, 150);
        });
    });
}

// Özel buton işlevlerini düzeltme
(function() {
    console.log("Button fix loaded...");
    
    // Butonları düzeltme işlevini çağır
    function initAllButtonFixes() {
        // Görseldeki butonları düzelt
        fixPurpleRoundedButtons();
        
        // Diğer butonları düzelt
        setupCustomButtons();
    }
    
    // Özel butonlar için olay dinleyicileri ekle
    function setupCustomButtons() {
        console.log("Setting up custom buttons");
        
        // Tüm özel butonları seç
        const allButtons = document.querySelectorAll(
            'a.hero-purple-button, a.hero-white-button, a.hero-purple-button-services, ' + 
            'a.service-button, a.detail-button, a.category-link, ' + 
            'a.list-group-item-action, .btn, button'
        );
        
        // Her buton için yeni olay dinleyicileri ekle
        allButtons.forEach(button => {
            // Purple rounded butonlarını atla (özel işlem yapıldı)
            if (button.classList.contains('purple-button-rounded')) {
                return;
            }
            
            // Önce olay dinleyicilerini temizle
            const newBtn = button.cloneNode(true);
            if (button.parentNode) {
                button.parentNode.replaceChild(newBtn, button);
            }
            
            // Yeni olay dinleyicileri ekle
            const href = newBtn.getAttribute('href');
            if (href) {
                // Standart tıklama olayı
                newBtn.addEventListener('click', function(e) {
                    e.preventDefault();
                    console.log("Button clicked: ", href);
                    
                    // URL'yi kontrol et
                    if (href.startsWith('#')) {
                        // Sayfa içi bağlantı ise, sayfa içinde kaydır
                        const targetElement = document.querySelector(href);
                        if (targetElement) {
                            targetElement.scrollIntoView({behavior: 'smooth'});
                        }
                    } else {
                        // Harici bağlantı ise, sayfayı yönlendir
                        window.location.href = href;
                    }
                });
                
                // Mobil dokunma olayları
                newBtn.addEventListener('touchend', function(e) {
                    e.preventDefault();
                    console.log("Button touched: ", href);
                    
                    // URL'yi kontrol et
                    if (href.startsWith('#')) {
                        // Sayfa içi bağlantı ise, sayfa içinde kaydır
                        const targetElement = document.querySelector(href);
                        if (targetElement) {
                            targetElement.scrollIntoView({behavior: 'smooth'});
                        }
                    } else {
                        // Harici bağlantı ise, sayfayı yönlendir
                        window.location.href = href;
                    }
                });
            }
        });
    }
    
    // Sayfa yüklendiğinde butonları ayarla
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initAllButtonFixes);
    } else {
        initAllButtonFixes();
    }
    
    // Sayfa tamamen yüklendikten sonra da kontrol et (genellikle dinamik içerik yüklenince)
    window.addEventListener('load', initAllButtonFixes);
    
    // 1 saniye sonra tekrar kontrol et
    setTimeout(initAllButtonFixes, 1000);
    
    // 3 saniye sonra bir kez daha kontrol et (tam emin olmak için)
    setTimeout(initAllButtonFixes, 3000);
})(); 