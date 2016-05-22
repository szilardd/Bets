var Mobile = (function () {

    var menuOpened = false;

    function onMenuOpened() {
        menuOpened = true;
        $('.mm-page').addClass('fixed'); //allow scrolling only in the menu
    }

    function onMenuClosed() {
        menuOpened = false;
        $('.mm-page').removeClass('fixed');
    }

    function initMenu() {

        $("#menu")
            .mmenu({ classes: "mm-slide", position: 'right', zposition : 'front' })
            .on("opened.mm", onMenuOpened)
            .on("closed.mm", onMenuClosed);

        var onScroll = function (e) {

            //if menu is opened, deny scroll
            if (menuOpened) {
                return false;
            }
        }

        $(document).on('scroll', onScroll);
    }

    function initFastClick() {

        window.addEventListener('load', function () {
            FastClick.attach(document.body);
        }, false);
    }

    function init() {
        initMenu();
        initFastClick();
    }

    return {
        init: init
    };
}());

$(function () { Mobile.init(); });