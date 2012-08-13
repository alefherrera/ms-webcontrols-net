function LoadDatePicker(ClientID, idioma, FechaDesde, FechaHasta) {
    jQuery(function ($) {
        $.datepicker.setDefaults($.datepicker.regional[idioma]);
    });
    
    $("#" + ClientID + "from").after("<input type='hidden' name='hd" + ClientID + "from' id='hd" + ClientID + "from'>");
    $("#" + ClientID + "to").after("<input type='hidden' name='hd" + ClientID + "to' id='hd" + ClientID + "to'>");
    $("#" + ClientID + "from").datepicker({
        defaultDate: "+1w",
        changeMonth: true,
        numberOfMonths: 1,
        onSelect: function (selectedDate) {
            $("#hd" + ClientID + "from").val(selectedDate);
            $("#" + ClientID + "to").datepicker("option", "minDate", selectedDate);
        }
    });
    $("#" + ClientID + "to").datepicker({
        defaultDate: "+1w",
        changeMonth: true,
        numberOfMonths: 1,
        onSelect: function (selectedDate) {
            $("#hd" + ClientID + "to").val(selectedDate);
            $("#" + ClientID + "from").datepicker("option", "maxDate", selectedDate);
        }
    });
    var from = FechaDesde.split("/");
    from = new Date(from[2], from[1] - 1, from[0]);
    var to = FechaHasta.split("/");
    to = new Date(to[2], to[1] - 1, to[0]);
    
    $("#" + ClientID + "from").datepicker("setDate", from );
    $("#" + ClientID + "to").datepicker("option", "minDate", from);
    $("#" + ClientID + "to").datepicker("setDate", to);
    $("#" + ClientID + "from").datepicker("option", "maxDate", to);
    $("#" + ClientID + "from").val(FechaDesde);
    $("#" + ClientID + "to").val(FechaHasta);
}