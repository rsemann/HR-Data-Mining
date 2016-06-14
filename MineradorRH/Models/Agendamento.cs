using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MineradorRH.Models
{
    public class Agendamento
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name="Frequência")]
        public FrequenciaAgendamento FrequenciaAgendamento { get; set; }
        
        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Horário")]
        public DateTime Horario { get; set; }

        public DateTime DataCriacao { get; set; }
        public DateTime? UltimaExecucao { get; set; }

        [Required]
        public bool Ativo { get; set; }

        [ForeignKey("ConfiguracaoArvore")]
        public int ConfiguracaoArvoreID { get; set; }
        public ConfiguracaoArvore ConfiguracaoArvore { get; set; }
    }

    public enum FrequenciaAgendamento
    {
        Dia,
        Semana,
        Mês
    }
}