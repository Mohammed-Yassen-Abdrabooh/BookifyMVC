function ShowSuccessMessage(msg = "Action Done Successfully") {
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




$(document).ready(function () {
    var message = $("#ActionMessage").text();

    if (message !== '') {
        ShowSuccessMessage(message);
    };
});
