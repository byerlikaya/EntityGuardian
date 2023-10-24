$(document).ready(function () {
    $('#entity-guardian-table').DataTable({
        "searching": false,
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "draw": 1,
            "start": 1,
            "length": 10,
            "dataFilter": function (data) {
                var json = jQuery.parseJSON(data);
                json.recordsTotal = json.dataCount;
                json.recordsFiltered = json.dataCount;
                return JSON.stringify(json);
            }
        }
    });
});