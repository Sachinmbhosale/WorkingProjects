using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreDAL;

namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class AnnotationsBL
    {

        public AnnotationsBL() { }

        public Results ManageAnnotatations(Annotations filter, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            AnnotationsDAL dal = new AnnotationsDAL();

            results.ResultDS = dal.GetUploadDocumentDetails(filter, action, loginOrgId, loginToken);


            return results;
        }
       

    }
}
