﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MIS.Models.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MISEntities : DbContext
    {
        public MISEntities()
            : base("name=MISEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Author> Author { get; set; }
        public virtual DbSet<Book> Book { get; set; }
        public virtual DbSet<BookShelf> BookShelf { get; set; }
        public virtual DbSet<Deliverier> Deliverier { get; set; }
        public virtual DbSet<Delivery> Delivery { get; set; }
        public virtual DbSet<Genre> Genre { get; set; }
        public virtual DbSet<Post> Post { get; set; }
        public virtual DbSet<Publsher> Publsher { get; set; }
        public virtual DbSet<Shelf> Shelf { get; set; }
        public virtual DbSet<User> User { get; set; }
    }
}
