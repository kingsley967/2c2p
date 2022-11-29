using _2C2P.Client.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Text.Json;
using Microsoft.VisualBasic.FileIO;
using System.Xml;
using System.Globalization;

namespace _2C2P.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration Configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Transaction()
        {
            try
            {
                TransactionRequest req = new TransactionRequest
                {
                    Currency = "",
                    StartDate = "",
                    EndDate = "",
                    Status = ""
                };

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Configuration.GetValue<string>("APIUrl") + "/query/transactions");
                var resp = client.PostAsJsonAsync("transactions", req).Result;
                string resultString = resp.Content.ReadAsStringAsync().Result;

                List<TransactionListResponse> model =  JsonSerializer.Deserialize<List<TransactionListResponse>>(resultString.ToString());
                return View(model);
            }
            catch(Exception ex)
            {

            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<string> UploadFile(IFormFile file)
        {
            try
            {
                ExcelImportRequest request = new ExcelImportRequest();
                List<ExcelData> data = new List<ExcelData>();
                bool dataErrorFlag = false;
                string errorMsg = "";

                var length = file.Length;
                if (file.Length > 1000000) // 1mb
                {
                    errorMsg = "File size was exceed 1mb";
                    return errorMsg;
                }

                DataTable dt = new DataTable();
                if (file.FileName.EndsWith(".csv"))
                {
                    using (var stream = file.OpenReadStream())
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        string[] headers = sr.ReadLine().Split(',');
                        foreach (string header in headers)
                        {
                            dt.Columns.Add(header);
                        }
                        while (!sr.EndOfStream)
                        {
                            TextFieldParser parser = new TextFieldParser(new StringReader(sr.ReadLine()));

                            parser.HasFieldsEnclosedInQuotes = true;
                            parser.SetDelimiters(",");

                            //string[] rows = sr.ReadLine().Split(',');
                            string[] rows = parser.ReadFields();
                            DataRow dr = dt.NewRow();
                            for (int i = 0; i < headers.Length; i++)
                            {
                                dr[i] = rows[i];
                            }
                            dt.Rows.Add(dr);
                        }
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i][0].ToString() == string.Empty
                            && dt.Rows[i][1].ToString() == string.Empty
                            && dt.Rows[i][2].ToString() == string.Empty
                            && dt.Rows[i][3].ToString() == string.Empty
                            && dt.Rows[i][4].ToString() == string.Empty)
                        {
                            break;
                        }

                        if (dt.Rows[i][0].ToString().Length > 50)
                        {
                            dataErrorFlag = true;
                            errorMsg = "TransactionId's max length is 50";
                            break;
                        }

                        if (!decimal.TryParse(dt.Rows[i][1].ToString(), out decimal outAmount))
                        {
                            dataErrorFlag = true;
                            errorMsg = "Invalid Amount";
                            break;
                        }

                        if (dt.Rows[i][2].ToString().Length > 3)
                        {
                            dataErrorFlag = true;
                            errorMsg = "Invalid ISO4217 format";
                            break;
                        }

                        DateTime d;
                        if (!DateTime.TryParse(dt.Rows[i][3].ToString(), out d))
                        {
                            dataErrorFlag = true;
                            errorMsg = "Invalid Datetime format";
                            break;
                        }

                        //if (!DateTime.TryParseExact(
                        //        dt.Rows[i][3].ToString(),
                        //        "dd/MM/yyyy hh:mm:ss",
                        //        CultureInfo.InvariantCulture,
                        //        DateTimeStyles.None,
                        //        out d))
                        //{
                        //    dataErrorFlag = true;
                        //    errorMsg = "Invalid Datetime format";
                        //    break;
                        //}

                        ExcelData dataRow = new ExcelData()
                        {
                            transactionId = dt.Rows[i][0].ToString(),
                            amount = dt.Rows[i][1].ToString(),
                            currencyCode = dt.Rows[i][2].ToString(),
                            transactionDate = d.ToString("yyyy-MM-dd HH:mm:ss"),
                            status = dt.Rows[i][4].ToString()
                        };

                        data.Add(dataRow);
                    }

                    if (dataErrorFlag) return errorMsg;
                    else
                    {
                        request.fileName = file.FileName;
                        request.fileFormat = file.ContentType;
                        request.data = data;
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri(Configuration.GetValue<string>("APIUrl") + "/query/import-transactions");
                        var resp = client.PostAsJsonAsync("import-transactions", request).Result;
                        string resultString = resp.Content.ReadAsStringAsync().Result;

                        return "";
                    }
                }
                else if (file.FileName.EndsWith(".xml"))
                {
                    bool newRecord = false;
                    string xmlTransactionId = "";
                    string xmlTransactionDate = "";
                    string xmlAmount = "";
                    string xmlCurrency = "";
                    string xmlStatus = "";
                    DateTime d = new DateTime();
                    XmlTextReader reader = new XmlTextReader(file.OpenReadStream());
                    while (reader.Read())
                    {
                        newRecord = false;
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name.ToString())
                            {
                                case "Transaction":
                                    xmlTransactionId = reader.GetAttribute("id").ToString();
                                    if (xmlTransactionId.Length > 50)
                                    {
                                        dataErrorFlag = true;
                                        errorMsg = "TransactionId's max length is 50";
                                    }

                                    break;
                                case "TransactionDate":
                                    xmlTransactionDate = reader.ReadString();
                                    if (!DateTime.TryParse(xmlTransactionDate, out d))
                                    {
                                        dataErrorFlag = true;
                                        errorMsg = "Invalid Datetime format";
                                    }

                                    break;
                                case "Amount":
                                    xmlAmount = reader.ReadString();
                                    if (!decimal.TryParse(xmlAmount, out decimal outAmount))
                                    {
                                        dataErrorFlag = true;
                                        errorMsg = "Invalid Amount";
                                    }
                                    break;
                                case "CurrencyCode":
                                    xmlCurrency = reader.ReadString();
                                    if (xmlCurrency.Length > 3)
                                    {
                                        dataErrorFlag = true;
                                        errorMsg = "Invalid ISO4217 format";
                                    }
                                    break;
                                case "Status":
                                    newRecord = true;
                                    xmlStatus = reader.ReadString();
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (newRecord)
                        {
                            ExcelData dataRow = new ExcelData()
                            {
                                transactionId = xmlTransactionId,
                                amount = xmlAmount,
                                currencyCode = xmlCurrency,
                                transactionDate = d.ToString("yyyy-MM-dd HH:mm:ss"),
                                status = xmlStatus
                            };

                            data.Add(dataRow);
                        }
                    }

                    if (dataErrorFlag) return errorMsg;
                    else
                    {
                        request.fileName = file.FileName;
                        request.fileFormat = file.ContentType;
                        request.data = data;
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri(Configuration.GetValue<string>("APIUrl") + "/query/import-transactions");
                        var resp = client.PostAsJsonAsync("import-transactions", request).Result;
                        string resultString = resp.Content.ReadAsStringAsync().Result;

                        return "";
                    }
                }
                else
                {
                    return "Unknown Format";
                }
            }
            catch (Exception ex)
            {
                return "Exception";
            }
        }


        [HttpPost]
        public IActionResult Transaction(string currency, string startDate, string endDate, string status)
        {
            try
            {
                TransactionRequest req = new TransactionRequest
                {
                    Currency = currency,
                    StartDate = startDate,
                    EndDate = endDate,
                    Status = status
                };

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Configuration.GetValue<string>("APIUrl") + "/query/transactions");
                var resp = client.PostAsJsonAsync("transactions", req).Result;
                string resultString = resp.Content.ReadAsStringAsync().Result;

                List<TransactionListResponse> model = JsonSerializer.Deserialize<List<TransactionListResponse>>(resultString.ToString());
                ViewBag.Currency = currency;
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                ViewBag.Status = status;
                return View(model);
            }
            catch (Exception ex)
            {

            }

            return View();
        }
    }
}