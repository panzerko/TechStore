// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(".list-group .list-group-item").click(function (e) {
    $(".list-group .list-group-item").removeClass("active");
    $(e.target).addClass("active");
});