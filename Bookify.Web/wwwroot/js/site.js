var updatedRow;

function ShowSuccessMessage(msg = "Action Done Successfully") {
    // Using SweatAlert2 for displaying success messages
    Swal.fire({
        icon: "success",
        title: "Success",
        text: msg,
        customClass: {
            confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary",
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
            confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary",
        }
    });
}

function ShowErrorMessage(msg = "Something went wrong!") {
    Swal.fire({
        icon: "error",
        title: "Oops...",
        text: msg,
        customClass: {
            confirmButton: "btn btn-outline btn-outline-dashed btn-outline-primary btn-active-light-primary",
        }
    });
}

function OnModalSuccess(item) {

    ShowSuccessMessage();
    $("#Modal").modal("hide");
    if (updatedRow === undefined) {
        $("tbody").append(item);
    } else {
        $(updatedRow).replaceWith(item);
        updatedRow = undefined; // Reset updatedRow after replacing the row
    }

    KTMenu.init(); // Re-initialize the menu after adding new item "Must do it if ypu use Metronic Theme"
    KTMenu.initHandlers(); // Re-initialize the menu handlers "Must do it if ypu use Metronic Theme"
}


$(document).ready(function () {
    var message = $("#ActionMessage").text();
    if (message !== '') {
        ShowSuccessMessage(message);
    };

    // Handle Bootstrap Modal
    // to fix proplem for : when you Edit new item not appling Editing on it because this eventListener Added After
    // the Document is Ready => to solve it "Select the parent of <a> which clicked on and use delegate Fuction in the place of on Function"
    $("body").delegate(".js-render-modal","click", function () {
        var btn = $(this);
        var modal = $("#Modal");


        modal.find("#ModalLabel").text(btn.data("title"));

        if (btn.data("update") !== undefined) {
            updatedRow = btn.parents("tr");
            console.log(updatedRow);
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
});
