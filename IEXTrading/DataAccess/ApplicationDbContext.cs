﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IEXTrading.Models;

namespace IEXTrading.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Equity> Equities { get; set; }
        public DbSet<Gainers> Gainers { get; set; }
        public DbSet<Losers> Losers { get; set; }
        public DbSet<FinancialsData> FinancialData { get; set; }
        public DbSet<Quote> Quotes { get; set; }
    }
}
