using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Library.DAL.Entities;

namespace Library.PL.Data
{
    public class LibraryPLContext : DbContext
    {
        public LibraryPLContext (DbContextOptions<LibraryPLContext> options)
            : base(options)
        {
        }

        public DbSet<Library.DAL.Entities.Users> Users { get; set; } = default!;
        public DbSet<Library.DAL.Entities.Books> Books { get; set; } = default!;
    }
}
