namespace Lotex.EnterpriseSolutions.CoreBE
{
    public class BatchUploadBE
    {
        public BatchUploadBE()
        {
            batchData = string.Empty;
            TotalRowcount = 0;
            RowsCount = 0;
        }
        public string batchData { get; set; }

        public int DocTypeId { get; set; }
        public int DepartmentId { get; set; }
        public int SerialNo { get; set; }
        public int ProcessId { get; set; }
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
        public string Column4 { get; set; }
        public string Column5 { get; set; }
        public string Column6 { get; set; }

        public int TotalRowcount { get; set; }
        public int RowsCount { get; set; }
    }
}
