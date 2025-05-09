$(document).ready(function () {
    function isMobile() {
        return /Mobi|Android/i.test(navigator.userAgent);
    }

    // Ejecutar el código solo si es un dispositivo móvil
    if (isMobile()) {
        $('.no-info-div').removeClass('ocultar');
    }else{
        $('.no-info-div').addClass('ocultar');
    }
    $.magnificPopup.open({
        items: {
            src: '#popupCarrusel'
        },
        type: 'inline',
        midClick: true,
        callbacks: {
            open: function () {
                // Iniciar Owl Carousel cuando se abra el popup
                $('#popupCarrusel .owl-carousel').owlCarousel({
                    items: 1,
                    loop: true,
                    nav: true,
                    dots: true,
                    autoplay: true,
                    margin: 20,
                    autoplayTimeout: 3000,
                    itemsDesktop: false,
                    itemsDesktopSmall: false,
                    itemsTablet: false,
                    itemsMobile: false,
                    autoPlay: true,
                    stopOnHover: true
                });
            },
            close: function () {
                // Destruir el carrusel al cerrar (opcional)
                $('#popupCarrusel .owl-carousel').trigger('destroy.owl.carousel').removeClass('owl-loaded').find('.owl-stage-outer').children().unwrap();
            }
        }
    });

    $('#filtros-avanzados').change(function () {
        if ($(this).is(':checked')) {
            $('.header-filtros-avanzados').slideDown();
        } else {
            $('.header-filtros-avanzados').slideUp();
        }
    });

    $(document).on('click', '.cerrar-popup', function () {
        $.magnificPopup.close();
    });

    $('.btn-info-empresas').on('click', function () {
        $.magnificPopup.open({
            items: {
                src: '#popupInfoEmpresas'
            },
            type: 'inline',
            midClick: true
        });
    });

    $('.abrir-detalle-empresa').on('click', function () {
        $.magnificPopup.open({
            items: {
                src: '#popupInfoDetallada'
            },
            type: 'inline',
            midClick: true
        });
    });

    $('.btn-filters-mobile').on('click', function () {
        $.magnificPopup.open({
            items: {
                src: '#popupBusquedaResponsive'
            },
            type: 'inline',
            midClick: true
        });
    });

    $('.btn-visualizacion').click(function (e) {
        e.preventDefault();
        let id = $(this).data('id');
        if (id == 'tabla') {
            $('.panel-index-grilla').fadeIn();
            $('.mapa-resultados-tarjetas').hide();
        } else {
            $('.panel-index-grilla').hide();
            $('.mapa-resultados-tarjetas').fadeIn();
            $('.mapa-resultado').fadeIn();
            $('.tarjetas-grid').removeClass('full');
            $('.no-info-div').addClass('ocultar');
        }
    });

    $('.ocultar-mapa').click(function (e) {
        e.preventDefault();
        $('.tarjetas-grid').addClass('full');
        $('.mapa-resultado').fadeOut();
    });

    $('.btn-show-map').click(function (e) {
        e.preventDefault();
        $(this).hide();
        $('.tarjetas-grid').hide();
        $('.mapa-resultado').fadeIn();
        $('.footer-resultados-responsive').addClass('mostrar');
        $('html, body').animate({scrollTop: 0}, 800);
    });

    $('.footer-resultados-responsive').click(function (e) {
        e.preventDefault();
        $(this).removeClass('mostrar');
        $('.tarjetas-grid, .btn-show-map').fadeIn();
        $('.mapa-resultado').hide();
        $('html, body').animate({scrollTop: 0}, 800);
    });

    $('.btn-search-responsive').on('click', function (e) {
        e.preventDefault();
        $('.no-info-div').remove();
        $('.mapa-resultado').hide();
        $('.loading').fadeIn();
        setTimeout(function () {
            $('.loading').fadeOut();
            $('.mapa-resultados-tarjetas').fadeIn();
        }, 3000);
        $.magnificPopup.close();
    });

});