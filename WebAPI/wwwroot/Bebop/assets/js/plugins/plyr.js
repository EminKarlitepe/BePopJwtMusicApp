(function (global, $) {
  "use strict";

  var init = function(){
    var plyrist = false;

    $(document).on('click', '.btn-play' ,function(e){

      if(!plyrist){
        plyrist = new Plyrist(
          {
            playlist: '#plyrist', 
            player: 'audio'
          },
          [],
          {
            theme: 2
          }
        );
        plyrist.player.on('play', event => {
          updateDisplay();
        });

        plyrist.player.on('pause', event => {
          updateDisplay();
        });
      }

      var self = $(this).closest('[data-id]'),
          id = self.attr('data-id');
      if(plyrist.getIndex(id) > 0){
        plyrist.play({id: id});
      }else{
        plyrist.play({
          id: id,
          title: self.find('.title').text(),
          uri: self.find('.title').attr('href'),
          author: self.find('.subtitle').text(),
          poster: self.find('.media-content').css('background-image').replace(/^url|[\(\)]/g, ''),
          type: 'audio',
          source: self.attr('data-source')
        });
      }
    });

    $(document).on('pjaxEnd', function(){
      updateDisplay();
    });

    function updateDisplay(){
      $('[data-id]').find('.list-item').removeClass('active')
      $('[data-id]').find('.btn-play').removeClass('active');
      if(!plyrist) return;
      var item = plyrist.getCurrent();
      var el = $('[data-id="'+item.id+'"]');
      if( plyrist.player.paused ){
        el.find('.list-item').removeClass('active');
        el.find('.btn-play').removeClass('active');
      }else{
        el.find('.list-item').addClass('active');
        el.find('.btn-play').addClass('active');
      }
    }

    $(document).on('show.bs.dropdown', '.list-action > div, .media-action', function () {
      $(this).closest('.list-item').addClass('pos-rlt z-index-1');
    });

    $(document).on('hide.bs.dropdown', '.list-action > div, .media-action', function () {
      $(this).closest('.list-item').removeClass('pos-rlt z-index-1');
    });

    $(document).on('click', '.btn-more', function (e) {
      e.preventDefault();
      var $trigger = $(this);
      var $dp = $trigger.next('.dropdown-menu');
      var songId = $trigger.closest('[data-id]').attr('data-id');

      $dp.empty();
      $dp.append(
        '<button type="button" class="dropdown-item js-play-now">Şimdi Çal</button>'+
        '<div class="dropdown-divider"></div>'+
        '<div class="dropdown-header">Listeme ekle</div>'+
        '<div class="px-3 py-2 small text-muted js-playlists-loading">Listeler yükleniyor...</div>'
      );

      $.getJSON('/Playlists/UserPlaylists')
        .done(function(list){
          $dp.find('.js-playlists-loading').remove();
          if(!list || !list.length){
            $dp.append('<div class="px-3 py-2 small text-muted">Henüz listen yok</div>');
            return;
          }
          list.forEach(function(pl){
            $dp.append(
              '<button type="button" class="dropdown-item js-add-to-playlist" data-playlist-id="'+pl.playlistId+'" data-song-id="'+songId+'">'+
                pl.name+
              '</button>'
            );
          });
        })
        .fail(function(){
          $dp.find('.js-playlists-loading').text('Listeler alınamadı');
        });
    });

    $(document).on('click', '.dropdown-menu .js-play-now', function(e){
      e.preventDefault();
      var $root = $(this).closest('.media-action, .list-action').find('.btn-play').first();
      if($root.length){
        $root.trigger('click');
      }
    });

    $(document).on('click', '.dropdown-menu .js-add-to-playlist', function(e){
      e.preventDefault();
      var playlistId = $(this).data('playlist-id');
      var songId = $(this).data('song-id');

      $.post('/Playlists/AddFromAjax',
        {
          playlistId: playlistId,
          songId: songId
        })
        .done(function(){
          if(typeof Swal !== 'undefined'){
            Swal.fire({
              toast: true,
              position: 'top-end',
              icon: 'success',
              title: 'Şarkı listeye eklendi',
              showConfirmButton: false,
              timer: 1500
            });
          }
        });
    });

  }

  // for ajax to init again
  global.plyr = {init: init};

})(this, jQuery);
