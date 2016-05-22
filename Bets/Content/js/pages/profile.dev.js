var ProfilePage = (function() {

	var img = $('.picture')[0];

	function init() {

		$('.btn-file').change(function(evt) {
		
			var files = evt.target.files,
				file;

			if (files.length > 0)
				file = files[0];

			if (file && typeof FileReader !== "undefined" && (/image/i).test(file.type)) {

				reader = new FileReader();

				reader.onload = (function (theImg) {
					return function (evt) {
						theImg.src = evt.target.result;
					};
				}(img));

				reader.readAsDataURL(file);
			}
		});
	}

	return {
		init : init
	};
}());

$(function() { ProfilePage.init(); });