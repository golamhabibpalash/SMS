
$(function () {
    $('#myTable').DataTable({
        "stateSave": true,

        stateSaveCallback: function (settings, data) {
            localStorage.setItem('DataTables_' + settings.sInstance, JSON.stringify(data))
        },
        stateLoadCallback: function (settings) {
            return JSON.parse(localStorage.getItem('DataTables_' + settings.sInstance))
        },
        "autoWidth": false,
        "bAutoWidth": false,
        "lengthMenu": [[30, 50, 100, -1], [30, 50, 100, "All"]],
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
        }]
    });
});

