﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace compWpf
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<виды> виды { get; set; }
        public virtual DbSet<дистанции> дистанции { get; set; }
        public virtual DbSet<слеты> слеты { get; set; }
        public virtual DbSet<туристы> туристы { get; set; }
        public virtual DbSet<школы> школы { get; set; }
        public virtual DbSet<штрафы> штрафы { get; set; }
        public virtual DbSet<этапы> этапы { get; set; }
        public virtual DbSet<экипажи> экипажи { get; set; }
        public virtual DbSet<результаты> результаты { get; set; }
        public virtual DbSet<суда> суда { get; set; }
    }
}
