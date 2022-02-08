$(document).ready(function () {
    function checkAll() {
        const checked = document.getElementById("checkAll").checked;
        const checkboxes = document.getElementsByName("SelectedIds");
        for(let i = 0; i < checkboxes.length ; i++)
            checkboxes[i].checked = checked;
    }
    $("#checkAll").on("change", checkAll);
});