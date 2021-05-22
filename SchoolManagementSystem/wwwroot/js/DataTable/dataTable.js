
$(function () {
    $('#myTable').DataTable({
        "autoWidth": false,
        "bAutoWidth": false,
        "lengthMenu": [[30, 50, 100, -1], [30, 50, 100, "All"]],
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
        }]
    });
});

