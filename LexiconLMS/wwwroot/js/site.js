$('#col-modules .btn').click(function () {
    $('#col-modules .btn').each(function () {
        $(this).css("margin-left", "0");
    });

    //$(this).addClass("btn-success");
    $('#col-activities').hide();
    $(this).animate({ "margin-left": '1em' });
    $('#col-activities').slideDown("medium");
});

