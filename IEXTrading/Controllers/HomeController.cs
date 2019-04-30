using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IEXTrading.Infrastructure.IEXTradingHandler;
using IEXTrading.Models;
using IEXTrading.Models.ViewModel;
using IEXTrading.DataAccess;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace MVCTemplate.Controllers
{
    
    public class HomeController : Controller
    {
        public ApplicationDbContext dbContext;
        private readonly AppSettings _appSettings;
        public const string SessionKeyName = "StockData";
        //List<Company> companies = new List<Company>();
        public HomeController(ApplicationDbContext context, IOptions<AppSettings> appSettings)
        {
            dbContext = context;
            _appSettings = appSettings.Value;
        }

        public IActionResult HelloIndex()
        {
            ViewBag.Hello = _appSettings.Hello;
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        /****
         * The Symbols action calls the GetSymbols method that returns a list of Companies.
         * This list of Companies is passed to the Symbols View.
        ****/
        public IActionResult Symbols()
        {
            IEXHandler webHandler = new IEXHandler();
            List<Company> companies = webHandler.GetSymbols();
            PopulateSymbols();
            return View(companies);
        }

        /****
         * The Chart action calls the GetChart method that returns 1 year's equities for the passed symbol.
         * A ViewModel CompaniesEquities containing the list of companies, prices, volumes, avg price and volume.
         * This ViewModel is passed to the Chart view.
        ****/
        public IActionResult Chart(string symbol)
        {
            PopulateSymbols();
            ViewBag.dbSuccessChart = 0;
            List<Equity> equities = new List<Equity>();
            if (symbol != null)
            {
                IEXHandler webHandler = new IEXHandler();
                equities = webHandler.GetChart(symbol);
                equities = equities.OrderBy(c => c.date).ToList(); //Make sure the data is in ascending order of date.
            }

            CompaniesEquities companiesEquities = getCompaniesEquitiesModel(equities);

            return View(companiesEquities);
        }

        /****
         * 
        ****/
        public IActionResult StockStat(string symbol)
        {
            //Set ViewBag variable first
            ViewBag.dbSuccessChart = 0;
            CompaniesStatistics companiesEquities = getCompaniesStatisticsModel(symbol);
            return View(companiesEquities);
        }

        /****
         * The Refresh action calls the ClearTables method to delete records from a or all tables.
         * Count of current records for each table is passed to the Refresh View.
        ****/
        public IActionResult Refresh(string tableToDel)
        {
            ClearTables(tableToDel);
            Dictionary<string, int> tableCount = new Dictionary<string, int>();
            tableCount.Add("Companies", dbContext.Companies.Count());
            tableCount.Add("Charts", dbContext.Equities.Count());
            tableCount.Add("Gainers", dbContext.Gainers.Count());
            tableCount.Add("Losers", dbContext.Losers.Count());
            tableCount.Add("FinancialData", dbContext.FinancialData.Count());
            tableCount.Add("Quotes", dbContext.Quotes.Count());
            return View(tableCount);
        }

        /****
         * Saves the Symbols in database.
        ****/
        public void PopulateSymbols()
        {
            IEXHandler webHandler = new IEXHandler();
            List<Company> companiesRes = webHandler.GetSymbols();
            String companiesData = JsonConvert.SerializeObject(companiesRes);
            List<Company> companies = null;
            if (companiesData != "")
            {
                 companies = JsonConvert.DeserializeObject<List<Company>>(companiesData);
            }
            
            foreach (Company company in companies)
            {
                //Database will give PK constraint violation error when trying to insert record with existing PK.
                //So add company only if it doesnt exist, check existence using symbol (PK)
                if (dbContext.Companies.Where(c => c.symbol.Equals(company.symbol)).Count() == 0)
                {
                    dbContext.Companies.Add(company);
                }
            }
            dbContext.SaveChanges();
        }
        /****
         * Saves the gainers in database.
        ****/
        public void PopulateGainers()
        {
            IEXHandler webHandler = new IEXHandler();
            List<StockStats> gainersListRes = webHandler.Gainers();
            String gainersData = JsonConvert.SerializeObject(gainersListRes);
            List<StockStats> gainersList = null;
            if (gainersData != "")
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                gainersList = JsonConvert.DeserializeObject<List<StockStats>>(gainersData, settings);
            }

            foreach (StockStats gainer in gainersList)
            {
                //Database will give PK constraint violation error when trying to insert record with existing PK.
                //So add company only if it doesnt exist, check existence using symbol (PK)
                if (dbContext.Gainers.Where(c => c.symbol.Equals(gainer.symbol)).Count() == 0)
                {
                    Gainers gainerData = new Gainers(gainer.symbol, gainer.companyName, gainer.primaryExchange, gainer.sector);
                    dbContext.Gainers.Add(gainerData);
                }
            }
            dbContext.SaveChanges();
        }
        /****
         * Saves the equities in database.
        ****/
        public IActionResult SaveCharts(string symbol)
        {
            IEXHandler webHandler = new IEXHandler();
            List<Equity> equities = webHandler.GetChart(symbol);
            //List<Equity> equities = JsonConvert.DeserializeObject<List<Equity>>(TempData["Equities"].ToString());
            foreach (Equity equity in equities)
            {
                if (dbContext.Equities.Where(c => c.date.Equals(equity.date)).Count() == 0)
                {
                    dbContext.Equities.Add(equity);
                }
            }

            dbContext.SaveChanges();
            ViewBag.dbSuccessChart = 1;

            CompaniesEquities companiesEquities = getCompaniesEquitiesModel(equities);

            return View("Chart", companiesEquities);
        }

        /****
         * Deletes the records from tables.
        ****/
        public void ClearTables(string tableToDel)
        {
            if ("all".Equals(tableToDel))
            {
                //First remove equities and then the companies
                dbContext.Equities.RemoveRange(dbContext.Equities);
                dbContext.Companies.RemoveRange(dbContext.Companies);
                dbContext.Gainers.RemoveRange(dbContext.Gainers);
                dbContext.Losers.RemoveRange(dbContext.Losers);
                dbContext.FinancialData.RemoveRange(dbContext.FinancialData);
                dbContext.Quotes.RemoveRange(dbContext.Quotes);
            }
            else if ("Companies".Equals(tableToDel))
            {
                //Remove only those that don't have Equity stored in the Equitites table
                dbContext.Companies.RemoveRange(dbContext.Companies
                                                         .Where(c => c.Equities.Count == 0)
                                                                      );
            }
            else if ("Charts".Equals(tableToDel))
            {
                dbContext.Equities.RemoveRange(dbContext.Equities);
            }
            else if ("Gainers".Equals(tableToDel))
            {
                dbContext.Gainers.RemoveRange(dbContext.Gainers);
            }
            else if ("Losers".Equals(tableToDel))
            {
                dbContext.Losers.RemoveRange(dbContext.Losers);
            }
            else if ("FinancialData".Equals(tableToDel))
            {
                dbContext.FinancialData.RemoveRange(dbContext.FinancialData);
            }
            else if ("Quotes".Equals(tableToDel))
            {
                dbContext.Quotes.RemoveRange(dbContext.Quotes);
            }
            dbContext.SaveChanges();
        }

        /****
         * Returns the ViewModel CompaniesEquities based on the data provided.
         ****/
        public CompaniesEquities getCompaniesEquitiesModel(List<Equity> equities)
        {
            List<Company> companies = dbContext.Companies.ToList();

            if (equities.Count == 0)
            {
                return new CompaniesEquities(companies, null, "", "", "", 0, 0);
            }

            Equity current = equities.Last();
            string dates = string.Join(",", equities.Select(e => e.date));
            string prices = string.Join(",", equities.Select(e => e.high));
            string volumes = string.Join(",", equities.Select(e => e.volume / 1000000)); //Divide vol by million
            float avgprice = equities.Average(e => e.high);
            double avgvol = equities.Average(e => e.volume) / 1000000; //Divide volume by million
            return new CompaniesEquities(companies, equities.Last(), dates, prices, volumes, avgprice, avgvol);
        }

        /****
         * Returns the ViewModel CompaniesStatistics based on the data provided.
         ****/
        public CompaniesStatistics getCompaniesStatisticsModel(String symbol)
        {
            List<Gainers> gainers = dbContext.Gainers.ToList();
            if (symbol == null)
            {
                return new CompaniesStatistics(gainers, null, 0, null, null);
            }
            IEXHandler webHandler = new IEXHandler();
            CompaniesStatistics stat = new CompaniesStatistics();
            stat.Companies = gainers;
            stat.symbol = symbol;
            return stat;
        }

        public IActionResult Financials(string symbol)
        {
            PopulateGainers();
            CompaniesStatistics stat = new CompaniesStatistics();
            stat.Companies = dbContext.Gainers.ToList();
            if (symbol != null)
            {
                stat.symbol = symbol;
                IEXHandler webHandler = new IEXHandler();
                stat.financials = webHandler.getFinancials(symbol);
                foreach (FinancialsData data in stat.financials.financials)
                {
                    data.symbol = symbol;
                    dbContext.FinancialData.Add(data);
                }
            }
            dbContext.SaveChanges();
            return View(stat);
        }

        public IActionResult Quotes(string symbol)
        {
            PopulateGainers();
            CompaniesStatistics stat = new CompaniesStatistics();
            stat.Companies = dbContext.Gainers.ToList();
            if (symbol != null)
            {
                stat.symbol = symbol;
                IEXHandler webHandler = new IEXHandler();
                stat.quote = webHandler.getQuotes(symbol);
                dbContext.Quotes.Add(stat.quote);
            }
            dbContext.SaveChanges();
            return View(stat);
        }

        public IActionResult Gainers()
        {
            IEXHandler webHandler = new IEXHandler();
            List<StockStats> gainersList = webHandler.Gainers();
            PopulateGainers();
            return View(gainersList);
        }

        public IActionResult Losers()
        {
            IEXHandler webHandler = new IEXHandler();
            List<StockStats> losersList = webHandler.Losers();
            PopulateLosers();
            return View(losersList);
        }

        public void PopulateLosers()
        {
            IEXHandler webHandler = new IEXHandler();
            List<StockStats> losersListRes = webHandler.Losers();
            String losersData = JsonConvert.SerializeObject(losersListRes);
            List<StockStats> losersList = null;
            if (losersData != "")
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                losersList = JsonConvert.DeserializeObject<List<StockStats>>(losersData, settings);
            }

            foreach (StockStats loser in losersList)
            {
                //Database will give PK constraint violation error when trying to insert record with existing PK.
                //So add company only if it doesnt exist, check existence using symbol (PK)
                if (dbContext.Losers.Where(c => c.symbol.Equals(loser.symbol)).Count() == 0)
                {
                    Losers loserData = new Losers(loser.symbol, loser.companyName, loser.primaryExchange, loser.sector);
                    dbContext.Losers.Add(loserData);
                }
            }
            dbContext.SaveChanges();
        }

    }
}
