using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBL;
using System.Net;

namespace DownloadWeb
{
    public partial class DownloadDocument : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            DocumentMailBL objDocumentMailBL = new DocumentMailBL();
            try
            {
                string downloadLink = string.Empty;
                string token = Request.QueryString["Token"] != null ? Request.QueryString["Token"] : string.Empty;
                if (token.Length > 0)
                {
                    // Update download count and get the link 
                    try
                    {
                        downloadLink = (new DocumentMailBL()).GetDocumentLinkBasedOnToken(token, "GetDownloadLink").Tables[0].Rows[0][0].ToString();
                    }
                    catch (Exception)
                    {

                        divmsg.InnerHtml = "Download link expired!";
                        divmsg.Attributes.Add("class", "alert-danger");
                       
                    }

                    if (downloadLink.Length > 0 && CheckUrlExists(downloadLink))
                        Response.Redirect(downloadLink);
                    else
                    {
                        if (downloadLink.Length > 0)
                        {
                            objDocumentMailBL.GetDocumentLinkBasedOnToken(token, "DownloadFail");
                            divmsg.InnerHtml = "File does not exists!";
                            divmsg.Attributes.Add("class", "alert-danger");
                        }
                        else
                        {
                            divmsg.InnerHtml = "Download link expired!";
                            divmsg.Attributes.Add("class", "alert-danger");
                        }
                    }
                }
                else
                {
                    divmsg.InnerHtml = "Invalid link!";
                    divmsg.Attributes.Add("class", "alert-danger");
                }
            }
            catch (Exception)
            {

            }
        }

        protected bool CheckUrlExists(string url)
        {
            // If the url does not contain Http. Add it.
            if (!url.Contains("http://"))
            {
                url = "http://" + url;
            }
            try
            {
                var request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "HEAD";
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch
            {
                return false;
            }
        }




    }
}