$(window).bind('resize', resizeGrid).trigger('resize');


$(document).ready(function() {

    bindCheckboxes();

    $("#race_grid").bind('jqGridLoadComplete', function (e, data) {
        GridLoadComplete(e, data)
    });

});

function GridLoadComplete(e, data)
{
    //update race info
    $.each(data.race, function (k, v) {
        $('#'+k).text(v);
    });

    if (data.race.isDone) {
        $('#refreshInfo').text = "<b>RACE IS DONE</b>";
    }

    $('#refreshInfo').html("DB was last updated " + data.dbUpdatedSecs + " secs before displaying<br/> \
                            Server was last updated " + data.serverUpdatedSecs + " secs before displaying<br/> \
                            Next client refresh in " + data.nextRefreshSecs + " secs");
    
    //set next refresh
    setTimeout(refreshGrid, data.nextRefreshSecs * 1000);
}

function refreshGrid() {
    isFirstRow = true;
    $("#race_grid").trigger("reloadGrid");
}

//this event handler updates the value of isDone when the grid is refreshed
var isFirstRow = true; //just for performance
function gridAfterInsertRow(rowid, rowdata, rowelem) {
    if (rowelem.isScratched)
        $('#' + rowid).addClass("scratched");

    isFirstRow = false;
}


function bindCheckboxes() {
    //when a checkbox is clicked, a column is shown/hidden
    $(".chk_settings").change(function () {
        if ($(this).is(":checked")) {
            jQuery("#race_grid").jqGrid('showCol', this.id.substring(4));
        } else if ($(this).not(":checked")) {
            jQuery("#race_grid").jqGrid('hideCol', this.id.substring(4));
        }
        resizeGrid();
    });
}

function currencyFormatter(cellvalue, options, rowObject) {
    return "$" + cellvalue;
}

function percentageFormatter(cellvalue, options, rowObject) {
    return cellvalue + "%";
}

function linkFormatter(cellvalue, options, rowObject) {
    return "<a href=\"../RunnerHistory/" + options.rowId + "?RaceId=" + $("#RaceId").val() + "\" onclick=\"window.open(this.href, 'mywin','left=20,top=20,width=500,height=500,toolbar=1,resizable=0'); return false;\" >" + cellvalue + "</a>";
}

//when the Races select changes, new race details and grid are loaded and the checkboxes shown/hidden
$(function () {
    $("#RacesList").change(function () {
        var raceId = $("#RacesList").val();
        if (raceId != "") {
            $.ajax({
                type: 'POST',
                url: '/Meetings/Race',
                data: {
                    id: raceId
                },
                success: function (data) {
                    var index = data.search('<!--GRID STARTS HERE-->');
                    var details = data.substring(0, index);
                    var grid = data.substring(index);
                    $("#race_details").empty();
                    $("#form-box").empty();
                    $('#race_details').append(details);
                    $("#form-box").append(grid);
                    showHideColumns();

                    $("#race_grid").bind('jqGridLoadComplete', function (e, data) {
                        GridLoadComplete(e, data)
                    });

                    $(".chks_container").show();
                    $("#btns_div").show();
                    $("#panel_right").show();
                }
            });
        }
        else {
            $("#race_details").empty();
            $("#form-box").empty();
            $(".chks_container").hide();
            $("#btns_div").hide();
            $("#panel_right").hide();
        }
    });
});


    var firstTime = true;
    function hideColumnsFirstTime(){
        if (firstTime){
            showHideColumns();
            firstTime = false;
        }
    }

    //every time a new grid or settings are loaded, this function hides the unchecked columns
    function showHideColumns() {

        var columnsShow = [];
        var columnsHide = [];
        $(".chk_settings").each(function (index, li) {
            if ($(this).is(":checked")) {
                columnsShow.push(this.id.substring(4));
            } else if ($(this).not(":checked")) {
                columnsHide.push(this.id.substring(4));
            }
            jQuery("#race_grid").jqGrid('showCol', columnsShow);
            jQuery("#race_grid").jqGrid('hideCol', columnsHide);
        });

        resizeGrid();
    }


    function resizeGrid() {
        $("#race_grid").setGridWidth($('#form-box').width(), true);
    }

    //function convertDateTime(dateTimeStr) {
    //    //dateTime = dateTimeStr.split(" ");

    //    //var date = dateTime[0].split("-");
    //    //var yyyy = date[0];
    //    //var mm = date[1] - 1;
    //    //var dd = date[2];

    //    //var time = dateTime[1].split(":");
    //    var time = dateTimeStr.split(":");
    //    var h = time[0];
    //    var m = time[1];
    //    var s = time[2];

    //    var result = new Date();
    //    result.setHours(h);
    //    result.setMinutes(m);
    //    result.setSeconds(s);
    //    return result;
    //}


    function defaultSettings() {

        var MeetingId = $("#MeetingId").val();
        var IsHK = $("#IsHK").val();

        if (MeetingId != "") {
            $.ajax({
                type: 'POST',
                url: '/Meetings/GridSettings',
                data: {
                    meetingId: MeetingId,
                    isHK: IsHK
                },
                success: function (data) {
                    $("#chks_container").empty();
                    $('#chks_container').append(data);
                    bindCheckboxes();
                    showHideColumns();
                }
            });
        }
    }


