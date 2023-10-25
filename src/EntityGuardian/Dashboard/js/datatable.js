$(document).ready(function () {
    $('#entity-guardian-table').DataTable({
        processing: true,
        serverSide: true,
        order: [[5, "desc"]],
        ajax:
        {
            draw: 1,
            start: 0,
            length: 10,
            url: "/data.html",
            dataSrc: "resultObject",
            data: function (request) {

            },
            dataFilter: function (data) {
                var json = jQuery.parseJSON(data);
                json.recordsTotal = json.dataCount;
                json.recordsFiltered = json.dataCount;
                return JSON.stringify(json);
            }
        },
        columns: [
            {
                data: "guid",
                render: function (data, type, row) {
                    return row.guid;
                }
            },
            {
                data: "username",
                render: function (data, type, row) {
                    return row.username;
                }
            },
            {
                data: "ipAddress",
                render: function (data, type, row) {
                    return row.ipAddress;
                }
            },
            {
                data: "targetName",
                render: function (data, type, row) {
                    return row.targetName;
                }
            },
            {
                data: "methodName",
                render: function (data, type, row) {
                    return row.methodName;
                }
            },
            {
                data: "transactionDate",
                render: function (data, type, row) {
                    return row.transactionDate;
                }
            },
            {
                width: "10%",
                searching: false,
                sorting: false,
                render: function (data, type, row) {
                    return "<a href='/details.html?guid=" + row.guid + "'class='btn btn-warning btn-icon-split btn-sm'><span class='icon'><i class='fas fa-arrow-right'></i></span><span class='text'>Details</span></a>";
                }
            }
        ]
    });
});