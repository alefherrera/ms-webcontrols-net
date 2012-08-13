using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;

namespace AjaxServerControl
{
    public class Grid : GridView
    {
        private string _CssSource = ConfigurationManager.AppSettings["Estilo"];

        public string CssSource
        {
            get { return _CssSource; }
            set { _CssSource = value; }
        }

        private string _TituloExport = "Export";

        public string TituloExport
        {
            get { return _TituloExport; }
            set { _TituloExport = value; }
        }

        /// <summary>
        /// Recibe HeaderText y devuelve el Indice de la columna
        /// </summary>
        public System.Collections.Generic.Dictionary<String, int> ColumnIndexes
        {
            get
            {
                return ((ViewState["ColumnIndexes"] == null) ? null : (System.Collections.Generic.Dictionary<String, int>)(ViewState["ColumnIndexes"]));
            }

            set
            {
                ViewState["ColumnIndexes"] = value;
            }
        }

        public System.Collections.Generic.Dictionary<String, int> VisibleColumnIndexes
        {
            get
            {
                return ((ViewState["VisibleColumnIndexes"] == null) ? null : (System.Collections.Generic.Dictionary<String, int>)(ViewState["VisibleColumnIndexes"]));
            }

            set
            {
                ViewState["VisibleColumnIndexes"] = value;
            }
        }

        /// <summary>
        /// Remueve una Columna de la grilla
        /// </summary>
        /// <param name="Columna">HeaderText de la Columna que deseas eliminar</param>
        public void ColumnRemove(string Columna)
        {
            this.Columns.RemoveAt(this.ColumnIndexes[Columna]);
            GenerarIndexes();
        }

        protected override void OnDataBound(EventArgs e)
        {
            base.OnDataBound(e);

            GenerarIndexes();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.ViewStateMode = ViewStateMode.Disabled;
            //this.AutoGenerateColumns = false;
            this.Style.Add("display", "none");

            string colorFunctions =
            Page.ClientScript.GetWebResourceUrl(typeof(Grid), "AjaxServerControl.Javascript.jquery.metadata.js");
            Page.ClientScript.RegisterClientScriptInclude("Metadata.js", colorFunctions);
            colorFunctions = Page.ClientScript.GetWebResourceUrl(typeof(Grid), "AjaxServerControl.Javascript.jquery.dataTables.min.js");
            Page.ClientScript.RegisterClientScriptInclude("Datatables.js", colorFunctions);
            //colorFunctions = Page.ClientScript.GetWebResourceUrl(typeof(Grid), "AjaxServerControl.Javascript.FixedColumns.nightly.min.js");
            //Page.ClientScript.RegisterClientScriptInclude("FixedColumns.js", colorFunctions);
            colorFunctions = Page.ClientScript.GetWebResourceUrl(typeof(Grid), "AjaxServerControl.Javascript.GridJquery.js");
            Page.ClientScript.RegisterClientScriptInclude("GridJquery.js", colorFunctions);
            

            HtmlHead head = (HtmlHead)Page.Header;

            if (head.FindControl("EstiloGrid1") == null)
            {
                HtmlLink link = new HtmlLink();
                link.ID = "EstiloGrid1";
                link.Attributes.Add("href", Page.ResolveClientUrl(this.CssSource));
                link.Attributes.Add("type", "text/css");
                link.Attributes.Add("rel", "stylesheet");
                head.Controls.Add(link);
            } if (head.FindControl("EstiloGrid2") == null)
            {
                HtmlLink link = new HtmlLink();
                link.ID = "EstiloGrid2";
                link.Attributes.Add("href", Page.ResolveClientUrl(ConfigurationManager.AppSettings["EstiloGrillasBase"]));
                link.Attributes.Add("type", "text/css");
                link.Attributes.Add("rel", "stylesheet");
                head.Controls.Add(link);
            }

            this.HeaderStyle.Wrap = true;
            this.RowStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;

            GenerarIndexes();
            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ClientScriptManager cs = this.Page.ClientScript;
            cs.RegisterStartupScript(typeof(Grid), "Load " + this.ClientID, "<script type='text/javascript'>LoadGrid('" + this.ClientID + "','" + this.TituloExport + "','" + this.Width + "');</script>");

        }

        public Object GetValue(System.Web.UI.WebControls.GridViewRowEventArgs e, string b)
        {
            return e.Row.Cells[this.VisibleColumnIndexes[b]].Text;
        }

        private void GenerarIndexes()
        {
            System.Collections.Generic.Dictionary<String, int> aux = new System.Collections.Generic.Dictionary<String, int>();
            System.Collections.Generic.Dictionary<String, int> auxv = new System.Collections.Generic.Dictionary<String, int>();
            int contador = 0;

            foreach (System.Web.UI.WebControls.DataControlField columna in this.Columns)
            {
                aux.Add(columna.HeaderText, this.Columns.IndexOf(columna));
                if (columna.Visible)
                {
                    auxv.Add(columna.HeaderText, contador);
                    contador++;
                }
            }

            this.ColumnIndexes = aux;
            this.VisibleColumnIndexes = auxv;
        }

    }
}
