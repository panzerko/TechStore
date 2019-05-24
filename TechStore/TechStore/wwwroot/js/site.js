// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.




$(document).ready(function () {

    var native_width = 0;
    var native_height = 0;
    $(".large").css("background", "url('" + $(".small").attr("src") + "') no-repeat");

    //Now the mousemove function
    $(".magnify").mousemove(function (e) {
        //When the user hovers on the image, the script will first calculate
        //the native dimensions if they don't exist. Only after the native dimensions
        //are available, the script will show the zoomed version.
        if (!native_width && !native_height) {
            //This will create a new image object with the same image as that in .small
            //We cannot directly get the dimensions from .small because of the 
            //width specified to 200px in the html. To get the actual dimensions we have
            //created this image object.
            var image_object = new Image();
            image_object.src = $(".small").attr("src");

            //This code is wrapped in the .load function which is important.
            //width and height of the object would return 0 if accessed before 
            //the image gets loaded.
            native_width = image_object.width;
            native_height = image_object.height;
        }
        else {
            //x/y coordinates of the mouse
            //This is the position of .magnify with respect to the document.
            var magnify_offset = $(this).offset();
            //We will deduct the positions of .magnify from the mouse positions with
            //respect to the document to get the mouse positions with respect to the 
            //container(.magnify)
            var mx = e.pageX - magnify_offset.left;
            var my = e.pageY - magnify_offset.top;

            //Finally the code to fade out the glass if the mouse is outside the container
            if (mx < $(this).width() && my < $(this).height() && mx > 0 && my > 0) {
                $(".large").fadeIn(50);
            }
            else {
                $(".large").fadeOut(50);
            }
            if ($(".large").is(":visible")) {
                //The background position of .large will be changed according to the position
                //of the mouse over the .small image. So we will get the ratio of the pixel
                //under the mouse pointer with respect to the image and use that to position the 
                //large image inside the magnifying glass
                var rx = Math.round(mx / $(".small").width() * native_width - $(".large").width() / 2) * -1;
                var ry = Math.round(my / $(".small").height() * native_height - $(".large").height() / 2) * -1;
                var bgp = rx + "px " + ry + "px";

                //Time to move the magnifying glass with the mouse
                var px = mx - $(".large").width() / 2;
                var py = my - $(".large").height() / 2;
                //Now the glass moves with the mouse
                //The logic is to deduct half of the glass's width and height from the 
                //mouse coordinates to place it with its center at the mouse coordinates

                //If you hover on the image now, you should see the magnifying glass in action
                $(".large").css({ left: px, top: py, backgroundPosition: bgp });
            }
        }
    })
})


var starId = ["mark-one", "mark-two", "mark-three", "mark-four", "mark-five"]
var mark = 0;
function Star(i) {
    for (index = 0; index < starId.length; index++) {
        document.getElementById(starId[index]).style.color = "#fff";
    }

    for (index = 0; index < i; index++) {
        document.getElementById(starId[index]).style.color = "#ffdb11";
    }

    mark = i;
    document.getElementById("mark").value = mark;
};


var isEditing = false;

function Edit(id) {
    if (!isEditing) {
        document.getElementById("editButton" + id.toString()).style.display = "none";
        document.getElementById("saveButton" + id.toString()).style.display = "inline-block";

        document.getElementById("reviewMessage" + id.toString()).style.display = "none";
        document.getElementById("hiddenInput" + id.toString()).style.display = "block";

        document.getElementById("starCount" + id.toString()).style.display = "none";
        document.getElementById("hiddenStarCount" + id.toString()).style.display = "block";

        isEditing = true;
    }
}

function CountDecrease(id, pricePar) {
    var value = Number(document.getElementById("goodCount" + id.toString()).value);
    var price = Number(document.getElementById("commonPrice").value);
    var count = Number(document.getElementById("goodCommonCount").value);

    price -= Number(pricePar);
    count -= 1;
    if (value == 1)
        return;

    document.getElementById("goodCount" + id.toString()).value = value - 1;
    document.getElementById("commonPrice").value = Number(price);
    document.getElementById("goodCommonCount").value = count;
}

function CountIncrease(id, pricePar) {
    var value = Number(document.getElementById("goodCount" + id.toString()).value);
    var price = Number(document.getElementById("commonPrice").value);
    var count = Number(document.getElementById("goodCommonCount").value);

    value += 1;
    price += Number(pricePar);
    count += 1;

    document.getElementById("goodCount" + id.toString()).value = value;
    document.getElementById("commonPrice").value = price;
    document.getElementById("goodCommonCount").value = count;
}

var map;
var markers = [];
function initMap() {
    map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: 49.8397, lng: 24.0297 },
        zoom: 12
    });
}

function setMarker(commonPrice) {
    var select = document.getElementById('EndPointCity');
    var city = select.options[select.selectedIndex].value;

    var adress = city + ", " +
        document.getElementById('EndPointStreet').value;
    var resultlat = ''; var resultlng = '';
    $.ajax({
        async: false,
        dataType: "json",
        url: 'https://maps.google.com/maps/api/geocode/json?address=' + adress + '&key=AIzaSyCFlwlczCmamaKRfISTv2XvFJNttALOfnI',
        success: function (data) {
            for (var key in data.results) {
                resultlat = data.results[key].geometry.location.lat;
                resultlng = data.results[key].geometry.location.lng;
            }
        }
    });

    deleteMarkers();
    var marker = new google.maps.Marker({ position: { lat: resultlat, lng: resultlng }, map: map });
    markers.push(marker);
    map.setZoom(17);
    map.panTo(new google.maps.LatLng(resultlat, resultlng));
    setDeliveryPrice(commonPrice);
}

function setDeliveryPrice(commonPrice) {
    var storages = document.getElementById('avaliableStorages');
    var additonalPrice = 35;

    if (storages.length == 0) {
        document.getElementById('deliveryPrice').textContent = additonalPrice;
        document.getElementById('commonPrice').textContent = commonPrice;
        var price = Number(document.getElementById('goodPrice').textContent);
        document.getElementById('commonPrice').value = (price + additonalPrice).toString();

        return;
    }

    var select = document.getElementById('EndPointCity');
    var city = select.options[select.selectedIndex].value;
    var isTheSamePlace = false;

    for (var i = 0; i < storages.length; i++) {
        if (storages[i].value == city) {
            document.getElementById('deliveryPrice').textContent = 10;
            document.getElementById('commonPrice').textContent = commonPrice;
            var price = Number(document.getElementById('goodPrice').textContent);
            document.getElementById('commonPrice').value = (price + 10).toString();
            isTheSamePlace = true;
        }
    }

    if (!isTheSamePlace) {
        document.getElementById('deliveryPrice').textContent = additonalPrice;
        document.getElementById('commonPrice').value = commonPrice;
        var price = Number(document.getElementById('goodPrice').textContent);
        document.getElementById('commonPrice').value = (price + additonalPrice).toString();
    }
}

function deleteMarkers() {
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(null);
    }
    markers = [];
};