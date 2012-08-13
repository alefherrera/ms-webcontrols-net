//webservice: url del webservice
//tipo: tipo de dato; nombre de la clase
//txtBoxID: textbox al que se le aplican las cosas
//SourceCompleto: bool
//minFilter: a partir de donde empezar a buscar en BD
//FiltroID: para filtrar por id directamente, default -1
//ParentID: en el caso que el combo dependa de otro, como por ejemplo Empresas - Personas, mandar aca el id de ese combo para que lo tome como referencia, default -1
function LoadCombo(webservice, tipo, txtBoxID, SourceCompleto, minFilter, FiltroID, ParentID, value, PlaceHolder, DataSource, ValorDefault) {
    //Genero un hidden por cada combo que cree en el HTML, para guardar el valor, a la vez recibo
    //ese valor porque cada vez que se crea nuevamente en los postback, pierden el valor y se los dejo por default
    var datos;

    //Para IE7 en donde no existe la libreria JSON y necesito usarla para mandar los parametros por AJAX al WebService
    if (!JSON) {
        var JSON = new Object();
        JSON.stringify = JSON.stringify || function (obj) {
            var t = typeof (obj);
            if (t != "object" || obj === null) {
                // simple data type
                if (t == "string") obj = '"' + obj + '"';
                return String(obj);
            }
            else {
                // recurse array or object
                var n, v, json = [], arr = (obj && obj.constructor == Array);
                for (n in obj) {
                    v = obj[n]; t = typeof (v);
                    if (t == "string") v = '"' + v + '"';
                    else if (t == "object" && v !== null) v = JSON.stringify(v);
                    json.push((arr ? "" : '"' + n + '":') + String(v));
                }
                return (arr ? "[" : "{") + String(json) + (arr ? "]" : "}");
            }
        };
    }

    //Objeto de la clase Filtros que es la misma que esta en el wcf, tiene que coincidir siempre
    var parameters = { oFiltro: { Filtro: "", FiltroID: FiltroID, FiltroParentID: -1, SourceCompleto: SourceCompleto, Tipo: tipo} };
    $("#" + txtBoxID).after("<input type='hidden' name='hd" + txtBoxID + "' id='hd" + txtBoxID + "' value='" + value + "'>");
    $("#" + txtBoxID).click(function () { this.select(); });
    $("#" + txtBoxID).keypress(function () { parameters.oFiltro.FiltroParentID = $('#hd' + ParentID).val() == null ? -1 : $('#hd' + ParentID).val(); });
    $("#" + txtBoxID).focus(function () { if ($(this).val() == PlaceHolder) { $(this).removeClass("defaultTextActive"); $(this).val(""); } });
    $("#" + txtBoxID).blur(function () { if ($(this).val() == "") { $(this).addClass("defaultTextActive"); $(this).val(PlaceHolder); $('#hd' + txtBoxID).val(ValorDefault); } });
    $(document).ready(function () { $("#" + txtBoxID).focus(); $("#" + txtBoxID).blur(); });
    $("#" + txtBoxID).autocomplete({
        //cuando se posiciona sobre el combo
        focus: function (event, ui) {
            $(this).val(ui.item.label);
            return false;
        },
        //Evento que ocurre cuando selecciona algo en el combo
        select: function (event, ui) {
            if (ui.item) {
                $('#hd' + txtBoxID).val(ui.item.desc);
            }
            else {
                $('#hd' + txtBoxID).val('-1');
                alert("No se selecciono ningun valor.");
            }
        },
        //a partir de que caracter va a empezar a buscar
        minLength: minFilter,
        //Eventos que ocurren cuando abre o cierra la lista de opciones
        open: function () {
            $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
        },
        close: function () {
            $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
        }
    });

    if (DataSource != null) {
        $.map(DataSource, function (a) { if (a.Value == value) $("#" + txtBoxID).val(a.Text); })
        datos = $.map(DataSource,function (item) {
            return {
                label: item.Text,
                value: item.Text,
                desc: item.Value
            }
        });
        $("#" + txtBoxID).autocomplete({ source: datos });
    }
    else {
        //No Filtra en el webservice, trae todo y busca en el datasource que genera la primera vez
        if (SourceCompleto == true) {
            $.ajax({

                url: webservice + "Llenar",
                data: JSON.stringify(parameters),
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataFilter: function (data) { return data; },
                success: function (data) {
                    datos = $.map(data,function (item) {
                        return {
                            label: item.Text,
                            value: item.Text,
                            desc: item.Value
                        }
                    });
                    $("#" + txtBoxID).autocomplete({ source: datos });
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $('#hd' + txtBoxID).val('-1');
                    alert(XMLHttpRequest + textStatus + errorThrown);
                }
            });

        }
        //Ingresa a la base de datos cada vez que deja de escribir y busca el resultado
        else {

            $("#" + txtBoxID).autocomplete({ source: function (request, response) {
                parameters.oFiltro.Filtro = request.term;
                $.ajax({

                    url: webservice + "Llenar",
                    data: JSON.stringify(parameters),
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataFilter: function (data) { return data; },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.Text,
                                value: item.Text,
                                desc: item.Value
                            }
                        }))
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        $('#hd' + txtBoxID).val('-1');
                        alert(XMLHttpRequest + textStatus + errorThrown);
                    }
                });
            }
            });
        }
    }



}