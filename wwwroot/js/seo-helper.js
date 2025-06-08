/**
 * SEO Helper for Tina Kuaför
 * This script adds additional structured data to the page based on the page's type
 * and implements hidden SEO techniques to improve search engine rankings
 */
document.addEventListener('DOMContentLoaded', function() {
    // Add service-specific structured data
    const pageTitle = document.title;
    const currentPath = window.location.pathname;

    // Check if this is a service details page
    if (currentPath.indexOf('/hizmet/') === 0) {
        const serviceName = document.querySelector('h1')?.innerText;
        const servicePrice = document.querySelector('.service-price')?.innerText;

        if (serviceName && servicePrice) {
            // Create service-specific structured data
            const serviceData = {
                "@context": "https://schema.org",
                "@type": "Service",
                "name": serviceName,
                "offers": {
                    "@type": "Offer",
                    "price": servicePrice.replace(/[^0-9]/g, ''),
                    "priceCurrency": "TRY"
                },
                "provider": {
                    "@type": "HairSalon",
                    "name": "Tina Bayan Kuaför Güzellik Salonu",
                    "url": "https://kuafortina.com"
                }
            };

            // Add structured data to page
            const script = document.createElement('script');
            script.type = 'application/ld+json';
            script.text = JSON.stringify(serviceData);
            document.head.appendChild(script);
        }
    }

    // Add page-specific schema based on URL pattern
    function addPageSpecificSchema() {
        if (currentPath === '/hakkimda' || currentPath === '/about') {
            const aboutData = {
                "@context": "https://schema.org",
                "@type": "AboutPage",
                "name": pageTitle,
                "description": document.querySelector('meta[name="description"]')?.content
            };

            const script = document.createElement('script');
            script.type = 'application/ld+json';
            script.text = JSON.stringify(aboutData);
            document.head.appendChild(script);
        }

        if (currentPath === '/iletisim' || currentPath === '/contact') {
            const contactData = {
                "@context": "https://schema.org",
                "@type": "ContactPage",
                "name": pageTitle,
                "description": document.querySelector('meta[name="description"]')?.content
            };

            const script = document.createElement('script');
            script.type = 'application/ld+json';
            script.text = JSON.stringify(contactData);
            document.head.appendChild(script);
        }
    }

    // Add enhanced local business schema with additional service offerings
    function addEnhancedLocalBusinessSchema() {
        // Create enhanced local business schema with additional service offerings
        const enhancedBusinessData = {
            "@context": "https://schema.org",
            "@type": "HairSalon",
            "name": "Tina Bayan Kuaför Güzellik Salonu - İzmir Balçova",
            "alternateName": "İzmir Balçova Kuaför ve Güzellik Merkezi",
            "url": "https://tinakuafor.com",
            "image": ["https://i.hizliresim.com/9lwenuc.png", "https://i.hizliresim.com/frlk5l2.jpg"],
            "description": "İzmir Balçova'nın en iyi kuaförü ve güzellik salonu. Profesyonel saç kesimi, saç boyama, kaş alımı, manikür, pedikür ve diğer güzellik hizmetleri sunan Tina Bayan Kuaför'de size özel hizmet alın.",
            "address": {
                "@type": "PostalAddress",
                "streetAddress": "Onur, Salkım Sk. 9/A",
                "addressLocality": "Balçova",
                "addressRegion": "İzmir",
                "postalCode": "35330",
                "addressCountry": "TR"
            },
            "geo": {
                "@type": "GeoCoordinates",
                "latitude": 38.391642271836524,
                "longitude": 27.051524376652498
            },
            "telephone": "+905375890642",
            "priceRange": "$$",
            "keywords": "as kuaför, balcova kuafor, balçova güzellik merkezi, balçova güzellik merkezleri, balçova güzellik salonu, balçova kaş alımı, balçova kuaför, balçova tırnak, izmir balçova güzellik merkezleri, izmir balçova kuaför, izmir kaş alımı, kaş alımı, kuafor en çok oy alanlar, kuaför, kuaför balçova, narlıdere güzellik salonları, pedikür",
            "hasOfferCatalog": {
                "@type": "OfferCatalog",
                "name": "Kuaför ve Güzellik Hizmetleri",
                "itemListElement": [
                    {
                        "@type": "Offer",
                        "itemOffered": {
                            "@type": "Service",
                            "name": "Saç Kesimi",
                            "description": "İzmir Balçova'da profesyonel saç kesimi"
                        }
                    },
                    {
                        "@type": "Offer",
                        "itemOffered": {
                            "@type": "Service",
                            "name": "Saç Boyama",
                            "description": "İzmir Balçova'da profesyonel saç boyama"
                        }
                    },
                    {
                        "@type": "Offer",
                        "itemOffered": {
                            "@type": "Service",
                            "name": "Kaş Alımı",
                            "description": "İzmir Balçova'da profesyonel kaş alımı"
                        }
                    },
                    {
                        "@type": "Offer",
                        "itemOffered": {
                            "@type": "Service",
                            "name": "Manikür",
                            "description": "İzmir Balçova'da profesyonel manikür"
                        }
                    },
                    {
                        "@type": "Offer",
                        "itemOffered": {
                            "@type": "Service",
                            "name": "Pedikür",
                            "description": "İzmir Balçova'da profesyonel pedikür"
                        }
                    },
                    {
                        "@type": "Offer",
                        "itemOffered": {
                            "@type": "Service",
                            "name": "Tırnak Bakımı",
                            "description": "İzmir Balçova'da profesyonel tırnak bakımı"
                        }
                    },
                    {
                        "@type": "Offer",
                        "itemOffered": {
                            "@type": "Service",
                            "name": "Güzellik Hizmetleri",
                            "description": "İzmir Balçova'da profesyonel güzellik hizmetleri"
                        }
                    }
                ]
            },
            "areaServed": [
                {
                    "@type": "City",
                    "name": "İzmir"
                },
                {
                    "@type": "City",
                    "name": "Balçova"
                },
                {
                    "@type": "City",
                    "name": "Narlıdere"
                }
            ],
            "serviceArea": {
                "@type": "GeoCircle",
                "geoMidpoint": {
                    "@type": "GeoCoordinates",
                    "latitude": 38.391642271836524,
                    "longitude": 27.051524376652498
                },
                "geoRadius": "10000"
            }
        };

        // Add structured data to page
        const script = document.createElement('script');
        script.type = 'application/ld+json';
        script.text = JSON.stringify(enhancedBusinessData);
        document.head.appendChild(script);
    }

    // Add hidden semantic HTML elements with targeted keywords
    function addHiddenSemanticElements() {
        // Create a container for hidden semantic elements
        const hiddenContainer = document.createElement('div');
        hiddenContainer.style.position = 'absolute';
        hiddenContainer.style.width = '1px';
        hiddenContainer.style.height = '1px';
        hiddenContainer.style.overflow = 'hidden';
        hiddenContainer.style.clip = 'rect(0 0 0 0)';
        hiddenContainer.setAttribute('aria-hidden', 'true');
        hiddenContainer.setAttribute('tabindex', '-1');

        // Add semantic elements with targeted keywords
        hiddenContainer.innerHTML = `
            <section itemscope itemtype="https://schema.org/HairSalon">
                <h2 itemprop="name">Tina Bayan Kuaför - İzmir Balçova'nın En İyi Kuaförü</h2>
                <p itemprop="description">İzmir Balçova'da profesyonel kadın kuaförü ve güzellik salonu. Saç kesimi, boyama, bakım, kaş alımı, manikür, pedikür ve daha fazlası için Tina Kuaför'e bekleriz.</p>
                <ul>
                    <li><span itemprop="keywords">as kuaför</span></li>
                    <li><span itemprop="keywords">balcova kuafor</span></li>
                    <li><span itemprop="keywords">balçova güzellik merkezi</span></li>
                    <li><span itemprop="keywords">balçova güzellik merkezleri</span></li>
                    <li><span itemprop="keywords">balçova güzellik salonu</span></li>
                    <li><span itemprop="keywords">balçova kaş alımı</span></li>
                    <li><span itemprop="keywords">balçova kuaför</span></li>
                    <li><span itemprop="keywords">balçova tırnak</span></li>
                    <li><span itemprop="keywords">izmir balçova güzellik merkezleri</span></li>
                    <li><span itemprop="keywords">izmir balçova kuaför</span></li>
                    <li><span itemprop="keywords">izmir kaş alımı</span></li>
                    <li><span itemprop="keywords">kaş alımı</span></li>
                    <li><span itemprop="keywords">kuafor en çok oy alanlar</span></li>
                    <li><span itemprop="keywords">kuaför</span></li>
                    <li><span itemprop="keywords">kuaför balçova</span></li>
                    <li><span itemprop="keywords">narlıdere güzellik salonları</span></li>
                    <li><span itemprop="keywords">pedikür</span></li>
                </ul>
                <div itemprop="address" itemscope itemtype="https://schema.org/PostalAddress">
                    <span itemprop="streetAddress">Onur, Salkım Sk. 9/A</span>,
                    <span itemprop="addressLocality">Balçova</span>,
                    <span itemprop="addressRegion">İzmir</span>,
                    <span itemprop="postalCode">35330</span>,
                    <span itemprop="addressCountry">Türkiye</span>
                </div>
            </section>
        `;

        // Add the hidden container to the page
        document.body.appendChild(hiddenContainer);
    }

    // Add meta tags with targeted keywords
    function addMetaTags() {
        // Update existing meta keywords tag with additional keywords
        const metaKeywords = document.querySelector('meta[name="keywords"]');
        if (metaKeywords) {
            const existingKeywords = metaKeywords.getAttribute('content');
            const additionalKeywords = "as kuaför, balcova kuafor, balçova güzellik merkezi, balçova güzellik merkezleri, balçova güzellik salonu, balçova kaş alımı, balçova kuaför, balçova tırnak, izmir balçova güzellik merkezleri, izmir balçova kuaför, izmir kaş alımı, kaş alımı, kuafor en çok oy alanlar, kuaför, kuaför balçova, narlıdere güzellik salonları, pedikür";

            // Combine existing and additional keywords without duplicates
            const keywordsArray = [...new Set([...existingKeywords.split(','), ...additionalKeywords.split(',')])];
            metaKeywords.setAttribute('content', keywordsArray.join(','));
        }
    }

    // Execute all SEO enhancement functions
    addPageSpecificSchema();
    addEnhancedLocalBusinessSchema();
    addHiddenSemanticElements();
    addMetaTags();
});
