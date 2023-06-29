using System.Data;
using Lotex.EnterpriseSolutions.CoreDAL;
using Lotex.EnterpriseSolutions.CoreBE;

namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class ReportBL
    {
        DataSet ds = new DataSet();
        public DataSet BillingSubReport(ReportBE Report, int loginOrgId, string loginToken)
        {
            return new ReportDAL().BillingSubReports(Report, loginOrgId, loginToken);
        }

        public DataSet BillingMainReport(ReportBE Report, int loginOrgId, string loginToken)
        {
            return new ReportDAL().BillingMainReports(Report, loginOrgId, loginToken);
        }

        public DataSet DocumentTypeGenerateReport(ReportBE Report, int loginOrgId, string loginToken)
        {
            return new ReportDAL().DocumentTypeGenerateReports(Report, loginOrgId, loginToken);
        }

        public DataSet ExpiryReport(ReportBE Report, int loginOrgId, string loginToken)
        {
            return new ReportDAL().ExpiryReports(Report, loginOrgId, loginToken);
        }

        public DataSet LogFormReport(ReportBE Report, int loginOrgId, string loginToken)
        {
            return new ReportDAL().LogFormReports(Report, loginOrgId, loginToken);
        }

        public DataSet TagChangeReport(ReportBE Report, int loginOrgId, string loginToken)
        {
            return new ReportDAL().TagChangeReports(Report, loginOrgId, loginToken);
        }
        
    }
}
