var updatedRow;
// These Variables for DataTable
var table;
var datatable;
var exportedCols = []; // Array to hold columns that do not have the class "js-no-export"

function ShowSuccessMessage(msg = "Action Done Successfully") {
    // Using SweatAlert2 for displaying success messages
    Swal.fire({
        icon: "success",
        title: "Success",
        text: msg,
        customClass: {
            confirmButton: "btn btn-primary",
        }
    })
}

function ShowDeletedMessage(msg = "Delete Done Successfully") {
    Swal.fire({
        icon: "warning",
        iconColor: "#f1416c",
        title: "Deleted Success",
        text: msg,
        customClass: {
            confirmButton: "btn btn-primary",
        }
    });
}

function ShowErrorMessage(msg = "Something went wrong!") {
    Swal.fire({
        icon: "error",
        title: "Oops...",
        text: msg,
        customClass: {
            confirmButton: "btn btn-primary",
        }
    });
}

function OnModalBegin() {
    $('body :submit').attr('disabled', 'disabled').attr("data-kt-indicator", "on"); // Disable all submit buttons to prevent multiple submissions
}
function OnModalSuccess(row) {

    ShowSuccessMessage();
    $("#Modal").modal("hide");
    // To Fix Searching in DataTable after adding new item ==> Using DataTable API to add new row and Draw it ..instead of using jQuery DOM append
    //datatable Not Has Update on Row but We Use a Design Pattern To Solve it:
    // 1- Remove the old row from DataTable

    if (updatedRow !== undefined) { 
        datatable.row(updatedRow).remove().draw(); // Remove the old row from DataTable
        updatedRow = undefined; // Reset updatedRow after replacing the row
    }

    // 2- Add the new row to DataTable
    var newRow = $(row);
    datatable.row.add(newRow).draw(); // Add new row to DataTable and redraw it

    // old way for Using jQuery to append new row to table body
    // $("tbody").append(item);

    KTMenu.init(); // Re-initialize the menu after adding new item "Must do it if you use Metronic Theme"
    KTMenu.initHandlers(); // Re-initialize the menu handlers "Must do it if you use Metronic Theme"
}

function OnModalComplete() {
    $('body :submit').removeAttr('disabled').removeAttr("data-kt-indicator"); // Disable all submit buttons to prevent multiple submissions
}

//DataTables:
// To Execlude Action Column To Get in Exported Files
// We Add Class "js-no-export" to the Action Column in the table at <th>
// then Define here an Empty Array ExportedCols to Push in it All Cols Not Have the Class "js-no-export"
// The iterate on Headers of Table Cols To Get the Col Has this Class and ignore it in ExportedCols Array
var headers = $('th');
$.each(headers, function (i) {
    var col = $(this);
    if (!col.hasClass("js-no-export")) {
        exportedCols.push(i);
    };
});

// Class definition : This Fuction for Make Search and Export files Work in Datatables
var KTDatatables = function () {


    // Private functions
    var initDatatable = function () {
        // Init datatable --- more info on datatables: https://datatables.net/manual/
        datatable = $(table).DataTable({
            "info": false,
            'pageLength': 10,
        });
    }

    // Hook export buttons
    var exportButtons = () => {
        const documentTitle = $(".js-datatables").data("document-title") + "__Bookify"; 
        var buttons = new $.fn.dataTable.Buttons(table, {
            buttons: [
                {
                    extend: 'copyHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'excelHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'csvHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'pdfHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                }
            ]
        }).container().appendTo($('#kt_datatable_example_buttons'));

        // Hook dropdown menu click event to datatable export buttons
        const exportButtons = document.querySelectorAll('#kt_datatable_example_export_menu [data-kt-export]');
        exportButtons.forEach(exportButton => {
            exportButton.addEventListener('click', e => {
                e.preventDefault();

                // Get clicked export value
                const exportValue = e.target.getAttribute('data-kt-export');
                const target = document.querySelector('.dt-buttons .buttons-' + exportValue);

                // Trigger click event on hidden datatable export buttons
                target.click();
            });
        });
    }

    // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    // Public methods
    return {
        init: function () {
            table = document.querySelector('.js-datatables');

            if (!table) {
                return;
            }

            initDatatable();
            exportButtons();
            handleSearchDatatable();
        }
    };
}();



$(document).ready(function () {
    // SweatAlert2
    var message = $("#ActionMessage").text();
    if (message !== '') {
        ShowSuccessMessage(message);
    };
    // DataTables

    KTUtil.onDOMContentLoaded(function () {
        KTDatatables.init();
    });

    // Handle Bootstrap Modal
    // to fix proplem for : when you Edit new item not appling Editing on it because this eventListener Added After
    // the Document is Ready => to solve it "Select the parent of <a> which clicked on and use delegate Fuction in the place of on Function"
    $("body").delegate(".js-render-modal","click", function () {
        var btn = $(this);
        var modal = $("#Modal");


        modal.find("#ModalLabel").text(btn.data("title"));

        if (btn.data("update") !== undefined) {
            updatedRow = btn.parents("tr");
        }



        $.get({
            url: btn.data("url"),
            success: function (PartialViewForm) {
                modal.find(".modal-body").html(PartialViewForm);
                // Re-parse the form to apply validation rules To Apply ClientSide Validation For Form in Modal this step
                // after adding "jquery-ajax-unobtrusive" Liberary and put it To Run into _ValidationPartialView
                $.validator.unobtrusive.parse(modal);
            },
            error: function () {
                ShowDeletedMessage();
            }
        })
        // modal("show") ==> this is a method in Bootstrap in this modal To Show it & modal("hide") ==> to hide it
        modal.modal('show');
    });

    // Handle Delete Button Toggle Status Action
    // to fix proplem for : when you add new item not appling toggle status on it because this eventListener Added After
    // the Document is Readt => to solve it "Select the parent of <a> which clicked on and use delegate Fuction in the place of on Function"
    $('body').delegate('.js-toggle-status', 'click', function () {
        const $menu = $(this).closest('.menu-sub-dropdown');
        $menu.hide();
        var btn = $(this);
        var id = btn.data('id');
        // console.log(id);
        // This Alert To Detect if Surely want To Change Status "Using Bootboxjs" as a nice shape from Default
        bootbox.confirm({
            message: 'Are you sure you want to toggle the status of this Item?',
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-danger'
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
                        url: btn.data('url'),
                        data: {
                            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                        },
                        success: function (lastUpdatedOn) {
                            var row = btn.parents('tr');
                            var status = row.find('.js-status');
                            var newStatus = status.text().trim() === 'Deleted' ? 'Available' : 'Deleted';
                            status.text(newStatus).toggleClass('badge-light-success badge-light-danger');
                            row.find('.js-updated-on').html(lastUpdatedOn);
                            // Add Animation on the Row <tr> using Animate.css
                            row.addClass('animate__animated animate__fadeIn');
                            setTimeout(function () {
                                row.removeClass('animate__animated animate__fadeIn');
                            }, 1000);
                            /// Toastr Library to show notification for successfuly or error  action
                            /// if(newStatus === 'Deleted') {
                            ///     toastr.error('Category has been deleted successfully.', 'Deleted');
                            /// } else {
                            ///     toastr.success('Category has been restored successfully.', 'Available');
                            /// }
                            // using Sweatalert2
                            if (newStatus === 'Deleted') {
                                var DeleteMessage = "This Item has been Deleted Now "
                                ShowDeletedMessage(DeleteMessage);
                            } else {
                                var SuccessMessage = "This Item has been Available Now "
                                ShowSuccessMessage(SuccessMessage);
                            };

                        },
                        error: function () {
                            ShowErrorMessage();
                        }
                    });
                }
            }
        });
    })
});
