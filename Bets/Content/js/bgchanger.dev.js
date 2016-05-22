var bgchangertimeout = 2; //2 min
var nrofbg = 16;

function changeBG() {

    Number.random = function (min, max) {
        return Math.floor(Math.random() * (max - min + 1) + min);
    };

    var x = Number.random(1, nrofbg);
		img = new Image();

	img.onload = function() {
	    $("html").css({ "background": "black url(" + img.src + ") center top no-repeat fixed" });
	};

	img.src = Config.BaseURL + "content/img/bgimg/bg" + x + ".jpg";

    setTimeout('changeBG()', parseInt(60 * 1000 * bgchangertimeout));
};

(function ($) {

	return;

	//ignore login page
	if ($('#login-page').length == 1)
		return;

    changeBG();
})(jQuery);

