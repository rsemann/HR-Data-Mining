function showModal(self, id) {
    $.ajax(
    {
        url: $(self).data('url') + "?id=" + id,
        type: 'GET',
        cache: false,
        success: function (result) {
            var div = document.createElement('div');
            document.body.appendChild(div);
            $(div).html(result);
            $(div).modal('show');
        },
        error: function (error) {
        }
    });
}

function buscarDefinicaoCampos() {
    $.ajax(
{
    url: "/Comando/BuscarDefinicaoCampos?sql=" + document.getElementById("sql").value,
    type: 'GET',
    cache: false,
    success: function (result) {
        $('#definicoesCampos').html(result);
    },
    error: function (error) {
    }
});
}