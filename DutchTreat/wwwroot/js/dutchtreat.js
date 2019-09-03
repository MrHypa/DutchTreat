$(document).ready(function () {
});


var author = "Jason Thomson";


console.log("Starting exotic cars application");

var theFrom = $("#theForm");
theFrom.hide();


var button = $("#addProduct");
button.on("click", function () {
    alert(author);
});


var productInfo = $(".product-props li");
productInfo.on("click", function () {
    console.log("You have clicked on " + $(this).text())
})

var $loginToggle = $("#loginToggle");
var $popupForm = $(".popup-form");

$loginToggle.on("click", function () {
    $popupForm.toggle(2000);
})