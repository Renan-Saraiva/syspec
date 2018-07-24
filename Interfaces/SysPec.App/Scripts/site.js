var language = {
    lengthMenu: "Exibir _MENU_ registros por página",
    zeroRecords: "Não foram encontrados registros",
    info: "Exibindo página _PAGE_ de _PAGES_ (Total de Registros: _MAX_)",
    infoEmpty: "Nenhum registro encontrado",
    infoFiltered: "(filtrado de _MAX_ registros)",
    search: "Pesquisar",
    paginate: {
        first: "Primeira",
        previous: "Anterior",
        next: "Próxima",
        last: "Última"
    }
};

$.extend(true, $.fn.dataTable.defaults, {
    select: true,
    responsive: true,
    language: language
});