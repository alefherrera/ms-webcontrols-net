using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Script.Serialization;
using System.Collections.Generic;


namespace AjaxServerControl
{

    [ToolboxData("<{0}:ComboBox runat='server' />")]
    public class ComboBox : TextBox
    {
        private bool _SourceCompleto = false;

        public bool SourceCompleto
        {
            get { return _SourceCompleto; }
            set
            {
                if (value)
                    this.MinFilter = 0;
                _SourceCompleto = value;
            }
        }

        public System.Data.DataTable DataSource
        {
            get { return ((ViewState["DataSource"] == null) ? null : (System.Data.DataTable)ViewState["DataSource"]); }
            set { ViewState["DataSource"] = value; }
        }

        private string _WebService = ConfigurationManager.AppSettings["WebServiceCombo"];

        public string WebService
        {
            get { return _WebService; }
            set { _WebService = value; }
        }

        private string _Servicio = string.Empty;

        public string Servicio
        {
            get { return _Servicio; }
            set { _Servicio = value; }
        }

        public object Value
        {
            get
            {
                return ((ViewState["Value"] == null) ? -1 : ViewState["Value"]);
            }

            set
            {
                ViewState["Value"] = value;
            }
        }

        public int FiltroID
        {
            get
            {
                return ((ViewState["FiltroID"] == null) ? -1 : Convert.ToInt32(ViewState["FiltroID"]));
            }

            set
            {
                ViewState["FiltroID"] = value;
            }
        }

        private string ParentClientID
        {
            get { return ((ViewState["ParentClientID"] == null) ? string.Empty : Convert.ToString(ViewState["ParentClientID"])); }
            set { ViewState["ParentClientID"] = value; }
        }

        public string ParentID
        {
            get { return ((ViewState["ParentID"] == null) ? string.Empty : Convert.ToString(ViewState["ParentID"])); }
            set { ViewState["ParentID"] = value; }
        }

        private int _minFilter = 3;

        public int MinFilter
        {
            get { return _minFilter; }
            set { _minFilter = value; }
        }

        private string _CssSource = ConfigurationManager.AppSettings["Estilo"];

        public string CssSource
        {
            get { return _CssSource; }
            set { _CssSource = value; }
        }

        private string _PlaceHolder;

        public string PlaceHolder
        {
            get { return _PlaceHolder; }
            set { _PlaceHolder = value; }
        }

        public string DataValueField
        {
            get { return ((ViewState["DataValueField"] == null) ? string.Empty : Convert.ToString(ViewState["DataValueField"])); }
            set { ViewState["DataValueField"] = value; }
        }

        public string DataTextField
        {
            get { return ((ViewState["DataTextField"] == null) ? string.Empty : Convert.ToString(ViewState["DataTextField"])); }
            set { ViewState["DataTextField"] = value; }
        }

        public string ValorDefault
        {
            get { return ((ViewState["ValorDefault"] == null) ? "-1" : Convert.ToString(ViewState["ValorDefault"])); }
            set { ViewState["ValorDefault"] = value.ToLower() == "empty" ? string.Empty : value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            string colorFunctions = Page.ClientScript.GetWebResourceUrl(typeof(ComboBox), "AjaxServerControl.Javascript.JqueryUIAutocomplete.js");
            Page.ClientScript.RegisterClientScriptInclude("JqueryUIAutocomplete.js", colorFunctions);
            colorFunctions = Page.ClientScript.GetWebResourceUrl(typeof(ComboBox), "AjaxServerControl.Javascript.ComboAjax.js");
            Page.ClientScript.RegisterClientScriptInclude("ComboAjax.js", colorFunctions);

            HtmlHead head = (HtmlHead)Page.Header;

            if (head.FindControl("EstiloCombo") == null)
            {
                HtmlLink link = new HtmlLink();
                link.ID = "EstiloCombo";
                link.Attributes.Add("href", Page.ResolveClientUrl(CssSource));
                link.Attributes.Add("type", "text/css");
                link.Attributes.Add("rel", "stylesheet");
                head.Controls.Add(link);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Validate();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.Page.Request.Form["hd" + this.ClientID] != null && !string.IsNullOrEmpty(this.Page.Request.Form["hd" + this.ClientID]))
            {
                this.Value = this.Page.Request.Form["hd" + this.ClientID];
            }

            if (string.IsNullOrEmpty(this.Text) || this.Text == PlaceHolder)
            {
                this.Value = this.ValorDefault;
            }

            if (!string.IsNullOrEmpty(this.ParentID))
            {
                Control ParentControl = FindControl(this.ParentID);
                if (ParentControl != null)
                {
                    this.ParentClientID = Convert.ToString(ParentControl.GetType().GetProperty("ClientID").GetValue(ParentControl, null));
                }
            }

            ClientScriptManager cs = this.Page.ClientScript;
            cs.RegisterStartupScript(typeof(ComboBox), "Load " + this.ClientID, "<script type='text/javascript'>LoadCombo('" + Page.ResolveClientUrl(WebService) + "/','" + Servicio + "','" + this.ClientID + "'," + this.SourceCompleto.ToString().ToLower() + "," + this.MinFilter + "," + this.FiltroID + ",'" + this.ParentClientID + "','" + this.Value + "','" + this.PlaceHolder + "'," + TabletoString(this.DataSource) + ",'" + this.ValorDefault +"')</script>");
        }

        private void Validate()
        {
            if (string.IsNullOrEmpty(this.Servicio) && DataSource == null) { throw new Exception("Falta Servicio o DataSource"); }
        }

        public class ComboReturn
        {
            public string Text;
            public object Value;

            public ComboReturn()
            { }

            public ComboReturn(string Text, object Value)
            {
                this.Value = Value;
                this.Text = Text;
            }
        }

        public void AddItem(ListItem item)
        {
            if (this.DataSource == null)
            {
                string TextField, ValueField;
                TextField = "Text"; ValueField = "Value";
                this.DataSource = new System.Data.DataTable();
                this.DataSource.Columns.Add(TextField);
                this.DataSource.Columns.Add(ValueField);
                this.DataTextField = TextField;
                this.DataValueField = ValueField;
            }

            System.Data.DataRow row = this.DataSource.NewRow();
            row[this.DataTextField] = item.Text;
            row[this.DataValueField] = item.Value;
            this.DataSource.Rows.Add(row);

        }

        private string TabletoString(System.Data.DataTable dt)
        {
            List<ComboReturn> items = new List<ComboReturn>();
            if (dt != null)
            {
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    items.Add(new ComboReturn(row[DataTextField].ToString(), row[DataValueField]));
                }
                return new JavaScriptSerializer().Serialize(items);
            }
            return "null";

        }

        public void SelectItem(ListItem item)
        {
            this.Value = item.Value;
            this.Text = item.Text;
        }

    }
}