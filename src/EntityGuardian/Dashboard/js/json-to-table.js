$(document).ready(function () {
    processJson();

    highlight();
});

function highlight() {
    const firstTable = document.querySelectorAll("#old_data_table td");
    const secondTable = document.querySelectorAll("#new_data_table td");

    // loop on one of the table if both are of identical structure, else it is pointless
    for (let i = 0; i < firstTable.length; i++) {
        if (firstTable[i].textContent !== secondTable[i].textContent) {
            secondTable[i].classList.add('highlight');// here do what you need to when not equal
        }
        // else { /* do what you need to if equal*/ }
    }
}

function processJson() {

    var o = $("#json_old_data").val();
    if (o != "") {
        $("#old_data").html(buildTable(getOldJsonVar(),"old_data_table"));
        showOldTree();
    }

    var n = $("#json_new_data").val();
    if (n != "") {
        $("#new_data").html(buildTable(getNewJsonVar(),"new_data_table"));
        showNewTree();
    }     
}

function getOldJsonVar() {
    try {
        var a = $.parseJSON($("#json_old_data").val());
        $("#json_old_data").val(JSON.stringify(a, void 0, 2));
        return a
    } catch (e) {
        return $("#error_msg").text(e.message),
            $("#errorModal").modal("show"),
            {}
    }
}
function getNewJsonVar() {
    try {
        var a = $.parseJSON($("#json_new_data").val());
        $("#json_new_data").val(JSON.stringify(a, void 0, 2));
        return a
    } catch (e) {
        return $("#error_msg").text(e.message),
            $("#errorModal").modal("show"),
            {}
    }
}
function showOldTree() {  
    var a = document.createElement("ol")
        , e = document.createElement("li")
        , d = "_" + Math.random().toString(36).substr(2, 9);
    e.innerHTML = "<label for='" + d + "' class='lbl_obj'>&nbsp;</label> <input type='checkbox' id='" + d + "' />";
    d = document.createElement("ol");
    e.appendChild(d);
    a.appendChild(e);
    buildTree(getOldJsonVar(), 0, d);
    $("#inner_tree_old").html(a)
}

function showNewTree() {
    var a = document.createElement("ol")
        , e = document.createElement("li")
        , d = "_" + Math.random().toString(36).substr(2, 9);
    e.innerHTML = "<label for='" + d + "' class='lbl_obj'>&nbsp;</label> <input type='checkbox' id='" + d + "' />";
    d = document.createElement("ol");
    e.appendChild(d);
    a.appendChild(e);
    buildTree(getNewJsonVar(), 0, d);
    $("#inner_tree_new").html(a)
}

function buildTable(a,id) {
    var e = document.createElement("table"), d, b;
    e.className = "table";
    e.id = id;
    if (isArray(a))
        return buildArray(a);
    for (var c in a)
        "object" != typeof a[c] || isArray(a[c]) ? "object" == typeof a[c] && isArray(a[c]) ? (d = e.insertRow(-1),
            b = d.insertCell(-1),
            b.colSpan = 2,
            b.innerHTML = encodeText(c) + '<table class="table">' + $(buildArray(a[c]), !1).html() + "</table>") : (d = e.insertRow(-1),
                b = d.insertCell(-1),
                b.innerHTML = encodeText(c),
                d = d.insertCell(-1),
                d.innerHTML = encodeText(a[c])) : (d = e.insertRow(-1),
                    b = d.insertCell(-1),
                    b.colSpan = 2,
                    b.innerHTML = encodeText(c) + '<table class="table">' + $(buildTable(a[c]), !1).html() + "</table>");
    return e
}
function buildArray(a) {
    var e = document.createElement("table"), d, b, c = !1, p = !1, m = {}, h = -1, n = 0, l;
    l = "";
    if (0 == a.length)
        return "<div></div>";
    d = e.insertRow(-1);
    for (var f = 0; f < a.length; f++)
        if ("object" != typeof a[f] || isArray(a[f]))
            "object" == typeof a[f] && isArray(a[f]) ? (b = d.insertCell(h),
                b.colSpan = 2,
                b.innerHTML = '<table class="table">' + $(buildArray(a[f]), !1).html() + "</table>",
                c = !0) : p || (h += 1,
                    p = !0,
                    b = d.insertCell(h),
                    m.empty = h,
                    b.innerHTML = "<div>&nbsp;</div>");
        else
            for (var k in a[f])
                l = "-" + k,
                    l in m || (c = !0,
                        h += 1,
                        b = d.insertCell(h),
                        m[l] = h,
                        b.innerHTML = encodeText(k));
    c || e.deleteRow(0);
    n = h + 1;
    for (f = 0; f < a.length; f++)
        if (d = e.insertRow(-1),
            "object" != typeof a[f] || isArray(a[f]))
            if ("object" == typeof a[f] && isArray(a[f]))
                for (h = m.empty,
                    c = 0; c < n; c++)
                    b = d.insertCell(c),
                        l = c == h ? '<table class="table">' + $(buildArray(a[f]), !1).html() + "</table>" : " ",
                        b.innerHTML = encodeText(l);
            else
                for (h = m.empty,
                    c = 0; c < n; c++)
                    b = d.insertCell(c),
                        l = c == h ? a[f] : " ",
                        b.innerHTML = encodeText(l);
        else {
            for (c = 0; c < n; c++)
                b = d.insertCell(c),
                    b.innerHTML = "<div>&nbsp;</div>";
            for (k in a[f])
                c = a[f],
                    l = "-" + k,
                    h = m[l],
                    b = d.cells[h],
                    "object" != typeof c[k] || isArray(c[k]) ? "object" == typeof c[k] && isArray(c[k]) ? b.innerHTML = '<table class="table">' + $(buildArray(c[k]), !1).html() + "</table>" : b.innerHTML = encodeText(c[k]) : b.innerHTML = '<table class="table">' + $(buildTable(c[k]), !1).html() + "</table>"
        }
    return e
}
function encodeText(a) {
    return $("<div />").text(a).html()
}
function isArray(a) {
    return "[object Array]" === Object.prototype.toString.call(a)
}

function buildTree(a, e, d) {
    e += 1;
    if ("undefined" === typeof a)
        log("undef!!", e);
    else
        for (var b in a)
            if ("object" == typeof a[b]) {
                var c = addTree(b, d, isArray(a[b]));
                buildTree(a[b], e, c)
            } else
                addLeaf(b, a, d)
}
function addTree(a, e, d) {
    var b = "lbl_obj";
    d && (b = "lbl_array");
    var c = "_" + Math.random().toString(36).substr(2, 9);
    d = document.createElement("li");
    d.innerHTML = "<label for='" + c + "' class='" + b + "'>" + encodeText(a) + "</label> <input type='checkbox' id='" + c + "' />";
    a = document.createElement("ol");
    d.appendChild(a);
    null != e && e.appendChild(d);
    return a
}
function addLeaf(a, e, d) {
    var b = "";
    isArray(e) || (b = a + " : ");
    b += e[a];
    Math.random().toString(36).substr(2, 9);
    a = document.createElement("li");
    a.className = "file";
    a.innerHTML = "<a>" + encodeText(b) + "</a>";
    d.appendChild(a)
}
function log(a, e, d) {
    console.log(a)
}
