(function () {
    function wireAutocomplete(options) {
        var $input = $(options.inputSelector);
        if ($input.length === 0) return;

        var $results = $(options.resultsSelector);
        var $hiddenId = $(options.hiddenSelector);
        var timer = null;

        function hideResults() {
            $results.hide().empty();
        }

        function render(items) {
            $results.empty();
            if (!items || items.length === 0) { hideResults(); return; }

            items.forEach(function (it) {
                var id = options.getId(it);
                var text = options.getText(it);
                if (!id || !text) return;
                var $el = $('<a href="#" class="list-group-item list-group-item-action"></a>');
                $el.text(text).data('id', id).data('label', text);
                $results.append($el);
            });
            $results.show();
        }

        $input.on('input', function () {
            var q = $(this).val();
            if ($hiddenId.length) $hiddenId.val('');
            if (timer) clearTimeout(timer);
            if (!q || q.length < 1) { hideResults(); return; }

            timer = setTimeout(function () {
                $.getJSON(options.url, { q: q })
                    .done(function (data) { render(data); })
                    .fail(function () { hideResults(); });
            }, 250);
        });

        $results.on('click', 'a', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            var label = $(this).data('label');
            if ($hiddenId.length) $hiddenId.val(id);
            $input.val(label);
            hideResults();
        });

        $(document).on('click', function (e) {
            if (!$(e.target).closest(options.resultsSelector + ', ' + options.inputSelector).length) {
                hideResults();
            }
        });
    }

    function initAutocomplete() {
        // Sanatçı autocomplete
        wireAutocomplete({
            inputSelector: '#artistSearch',
            resultsSelector: '#artistResults',
            hiddenSelector: '#ArtistId',
            url: '/Admin/SearchArtists',
            getId: function (it) { return it.artistId || it.ArtistId; },
            getText: function (it) { return it.name || it.Name; }
        });

        // Albüm autocomplete (başlık + sanatçı ismi)
        wireAutocomplete({
            inputSelector: '#albumSearch',
            resultsSelector: '#albumResults',
            hiddenSelector: '#AlbumId',
            url: '/Admin/SearchAlbums',
            getId: function (it) { return it.albumId || it.AlbumId; },
            getText: function (it) {
                var title = it.title || it.Title || '';
                var artist = it.artistName || it.ArtistName || '';
                return artist ? (title + ' — ' + artist) : title;
            }
        });
    }

    function initFileLabels() {
        var mp3 = document.getElementById('mp3Input');
        if (mp3) {
            mp3.addEventListener('change', function () {
                var label = this.nextElementSibling;
                if (label) label.textContent = this.files[0] ? this.files[0].name : 'Dosya seçin...';
            });
        }
        var cover = document.getElementById('coverInput');
        if (cover) {
            cover.addEventListener('change', function () {
                var label = this.nextElementSibling;
                if (label) label.textContent = this.files[0] ? this.files[0].name : 'Resim seçin...';
            });
        }
    }

    // Initialize on DOM ready
    $(function () {
        initAutocomplete();
        initFileLabels();
    });
})();
