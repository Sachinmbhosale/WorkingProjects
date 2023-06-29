using System.ServiceModel.Web;
namespace DataAccess
{
    public class WebContext
    {
        public static string GetIncomingRequestHeaders(WebOperationContext objWebOperationContext)
        {
            string strHeaders = string.Empty;
            //for (int i = 0; i <= objWebOperationContext.IncomingRequest.Headers.Count; i++)
            //{
            //    objWebOperationContext.IncomingRequest.Headers[i].
            //}
            foreach (string name in objWebOperationContext.IncomingRequest.Headers)
            {
                strHeaders = strHeaders + ";" + name + "=" + objWebOperationContext.IncomingRequest.Headers[name];
            }
            return strHeaders;
        }
        public static string GetOutgoingResponseHeaders(WebOperationContext objWebOperationContext)
        {
            string strHeaders = string.Empty;
            //for (int i = 0; i <= objWebOperationContext.IncomingRequest.Headers.Count; i++)
            //{
            //    objWebOperationContext.IncomingRequest.Headers[i].
            //}
            foreach (string name in objWebOperationContext.OutgoingResponse.Headers)
            {
                strHeaders = strHeaders + ";" + name + "=" + objWebOperationContext.OutgoingResponse.Headers[name];
            }
            return strHeaders;
        }
    }
}
