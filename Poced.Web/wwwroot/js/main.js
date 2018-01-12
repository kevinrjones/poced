$(function() {

    $("#queue .item").each(function (a, item) {

        var listItem$ = $(item);
        var listItemHeight = listItem$.height();
        var titleHeight = listItem$.find(".title").height();
        var footerHeight = listItem$.find("ul").height();

        listItem$.find('span').height(listItemHeight - titleHeight - footerHeight);

    });
})