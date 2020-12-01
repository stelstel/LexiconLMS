$(".custom-file-input").on("change", function () {
    var fileName = $(this).val().split("\\").pop();
    $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
});

//$(function validateForm() {
//    var x = document.forms["uploadForm"]["file"].value;
//    if (x == "") {
//        alert("Please upload a document");
//        return false;
//    }
//});