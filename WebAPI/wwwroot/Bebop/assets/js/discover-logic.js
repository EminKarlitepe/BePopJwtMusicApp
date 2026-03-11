const slides = [
    {
        bg: '/Bebop/assets/img/b9.jpg',
        title: 'Müziğin<br><span>Büyüsüne Kapıl</span>',
        desc: 'Ruhunu harekete geçiren harika şarkılara dal. Yeni sesleri keşfet, favori hitlerini yeniden keşfet.'
    },
    {
        bg: '/Bebop/assets/img/b0.jpg',
        title: 'En Popüler<br><span>Listeler</span>',
        desc: "Bepop'un en popüler listelerine göz at. Trend olan hitlerden zamansız klasiklere kadar."
    },
    {
        bg: '/Bebop/assets/img/b19.jpg',
        title: 'Favori<br><span>Sanatçını Bul</span>',
        desc: 'Favori sanatçılarını keşfet ve en iyi parçalarını dinle.'
    }
];

let current = 0;
const dots = document.querySelectorAll('.hero-dot');
const heroBg = document.getElementById('heroBg');
const heroTitle = document.getElementById('heroTitle');
const heroDesc = document.getElementById('heroDesc');

function changeSlide(idx) {
    if (!heroBg) return;
    current = idx;
    heroBg.style.backgroundImage = `url(${slides[idx].bg})`;
    dots.forEach((d, i) => d.classList.toggle('active', i === idx));
    heroTitle.innerHTML = slides[idx].title;
    heroDesc.textContent = slides[idx].desc;
}

// Otomatik Slider Başlat
const sliderInterval = setInterval(() => {
    if (document.getElementById('heroBg')) {
        changeSlide((current + 1) % slides.length);
    } else {
        clearInterval(sliderInterval);
    }
}, 5000);

// Global Tıklama Dinleyici (Player ile Entegre)
document.addEventListener('click', function (e) {
    const btn = e.target.closest('.btn-play');
    if (btn) {
        e.preventDefault();
        e.stopPropagation();

        const card = btn.closest('[data-id]') || btn;
        const songData = {
            id: btn.dataset.id || card.dataset.id,
            source: btn.dataset.source || card.dataset.source,
            title: btn.dataset.title || card.dataset.title || '',
            artist: btn.dataset.artist || card.dataset.artist || '',
            cover: btn.dataset.cover || card.dataset.cover || ''
        };

        if (songData.id) {
            playSongInternal(songData);
        }
        return;
    }

    const card = e.target.closest('.track-card, .song-list-item');
    if (card && !e.target.closest('a') && !e.target.closest('button')) {
        const userLevel = parseInt(
            document.querySelector('[data-user-level]')?.dataset.userLevel ||
            document.body.dataset.userLevel ||
            "1"
        );
        const trackLevel = parseInt(card.dataset.level || "1");

        if (userLevel >= trackLevel) {
            playSongInternal({
                id: card.dataset.id,
                source: card.dataset.source,
                title: card.dataset.title,
                artist: card.dataset.artist,
                cover: card.dataset.cover
            });
        }
    }
});

function playSongInternal(song) {
    fetch(`/Songs/Play?songId=${song.id}`, {
        headers: { 'X-Requested-With': 'XMLHttpRequest' }
    })
        .then(res => res.json())
        .then(data => {
            const finalUrl = data.url || song.source;
            // Layout'taki player-logic.js içindeki global fonksiyonu veya audio elementini tetikle
            if (window.audio) {
                window.audio.src = finalUrl;
                window.audio.play();

                // Player UI Güncelle
                const ui = {
                    title: document.getElementById('playerTitle'),
                    artist: document.getElementById('playerArtist'),
                    cover: document.getElementById('playerCover'),
                    panel: document.getElementById('bepop-player')
                };
                if (ui.title) ui.title.textContent = song.title;
                if (ui.artist) ui.artist.textContent = song.artist;
                if (ui.cover) ui.cover.style.backgroundImage = `url('${song.cover}')`;
                if (ui.panel) ui.panel.classList.remove('hidden');
            }
        })
        .catch(err => console.error("Çalma hatası:", err));
}