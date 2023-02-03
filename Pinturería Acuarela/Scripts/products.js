let btBuscar = document.getElementById("btSearch");
let header = ["Codigo Interno", "Descripcion", "Marca", "Categoria", "Subcategoria", "Color", "Capacidad","Cantidad","Operadores"];

btBuscar.onclick = function () {
    let txtBuscar = document.getElementById("txtSearch").value;
    $.get("../Products/FilterSearch/?nom=" + txtBuscar, function (date) {
        if (date.length != 0) {
            createTable(date, header);
        }
        else { $("#contentTable").html("No se encontro el producto"); }
    })
}

$('#btnFilter').on('click', function (ev) {
    let id_brand = $('#id_brand').val();
    let id_category = $('#id_category').val();
    let id_subcategory = $('#id_subcategory').val();
    let id_color = $('#id_color').val();
    let id_capacity = $('#id_capacity').val();

    $.get("../Products/FilterProducts/?id_brand=" + id_brand + "&id_category=" + id_category + "&id_subcategory=" + id_subcategory + "&id_color=" + id_color + "&id_capacity=" + id_capacity, function (data) {
        if (data.length != 0) {
            createTable(data, header);
        }
        else { $("#contentTable").html("No se encontro el producto"); }
    });
});   

function createTable(data, header) {
    let content = "";
    content += "<table class='table table-success table-striped table-bordered table-hover' align='center'>";
    content += "<thead class='table-dark'>";
    content += "<tr>";
    for (let i = 0; i < header.length; i++) {
        content += "<td class='text-center'>";
        content += header[i];
        content += "</td>";
    }
    content += "</tr>";
    content += "</thead>";
    content += "<tbody>";
    for (let i = 0; i < data.length; i++) {
        content += "<tr class='text-center'>";
        content += "<td>" + (data[i].internal_code != null ? data[i].internal_code : "-") + "</td>";
        content += "<td>" + (data[i].description != null ? data[i].description : "-") + "</td>";
        content += "<td>" + (data[i].brand != null ? data[i].brand : "-") + "</td>";
        content += "<td>" + (data[i].category != null ? data[i].category  : "-" )+ "</td>";
        content += "<td>" + (data[i].subcategory != null ? data[i].subcategory : "-") + "</td>";
        if (data[i].color != null) {
            content += "<td>";
            content += "<div class='d-flex flex-row justify-content-between align-items-center' >";
            content += data[i].color;
            content += "<span class='dot' style='background-color: " + data[i].rgb_hex_code + "'></span>";
            content += "</div>";
            content += "</td>";
        }else {content += "<td></td>";}
        content += "<td>" + (data[i].capacity != null ? data[i].capacity : "-") + "</td>";
        content += "<td>" + (data[i].quantity != null ? data[i].quantity : "-") + "</td>";
        content += "<td>";
        content += "<div class='d-flex flex-row justify-content-center'>";
        content += "<a class='btn btn-success me-4' href='Products/Edit/"+data[i].id+"'><i class='bi bi-pencil-square'></i></a>";
        content += "<a class='btn btn-danger ms-4' href='Products/Delete/"+data[i].id+"'><i class='bi bi-trash3'></i></a>";
        content += "</div>";
        content += "</td>";
        content += "</tr>";
    }
    content += "</tbody>";
    content += "</table>";
    $("#contentTable").html(content);
}