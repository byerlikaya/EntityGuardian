$(document).ready(function () {
    $("#entity-guardian-table").DataTable({
        processing: true,
        serverSide: true,
        order: [[5, "desc"]],
        ajax:
        {
            draw: 1,
            start: 0,
            length: 10,
            url: "data.html?type=changewrappers",
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
                    return "<a href='change-wrapper-detail.html?guid=" + row.guid + "'class='btn btn-warning btn-icon-split btn-sm'><span class='icon'><i class='fas fa-arrow-right'></i></span><span class='text'>Details</span></a>";
                }
            }
        ]
    });

    function getUrl() {
        var guid = $("#guid").data("guid");

        return "data.html?type=changes&guid=" + guid;
    }

    $("#entity-guardian-detail-table").DataTable({
        processing: true,
        serverSide: true,
        ajax:
        {
            draw: 1,
            start: 0,
            length: 10,
            url: getUrl(),
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
                data: "actionType",
                render: function (data, type, row) {
                    return row.actionType;
                }
            },
            {
                data: "entityName",
                render: function (data, type, row) {
                    return row.entityName;
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
                    return "<a href='change-detail.html?guid=" + row.guid + "'class='btn btn-warning btn-icon-split btn-sm'><span class='icon'><i class='fas fa-arrow-right'></i></span><span class='text'>Details</span></a>";
                }
            }
        ]
    });
});