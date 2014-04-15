$(function () {
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
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                }
            });
        }

        return false; // avoid to execute the actual submit of the form.
    });

});
