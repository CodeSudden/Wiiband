using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wiijump.Data
{
    internal class WiijumpDbContext : DbContext
    {
        public DbSet<Wiijump> Wiijumps { get; set; }
    }
}
