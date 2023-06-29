namespace Lotex.EnterpriseSolutions.CoreBE
{
    public class ReportBE
    {
       public ReportBE()
       {
           OrgId = 0;
           CreatedDateFrom = string.Empty;
           EndDate = string.Empty;

           DocumentTypeReportID = 0;
           DocumentTypeReportActionId = 0;
       }
       //BillingReport Properties
       public int OrgId { get; set; }
       public string CreatedDateFrom { get; set; }
       public string EndDate { get; set; }

       //DocumentTypeReport properties
       public int DocumentTypeReportID { get; set; }
       public int DocumentTypeReportActionId { get; set; }
    }
}
