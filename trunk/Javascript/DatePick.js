function LoadDatePick(ClientID, idioma, Fecha) {
    jQuery(function ($) {
        $.datepicker.setDefaults($.datepicker.regional[idioma]);
    });
    
    $("#" + ClientID).after("<input type='hidden' name='hd" + ClientID + "' id='hd" + ClientID + "'>");
    
    $("#" + ClientID).datepicker({
        defaultDate: "+1w",
        changeMonth: true,
        numberOfMonths: 1,
        onSelect: function (selectedDate) {
            $("#hd" + ClientID).val(selectedDate);
        }
    });
    var date = Fecha.split("/");
    date = new Date(date[2], date[1] - 1, date[0]);    
    $("#" + ClientID).datepicker("setDate", date );
    $("#" + ClientID).val(Fecha);

}