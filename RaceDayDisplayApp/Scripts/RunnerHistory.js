$(function () {
    /* For zebra striping */
    $("table tr:nth-child(odd)").addClass("odd-row");
    /* For cell text alignment */
    $("table td:first-child, table th:first-child").addClass("first");
    /* For removing the last border */
    $("table td:last-child, table th:last-child").addClass("last");

    //click first button
    $("#btn_0").click();
});

function showFields(index, fields) {
    $(".btn_hist").css('background-color', '#e1e0e1');
    $("#btn_" + index).css('background-color', '#cccccc');

    $("table td, table th").hide(); //css('display', 'table-cell');

    $.each(fields, function (index, value) {
        $("table td:nth-child(" + value + ")").show();
        $("table th:nth-child(" + value + ")").show();
    });
}
