
$(function () {
    $('#myTable').DataTable({
        "responsive": true,
        "stateSave": true,
        "pagingType": "full_numbers",
        "lengthMenu": [[30, 50, 100, -1], [30, 50, 100, "All"]],
        //dom: 'Bfrtip',
        "dom": '<"top"Bf>rt<"bottom"lip><"clear">',
        buttons: [
            'copy', 'csv', 'excel', 'pdf', 'print','pageLength'
        ],
        stateSaveCallback: function (settings, data) {
            localStorage.setItem('DataTables_' + settings.sInstance, JSON.stringify(data))
        },
        stateLoadCallback: function (settings) {
            return JSON.parse(localStorage.getItem('DataTables_' + settings.sInstance))
        },
        "autoWidth": false,
        "bAutoWidth": false,
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
        }]
    });
});
