using System;
using System.Collections.Generic;

namespace _2C2P.Database.DataDB
{
    public partial class Transaction
    {
        public long Id { get; set; }
        public long ExcelId { get; set; }
        public string TransactionId { get; set; } = null!;
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = null!;
        public string TransactionDate { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
