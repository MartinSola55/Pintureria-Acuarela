const header = ["Producto", "Marca", "Categoría", "Subcategoría", "Color", "Capacidad", "Cantidad", "Añadir"]

function createTable(data) {
    let content = "";
    content += "<table id='tabla-generic' class='container table table-light table-striped table-bordered table-hover'>";
    content += "<thead class='table-dark'>";
    content += "<tr class='fw-bold'>";
    for (let i = 0; i < header.length; i++) {
        content += "<td class='text-center'>";
        content += header[i];
        content += "</td>";
    }
    content += "</tr>";
    content += "</thead>";
    content += "<tbody>";
    for (let i = 0; i < data.length; i++) {
        content += "<tr>";
        content += "<td>" + data[i].product ?? "" + "</td>";
        content += "<td>" + data[i].brand + "</td>";
        let category = data[i].category != null ? data[i].category : "";
        content += "<td>" + category + "</td>";
        let subcategory = data[i].subcategory != null ? data[i].subcategory : "";
        content += "<td>" + subcategory + "</td>";
        if (data[i].color != null) {
            content += "<td>";
            content += "<div class='d-flex flex-row justify-content-between align-items-center' >";
            content += data[i].color;
            content += "<span class='dot' style='background-color: " + data[i].hex_color + "'></span>";
            content += "</div>";
            content += "</td>";
        }
        let capacity = data[i].capacity != null ? data[i].capacity : "";
        content += "<td>" + capacity + "</td>";
        content += "<td>";
        content += "<div class='d-flex flex-row justify-content-center'>";
        content += "<div class='value-button' id='decrease' onclick='decreaseValue()' value='Decrease Value'>-</div>";
        content += "<input type='number' id='number' value='0' />";
        content += "<div class='value-button' id='increase' onclick='increaseValue()' value='Increase Value'>+</div>";
        content += "</div>";
        content += "</td>";
        content += "<td>";
        content += "<div class='d-flex justify-content-center'>";
        content += "<button class='btn btn-success' onclick='addToCart(" + data[i].product_id + ")'><i class='bi bi-plus-circle'></i></button>";
        content += "</div>";
        content += "</td>";
        content += "</tr>";
    }
    content += "</tbody>";
    content += "</table>";
    $("#contentTable").html(content);
}

function increaseValue() {
    let value = parseInt(document.getElementById('number').value, 10);
    value = isNaN(value) ? 0 : value;
    value++;
    document.getElementById('number').value = value;
}

function decreaseValue() {
    let value = parseInt(document.getElementById('number').value, 10);
    value = isNaN(value) ? 0 : value;
    value < 1 ? value = 1 : '';
    value--;
    document.getElementById('number').value = value;
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