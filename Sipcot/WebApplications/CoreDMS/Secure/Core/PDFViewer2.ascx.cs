using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Text.RegularExpressions;
using PDFViewASP;
using System.Web.Caching;

namespace Lotex.EnterpriseSolutions.WebUI.Secure.Core
{
    public partial class PDFViewer2 : System.Web.UI.UserControl
    {
        

        

   
    public Hashtable parameterHash;
    private float panelHeightFactor = 0.9f;
    private float panelWidthFactor = 0.73f;
    private float panelBookWidthFactor = 0.24f;
    private float zoomFactor = 1.25f;
    private int minDPI = 20;
    private int maxDPI = 400;
   
    private int baseDPI = 150;

   

    public string FileName
    {
        get { return parameterHash["PDFFileName"].ToString(); }
        set
        {
            if (null == value | string.IsNullOrEmpty(value))
            {
                this.Enabled = false;
                return;
            }
            if (ImageUtil.IsPDF(value))
            {
                this.Enabled = true;
                InitUserVariables();
                parameterHash["PDFFileName"] = value;
                //InitPageRange();
                InitRotation();
                parameterHash["PagesOnly"] = false;
                //InitBookmarks();
                //FitToWidthButton_Click(null, null);
            }
        }
    }

    public bool Enabled
    {
        set { MainPanel.Enabled = value; }
    }

   

    #region "Control based events"

    //Need to delete the last image of the page viewed
      
    private void Page_Init(object sender, System.EventArgs e)
    {
        
        //Persist User control state
        Page.RegisterRequiresControlState(this);
    }
     private void Page_Load(object sender, System.EventArgs e)
    {
        
        //Persist User control state
        Control_load();
    }
    protected override object SaveControlState()
    {
        return parameterHash;
    }

    protected override void LoadControlState(object savedState)
    {
        parameterHash = (Hashtable)savedState;
    }

    protected void Control_load()
    {
        ResizePanels();
        if ((null != parameterHash))
        {
            parameterHash["SearchText"] = SearchTextBox.Text;
        }
        PreviousPageButton.Attributes.Add("onclick", "getBrowserDimensions()");
        NextPageButton.Attributes.Add("onclick", "getBrowserDimensions()");
        ZoomInButton.Attributes.Add("onclick", "getBrowserDimensions()");
        ZoomOutButton.Attributes.Add("onclick", "getBrowserDimensions()");
        RotateCCButton.Attributes.Add("onclick", "getBrowserDimensions()");
        RotateCButton.Attributes.Add("onclick", "getBrowserDimensions()");

        HiddenPageNav.Click += new System.EventHandler(this.HiddenPageNav_Click);
        PageNumberTextBox.TextChanged += new System.EventHandler(this.PageNumberTextBox_TextChanged);
        PreviousPageButton.Click += new System.Web.UI.ImageClickEventHandler(this.PreviousPageButton_Click);
        NextPageButton.Click += new System.Web.UI.ImageClickEventHandler(this.NextPageButton_Click);
        ZoomOutButton.Click += new System.Web.UI.ImageClickEventHandler(this.ZoomOutButton_Click);
        ZoomInButton.Click += new System.Web.UI.ImageClickEventHandler(this.ZoomInButton_Click);
        RotateCCButton.Click += new System.Web.UI.ImageClickEventHandler(this.RotateCCButton_Click);
        RotateCButton.Click += new System.Web.UI.ImageClickEventHandler(this.RotateCButton_Click);
        //FitToScreenButton.Click += new System.Web.UI.ImageClickEventHandler(this.FitToScreenButton_Click);
        //FitToWidthButton.Click += new System.Web.UI.ImageClickEventHandler(this.FitToWidthButton_Click);
        //ActualSizeButton.Click += new System.Web.UI.ImageClickEventHandler(this.ActualSizeButton_Click);
        //SearchButton.Click += new System.Web.UI.ImageClickEventHandler(this.SearchButton_Click);
        //SearchNextButton.Click += new System.Web.UI.ImageClickEventHandler(this.SearchNextButton_Click);
        //SearchPreviousButton.Click += new System.Web.UI.ImageClickEventHandler(this.SearchPreviousButton_Click);

    }

    //protected void FitToScreenButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    //{
    //    Size panelsize = new Size(Convert.ToInt16(Convert.ToDouble(HiddenBrowserWidth.Value) * panelWidthFactor), Convert.ToInt16(Convert.ToDouble(HiddenBrowserHeight.Value) * panelHeightFactor));
    //    parameterHash["DPI"] = AFPDFLibUtil.GetOptimalDPI(Convert.ToString(parameterHash["PDFFileName"]), Convert.ToInt16(parameterHash["CurrentPageNumber"]), ref panelsize);
    //    DisplayCurrentPage(false);
    //}

    //protected void FitToWidthButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    //{
    //    Size panelsize = new Size(Convert.ToInt16(Convert.ToDouble(HiddenBrowserWidth.Value) * panelWidthFactor), Convert.ToInt16(Convert.ToDouble(HiddenBrowserHeight.Value) * 4));
    //    parameterHash["DPI"] = AFPDFLibUtil.GetOptimalDPI(Convert.ToString(parameterHash["PDFFileName"]), Convert.ToInt16(parameterHash["CurrentPageNumber"]), ref panelsize);
    //    DisplayCurrentPage(false);
    //}

    protected void ActualSizeButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        parameterHash["DPI"] = 150;
        //DisplayCurrentPage(false);
    }

    //protected void SearchButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    //{
    //    parameterHash["SearchDirection"] = AFPDFLibUtil.SearchDirection.FromBeginning;
    //    DisplayCurrentPage(true);
    //    //BookmarkContentCell.Text = AFPDFLibUtil.BuildHTMLBookmarksFromSearchResults( _
    //    //AFPDFLibUtil.GetAllSearchResults(parameterHash["PDFFileName"), parameterHash["SearchText")) _
    //    //)
    //}

    //protected void SearchNextButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    //{
    //    parameterHash["SearchDirection"] = AFPDFLibUtil.SearchDirection.Forwards;
    //    DisplayCurrentPage(true);
    //}

    //protected void SearchPreviousButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    //{
    //    parameterHash["SearchDirection"] = AFPDFLibUtil.SearchDirection.Backwards;
    //    DisplayCurrentPage(true);
    //}

    protected void HiddenPageNav_Click(object sender, EventArgs e)
    {
        parameterHash["CurrentPageNumber"] = HiddenPageNumber.Value;
        //DisplayCurrentPage(false);
    }
    protected void PageNumberTextBox_TextChanged(object sender, EventArgs e)
    {
        if (Regex.IsMatch(PageNumberTextBox.Text, "^\\d+$"))
        {
            parameterHash["CurrentPageNumber"] = PageNumberTextBox.Text;
            //DisplayCurrentPage(false);
        }
    }
    protected void PreviousPageButton_Click(object sender, System.Web.UI.ImageClickEventArgs e) 
    {
        parameterHash["CurrentPageNumber"] = Convert.ToInt16(parameterHash["CurrentPageNumber"]) - 1;
        //DisplayCurrentPage(false);
    }

    protected void NextPageButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        parameterHash["CurrentPageNumber"] = Convert.ToInt16(parameterHash["CurrentPageNumber"]) + 1;
        //DisplayCurrentPage(false);
    }
   

    protected void ZoomOutButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        parameterHash["DPI"] = Convert.ToDouble(parameterHash["DPI"]) / zoomFactor;
        if (Convert.ToDouble(parameterHash["DPI"]) < minDPI)
        {
            parameterHash["DPI"] = minDPI;
        }
        //DisplayCurrentPage(false);
    }

    protected void ZoomInButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        parameterHash["DPI"] = Convert.ToDouble(parameterHash["DPI"])  * zoomFactor;
        if (Convert.ToDouble(parameterHash["DPI"]) > maxDPI)
        {
            parameterHash["DPI"] = Convert.ToInt32(maxDPI);
        }
        //DisplayCurrentPage(false);
    }

    protected void RotateCCButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        int indexNum = Convert.ToInt32(parameterHash["CurrentPageNumber"]) - 1;
         ((List<int>)parameterHash["RotationPage"])[indexNum] -= 1;
       // ((List<int>)parameterHash["RotationPage"])[indexNum] = 1;
        //DisplayCurrentPage(false);
    }

    protected void RotateCButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
    {
        int indexNum = (Convert.ToInt32(parameterHash["CurrentPageNumber"]) - 1);
        //parameterHash["RotationPage"](indexNum) += 1;
        ((List<int>)parameterHash["RotationPage"])[indexNum] += 1;
        //DisplayCurrentPage(false);
    }

    #endregion

    #region "Constraints"

    private void CheckPageBounds()
    {
        int pageCount = (int)parameterHash["PDFPageCount"];

        if (Convert.ToInt16(parameterHash["CurrentPageNumber"]) >= pageCount)
        {
            parameterHash["CurrentPageNumber"] = pageCount;
            NextPageButton.Enabled = false;
        }
        else if (Convert.ToInt16(parameterHash["CurrentPageNumber"]) <= 1)
        {
            parameterHash["CurrentPageNumber"] = 1;
            PreviousPageButton.Enabled = false;
        }

        if (Convert.ToInt16(parameterHash["CurrentPageNumber"]) < pageCount & pageCount > 1 & Convert.ToInt16(parameterHash["CurrentPageNumber"]) > 1)
        {
            NextPageButton.Enabled = true;
            PreviousPageButton.Enabled = true;
        }

        if (Convert.ToInt16(parameterHash["CurrentPageNumber"]) == pageCount & pageCount > 1 & Convert.ToInt16(parameterHash["CurrentPageNumber"]) > 1)
        {
            PreviousPageButton.Enabled = true;
        }

        if (Convert.ToInt16(parameterHash["CurrentPageNumber"]) == 1 & pageCount > 1)
        {
            NextPageButton.Enabled = true;
        }

        if (pageCount == 1)
        {
            NextPageButton.Enabled = false;
            PreviousPageButton.Enabled = false;
        }

    }

    #endregion

    #region "Helper Functions"

    private void InitUserVariables()
    {
        parameterHash = new Hashtable();
        parameterHash.Add("PDFFileName", "");
        parameterHash.Add("PDFPageCount", 0);
        parameterHash.Add("CurrentPageNumber", 1);
        parameterHash.Add("UserPassword", "");
        parameterHash.Add("OwnerPassword", "");
        parameterHash.Add("Password", "");
        parameterHash.Add("DPI", baseDPI);
        parameterHash.Add("PagesOnly", false);
        parameterHash.Add("CurrentImageFileName", "");
        parameterHash.Add("Rotation", new List<int>());
        parameterHash.Add("Bookmarks", "");
        parameterHash.Add("SearchText", "");
        //parameterHash.Add("SearchDirection", AFPDFLibUtil.SearchDirection.FromBeginning);
    }

    private void UpdatePageLabel()
    {
        PageLabel.Text = "Page " + parameterHash["CurrentPageNumber"] + " of " + parameterHash["PDFPageCount"];
        PageNumberTextBox.Text = parameterHash["CurrentPageNumber"].ToString();
    }

    //private void InitPageRange()
    //{
    //    parameterHash["PDFPageCount"] = ImageUtil.GetImageFrameCount(parameterHash["PDFFileName"].ToString(), parameterHash["Password"].ToString());
    //    parameterHash["CurrentPageNumber"] = 1;
    //}

    //private void InitBookmarks()
    //{
    //    PDFLibNet.PDFWrapper pdfDoc = default(PDFLibNet.PDFWrapper);
    //    try
    //    {
    //        pdfDoc = new PDFLibNet.PDFWrapper();
    //        pdfDoc.LoadPDF(parameterHash["PDFFileName"].ToString());
    //    }
    //    catch (Exception ex)
    //    {
    //        //pdfDoc failed
    //        if ((null != pdfDoc))
    //        {
    //            pdfDoc.Dispose();
    //        }
    //    }
    //    string bookmarkHtml = "";
    //    if ((null != pdfDoc))
    //    {
    //        bool pagesOnly = (bool) parameterHash["PagesOnly"];
    //        bookmarkHtml = AFPDFLibUtil.BuildHTMLBookmarks(ref pdfDoc ,pagesOnly);
    //        pdfDoc.Dispose();
    //    }
    //    BookmarkContentCell.Text = bookmarkHtml;
    //    if (Regex.IsMatch(bookmarkHtml, "\\<\\!--PageNumberOnly--\\>"))
    //    {
    //        parameterHash["PagesOnly"] = true;
    //    }
    //}

    private void InitRotation()
    {
        parameterHash["RotationPage"] = new List<int>();
        for (int i = 1; i <= (int)parameterHash["PDFPageCount"]; i++)
        {
            ((List<int>)parameterHash["RotationPage"]).Add(0);
        }
    }

    private void ResizePanels()
    {
        if (!string.IsNullOrEmpty(HiddenBrowserWidth.Value) & !string.IsNullOrEmpty(HiddenBrowserHeight.Value))
        {
            BookmarkPanel.Width = Convert.ToInt32(100 * panelBookWidthFactor);

            BookmarkPanel.Width = Convert.ToInt32(Convert.ToDouble(HiddenBrowserWidth.Value) * panelBookWidthFactor);
            BookmarkPanel.Height = Convert.ToInt32(Convert.ToDouble(HiddenBrowserHeight.Value) * panelHeightFactor); 
            ImagePanel.Width = Convert.ToInt32(Convert.ToDouble(HiddenBrowserWidth.Value) * panelWidthFactor);
            ImagePanel.Height = Convert.ToInt32(Convert.ToDouble(HiddenBrowserHeight.Value) * panelHeightFactor); 
        }
    }

    //private void DisplayCurrentPage(bool doSearch )
    //{
    //    //Set how long to wait before deleting the generated PNG file
    //    DateTime expirationDate = DateTime.Now.AddMinutes(5);
    //    TimeSpan noSlide = System.Web.Caching.Cache.NoSlidingExpiration;
    //    CacheItemRemovedCallback callBack = new CacheItemRemovedCallback(OnCacheRemove);
    //    ResizePanels();
    //    CheckPageBounds();
    //    UpdatePageLabel();
    //    InitBookmarks();
    //    string destPath = Request.MapPath("render");
    //    int indexNum = (Convert.ToInt16(parameterHash["CurrentPageNumber"]) - 1);
    //    int numRotation = Convert.ToInt16(((List<int>)parameterHash["RotationPage"])[indexNum]);
    //    string imageLocation = null;
    //    int CurrentPageNumber = Convert.ToInt16( parameterHash["CurrentPageNumber"]);
    //    if (doSearch == false)
    //    {
    //        imageLocation = ASPPDFLib.GetPageFromPDF(Convert.ToString(parameterHash["PDFFileName"]), destPath, ref CurrentPageNumber, Convert.ToInt32(parameterHash["DPI"]), Convert.ToString(parameterHash["Password"]), numRotation, "", 0);
    //    }
    //    else
    //    {
    //        imageLocation = ASPPDFLib.GetPageFromPDF(Convert.ToString(parameterHash["PDFFileName"]), destPath, ref CurrentPageNumber, Convert.ToInt32(parameterHash["DPI"]), Convert.ToString(parameterHash["Password"]), numRotation, Convert.ToString(parameterHash["SearchText"]), Convert.ToInt32(parameterHash["SearchDirection"]));
    //        UpdatePageLabel();
    //    }
    //    ImageUtil.DeleteFile(parameterHash["CurrentImageFileName"].ToString());
    //    parameterHash["CurrentImageFileName"] = imageLocation;
    //    //Add full filename to the Cache with an expiration
    //    //When the expiration occurs, it will call OnCacheRemove whih will delete the file
    //    //string sguid = new Guid().ToString()
    //    Cache.Insert(new Guid().ToString() + "_DeleteFile", imageLocation, null, expirationDate, noSlide, System.Web.Caching.CacheItemPriority.Default, callBack);
    //    string matchString = Request.MapPath("").Replace("\\", "\\\\");
    //    // escape backslashes
    //    CurrentPageImage.ImageUrl = Regex.Replace(imageLocation, matchString + "\\\\", "~/");
    //}

    private void OnCacheRemove(string key, object val, CacheItemRemovedReason reason)
    {
        if (Regex.IsMatch(key, "DeleteFile"))
        {
            ImageUtil.DeleteFile(val.ToString());
        }
    }

    #endregion

    public void PDFViewer()
    {
     
      //  Init += Page_Init;
    }

    }
}