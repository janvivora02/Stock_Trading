using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IEXTrading.Models
{
    public class Financials
    {
        public string symbol { get; set; }
        public FinancialsData[] financials { get; set; }
    }

    public class FinancialsData
    {
        [Key]
        public int sno { get; set; }
        public string symbol { get; set; }
        public string reportDate { get; set; }
        public double grossProfit { get; set; }
        public double costOfRevenue { get; set; }
        public double operatingRevenue { get; set; }
        public double totalRevenue { get; set; }
        public double operatingIncome { get; set; }
        public double netIncome { get; set; }
        public double researchAndDevelopment { get; set; }
        public double operatingExpense { get; set; }
        public double currentAssets { get; set; }
        public double totalAssets { get; set; }
        public double totalLiabilities { get; set; }
        public double currentCash { get; set; }
        public double currentDebt { get; set; }
        public double totalCash { get; set; }
        public double shareholderEquity { get; set; }
        public double cashChange { get; set; }
        public double cashFlow { get; set; }
        public string operatingGainsLosses { get; set; }
    }
}
