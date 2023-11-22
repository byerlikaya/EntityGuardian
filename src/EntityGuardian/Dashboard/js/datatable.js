$(document).ready(function () {
    $("#entity-guardian-table").DataTable({
        processing: true,
        serverSide: true,
        searching: false,
        order: [[4, "desc"]],
        ajax:
        {
            draw: 1,
            start: 0,
            length: 10,
            url: "data.html?type=changewrappers",
            dataSrc: "resultObject",
            "data": function (d) {
               
                d.mainEntity = $("#mainEntity").val();
                d.transactionCount = $("#transactionCount").val();
                d.userName = $("#username").val();
                d.ipAddress = $("#ipaddress").val();
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
                data: "mainEntity",
                render: function (data, type, row) {
                    return row.mainEntity;
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
                data: "transactionCount",
                render: function (data, type, row) {
                    return row.transactionCount;
                }
            },
            {
                data: "transactionDate",
                render: function (data, type, row) {
                    return row.transactionDate;
                }
            },
            {
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
        searching: false,
        order: [[0, "asc"]],
        ajax:
        {
            draw: 1,
            start: 0,
            length: 10,
            url: getUrl(),
            dataSrc: "resultObject",
            "data": function (d) {

                d.queue = $("#queue").val();
                d.transactionType = $("#transactionType").val();
                d.entityName = $("#entityName").val();
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
                data: "rank",
                render: function (data, type, row) {
                    return row.rank;
                }
            },
            {
                data: "transactionType",
                render: function (data, type, row) {
                    return row.transactionType;
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
                searching: false,
                sorting: false,
                render: function (data, type, row) {
                    return "<a href='change-detail.html?guid=" + row.guid + "&change-wrapper-guid=" + row.changeWrapperGuid +"'class='btn btn-warning btn-icon-split btn-sm'><span class='icon'><i class='fas fa-arrow-right'></i></span><span class='text'>Details</span></a>";
                }
            }
        ]
    });
});