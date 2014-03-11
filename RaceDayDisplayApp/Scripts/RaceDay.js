$(window).bind('resize', resizeGrid).trigger('resize');


$(document).ready(function() {

    bindCheckboxes();

    //auto refresh grid
    startRefresh();

});


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

//this event handler updates the value of isDone when the grid is refreshed
var isFirstRow = true; //just for performance
function gridAfterInsertRow(rowid, rowdata, rowelem) {
    if (isFirstRow) {
        var isDone = $('#isDone').val() == "True";
        if (!isDone && rowelem.isDone)
            $('#isDone').val("True"); //the race has just finished, so we save it in the hidden field
    }
    if (rowelem.isScratched)
        $('#'+rowid).addClass("scratched");

    isFirstRow = false;
}

function currencyFormatter(cellvalue, options, rowObject) {
    return "$"+ cellvalue;
}

function percentageFormatter(cellvalue, options, rowObject)
{
    return cellvalue + "%";
}

function startRefresh() {
    var refreshIntervalStr = $("#RefreshIntervalSeconds").val();
    if (typeof refreshIntervalStr != "undefined") {

        //Every 30 seconds the grid is reloaded
        var refreshInterval = parseInt(refreshIntervalStr, 10) * 1000;
        setTimeout(startRefresh, refreshInterval);

        var isDoneStr = $('#isDone').val();
        if (typeof isDoneStr != "undefined") {

            var mustRefresh = false;

            if (isDoneStr != "True") { //Race is Done -> no refresh
                
                if ($('#isStarted').val() == "True") { //Race is started -> do refresh
                    mustRefresh = true;
                }
                else { //otherwise -> check jumptime
                    var jumpTimeStr = $('#RaceJumpTimeLocal').val();
                    if (typeof jumpTimeStr != "undefined") {

                        var jumpTime = convertDateTime(jumpTimeStr);
                        var minutes = parseInt($("#MinutesBeforeJumpTimeToStartRefresh").val(), 10);

                        //substract 25 minutes from jumpTime
                        var MS_PER_MINUTE = 60000;
                        var startTime = new Date(jumpTime - (minutes * MS_PER_MINUTE));

                        var currentDT = new Date($.now());

                        //only refresh the grid between (jumptime-25min) to raceIsDone
                        if (currentDT >= startTime) {
                            $('#isStarted').val("True"); //TODO convert to universal time
                            mustRefresh = true;
                        }

                    }
                }
            }
            if (mustRefresh) {
                refreshGrid();
            }
        }
    }
};

function refreshGrid() {
    isFirstRow = true;
    var raceId = $('#RaceId').val();
    var isStarted = $('#isStarted').val() == "True";
    var isDone = $('#isDone').val() == "True";
    //the url is updated with the last values of isStarted and isDone
    $("#race_grid").setGridParam({ url: '/Meetings/GridData/'+raceId+'?isStarted='+isStarted+'&isDone='+isDone, page:1 });
    $("#race_grid").trigger("reloadGrid");
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

function convertDateTime(dateTimeStr) {
    //dateTime = dateTimeStr.split(" ");

    //var date = dateTime[0].split("-");
    //var yyyy = date[0];
    //var mm = date[1] - 1;
    //var dd = date[2];

    //var time = dateTime[1].split(":");
    var time = dateTimeStr.split(":");
    var h = time[0];
    var m = time[1];
    var s = time[2];

    var result = new Date();
    result.setHours(h);
    result.setMinutes(m);
    result.setSeconds(s);
    return result;
}


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


