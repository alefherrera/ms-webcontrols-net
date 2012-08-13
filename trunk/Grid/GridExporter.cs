using System;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace AjaxServerControl
{
    public class GridExporter : ImageButton
    {
        public enum Propiedades
        {
            BackColor,
            ForeColor
        }

        public string DataSourceMethod
        {
            get { return ((ViewState["DataSourceMethod"] == null) ? "DataSourceGrilla" : Convert.ToString(ViewState["DataSourceMethod"])); }
            set { ViewState["DataSourceMethod"] = value; }
        }

        private string _GridID;

        public string GridID
        {
            get { return _GridID; }
            set { _GridID = value; }
        }

        private System.Data.DataTable DataSource;

        private string _NombreDoc;

        public string NombreDoc
        {
            get { return _NombreDoc; }
            set { _NombreDoc = value; }
        }

        private Dictionary<int, Dictionary<String, System.Drawing.Color>> ColorReference
        {
            get
            {
                return ((ViewState["ColorReference"] == null) ? new Dictionary<int, Dictionary<String, System.Drawing.Color>>() : (Dictionary<int, Dictionary<String, System.Drawing.Color>>)(ViewState["ColorReference"]));
            }

            set
            {
                ViewState["ColorReference"] = value;
            }
        }

        private Dictionary<String, Dictionary<String, System.Drawing.Color>> Columna_Colores
        {
            get
            {
                return ((ViewState["Columna_Colores"] == null) ? new Dictionary<String, Dictionary<String, System.Drawing.Color>>() : (Dictionary<String, Dictionary<String, System.Drawing.Color>>)(ViewState["Columna_Colores"]));
            }

            set
            {
                ViewState["Columna_Colores"] = value;
            }
        }

        private Dictionary<String, Propiedades> Columna_Propiedad
        {
            get
            {
                return ((ViewState["Columna_Propiedad"] == null) ? new Dictionary<String, Propiedades>() : (Dictionary<String, Propiedades>)(ViewState["Columna_Propiedad"]));
            }

            set
            {
                ViewState["Columna_Propiedad"] = value;
            }
        }

        private Dictionary<int, Propiedades> PropiedadReference
        {
            get
            {
                return ((ViewState["PropiedadReference"] == null) ? new Dictionary<int, Propiedades>() : (Dictionary<int, Propiedades>)(ViewState["PropiedadReference"]));
            }

            set
            {
                ViewState["PropiedadReference"] = value;
            }
        }

        public int PosicionRef
        {
            get { return ((ViewState["PosicionRef"] == null) ? 1 : (int)(ViewState["PosicionRef"])); }
            set { ViewState["PosicionRef"] = value; }
        }

        public void AgregarColumna(string Columna, Dictionary<String, System.Drawing.Color> Referencia, Propiedades Propiedad)
        {
            Dictionary<String, Dictionary<String, System.Drawing.Color>> Aux = Columna_Colores;
            Aux.Add(Columna, Referencia);
            this.Columna_Colores = Aux;

            Dictionary<String, Propiedades> AuxPropiedades = Columna_Propiedad;
            AuxPropiedades.Add(Columna, Propiedad);
            this.Columna_Propiedad = AuxPropiedades;

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.Style.Add("display", "none");
            string colorFunctions =
            Page.ClientScript.GetWebResourceUrl(typeof(Grid), "AjaxServerControl.Javascript.GridExporter.js");
            Page.ClientScript.RegisterClientScriptInclude("GridExporter.js", colorFunctions);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ClientScriptManager cs = this.Page.ClientScript;
            cs.RegisterStartupScript(typeof(Grid), "Load " + this.ClientID, "<script type='text/javascript'>LoadExporter('" + this.ClientID + "','" + FindControl(this.GridID).ClientID + "');</script>");

        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            this.ImageUrl = ConfigurationManager.AppSettings["imgExportar"];
        }

        protected override void OnClick(ImageClickEventArgs e)
        {
            this.DataSource = (System.Data.DataTable)Page.GetType().GetMethod(this.DataSourceMethod).Invoke(Page, null);
            ExportToExcel();
        }

        public void ExportToExcel()
        {
            GridView wControl = new GridView();
            wControl.AutoGenerateColumns = false;
            System.Web.UI.WebControls.BoundField columnaAdd = new System.Web.UI.WebControls.BoundField();
            List<int> ColumnasColor = new List<int>();
            foreach (System.Web.UI.WebControls.DataControlField columna in ((Grid)FindControl(this.GridID)).Columns)
            {
                if (columna.GetType().Equals(new System.Web.UI.WebControls.BoundField().GetType()))
                {

                     columnaAdd = (System.Web.UI.WebControls.BoundField)columna;
                     columnaAdd.Visible = true;
                     columnaAdd.HtmlEncode = false;
                     wControl.Columns.Add(columnaAdd);

                    if (Columna_Colores.ContainsKey(columna.HeaderText))
                    {
                        Dictionary<int, Dictionary<String, System.Drawing.Color>> Aux = ColorReference;
                        Aux.Add(wControl.Columns.IndexOf(columna), Columna_Colores[columna.HeaderText]);
                        ColorReference = Aux;

                        Dictionary<int, Propiedades> AuxPropiedad = PropiedadReference;
                        AuxPropiedad.Add(wControl.Columns.IndexOf(columna), Columna_Propiedad[columna.HeaderText]);
                        PropiedadReference = AuxPropiedad;
                    }


                }
            }


            wControl.HeaderStyle.Wrap = true;
            wControl.RowStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
            wControl.DataSource = this.DataSource;
            wControl.DataBind();


            foreach (GridViewRow row in wControl.Rows)
            {

                for (int i = 0; i < wControl.Columns.Count; i++)
                {
                    row.Cells[i].BackColor = System.Drawing.Color.White;
                }

                foreach (KeyValuePair<int, Dictionary<String, System.Drawing.Color>> Par in ColorReference)
                {
                    string[] Vaux;
                    string Valor;
                    Vaux = row.Cells[Par.Key].Text.Split('|');
                    Valor = Vaux.Length == 1 ? Vaux[0] : Vaux[PosicionRef];
                    if (!String.IsNullOrEmpty(Valor) && Valor != "&nbsp;" && Valor != "-")
                    {
                        row.Cells[Par.Key].GetType().GetProperty(PropiedadReference[Par.Key].ToString()).SetValue(row.Cells[Par.Key], ColorReference[Par.Key][Valor], null);
                    }

                }

                for (int i = 0; i < wControl.Columns.Count; i++)
                {
                    row.Cells[i].Text = HttpUtility.HtmlDecode(row.Cells[i].Text);

                    if (row.Cells[i].Text.Contains("|"))
                    {
                        row.Cells[i].Text = row.Cells[i].Text.Split('|')[0];
                    }
                }

            }

            HttpResponse responsePage = Page.Response;

            responsePage.AddHeader("Content-Disposition", "attachment;filename=" + this.NombreDoc + " - " + DateTime.Now + ".xlsx");

            ExcelDocument doc = new ExcelDocument(wControl);

            responsePage.BinaryWrite(doc.CreateExcel());
            responsePage.End();

        }


    }
}
