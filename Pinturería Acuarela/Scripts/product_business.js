
let header = ["Codigo Interno", "Descripcion", "Marca", "Categoria", "Subcategoria", "Color", "Capacidad", "Operadores"];

let btBuscar = document.getElementById("btSearch");
btBuscar.onclick = function () {
    let txtBuscar = document.getElementById("txtSearch").value;
    $.get("../Products/FilterSearch/?nom=" + txtBuscar, function (date) {
        if (date.length != 0) {
            createTable(date, header);
        }
        else { $("#contentTable").html("No se encontro el producto"); }
    })
}

function FilterProducts(){
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
}
// NO HACER OTRA TABLA, APRETAR SELECCIONAR Y QUE SALGA UN MODAL O ALGO
// PARA PONER LA CANTIDAD, LUEGO DE ACEPTAR Y SE AGREGA A porducts_businnes
function addProduct(BUSINESS_ID) {
    let PRODUCT_ID = document.getElementById("idProduct").value;
    let STOCK = document.getElementById("numCant").value;
    let MINSTOCK = document.getElementById("numMinCant").value;
    var frm = new FormData();
    frm.append("id_product", PRODUCT_ID);
    frm.append("stock", STOCK);
    frm.append("minimum_stock", MINSTOCK);
    frm.append("id_business", BUSINESS_ID);
    $.ajax({
        type: "POST",
        url: "../Product_Business/AddProduct",
        data: frm,
        contentType: false,
        processData: false,
        success: function (data) {
            
        }
    });
}
function OpenModal(id) {
    document.getElementById("idProduct").value = id;
    //let content = "";
    //document.getElementById("numCant").value = 1;
    //$('#numCant').value = 1;
    /*
    content += "<div class='col-12'>";
    content += "<div class='d-flex justify-content-between'>";
    content += "<div>";
    content += "<p class='text-dark'>Cantidad</p>";
    content += "</div>";
    content += "<div class='input-group w-auto justify-content-end align-items-center'>";
    content += "<input type='button' value='-' class='button-minus border rounded-circle icon-shape icon-sm mx-1' data-field='quantity'>";
    content += "<input type='number' step='1' max='10' value='1' name='quantity' class='quantity-field border-0 text-center w-25'>";
    content += "<input type='button' value='+' class='button-plus border rounded-circle icon-shape icon-sm' data-field='quantity'>";
    content += "</div>";
    content += "</div>";
    content += "</div>";
    content += "<div class='col-12'>";
    content += "<div class='d-flex justify-content-between'>";
    content += "<div>";
    content += "<p class='text-dark'>Stock Minimo</p>";
    content += "</div>";
    content += "<div class='input-group w-auto justify-content-end align-items-center'>";
    content += "<input type='button' value='-' class='button-minus border rounded-circle icon-shape icon-sm mx-1' data-field='quantity'>";
    content += "<input type='number' step='1' max='10' value='1' name='quantity' class='quantity-field border-0 text-center w-25'>";
    content += "<input type='button' value='+' class='button-plus border rounded-circle icon-shape icon-sm' data-field='quantity'>";
    content += "</div>";
    content += "</div>";
    content += "</div>";
    $("#contentBntIncrement").html(content);*/
}

function createTable(data, header) {
    let content = "";
    content += "<table class='table table-light table-bordered table-hover' align='center'>";
    content += "<thead class='table-success'>";
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
        content += "<td>" + (data[i].category != null ? data[i].category : "-") + "</td>";
        content += "<td>" + (data[i].subcategory != null ? data[i].subcategory : "-") + "</td>";
        if (data[i].color != null) {
            content += "<td>";
            content += "<div class='d-flex flex-row justify-content-between align-items-center' >";
            content += data[i].color;
            content += "<span class='dot' style='background-color: " + data[i].rgb_hex_code + "'></span>";
            content += "</div>";
            content += "</td>";
        } else { content += "<td></td>"; }
        content += "<td>" + (data[i].capacity != null ? data[i].capacity : "-") + "</td>";
        content += "<td>";
        content += "<div class='d-flex flex-row justify-content-center'>";
        content += "<button type='button' onclick = 'OpenModal(" + data[i].id +");' class='btn btn-primary' data-bs-toggle='modal' data-bs-target='#exampleModal'>Seleccionar</button>";
        content += "</div>";
        content += "</td>";
        content += "</tr>";
    }
    content += "</tbody>";
    content += "</table>";
    $("#contentTable").html(content);
}

function incrementValue(e) {
    e.preventDefault();
    var fieldName = $(e.target).data('field');
    var parent = $(e.target).closest('div');
    var currentVal = parseInt(parent.find('input[name=' + fieldName + ']').val(), 10);

    if (!isNaN(currentVal)) {
        parent.find('input[name=' + fieldName + ']').val(currentVal + 1);
    } else {
        parent.find('input[name=' + fieldName + ']').val(0);
    }
}

function decrementValue(e) {
    e.preventDefault();
    var fieldName = $(e.target).data('field');
    var parent = $(e.target).closest('div');
    var currentVal = parseInt(parent.find('input[name=' + fieldName + ']').val(), 10);

    if (!isNaN(currentVal) && currentVal > 0) {
        parent.find('input[name=' + fieldName + ']').val(currentVal - 1);
    } else {
        parent.find('input[name=' + fieldName + ']').val(0);
    }
}

$('.input-group').on('click', '.button-plus', function (e) {
    incrementValue(e);
});

$('.input-group').on('click', '.button-minus', function (e) {
    decrementValue(e);
});

