function LoadExporter(ExporterID, GridID) {
    grid = $("#" + GridID);
    griddiv = $("#" + GridID + "_info");
    exporter = $("#" + ExporterID);
    if (grid.find("tbody:first > tr").length > 0) {
        exporter.show();
    } else exporter.hide();
    
    griddiv.after(exporter);

}