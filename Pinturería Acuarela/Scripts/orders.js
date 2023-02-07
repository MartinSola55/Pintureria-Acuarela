﻿function createTable(data) {
    let contenido = "";
    for (let i = 0; i < data.length; i++) {
        contenido += "<tr>";
        contenido += "<td>" + data[i].product ?? "" + "</td>";
        contenido += "<td>" + data[i].brand + "</td>";
        let category = data[i].category != null ? data[i].category : "";
        contenido += "<td>" + category + "</td>";
        let subcategory = data[i].subcategory != null ? data[i].subcategory : "";
        contenido += "<td>" + subcategory + "</td>";
        if (data[i].color != null) {
            contenido += "<td>";
            contenido += "<div class='d-flex flex-row justify-content-between align-items-center' >";
            contenido += data[i].color;
            contenido += "<span class='dot' style='background-color: " + data[i].hex_color + "'></span>";
            contenido += "</div>";
            contenido += "</td>";
        } else {
            contenido += "<td>";
            contenido += "</td>";
        }
        let capacity = data[i].capacity != null ? data[i].capacity : "";
        contenido += "<td>" + capacity + "</td>";
        contenido += "<td>";
        contenido += "<div class='d-flex flex-row justify-content-center'>";
        contenido += "<div class='value-button' id='decrease' onclick='decreaseValue(" + data[i].product_id + ")' value='Decrease Value'>-</div>";
        contenido += "<input type='number' id='number" + data[i].product_id + "' class='number' value='0' />";
        contenido += "<div class='value-button' id='increase' onclick='increaseValue(" + data[i].product_id + ")' value='Increase Value'>+</div>";
        contenido += "</div>";
        contenido += "</td>";
        contenido += "<td>";
        contenido += "<div class='d-flex justify-content-center'>";
        contenido += "<button class='btn btn-success' onclick='addToCart(" + data[i].product_id + ")'><i class='bi bi-plus-circle'></i></button>";
        contenido += "</div>";
        contenido += "</td>";
        contenido += "</tr>";
    }
    $("#contentTable").html(contenido);
}

function increaseValue(id) {
    let value = parseInt(document.getElementById('number' + id).value, 10);
    value = isNaN(value) ? 0 : value;
    value++;
    document.getElementById('number' + id).value = value;
}

function decreaseValue(id) {
    let value = parseInt(document.getElementById('number' + id).value, 10);
    value = isNaN(value) ? 0 : value;
    value < 1 ? value = 1 : '';
    value--;
    document.getElementById('number' + id).value = value;
}

function addToCart(id) {
    $.get("AddToCart/?id_prod=" + id + "&quant=" + $("#number" + id).val(), function (data) {
        $("#basketCount").html(data);
    });
}

$('#btnFilter').on('click', function (ev) {
    let id_brand = $('#id_brand').val();
    let id_category = $('#id_category').val();
    let id_subcategory = $('#id_subcategory').val();
    let id_color = $('#id_color').val();
    let id_capacity = $('#id_capacity').val();
    
    $.get("FilterProducts/?id_brand=" + id_brand + "&id_category=" + id_category + "&id_subcategory=" + id_subcategory + "&id_color=" + id_color + "&id_capacity=" + id_capacity, function (data) {
        createTable(data);
    });
});