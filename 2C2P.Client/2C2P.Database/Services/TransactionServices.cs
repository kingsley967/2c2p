using _2C2P.Database.DataDB;
using _2C2P.Database.Models;
using System.Text.Json;

namespace _2C2P.Database.Services
{
    class TransactionServices : ITransactionServices
    {
        public TransactionServices()
        {

        }

        public async Task<List<TransactionViewModel>> QueryTransactions(string? Currency, string? StartDate, string? EndDate, string? Status)
        {
            List<TransactionViewModel> models = new List<TransactionViewModel>();
            try
            {
                _2c2pContext _2c2pContext = new _2c2pContext();
                var transactions = _2c2pContext.Transactions.ToList();

                if (!string.IsNullOrEmpty(Currency))
                {
                    transactions = transactions.Where(m => m.CurrencyCode == Currency.ToUpper()).ToList();
                }

                if (!string.IsNullOrEmpty(Status))
                {
                    if (Status.ToUpper() == "A")
                    {
                        transactions = transactions.Where(m => m.Status == "Approved").ToList();
                    }
                    else if (Status.ToUpper() == "R")
                    {
                        transactions = transactions.Where(m => m.Status == "Failed" || m.Status == "Rejected").ToList();
                    }
                    else
                        transactions = transactions.Where(m => m.Status == "Finished" || m.Status == "Done").ToList();
                }

                if (!string.IsNullOrEmpty(StartDate))
                {
                    transactions = transactions.Where(m => DateTime.Parse(m.TransactionDate) >= DateTime.Parse(StartDate)).ToList();
                }

                if (!string.IsNullOrEmpty(EndDate))
                {
                    transactions = transactions.Where(m => DateTime.Parse(m.TransactionDate) <= DateTime.Parse(EndDate)).ToList();
                }

                if (transactions.Count > 0)
                {
                    foreach (var tx in transactions)
                    {
                        TransactionViewModel m = new TransactionViewModel();
                        m.id = tx.TransactionId;
                        m.payment = tx.Amount + " " + tx.CurrencyCode;
                        m.status = transactionStatus(tx.Status);

                        models.Add(m);
                    }
                }

                return models;
            }
            catch (Exception ex)
            {

            }

            return models;
        }

        public async Task<string> ImportTransactions(string FileName, string FileFormat, List<DataList> Data)
        {
            try
            {
                _2c2pContext _2c2pContext = new _2c2pContext();
                ImportExcel ie = new ImportExcel();
                ie.Name = FileName;
                ie.Body = "";
                ie.Format = FileFormat;
                ie.CreatedAt = DateTime.Now;
                _2c2pContext.ImportExcels.Add(ie);
                _2c2pContext.SaveChanges();

                if (Data.Count > 0)
                {
                    foreach (var d in Data)
                    {
                        Transaction transaction = new Transaction();
                        transaction.ExcelId = ie.Id;
                        transaction.TransactionId = d.transactionId;
                        transaction.Amount = decimal.Parse(d.amount);
                        transaction.CurrencyCode = d.currencyCode;
                        transaction.TransactionDate = d.transactionDate;
                        transaction.Status = d.status;
                        transaction.CreatedAt = DateTime.Now;
                        _2c2pContext.Transactions.Add(transaction);
                        _2c2pContext.SaveChanges();
                    }
                }
                
                return JsonSerializer.Serialize("Success");
            }
            catch (Exception ex)
            {

            }

            return "";
        }
        
        private string transactionStatus(string status)
        {
            string statusReturn = "";
            switch (status)
            {
                case "Approved":
                    statusReturn = "A";
                    break;
                case "Failed":
                case "Rejected":
                    statusReturn = "R";
                    break;
                case "Finished":
                case "Done":
                    statusReturn = "D";
                    break;
                default:
                    break;
            }

            return statusReturn;
        }
    }
}
