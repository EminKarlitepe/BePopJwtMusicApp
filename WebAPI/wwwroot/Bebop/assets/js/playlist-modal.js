(function () {
    'use strict';

    var modal = null;
    var modalList = null;
    var modalLoading = null;
    var modalEmpty = null;
    var currentSongId = null;

    function init() {
        modal = document.getElementById('playlistModal');
        modalList = document.getElementById('plModalList');
        modalLoading = document.getElementById('plModalLoading');
        modalEmpty = document.getElementById('plModalEmpty');

        if (!modal) return;

        document.getElementById('plModalClose').addEventListener('click', closeModal);
        modal.addEventListener('click', function (e) {
            if (e.target === modal) closeModal();
        });

        // Tüm sayfalardaki .btn-add-playlist ve .btn-add-playlist-item butonlarına delegasyon
        document.addEventListener('click', function (e) {
            var btn = e.target.closest('.btn-add-playlist') || e.target.closest('.btn-add-playlist-item');
            if (btn) {
                e.preventDefault();
                e.stopPropagation();
                var songId = btn.dataset.songId;
                if (songId) openModal(parseInt(songId));
            }
        });
    }

    function openModal(songId) {
        currentSongId = songId;
        modal.style.display = 'flex';
        modalList.style.display = 'none';
        modalLoading.style.display = 'block';
        modalEmpty.style.display = 'none';

        fetch('/Playlists/GetUserPlaylists')
            .then(function (r) {
                if (!r.ok) throw new Error('Unauthorized');
                return r.json();
            })
            .then(function (playlists) {
                modalLoading.style.display = 'none';
                if (!playlists || playlists.length === 0) {
                    modalEmpty.style.display = 'block';
                    return;
                }
                renderPlaylists(playlists);
                modalList.style.display = 'block';
            })
            .catch(function () {
                modalLoading.style.display = 'none';
                modalEmpty.textContent = 'Listeler yüklenemedi.';
                modalEmpty.style.display = 'block';
            });
    }

    function renderPlaylists(playlists) {
        modalList.innerHTML = '';
        playlists.forEach(function (pl) {
            var div = document.createElement('div');
            div.className = 'pl-item';
            div.innerHTML =
                '<div class="pl-item-icon">&#9835;</div>' +
                '<div class="pl-item-info">' +
                '<div class="pl-item-name">' + escHtml(pl.name) + '</div>' +
                '<div class="pl-item-count">' + pl.songCount + ' şarkı</div>' +
                '</div>';
            div.addEventListener('click', function () {
                addSong(pl.playlistId, pl.name);
            });
            modalList.appendChild(div);
        });
    }

    function addSong(playlistId, playlistName) {
        var fd = new FormData();
        fd.append('playlistId', playlistId);
        fd.append('songId', currentSongId);

        fetch('/Playlists/AddFromAjax', { method: 'POST', body: fd })
            .then(function (r) {
                if (!r.ok) throw new Error('Error');
                return r.json();
            })
            .then(function () {
                closeModal();
                showToast('"' + playlistName + '" listesine eklendi', 'success');
            })
            .catch(function () {
                showToast('Şarkı eklenemedi, tekrar dene.', 'error');
            });
    }

    function closeModal() {
        if (modal) modal.style.display = 'none';
    }

    function showToast(message, type) {
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                toast: true,
                position: 'bottom-end',
                icon: type,
                title: message,
                showConfirmButton: false,
                timer: 2500,
                background: '#1a1a2e',
                color: '#fff'
            });
        }
    }

    function escHtml(str) {
        return String(str)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }

    // DOM hazır olunca başlat
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    // PJAX desteği
    document.addEventListener('pjax:complete', init);

})();