$(document).ready(function () {

    $("#targetName").keyup(function () {
        $('#entity-guardian-table').DataTable().ajax.reload();
    });

    $("#methodName").keyup(function () {
        $('#entity-guardian-table').DataTable().ajax.reload();
    });

    $("#username").keyup(function () {
        $('#entity-guardian-table').DataTable().ajax.reload();
    });

    $("#ipaddress").keyup(function () {
        $('#entity-guardian-table').DataTable().ajax.reload();
    });

    $("#order").keyup(function () {
        $('#entity-guardian-detail-table').DataTable().ajax.reload();
    });

    $("#transactionType").keyup(function () {
        $('#entity-guardian-detail-table').DataTable().ajax.reload();
    });

    $("#entityName").keyup(function () {
        $('#entity-guardian-detail-table').DataTable().ajax.reload();
    });
});