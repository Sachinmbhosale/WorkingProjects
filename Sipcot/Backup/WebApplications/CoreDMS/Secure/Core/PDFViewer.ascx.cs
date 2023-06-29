using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using OfficeConverter;
using System.IO;
using Lotex.EnterpriseSolutions.CoreBL;
using Lotex.EnterpriseSolutions.CoreBE;
using System.Net;

namespace PdfViewer
{
    public partial class PDFViewer : System.Web.UI.UserControl
    {
        public bool NavigationToolbarVisible = true;
        public bool AnnotationToolbarVisible = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            ImageControls_Part2.Visible = AnnotationToolbarVisible;
        }

        public void JustPostbakUserControl(object sender, EventArgs e)
        {
            Page_Load(sender, e);
        }
    }
}