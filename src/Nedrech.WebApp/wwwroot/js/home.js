$(document).ready(function () {
    $("#checkedAll").click(function () {
        $("input:checkbox").not(this).prop("checked", this.checked);
        $(".btn-act").prop("disabled", !this.checked);
    });
    $(".checkedSingle").click(function () {
        let isAllChecked = 1;
        let someOneChecked = 0;
        $(".checkedSingle").each(function () {
            if (this.checked)
                someOneChecked = 1;
            else
                isAllChecked = 0;
        });
        if (isAllChecked)
            $("#checkedAll").prop("checked", true)
                .prop("indeterminate", false);
        else if (someOneChecked) {
            $("#checkedAll").prop("indeterminate", true);
            $(".btn-act").prop("disabled", false);
        }
        else {
            $("#checkedAll").prop("checked", false)
                .prop("indeterminate", false);
            $(".btn-act").prop("disabled", true);
        }
    });
});