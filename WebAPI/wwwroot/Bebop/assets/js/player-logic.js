const audio = new Audio();
audio.volume = 0.8;

const player = document.getElementById('bepop-player');
const btnPlay = document.getElementById('btnPlayPause');
const iconPlay = document.getElementById('iconPlay');
const iconPause = document.getElementById('iconPause');
const progressFill = document.getElementById('progressFill');
const currentTimeEl = document.getElementById('currentTime');
const totalTimeEl = document.getElementById('totalTime');
const progressBar = document.getElementById('progressBar');
const volumeSlider = document.getElementById('volumeSlider');
const playerTitle = document.getElementById('playerTitle');
const playerArtist = document.getElementById('playerArtist');
const playerCover = document.getElementById('playerCover');

function formatTime(s) {
    if (isNaN(s)) return '0:00';
    const m = Math.floor(s / 60);
    const sec = Math.floor(s % 60);
    return m + ':' + (sec < 10 ? '0' : '') + sec;
}

audio.addEventListener('timeupdate', () => {
    if (!audio.duration) return;
    progressFill.style.width = (audio.currentTime / audio.duration * 100) + '%';
    currentTimeEl.textContent = formatTime(audio.currentTime);
});

audio.addEventListener('loadedmetadata', () => {
    totalTimeEl.textContent = formatTime(audio.duration);
});

audio.addEventListener('play', () => {
    iconPlay.style.display = 'none';
    iconPause.style.display = 'block';
    player.classList.remove('hidden');
});

audio.addEventListener('pause', () => {
    iconPlay.style.display = 'block';
    iconPause.style.display = 'none';
});

btnPlay.addEventListener('click', () => {
    audio.paused ? audio.play() : audio.pause();
});

progressBar.addEventListener('click', (e) => {
    if (!audio.duration) return;
    const rect = progressBar.getBoundingClientRect();
    audio.currentTime = ((e.clientX - rect.left) / rect.width) * audio.duration;
});

volumeSlider.addEventListener('input', (e) => audio.volume = e.target.value / 100);

document.getElementById('btnMute').addEventListener('click', function () {
    audio.muted = !audio.muted;
    this.style.opacity = audio.muted ? '0.4' : '1';
});

function extractYouTubeId(url) {
    const m = url.match(/(?:v=|youtu\.be\/|embed\/)([^&?\/]+)/i);
    return m && m[1] ? m[1] : null;
}

function playYouTubeInside(url) {
    const vid = extractYouTubeId(url);
    if (!vid) return false;
    document.getElementById('yt-player-iframe').src = `https://www.youtube.com/embed/${vid}?autoplay=1`;
    document.getElementById('yt-player-container').style.display = 'block';
    return true;
}

document.getElementById('yt-player-close').addEventListener('click', () => {
    document.getElementById('yt-player-iframe').src = '';
    document.getElementById('yt-player-container').style.display = 'none';
});

document.addEventListener("click", function (e) {
    const btn = e.target.closest(".btn-play");
    if (!btn) return;
    e.preventDefault();
    const container = btn.closest("[data-id]") || btn.closest("[data-source]");
    const songId = container?.getAttribute("data-id");
    const fallback = container?.getAttribute("data-source") || btn.getAttribute("data-source");

    const playData = {
        url: fallback,
        title: container?.getAttribute("data-title") || "",
        artist: container?.getAttribute("data-artist") || "",
        cover: container?.getAttribute("data-cover") || ""
    };

    const doPlay = (url) => {
        if (!url) return;
        if (playYouTubeInside(url)) return;
        audio.src = url;
        audio.play().catch(console.error);
        if (playData.title) playerTitle.textContent = playData.title;
        if (playData.artist) playerArtist.textContent = playData.artist;
        if (playData.cover) playerCover.style.backgroundImage = `url(${playData.cover})`;
    };

    if (songId) {
        fetch(`/Songs/Play?songId=${songId}`, { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
            .then(r => {
                if (r.status === 403) {
                    // Show nice SweetAlert and do not fallback to playing
                    if (typeof Swal !== 'undefined') {
                        Swal.fire({
                            title: 'Erişim Engellendi',
                            text: 'Bu şarkıyı dinlemek için paket yükseltmeniz gerekiyor.',
                            icon: 'warning',
                            showCancelButton: true,
                            confirmButtonText: 'Paket Yükselt',
                            cancelButtonText: 'İptal'
                        }).then(result => {
                            if (result.isConfirmed) {
                                window.location.href = '/Membership/Upgrade';
                            }
                        });
                    } else {
                        alert('Bu şarkıyı dinlemek için paket yükseltmeniz gerekiyor.');
                    }
                    throw new Error('forbidden');
                }
                if (!r.ok) throw new Error('network');
                return r.json();
            })
            .then(data => {
                if (data && data.url) doPlay(data.url);
            })
            .catch(err => {
                if (err && err.message === 'forbidden') return; // don't fallback
                // network or other error -> try fallback
                doPlay(fallback);
            });
    } else doPlay(fallback);
});

document.addEventListener("DOMContentLoaded", () => {
    feather.replace();
    const path = window.location.pathname.toLowerCase();
    document.querySelectorAll("#header .navbar-nav .nav-link").forEach(link => {
        if (path.startsWith(link.getAttribute("href")?.toLowerCase())) {
            link.closest(".nav-item").classList.add("active");
        }
    });
});