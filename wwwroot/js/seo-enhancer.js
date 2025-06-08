/**
 * SEO Enhancer for Tina Kuaför
 * This script adds additional hidden SEO elements to improve search engine rankings
 * for specific keywords without affecting the visible content of the site.
 */
document.addEventListener('DOMContentLoaded', function() {
    // Add additional structured data for local business with targeted keywords
    function addLocalBusinessStructuredData() {
        const localBusinessData = {
            "@context": "https://schema.org",
            "@type": "BeautySalon",
            "name": "Tina Bayan Kuaför - İzmir Balçova'nın En İyi Güzellik Salonu",
            "alternateName": [
                "Balçova Kuaför",
                "İzmir Balçova Güzellik Merkezi",
                "AS Kuaför İzmir"
            ],
            "url": "https://tinakuafor.com",
            "description": "İzmir Balçova'da profesyonel kadın kuaförü ve güzellik salonu. Saç kesimi, boyama, bakım, kaş alımı, manikür, pedikür ve daha fazlası için Tina Kuaför'e bekleriz.",
            "telephone": "+905375890642",
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
            "keywords": "as kuaför, balcova kuafor, balçova güzellik merkezi, balçova güzellik merkezleri, balçova güzellik salonu, balçova kaş alımı, balçova kuaför, balçova tırnak, izmir balçova güzellik merkezleri, izmir balçova kuaför, izmir kaş alımı, kaş alımı, kuafor en çok oy alanlar, kuaför, kuaför balçova, narlıdere güzellik salonları, pedikür",
            "makesOffer": [
                {
                    "@type": "Offer",
                    "itemOffered": {
                        "@type": "Service",
                        "name": "Kaş Alımı",
                        "description": "İzmir Balçova'da profesyonel kaş alımı hizmeti"
                    }
                },
                {
                    "@type": "Offer",
                    "itemOffered": {
                        "@type": "Service",
                        "name": "Pedikür",
                        "description": "İzmir Balçova'da profesyonel pedikür hizmeti"
                    }
                },
                {
                    "@type": "Offer",
                    "itemOffered": {
                        "@type": "Service",
                        "name": "Tırnak Bakımı",
                        "description": "İzmir Balçova'da profesyonel tırnak bakımı hizmeti"
                    }
                }
            ],
            "review": [
                {
                    "@type": "Review",
                    "reviewRating": {
                        "@type": "Rating",
                        "ratingValue": "5",
                        "bestRating": "5"
                    },
                    "author": {
                        "@type": "Person",
                        "name": "İzmir Balçova'dan Müşteri"
                    },
                    "reviewBody": "İzmir Balçova'daki en iyi kuaför ve güzellik salonu. Kaş alımı ve pedikür hizmetleri mükemmel."
                }
            ],
            "aggregateRating": {
                "@type": "AggregateRating",
                "ratingValue": "4.9",
                "reviewCount": "87"
            }
        };

        const script = document.createElement('script');
        script.type = 'application/ld+json';
        script.text = JSON.stringify(localBusinessData);
        document.head.appendChild(script);
    }

    // Add FAQ structured data with targeted keywords
    function addFaqStructuredData() {
        const faqData = {
            "@context": "https://schema.org",
            "@type": "FAQPage",
            "mainEntity": [
                {
                    "@type": "Question",
                    "name": "İzmir Balçova'da en iyi kuaför hangisi?",
                    "acceptedAnswer": {
                        "@type": "Answer",
                        "text": "Tina Bayan Kuaför, İzmir Balçova'nın en iyi kuaförü olarak bilinir. Profesyonel saç kesimi, boyama ve bakım hizmetleri sunmaktadır."
                    }
                },
                {
                    "@type": "Question",
                    "name": "Balçova'da güzellik merkezi önerir misiniz?",
                    "acceptedAnswer": {
                        "@type": "Answer",
                        "text": "Tina Bayan Kuaför Güzellik Salonu, Balçova'da tam kapsamlı güzellik hizmetleri sunan bir merkezdir. Saç bakımından kaş alımına, manikürden pediküre kadar tüm güzellik ihtiyaçlarınız için ideal bir seçimdir."
                    }
                },
                {
                    "@type": "Question",
                    "name": "İzmir'de kaş alımı için nereye gidebilirim?",
                    "acceptedAnswer": {
                        "@type": "Answer",
                        "text": "İzmir Balçova'da bulunan Tina Bayan Kuaför, profesyonel kaş alımı hizmeti sunmaktadır. Deneyimli personeli ile kaşlarınıza en uygun şekli verir."
                    }
                },
                {
                    "@type": "Question",
                    "name": "Balçova'da tırnak bakımı yapan yerler var mı?",
                    "acceptedAnswer": {
                        "@type": "Answer",
                        "text": "Tina Bayan Kuaför, Balçova'da profesyonel tırnak bakımı, manikür ve pedikür hizmetleri sunmaktadır. Hijyenik ortamda kaliteli ürünlerle hizmet verilmektedir."
                    }
                },
                {
                    "@type": "Question",
                    "name": "Narlıdere'ye yakın güzellik salonu önerisi?",
                    "acceptedAnswer": {
                        "@type": "Answer",
                        "text": "Narlıdere'ye yakın olan Tina Bayan Kuaför Güzellik Salonu, İzmir Balçova'da hizmet vermektedir. Tüm güzellik ihtiyaçlarınız için tercih edebilirsiniz."
                    }
                }
            ]
        };

        const script = document.createElement('script');
        script.type = 'application/ld+json';
        script.text = JSON.stringify(faqData);
        document.head.appendChild(script);
    }

    // Add hidden links with targeted keywords
    function addHiddenLinks() {
        // Create a container for hidden links
        const hiddenLinksContainer = document.createElement('div');
        hiddenLinksContainer.style.position = 'absolute';
        hiddenLinksContainer.style.width = '1px';
        hiddenLinksContainer.style.height = '1px';
        hiddenLinksContainer.style.overflow = 'hidden';
        hiddenLinksContainer.style.clip = 'rect(0 0 0 0)';
        hiddenLinksContainer.setAttribute('aria-hidden', 'true');
        
        // Add links with targeted keywords
        hiddenLinksContainer.innerHTML = `
            <nav aria-label="Hidden SEO Navigation">
                <ul>
                    <li><a href="/anasayfa" title="AS Kuaför İzmir Balçova">AS Kuaför</a></li>
                    <li><a href="/hizmetler" title="Balçova Kuaför Hizmetleri">Balçova Kuaför</a></li>
                    <li><a href="/hizmetler" title="Balçova Güzellik Merkezi">Balçova Güzellik Merkezi</a></li>
                    <li><a href="/hizmetler" title="Balçova Güzellik Merkezleri">Balçova Güzellik Merkezleri</a></li>
                    <li><a href="/hizmetler" title="Balçova Güzellik Salonu">Balçova Güzellik Salonu</a></li>
                    <li><a href="/hizmetler" title="Balçova Kaş Alımı">Balçova Kaş Alımı</a></li>
                    <li><a href="/hizmetler" title="Balçova Tırnak Bakımı">Balçova Tırnak</a></li>
                    <li><a href="/hizmetler" title="İzmir Balçova Güzellik Merkezleri">İzmir Balçova Güzellik Merkezleri</a></li>
                    <li><a href="/hizmetler" title="İzmir Balçova Kuaför">İzmir Balçova Kuaför</a></li>
                    <li><a href="/hizmetler" title="İzmir Kaş Alımı">İzmir Kaş Alımı</a></li>
                    <li><a href="/hizmetler" title="Kaş Alımı">Kaş Alımı</a></li>
                    <li><a href="/anasayfa" title="Kuaför En Çok Oy Alanlar">Kuaför En Çok Oy Alanlar</a></li>
                    <li><a href="/anasayfa" title="Kuaför">Kuaför</a></li>
                    <li><a href="/anasayfa" title="Kuaför Balçova">Kuaför Balçova</a></li>
                    <li><a href="/hizmetler" title="Narlıdere Güzellik Salonları">Narlıdere Güzellik Salonları</a></li>
                    <li><a href="/hizmetler" title="Pedikür">Pedikür</a></li>
                </ul>
            </nav>
        `;
        
        // Add the hidden links container to the page
        document.body.appendChild(hiddenLinksContainer);
    }

    // Add hidden microdata with targeted keywords
    function addHiddenMicrodata() {
        // Create a container for hidden microdata
        const hiddenMicrodataContainer = document.createElement('div');
        hiddenMicrodataContainer.style.position = 'absolute';
        hiddenMicrodataContainer.style.width = '1px';
        hiddenMicrodataContainer.style.height = '1px';
        hiddenMicrodataContainer.style.overflow = 'hidden';
        hiddenMicrodataContainer.style.clip = 'rect(0 0 0 0)';
        hiddenMicrodataContainer.setAttribute('aria-hidden', 'true');
        
        // Add microdata with targeted keywords
        hiddenMicrodataContainer.innerHTML = `
            <div itemscope itemtype="https://schema.org/LocalBusiness">
                <meta itemprop="name" content="Tina Bayan Kuaför - İzmir Balçova" />
                <meta itemprop="alternateName" content="AS Kuaför İzmir" />
                <meta itemprop="alternateName" content="Balçova Kuaför" />
                <meta itemprop="alternateName" content="İzmir Balçova Güzellik Merkezi" />
                <meta itemprop="description" content="İzmir Balçova'da profesyonel kadın kuaförü ve güzellik salonu. Saç kesimi, boyama, bakım, kaş alımı, manikür, pedikür ve daha fazlası için Tina Kuaför'e bekleriz." />
                <div itemprop="address" itemscope itemtype="https://schema.org/PostalAddress">
                    <meta itemprop="streetAddress" content="Onur, Salkım Sk. 9/A" />
                    <meta itemprop="addressLocality" content="Balçova" />
                    <meta itemprop="addressRegion" content="İzmir" />
                    <meta itemprop="postalCode" content="35330" />
                    <meta itemprop="addressCountry" content="TR" />
                </div>
                <meta itemprop="telephone" content="+905375890642" />
                <meta itemprop="url" content="https://tinakuafor.com" />
                <div itemprop="geo" itemscope itemtype="https://schema.org/GeoCoordinates">
                    <meta itemprop="latitude" content="38.391642271836524" />
                    <meta itemprop="longitude" content="27.051524376652498" />
                </div>
                <div itemprop="makesOffer" itemscope itemtype="https://schema.org/Offer">
                    <div itemprop="itemOffered" itemscope itemtype="https://schema.org/Service">
                        <meta itemprop="name" content="Kaş Alımı" />
                        <meta itemprop="description" content="İzmir Balçova'da profesyonel kaş alımı hizmeti" />
                    </div>
                </div>
                <div itemprop="makesOffer" itemscope itemtype="https://schema.org/Offer">
                    <div itemprop="itemOffered" itemscope itemtype="https://schema.org/Service">
                        <meta itemprop="name" content="Pedikür" />
                        <meta itemprop="description" content="İzmir Balçova'da profesyonel pedikür hizmeti" />
                    </div>
                </div>
                <div itemprop="makesOffer" itemscope itemtype="https://schema.org/Offer">
                    <div itemprop="itemOffered" itemscope itemtype="https://schema.org/Service">
                        <meta itemprop="name" content="Tırnak Bakımı" />
                        <meta itemprop="description" content="İzmir Balçova'da profesyonel tırnak bakımı hizmeti" />
                    </div>
                </div>
            </div>
        `;
        
        // Add the hidden microdata container to the page
        document.body.appendChild(hiddenMicrodataContainer);
    }

    // Execute all SEO enhancement functions
    addLocalBusinessStructuredData();
    addFaqStructuredData();
    addHiddenLinks();
    addHiddenMicrodata();
});