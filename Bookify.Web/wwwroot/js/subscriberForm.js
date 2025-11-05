$(document).ready(function () {
    $("#GovernorateId").on('change', function () {
        var governorateId = $(this).val();
        var areasList = $('#AreaId');
        areasList.empty();
        areasList.append($('<option></option>').attr("value", "").text("Select Area"));

        if (governorateId != "") {
            $.ajax({
                url: '/Subscriber/GetAreas?governorateId=' + governorateId,
                type: 'GET',
                success: function (areas) {
                    $.each(areas, function (i, area) {
                        var item = $('<option></option>').attr("value", area.value).text(area.text);
                        areasList.append(item);
                    })
                },
                error: function () {
                    ShowErrorMessage("There is an Erro To Get Areas For a Governorate at subscriberForm.js");
                }
            });
        };
    });
});
