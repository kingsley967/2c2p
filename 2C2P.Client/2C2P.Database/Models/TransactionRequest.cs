namespace _2C2P.Database.Models
{
    public class TransactionRequest
    {
        public string? Currency { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? Status { get; set; }
    }

    public class ImportTransactions
    {
        public string fileName { get; set; }
        public string fileFormat { get; set; }
        public List<DataList> data { get; set; }
    }

    public class DataList
    {
        public string transactionId { get; set; }
        public string amount { get; set; }
        public string currencyCode { get; set; }
        public string transactionDate { get; set; }
        public string status { get; set; }
    }
}
