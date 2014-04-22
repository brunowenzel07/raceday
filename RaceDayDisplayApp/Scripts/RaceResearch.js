
$(function () {

    //jqueryze the dropdowns
    $('select').combobox();

    //$('#go-button').button()
    //   .click(function () {
    //       go($('#SelectedCountryId').combobox('value'),
    //          $('#SelectedRaceCourseId').combobox('value'),
    //          $('#SelectedSeasonId').combobox('value'),
    //          $('#SelectedSuperMeetTypeId').combobox('value'),
    //          $('#SelectedSuperRaceTypeId').combobox('value'),
    //          $('#SelectedNumRunnersId').combobox('value'))
    //   });


    $("#SelectedCountryId").combobox({
        select: function (event, ui) {
            loadCombo('AllRaceCourseItems', 'SelectedRaceCourseId', this.value);
            loadCombo('AllSuperMeetTypeItems', 'SelectedSuperMeetTypeId', this.value);
            loadCombo('AllSuperRaceTypeItems', 'SelectedSuperRaceTypeId', this.value);
        }
    });


    // this is the id of the form
    $("#filters-form").submit(function () {

        var error = false;
        $.each($('select'), function (index, value) {
            var txt = '';
            if (value.value == "") {
                txt = 'Please select a valid value';
                error = true;
            }
            $('#error_' + value.id).text(txt);
        });

        if (!error) {
            $.ajax({
                type: "POST",
                url: '/RaceResearch/Tables',
                data: $("#filters-form").serialize(), // serializes the form's elements.
                success: function (data) {

                    var tabCont = $("#tables-container");
                    tabCont.empty();
                    tabCont.append(data);

                    /* For zebra striping */
                    $("table tr:nth-child(odd)").addClass("odd-row");
                    /* For cell text alignment */
                    $("table td:first-child, table th:first-child").addClass("first");
                    /* For removing the last border */
                    $("table td:last-child, table th:last-child").addClass("last");

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            });
        }

        return false; // avoid to execute the actual submit of the form.
    });

});


function loadCombo(fieldId, comboId, selectedCountryId) {

    // remove old options but first
    //$('#SelectedRaceCourseId option:gt(0)').remove();

    var newOptions = jQuery.parseJSON($('#' + fieldId).val());

    var $el = $("#" + comboId);
    $el.empty();
    $.each(newOptions, function (i, v) {
        if (v.CountryId == selectedCountryId) {
            $el.append($("<option></option>")
               .attr("value", v.Id).text(v.Name));
        }
    });

    $el.combobox('value', $('#' + comboId + ' option:first-child').val());

}