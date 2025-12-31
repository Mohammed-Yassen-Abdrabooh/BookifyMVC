var selectedCopies = [];

$(document).ready(function () {
    $(".js-search").on("click", function (e) {
        e.preventDefault();

        var serial = $("#SearchValue").val();

        if (selectedCopies.find(c => c.serial == serial)) {
            ShowErrorMessage("You cannot add the same copy.");
            return;
        }

        if (selectedCopies.length >= maxAllowedCopies) {
            ShowErrorMessage(`You cannot add more than ${maxAllowedCopies} books.`);
            return;
        }

        $("#SearchForm").submit();
    });
});

function OnAddCopySuccess(copy) {
    serial = $("#SearchValue").val("");

    var bookId = $(copy).find(".js-copy").data("book-id");
    if (selectedCopies.find(c => c.BookId == bookId)){
        ShowErrorMessage(`You cannot add more than one copy for the same book.`);
        return;
    }
    $("#CopiesFormForCreateNewRental").prepend(copy);

    var copy = $(".js-copy");
    selectedCopies = [];
    $.each(copy, function (i, input) {
        var $input = $(input);
        selectedCopies.push({ serial: $input.val(), BookId: $input.data("book-id")});
        $input.attr("name", `SelectedCopies[${i}]`).attr("id", `SelectedCopies_${i}_`);

    });
}