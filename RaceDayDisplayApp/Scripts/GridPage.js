$(window).bind('resize', resizeGrid).trigger('resize');


$(document).ready(function() {

    bindCheckboxes();

    $("#race_grid").bind('jqGridLoadComplete', function (e, data) {
        GridLoadComplete(e, data)
    });

    $("#race-flip").click(function () {
        displayHideStats("#race-span", "#race-form", "#race-history")
    });

    $("#meeting-flip").click(function () {
        displayHideStats("#meeting-span", "#meeting-form", "#meeting-history")
    });

});

function displayHideStats(spanSelector, formSelector, destPanelSelector) {
    //TODO submit form by ajax
    $.ajax({
        type: "POST",
        url: '/RaceResearch/StatsTable',
        data: $(formSelector).serialize(), // serializes the form's elements.
        success: function (data) {
            var panel = $(destPanelSelector);
            panel.empty();
            panel.append(data);
            panel.slideToggle("slow");

            var span = $(spanSelector)
            if (span.text() == 'Show') {
                span.text("Hide");
            }
            else {
                span.text("Show");
            }

        }
    });

    //send user token or something to control security
    //$.ajax({
    //    type: 'POST',
    //    url: '/Meetings/Race',
    //    data: {
    //        id: raceId
    //    },
    //    success: function (data) {
    //        var index = data.search('<!--GRID STARTS HERE-->');
    //        var details = data.substring(0, index);
    //        var grid = data.substring(index);
    //        $("#race_details").empty();
    //        $("#form-box").empty();
    //        $('#race_details').append(details);
    //        $("#form-box").append(grid);
    //        showHideColumns();

    //        $("#race_grid").bind('jqGridLoadComplete', function (e, data) {
    //            GridLoadComplete(e, data)
    //        });

    //        $(".chks_container").show();
    //        $("#btns_div").show();
    //        $("#panel_right").show();
    //    }
    //});

}

function GridLoadComplete(e, data)
{
    //update race info
    $.each(data.race, function (k, v) {
        $('#'+k).text(v);
    });

    var txtRefresh;
    if (data.race.isDone) {
        txtRefresh = "<b>RACE IS DONE</b>";
    }
    else {
        txtRefresh = "Next client refresh in " + data.nextRefreshSecs + " secs";
        //set next refresh
        setTimeout(refreshGrid, data.nextRefreshSecs * 1000);
    }
    
    var txtDbUpdated;
    if (data.dbUpdatedSecs < 0) {
        txtDbUpdated = "DB has never been updated<br/>";
    }
    else{
        txtDbUpdated = "DB was last updated " + data.dbUpdatedSecs + " secs before displaying<br/>";
    }

    $('#refreshInfo').html( txtDbUpdated +
                            "Server was last updated " + data.serverUpdatedSecs + " secs before displaying<br/>"
                            + txtRefresh);
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
    return "<a href=\"../RunnerHistory/" + rowObject.HorseId + "\" onclick=\"window.open(this.href, 'mywin','left=20,top=20,width=1120,height=500,toolbar=1,resizable=0'); return false;\" >" + cellvalue + "</a>";
    //+ "?RaceId=" + $("#RaceId").val() 
}

function winOddsFormatter(cellvalue, options, rowObject) {
    var classTxt = "";
    if (rowObject.isWinFavorite) {
        classTxt = "class=\"cell_favorite\"";
    }
    else if (rowObject.WinDropby20) {
        classTxt = "class=\"cell_dropBy20\"";
    }
    else if (rowObject.WinDropby50) {
        classTxt = "class=\"cell_dropBy50\"";
    }

    return "<span " + classTxt + ">$" + cellvalue + "</span>";
}

function placeOddsFormatter(cellvalue, options, rowObject) {
    var classTxt = "";
    //if (rowObject.isPlaceFavorite) {
    //    classTxt = "class=\"cell_favorite\"";
    //}
    //else
    if (rowObject.PlaceDropby20) {
        classTxt = "class=\"cell_dropBy20\"";
    }
    else if (rowObject.PlaceDropby50) {
        classTxt = "class=\"cell_dropBy50\"";
    }

    return "<span " + classTxt + ">$" + cellvalue + "</span>";
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

    var CountryEnum = $("#CountryEnum").val();

    if (MeetingId != "") {
        $.ajax({
            type: 'POST',
            url: '/Meetings/GridSettings',
            data: {
                country: CountryEnum
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

