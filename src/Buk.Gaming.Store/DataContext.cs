using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;

namespace Buk.Gaming.Store
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
    }
}
