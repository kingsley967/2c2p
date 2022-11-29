using _2C2P.Database.Models;

namespace _2C2P.Database.Services
{
    interface ITransactionServices
    {
        public Task<List<TransactionViewModel>> QueryTransactions(string? Currency, string? StartDate, string? EndDate, string? Status);
        public Task<string> ImportTransactions(string FileName, string FileFormat, List<DataList> Data);
    }
}
