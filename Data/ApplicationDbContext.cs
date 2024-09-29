﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using XDeco.Models;

namespace Proyecto.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Categoria> Categorias { get; set; }

    public DbSet<Producto> Productos { get; set; }

    public DbSet<Dimensions> Dimensiones { get; set; }
}
