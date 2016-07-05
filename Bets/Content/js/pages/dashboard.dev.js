var DashboardPage = (function () {

    function bindEvents() {

        $('#global-results .btn-view').click(function () {

            var $this = $(this),
				$row = $this.siblings('.global-container');

            if ($this.hasClass('active')) {
                $this.removeClass('active');
                $this.val("View Bets");
                $row.empty().addClass('removed');
            }
            else {

                $row.removeClass('removed');
                $this.val("Close");
                $this.addClass('active');

                //inline block
                $this.toggleClass('loading');
                $this.attr('disabled', true);

                AjaxUtils.post({
                    url: $this.data('type'),
                    success: function (html) {

                        $row.html(html);

                        $this.toggleClass('loading');
                        $this.removeAttr('disabled').removeClass('loading');
                    }
                });
            }
        });
    }

    function init() {
        bindEvents();
    }

    return {
        init: init
    };
}());

$(function () { DashboardPage.init(); })