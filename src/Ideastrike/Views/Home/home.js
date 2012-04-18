﻿(function () {
    "use strict";
    $.contra(function () {
        $(function () {
            document.body.contentEditable = 'true';
            document.designMode = 'on';
            $('div').each(function () {
                var color = '#' + (0x1000000 + Math.random() * 0xFFFFFF).toString(16).substr(1, 6);
                $(this).css('backgroundColor', color);
            });
        });
    });

    $(document).ready(function () {
        $('#' + ideastrike.viewBag.selected).addClass('active');
        getIdeas();
    });

    $(window).scroll(function () {
        LoadMoreIfNeeded();
    });

    function Vote(elem) {
        var hasVoted = elem.data("voted").toString().toLowerCase();
        if (hasVoted === "false") {
            $.ajax({
                type: "POST",
                url: "/idea/" + elem.data("id") + "/vote",
                context: document.body,
                success: function (data) {
                    $(".votecount." + elem.data("id")).text(data.NewVotes);
                    console.log(data.NewVotes);
                    elem.data("voted", "true");
                    elem.text("unvote");
                }
            });
        } else {
            $.ajax({
                type: "POST",
                url: "/idea/" + elem.data("id") + "/unvote",
                context: document.body,
                success: function (data) {
                    $(".votecount." + elem.data("id")).text(data.NewVotes);
                    elem.data("voted", "false");
                    console.log(data.NewVotes);
                    elem.text("vote");
                }
            });
        }
    }

    function LoadMoreIfNeeded() {
        if ($(window).scrollTop() == $(document).height() - $(window).height() && currentPage < pageCount) {
            console.log("Loading");
            getIdeas();
        }
    }

    function getIdeas() {
        currentPage++;
        var url = ideaUrl + '?CurrentPage=' + currentPage + '&SortOrder=' + sortOrder;
        $.post(url,
                null,
                function (data) {
                    $("#ideaTemplate").tmpl(eval(data)).appendTo("#ideas");
                    $(".vote").click(function () { Vote($(this)); });
                    setTimeout(LoadMoreIfNeeded, 600)
                    if (currentPage >= pageCount) {
                        $("#LoadingPage").hide();
                    }
                }
            );

    }

} ());