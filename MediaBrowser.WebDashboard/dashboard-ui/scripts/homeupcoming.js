﻿(function ($, document) {

    function loadUpcoming(page) {
        Dashboard.showLoadingMsg();

        var limit = AppInfo.hasLowImageBandwidth ?
         24 :
         40;

        var query = {

            Limit: limit,
            Fields: "AirTime,UserData,SeriesStudio,SyncInfo",
            UserId: Dashboard.getCurrentUserId(),
            ImageTypeLimit: 1,
            EnableImageTypes: "Primary,Backdrop,Banner,Thumb"
        };

        ApiClient.getJSON(ApiClient.getUrl("Shows/Upcoming", query)).done(function (result) {

            var items = result.Items;

            if (items.length) {
                page.querySelector('.noItemsMessage').style.display = 'none';
            } else {
                page.querySelector('.noItemsMessage').style.display = 'block';
            }

            var elem = page.querySelector('#upcomingItems');
            renderUpcoming(elem, items);

            Dashboard.hideLoadingMsg();

            LibraryBrowser.setLastRefreshed(page);
        });
    }

    function enableScrollX() {
        return $.browser.mobile && AppInfo.enableAppLayouts;
    }

    function getThumbShape() {
        return enableScrollX() ? 'overflowBackdrop' : 'backdrop';
    }

    function renderUpcoming(elem, items) {

        var groups = [];

        var currentGroupName = '';
        var currentGroup = [];

        var i, length;

        for (i = 0, length = items.length; i < length; i++) {

            var item = items[i];

            var dateText = '';

            if (item.PremiereDate) {
                try {

                    dateText = LibraryBrowser.getFutureDateText(parseISO8601Date(item.PremiereDate, { toLocal: true }), true);

                } catch (err) {
                }
            }

            if (dateText != currentGroupName) {

                if (currentGroup.length) {
                    groups.push({
                        name: currentGroupName,
                        items: currentGroup
                    });
                }

                currentGroupName = dateText;
                currentGroup = [];
            } else {
                currentGroup.push(item);
            }
        }

        var html = '';

        for (i = 0, length = groups.length; i < length; i++) {

            var group = groups[i];

            html += '<div class="homePageSection">';
            html += '<h1 class="listHeader">' + group.name + '</h1>';

            if (enableScrollX()) {
                html += '<div class="itemsContainer hiddenScrollX">';
            } else {
                html += '<div class="itemsContainer">';
            }

            html += LibraryBrowser.getPosterViewHtml({
                items: group.items,
                showLocationTypeIndicator: false,
                shape: getThumbShape(),
                showTitle: true,
                showPremiereDate: true,
                preferThumb: true,
                lazy: true,
                showDetailsMenu: true,
                centerText: true

            });
            html += '</div>';

            html += '</div>';
        }

        elem.innerHTML = html;
        ImageLoader.lazyChildren(elem);
    }

    $(document).on('pageinitdepends', "#indexPage", function () {

        var page = this;

        $(page.querySelector('neon-animated-pages')).on('tabchange', function () {

            if (parseInt(this.selected) == 3) {
                var tabContent = page.querySelector('.homeUpcomingTabContent');

                if (LibraryBrowser.needsRefresh(tabContent)) {
                    loadUpcoming(tabContent);
                }
            }
        });
    });

})(jQuery, document);