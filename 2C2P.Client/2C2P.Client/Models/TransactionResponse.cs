namespace _2C2P.Client.Models
{
    public class TransactionResponse
    {
        public List<TransactionListResponse> data { get; set; }
    }

    public class TransactionListResponse
    {
        public string id { get; set; }
        public string payment { get; set; }
        public string status { get; set; }
    }
}
