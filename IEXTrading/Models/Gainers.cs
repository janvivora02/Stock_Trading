using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IEXTrading.Models
{
    public class Gainers
    {
        [Key]
        public string symbol { get; set; }
        public string companyName { get; set; }
        public string primaryExchange { get; set; }
        public string sector { get; set; }

        public Gainers()
        {
            // parameterless constructor
        }

        public Gainers(string sym, string name, string primaryExc, string sec)
        {
            symbol = sym;
            companyName = name;
            primaryExchange = primaryExc;
            sector = sec;
        }
    }
}
