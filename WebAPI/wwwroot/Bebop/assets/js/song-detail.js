document.addEventListener('DOMContentLoaded', function () {
    const mainPlayBtn = document.getElementById('mainPlayBtn');
    const coverWrap = document.getElementById('coverWrap');
    const favBtn = document.getElementById('favBtn');

    // Kapak animasyonu ve çalma kontrolü
    if (mainPlayBtn) {
        mainPlayBtn.addEventListener('click', function () {
            coverWrap?.classList.toggle('playing');
        });
    }

    // Favori butonu etkileşimi
    if (favBtn) {
        favBtn.addEventListener('click', function () {
            const isActive = this.style.color === 'rgb(250, 92, 123)'; // var(--accent3)
            if (!isActive) {
                this.style.color = '#fa5c7b';
                this.querySelector('svg').setAttribute('fill', 'currentColor');
            } else {
                this.style.color = '';
                this.querySelector('svg').setAttribute('fill', 'none');
            }
        });
    }
});

// Paylaşma fonksiyonu (Global kapsamda olmalı)
function shareSong(title, artist, url) {
    if (navigator.share) {
        navigator.share({
            title: title + ' - ' + artist,
            url: url || window.location.href
        }).catch(console.error);
    } else {
        navigator.clipboard.writeText(url || window.location.href);
        alert('Link kopyalandı!');
    }
}