$(document).ready(function () {

    printJsonData();    

    function printJsonData() {
        var oldData = JSON.stringify(JSON.parse($("#old").val()), null, 4);  
        var newData = JSON.stringify(JSON.parse($("#new").val()), null, 4); 

        document.getElementById("old-data").innerHTML = oldData;
        document.getElementById("new-data").innerHTML = newData;
    }

});