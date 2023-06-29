using System;
using System.Data;
using Lotex.EnterpriseSolutions.CoreDAL;

namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class DocumentMailBL
    {
       /// <summary>
       /// Save Data for Mail sending
       /// </summary>
       /// <param name="strTo"></param>
       /// <param name="strSubject"></param>
       /// <param name="strMailBody"></param>
       /// <param name="dtsendtimeAt"></param>
       /// <param name="dtCreatedDate"></param>
       /// <param name="strDescription"></param>
       /// <param name="dtsendTime"></param>
       /// <param name="bActive"></param>
       /// <param name="inAttempts"></param>
       /// <returns></returns>
       public DataSet SendDocumentMail(string strAction, int inMailId, string strTo, string strSubject, string strMailBody,string strToken, string strDownloadlink,string Token,int OrgId, out string message, out int ErrorState, out int ErrorSeverity)
       {
           DocumentMailDAL objDocumentMailDAL = new DocumentMailDAL();
           DataSet dsMailDetails = new DataSet();
           try
           {
               dsMailDetails = objDocumentMailDAL.SendDocumentMail(strAction, inMailId, strTo, strSubject, strMailBody,strToken,strDownloadlink,Token,OrgId, out message, out ErrorState, out ErrorSeverity);
           }
           catch (Exception)
           {
               
               throw; 
           }
           return dsMailDetails;
       }
       public DataSet GetDocumentLinkBasedOnToken(string strToken,string strAction)
       {
           DocumentMailDAL objDocumentMailDAL = new DocumentMailDAL();
           DataSet dsDetails = new DataSet();
           try
           {
               dsDetails = objDocumentMailDAL.GetDocumentLinkBasedOnToken(strToken, strAction);
           }
           catch (Exception)
           {
               
               throw;
           }
           return dsDetails;
       }
    }
}
