$(function () {

    $('select').combobox();

    $('#SelectedSeasonId').combobox('value', $('#SelectedSeasonId option:last-child').val());

    $('#go-button').button()
       .click(function () {
           go($('#SelectedCountryId').combobox('value'),
              $('#SelectedRaceCourseId').combobox('value'),
              $('#SelectedSeasonId').combobox('value'),
              $('#SelectedSuperMeetTypeId').combobox('value'),
              $('#SelectedSuperRaceTypeId').combobox('value'),
              $('#SelectedNumRunnersId').combobox('value'))
       });


    $("#SelectedCountryId").combobox({
        select: function (event, ui) {

            // remove old options but first
            //$('#SelectedRaceCourseId option:gt(0)').remove();

            var newOptions = jQuery.parseJSON($('#AllRaceCourseItems').val());
            var selectedCountryId = this.value;

            var $el = $("#SelectedRaceCourseId");
            $el.empty();
            $.each(newOptions, function (i, v) {
                var CountryId, Id, Name;
                $.each(v, function (id, value) {
                    if (value.Key == 'CountryId') { CountryId = value.Value; }
                    else if (value.Key == 'Id') { Id = value.Value; }
                    else if (value.Key == 'Name') { Name = value.Value; }
                });
                if (CountryId == selectedCountryId) {
                    $el.append($("<option></option>")
                       .attr("value", Id).text(Name));
                }
            });

            $('#SelectedRaceCourseId').combobox('value', $('#SelectedRaceCourseId option:first-child').val());
            //$('#SelectedSeasonId').combobox('value', $('#SelectedSeasonId option:last-child').val());
            //$('#SelectedSuperMeetTypeId').combobox('value', $('#SelectedSuperMeetTypeId option:first-child').val());

            //$('#SelectedRaceCourseId').val($('#SelectedRaceCourseId option:first-child').val());
            //$('#SelectedRaceCourseId option:first-child').attr("selected", "selected");
            //$("#SelectedRaceCourseId").val($("#SelectedRaceCourseId option:first").val());
        }
    });

    //$("#SelectedRaceCourseId").combobox({
    //    select: function (event, ui) {
    //        $('#SelectedSeasonId').combobox('value', $('#SelectedSeasonId option:last-child').val());
    //        $('#SelectedSuperMeetTypeId').combobox('value', $('#SelectedSuperMeetTypeId option:first-child').val());
    //    }
    //});

    //$("#SelectedSeasonId").combobox({
    //    select: function (event, ui) {
    //        $('#SelectedSuperMeetTypeId').combobox('value', $('#SelectedSuperMeetTypeId option:first-child').val());
    //    }
    //});

    //$("#SelectedSuperMeetTypeId").combobox({
    //    select: function (event, ui) {
    //    }
    //});

    //$("#SelectedSuperRaceTypeId").combobox({
    //    select: function (event, ui) {
    //    }
    //});

});

function go(country, racecourse, season, meetType, raceType, noRunners) {
    $.ajax({
        type: 'POST',
        url: '/RaceResearch/Tables',
        data: {
            country: country,
            racecourse: racecourse, 
            season: season, 
            meetType: meetType, 
            raceType: raceType, 
            noRunners: noRunners
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
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
        }
    });
}