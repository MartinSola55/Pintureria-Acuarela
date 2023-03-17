function parseDate(date) {
    return moment(date).format("DD/MM/YYYY").toUpperCase()
}

let hoy = new Date();
let dd = String(hoy.getDate()).padStart(2, '0');
let mm = String(hoy.getMonth() + 1).padStart(2, '0');
let yyyy = hoy.getFullYear();
hoy = dd + '/' + mm + '/' + yyyy;
$("#date").removeAttr("data-val-date");


$("#datepicker").on("apply.daterangepicker", function (ev, picker) {
    let date_from = picker.startDate.format('YYYY-MM-DD');
    let date_to = picker.endDate.format('YYYY-MM-DD');
    let dates = [date_from, date_to];
    $("#dataTable").html("");
    $.get("../Sells/ShowSales/?dates=" + dates, function (data) {
        createTable(data);
    })
});

function createTable(data) {
    let content = "";
    content += `
        <thead class='table-dark'>
            <tr class="fw-bold text-center">
                <td>
                    Fecha venta
                </td>
            </tr>
        </thead>`;
    for (let i = 0; i < data.length; i++) {
        content += `
                    <tr class='clickable-row text-center' data-href='/Sells/DetailsSale/`+ data[i].id +`'>
                        <td>
                            Día: `+ data[i].date+`
                            <br>
                            Hora: `+ data[i].date +`
                        </td>
                   </tr>`;
    }
    $("#dataTable").append(content);
}

moment.locale('es');
$(function () {
    $("#datepicker").daterangepicker({
        "locale": {
            "applyLabel": "Aplicar",
            "cancelLabel": "Cancelar",
            "fromLabel": "Hasta",
            "toLabel": "Desde",
        },
        singleDatePicker: false,
        opens: 'right',
        autoUpdateInput: true,
        autoApply: false
    })
});