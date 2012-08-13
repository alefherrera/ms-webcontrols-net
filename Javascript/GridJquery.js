//Ordenar Numeros Float
jQuery.fn.dataTableExt.aTypes.unshift(
    function (sData) {
        var sValidChars = "0123456789,.";
        var Char;
        var bDecimal = false;
        var iStart = 0;

        /* Negative sign is valid -  the number check start point */
        if (sData.charAt(0) === '-') {
            iStart = 1;
        }

        /* Check the numeric part */
        for (i = iStart; i < sData.length; i++) {
            Char = sData.charAt(i);
            if (sValidChars.indexOf(Char) == -1) {
                return null;
            }
        }

        return 'numeric-comma';
    }
);

jQuery.fn.dataTableExt.oSort['numeric-comma-asc'] = function (a, b) {
    var x = (a == "-") ? 0 : a.replace(/,/, ".");
    var y = (b == "-") ? 0 : b.replace(/,/, ".");
    x = parseFloat(x);
    y = parseFloat(y);
    return ((x < y) ? -1 : ((x > y) ? 1 : 0));
};

jQuery.fn.dataTableExt.oSort['numeric-comma-desc'] = function (a, b) {
    var x = (a == "-") ? 0 : a.replace(/,/, ".");
    var y = (b == "-") ? 0 : b.replace(/,/, ".");
    x = parseFloat(x);
    y = parseFloat(y);
    return ((x < y) ? 1 : ((x > y) ? -1 : 0));
};

//Ordenar Fecha con formato dd/mm/yyyy
jQuery.extend(jQuery.fn.dataTableExt.oSort, {
    "date-eu-pre": function (date) {
        var date = date.replace(" ", "");

        if (date.indexOf('-') > 0) {
            var eu_date = date.split('-');
        } else {
            var eu_date = date.split('/');
        }

        /*year*/
        var year = eu_date[2];
        if (year.length == 1) {
            year = 0 + year;
        }

        /*month*/
        var month = eu_date[1];
        if (month.length == 1) {
            month = 0 + month;
        }

        /*day*/
        var day = eu_date[0];
        if (day.length == 1) {
            day = 0 + day;
        }
        new Date(to[2], to[1] - 1, to[0]);

        return new Date(year, month - 1, day);
    },

    "date-eu-asc": function (a, b) {
        return ((a < b) ? -1 : ((a > b) ? 1 : 0));
    },

    "date-eu-desc": function (a, b) {
        return ((a < b) ? 1 : ((a > b) ? -1 : 0));
    }
});

jQuery.fn.dataTableExt.aTypes.unshift(
    function (sData) {
        if (sData !== null && sData.match(/^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[012])\/(19|20|21)\d\d$/)) {
            return 'date-eu';
        }
        return null;
    }
);




function LoadGrid(gridID, tituloExport, width) {
    var asInitVals = new Array();
    // Indicar al pluging Metadata que los metadatos deberá buscarlos en el atributo class.
    $.metadata.setType("class");

    var grid = $("#" + gridID);
    //var oTable;
    grid.before("<div id='div" + gridID + "' style='display: none;width:" + width + "; ' ></div>");
    $("#div" + gridID).append(grid);

    // Por cada GridView que se encuentre modificar el código HTML generado para agregar el THEAD.
    if (grid.find("tbody > tr > th").length > 0) {
        grid.find("tbody").before("<thead><tr></tr></thead>");
        grid.find("thead:first tr").append(grid.find("th"));
        grid.find("tbody tr:first").remove();
    }

    //        grid.find("tbody").after("<tfoot><tr></tr></tfoot>");

    //        grid.find("th").each(function () {
    //            var textbox = document.createElement("input");
    //            textbox.type = "text";
    //            var th = document.createElement("th");
    //            th.appendChild(textbox);
    //            grid.find("tfoot:first tr").append(th);
    //        });


    if (grid.find("tbody:first > tr").length > 0) {

        //oTable = 
        grid.dataTable({
            "bJQueryUI": true,
            "sPaginationType": "full_numbers",
            "sDom": '<"H"lfr>t<"F"ip>',
            "bAutoWidth": false,
            "oLanguage": {
                "sProcessing": "Procesando...",
                "sLengthMenu": "Mostrar _MENU_ registros",
                "sZeroRecords": "No se encontraron resultados",
                "sInfo": "Mostrando desde _START_ ate _END_ de _TOTAL_ registros",
                "sInfoEmpty": "Mostrando desde 0 hasta 0 de 0 registros",
                "sInfoFiltered": "(filtrado de _MAX_ registros en total)",
                "sInfoPostFix": "",
                "sSearch": "Pesquisar:",
                "sUrl": "",
                "oPaginate": {
                    "sFirst": "Primeira",
                    "sPrevious": "Anterior",
                    "sNext": "Proxima",
                    "sLast": "Ultima"
                }
            }
        });
    }
    grid.css("width", "100%");

    //        $("tfoot input").keyup(function () {
    //            /* Filter on the column (the index) of this element */
    //            oTable.fnFilter(this.value, $("tfoot input").index(this));
    //        });



    //        /*
    //        * Support functions to provide a little bit of 'user friendlyness' to the textboxes in 
    //        * the footer
    //        */
    //        $("tfoot input").each(function (i) {
    //            asInitVals[i] = this.value;
    //        });

    //        $("tfoot input").focus(function () {
    //            if (this.className == "search_init") {
    //                this.className = "";
    //                this.value = "";
    //            }
    //        });

    //        $("tfoot input").blur(function (i) {
    //            if (this.value == "") {
    //                this.className = "search_init";
    //                this.value = asInitVals[$("tfoot input").index(this)];
    //            }
    //        });


    $(document).ready(function () { $("#div" + gridID).slideDown("slow"); $('#' + gridID).show(); })

}

