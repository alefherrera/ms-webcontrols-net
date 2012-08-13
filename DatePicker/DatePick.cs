using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace AjaxServerControl
{
    [ToolboxData("<{0}:DatePick runat='server'></{0}:DatePick>")]
    public class DatePick : TextBox
    {
        public string Fecha
        {
            get { return ((ViewState["Fecha"] == null) ? string.Empty : Convert.ToString(ViewState["Fecha"])); }
            set { ViewState["Fecha"] = value; }
        }

        private string _Idioma = "pt-BR";

        public string Idioma
        {
            get { return _Idioma; }
            set { _Idioma = value; }
        }

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

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Attributes.Add("readonly", "readonly");
            this.Width = 70;
            string colorFunctions = Page.ClientScript.GetWebResourceUrl(typeof(DatePick), "AjaxServerControl.Javascript.JqueryUIDatePicker.js");
            Page.ClientScript.RegisterClientScriptInclude("JqueryUIDatePicker.js", colorFunctions);
            colorFunctions = Page.ClientScript.GetWebResourceUrl(typeof(DatePick), "AjaxServerControl.Javascript.DatePick.js");
            Page.ClientScript.RegisterClientScriptInclude("DatePick.js", colorFunctions);

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

            if (this.Page.Request.Form["hd" + this.ClientID] != null && !string.IsNullOrEmpty(this.Page.Request.Form["hd" + this.ClientID]))
            {
                this.Fecha = this.Page.Request.Form["hd" + this.ClientID];
            }
            else if (string.IsNullOrEmpty(this.Fecha))
            {
                this.Fecha = DateTime.Today.ToShortDateString().Replace("-", "/");
            }

            ClientScriptManager cs = this.Page.ClientScript;
            cs.RegisterStartupScript(typeof(DatePick), "Load " + this.ClientID, "<script type='text/javascript'>LoadDatePick('" + this.ClientID + "','" + this.Idioma + "','" + this.Fecha + "')</script>");
        }

    }
}
