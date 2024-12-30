using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class ApplicationContext(DbContextOptions options) : IdentityDbContext<
        User, 
        IdentityRole<Guid>,
        Guid,
        IdentityUserClaim<Guid>,
        IdentityUserRole<Guid>,
        IdentityUserLogin<Guid>, 
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>>(options)
    {
        public override DbSet<User> Users { get; set; }
        //TODO: another dbSet
    }
}
