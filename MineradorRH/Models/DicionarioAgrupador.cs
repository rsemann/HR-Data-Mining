using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MineradorRH.Models
{
    public class DicionarioAgrupador
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Palavra { get; set; }

        [Required]
        [DisplayName("Palavra sinônimo")]
        public string PalavraSinonimo { get; set; }

        [ForeignKey("PalavraAgrupadora")]
        [DisplayName("Agrupador")]
        public int PalavraAgrupadoraID { get; set; }

        [DisplayName("Agrupador")]
        public PalavraAgrupadora PalavraAgrupadora { get; set; }
    }
}