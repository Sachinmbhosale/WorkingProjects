namespace Lotex.EnterpriseSolutions.CoreBE
{
    public class Price
    {
        public Price()
        {
            PriceId = 0;
            CustomerId = 0;
            DocumentTypeId = 0;
            BillType = string.Empty;
            Charges = 0;
            Currency = string.Empty;
        }

        public int PriceId { get; set; }
        public int CustomerId { get; set; }
        public int DocumentTypeId { get; set; }
        public string BillType { get; set; }
        public decimal Charges { get; set; }
        public string Currency { get; set; }
    }
}
