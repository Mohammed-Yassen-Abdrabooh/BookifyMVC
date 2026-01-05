$(document).ready(function () {
    $(".js-renew").on('click', function () {
        var subscriberId = $(this).data("key");
        // This Alert To Detect if Surely want To Renew Subscription For Subscriber "Using Bootboxjs" as a nice shape from Default
        bootbox.confirm({
            message: 'Are you sure that you need to renew this Subscription?',
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-secondary'
                }
            },
            callback: function (result) {
                if (result) {
                    $.post({
                        //if you write URL not url ==> it will Get Error 404
                        url: `/Subscriber/RenewSubscription?sKey=${subscriberId}`,
                        data: {
                            '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (row) {
                            $('#subscriptionsTable').find('tbody').append(row);

                            var activeIcon = $('#activeStatusIcon');
                            activeIcon.removeClass('d-none');
                            activeIcon.siblings('svg').remove();
                            activeIcon.parents('.card').removeClass('bg-warning').addClass('bg-success');

                            $('#RentalBtn').removeClass('d-none');
                            $('#cardStatus').text('Active Subscriber');
                            $('#statusBadge').removeClass('badge-light-warning').addClass('badge-light-success').text('Active Subscriber');
                            ShowSuccessMessage();

                        },
                        error: function () {
                            ShowErrorMessage();
                        }
                    });
                }
            }
        });
    })
})