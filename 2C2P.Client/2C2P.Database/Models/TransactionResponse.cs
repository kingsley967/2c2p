namespace _2C2P.Database.Models
{
    public class TransactionResponse
    {
        public List<TransactionViewModel> data { get; set; }
    }
    public class TransactionViewModel
    {
        public string id { get; set; }
        public string payment { get; set; }
        public string status { get; set; }
    }
}
