using System;

namespace ConsoleRoutine.Models
{
    public class QuotationData
    {
        public string vlr_cotacao { get; set; }
        public string cod_cotacao { get; set; }
        public DateTime dat_cotacao { get; set; }
    }

    public class FromToQuotation
    {
        public string ID_MOEDA { get; set; }
        public string cod_cotacao { get; set; }
    }

}
