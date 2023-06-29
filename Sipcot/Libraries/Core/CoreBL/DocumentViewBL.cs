using System.Data;
using Lotex.EnterpriseSolutions.CoreDAL;

namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class DocumentViewBL
    {
        public DataSet GetAllTreeViewData(string LoginToken, int LoginOrgId, string ToolTip, int NodeId,int ParentNodeId,int DocumentTypeId,int DepartmentId)
        {
            DataSet dsData = new DataSet();
            DocumentViewDAL objDocumentViewDAL = new DocumentViewDAL();
            dsData = objDocumentViewDAL.GetAllTreeViewData(LoginToken, LoginOrgId, ToolTip, NodeId, ParentNodeId, DocumentTypeId, DepartmentId);
            return dsData;
        }
    }
}
