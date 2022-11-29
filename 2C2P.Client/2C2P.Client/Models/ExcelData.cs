namespace _2C2P.Client.Models
{
    public class ExcelImportRequest
    {
        public string fileName { get; set; }
        public string fileFormat { get; set; }
        public List<ExcelData> data { get; set; }
    }
    public class ExcelData
    {
        public string transactionId { get; set; }
        public string amount { get; set; }
        public string currencyCode { get; set; }
        public string transactionDate { get; set; }
        public string status { get; set; }
    }
}
