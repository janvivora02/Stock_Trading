using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IEXTrading.Models.ViewModel
{
    public class CompaniesStatistics
    {
        // current
        public List<Gainers> Companies { get; set; }
        public string symbol { get; set; }
        public double price { get; set; }
        public Quote quote { get; set; }
        public Financials financials { get; set; }

        public CompaniesStatistics(List<Gainers> gainers, string sym, float pric,
            Quote quot, Financials financial)
        {
            Companies = gainers;
            symbol = sym;
            price = pric;
            quote = quot;
            financials = financial;
        }

        public CompaniesStatistics()
        {
        }
    }
}
