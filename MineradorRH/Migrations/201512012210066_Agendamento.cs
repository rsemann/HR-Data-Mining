namespace MineradorRH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Agendamento : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AcessoBaseRh",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Servidor = c.String(nullable: false),
                        Base = c.String(nullable: false),
                        UsuarioAcesso = c.String(nullable: false),
                        SenhaAcesso = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Agendamento",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FrequenciaAgendamento = c.Int(nullable: false),
                        Horario = c.DateTime(nullable: false),
                        DataCriacao = c.DateTime(nullable: false),
                        UltimaExecucao = c.DateTime(),
                        Ativo = c.Boolean(nullable: false),
                        ConfiguracaoArvoreID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ConfiguracaoArvore", t => t.ConfiguracaoArvoreID, cascadeDelete: true)
                .Index(t => t.ConfiguracaoArvoreID);
            
            CreateTable(
                "dbo.ConfiguracaoArvore",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false),
                        DataCriacao = c.DateTime(nullable: false),
                        Sql = c.String(nullable: false),
                        Tabela = c.String(),
                        Descricao = c.String(nullable: false),
                        Poda = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ArvoreGerada",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        XML = c.String(),
                        JSON = c.String(),
                        JsonNuvemPalavras = c.String(),
                        DataGeracao = c.DateTime(nullable: false),
                        UsuarioGeracao = c.String(),
                        XmlPmml = c.String(),
                        ClasseMeta = c.String(),
                        ConfiguracaoArvoreID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ConfiguracaoArvore", t => t.ConfiguracaoArvoreID, cascadeDelete: true)
                .Index(t => t.ConfiguracaoArvoreID);
            
            CreateTable(
                "dbo.ConfiguracaoAtributo",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false),
                        Tipo = c.Int(nullable: false),
                        Nivel = c.Int(nullable: false),
                        Legenda = c.String(nullable: false),
                        ClasseMeta = c.String(),
                        ConfiguracaoArvoreID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ConfiguracaoArvore", t => t.ConfiguracaoArvoreID, cascadeDelete: true)
                .Index(t => t.ConfiguracaoArvoreID);
            
            CreateTable(
                "dbo.DicionarioAgrupador",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Palavra = c.String(nullable: false),
                        PalavraSinonimo = c.String(nullable: false),
                        PalavraAgrupadoraID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.PalavraAgrupadora", t => t.PalavraAgrupadoraID, cascadeDelete: true)
                .Index(t => t.PalavraAgrupadoraID);
            
            CreateTable(
                "dbo.PalavraAgrupadora",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Palavra = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.StopWord",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Palavra = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DicionarioAgrupador", "PalavraAgrupadoraID", "dbo.PalavraAgrupadora");
            DropForeignKey("dbo.ConfiguracaoAtributo", "ConfiguracaoArvoreID", "dbo.ConfiguracaoArvore");
            DropForeignKey("dbo.ArvoreGerada", "ConfiguracaoArvoreID", "dbo.ConfiguracaoArvore");
            DropForeignKey("dbo.Agendamento", "ConfiguracaoArvoreID", "dbo.ConfiguracaoArvore");
            DropIndex("dbo.DicionarioAgrupador", new[] { "PalavraAgrupadoraID" });
            DropIndex("dbo.ConfiguracaoAtributo", new[] { "ConfiguracaoArvoreID" });
            DropIndex("dbo.ArvoreGerada", new[] { "ConfiguracaoArvoreID" });
            DropIndex("dbo.Agendamento", new[] { "ConfiguracaoArvoreID" });
            DropTable("dbo.StopWord");
            DropTable("dbo.PalavraAgrupadora");
            DropTable("dbo.DicionarioAgrupador");
            DropTable("dbo.ConfiguracaoAtributo");
            DropTable("dbo.ArvoreGerada");
            DropTable("dbo.ConfiguracaoArvore");
            DropTable("dbo.Agendamento");
            DropTable("dbo.AcessoBaseRh");
        }
    }
}
