using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace MineradorRH.DAL
{
    public class DataContext : DbContext
    {
        public DataContext()
            : base("Conexao")
        {
        }

        public DbSet<MineradorRH.Models.ArvoreGerada> ArvoreGerada { get; set; }
        public DbSet<MineradorRH.Models.ConfiguracaoArvore> ConfiguracaoArvore { get; set; }
        public DbSet<MineradorRH.Models.ConfiguracaoAtributo> ConfiguracaoAtributo { get; set; }
        public DbSet<MineradorRH.Models.AcessoBaseRh> AcessoBaseRh { get; set; }
        public DbSet<MineradorRH.Models.Agendamento> Agendamento { get; set; }

        public DbSet<MineradorRH.Models.StopWord> StopWord { get; set; }

        public DbSet<MineradorRH.Models.PalavraAgrupadora> PalavraAgrupadora { get; set; }

        public DbSet<MineradorRH.Models.DicionarioAgrupador> DicionarioAgrupador { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}