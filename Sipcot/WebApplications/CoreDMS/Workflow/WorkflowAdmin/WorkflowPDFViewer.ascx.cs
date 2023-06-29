using System;

namespace WorkflowPdfViewer
{
    public partial class WorkflowPDFViewer : System.Web.UI.UserControl
    {

        public bool NavigationToolbarVisible = true;
        public bool AnnotationToolbarVisible = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            ImageControls_Part2.Visible = AnnotationToolbarVisible;
        }

        public void JustPostbakUserControl(object sender, EventArgs e) { }


    }
}