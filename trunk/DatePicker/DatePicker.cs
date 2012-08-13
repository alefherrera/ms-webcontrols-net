using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace AjaxServerControl
{
    [ToolboxData("<{0}:DatePicker runat='server'></{0}:DatePicker>")]
    public class DatePicker : WebControl
    {
        public string Desde
        {
            get { return ((ViewState["Desde"] == null) ? string.Empty : Convert.ToString(ViewState["Desde"])); }
            set { ViewState["Desde"] = value; }
        }

        public string Hasta
        {
            get { return ((ViewState["Hasta"] == null) ? string.Empty : Convert.ToString(ViewState["Hasta"])); }
            set { ViewState["Hasta"] = value; }
        }

        private string _Idioma = "pt-BR";

        public string Idioma
        {
            get { return _Idioma; }
            set { _Idioma = value; }
        }

        private string TextDesde;
        private string TextHasta;

        private string _CssTextRows;

        public string CssTextRows
        {
            get { return _CssTextRows; }
            set { _CssTextRows = value; }
        }

        private string _CssTextDatePicker;

        public string CssTextDatePicker
        {
            get { return _CssTextDatePicker; }
            set { _CssTextDatePicker = value; }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (Idioma == "pt-BR") { TextDesde = "Desde"; TextHasta = "Ate"; }
            using (PlaceHolder plh = new PlaceHolder())
            {
                //if (DesignMode || Page.Header == null)
                //RegisterCSSInclude(plh);
                Table table = new Table();
                table.CellPadding = 0;
                table.CellSpacing = 0;
                table.Rows.Add(new TableRow());
                table.Rows[0].Cells.Add(new TableCell());
                table.Rows[0].Cells.Add(new TableCell());
                table.Rows[0].Cells.Add(new TableCell());
                table.Rows[0].Cells.Add(new TableCell());
                table.Rows[0].Cells[0].Text = TextDesde;
                table.Rows[0].Cells[0].CssClass = CssTextRows;
                table.Rows[0].Cells[2].Text = TextHasta;
                table.Rows[0].Cells[2].CssClass = CssTextRows;
                HtmlGenericControl txtboxFrom = new HtmlGenericControl("input");
                txtboxFrom.EnableViewState = false;
                txtboxFrom.Attributes.Add("maxlength", "10");
                txtboxFrom.Style.Add("width", "70px");
                txtboxFrom.Attributes.Add("id", ClientID + "from");
                txtboxFrom.Attributes.Add("name", UniqueID + "from");
                txtboxFrom.Attributes.Add("readonly", "readonly");
                table.Rows[0].Cells[1].CssClass = CssTextDatePicker;
                table.Rows[0].Cells[1].Controls.Add(txtboxFrom);
                HtmlGenericControl txtboxTo = new HtmlGenericControl("input");
                txtboxTo.EnableViewState = false;
                txtboxTo.Attributes.Add("maxlength", "10");
                txtboxTo.Style.Add("width", "70px");
                txtboxTo.Attributes.Add("id", ClientID + "to");
                txtboxTo.Attributes.Add("name", UniqueID + "to");
                txtboxTo.Attributes.Add("readonly", "readonly");
                table.Rows[0].Cells[3].CssClass = CssTextDatePicker;
                table.Rows[0].Cells[3].Controls.Add(txtboxTo);
                table.Attributes.Add("width", "100%");
                table.CellPadding = 0;
                table.CellSpacing = 0;
                
                table.Rows[0].Cells[0].Width = 10;
                table.Rows[0].Cells[0].Style.Add("padding-right", "10px");
                table.Rows[0].Cells[1].Width = 10;
                table.Rows[0].Cells[1].Style.Add("padding-right", "10px");
                table.Rows[0].Cells[2].Width = 10;
                table.Rows[0].Cells[2].Style.Add("padding-right", "10px");
                plh.Controls.Add(table);
                plh.RenderControl(writer);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            string colorFunctions = Page.ClientScript.GetWebResourceUrl(typeof(DatePicker), "AjaxServerControl.Javascript.JqueryUIDatePicker.js");
            Page.ClientScript.RegisterClientScriptInclude("JqueryUIDatePicker.js", colorFunctions);
            colorFunctions = Page.ClientScript.GetWebResourceUrl(typeof(DatePicker), "AjaxServerControl.Javascript.DatePicker.js");
            Page.ClientScript.RegisterClientScriptInclude("DatePicker.js", colorFunctions);

            HtmlHead head = (HtmlHead)Page.Header;

            if (head.FindControl("EstiloPicker") == null)
            {
                HtmlLink link = new HtmlLink();
                link.ID = "EstiloPicker";
                link.Attributes.Add("href", Page.ResolveClientUrl(ConfigurationManager.AppSettings["Estilo"]));
                link.Attributes.Add("type", "text/css");
                link.Attributes.Add("rel", "stylesheet");
                head.Controls.Add(link);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.Page.Request.Form["hd" + this.ClientID + "from"] != null && !string.IsNullOrEmpty(this.Page.Request.Form["hd" + this.ClientID + "from"]))
            {
                this.Desde = this.Page.Request.Form["hd" + this.ClientID + "from"];
            }
            else if (string.IsNullOrEmpty(this.Desde))
            {
                this.Desde = DateTime.Today.ToShortDateString().Replace("-", "/");
            }

            if (this.Page.Request.Form["hd" + this.ClientID + "to"] != null && !string.IsNullOrEmpty(this.Page.Request.Form["hd" + this.ClientID + "to"]))
            {
                this.Hasta = this.Page.Request.Form["hd" + this.ClientID + "to"];
            }
            else if (string.IsNullOrEmpty(this.Hasta))
            {
                this.Hasta = DateTime.Today.ToShortDateString().Replace("-", "/");
            }

            ClientScriptManager cs = this.Page.ClientScript;
            cs.RegisterStartupScript(typeof(DatePicker), "Load " + this.ClientID, "<script type='text/javascript'>LoadDatePicker('" + this.ClientID + "','" + this.Idioma + "','" + this.Desde + "','" + this.Hasta + "')</script>");
        }

    }
}
