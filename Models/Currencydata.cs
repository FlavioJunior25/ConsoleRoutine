using System;

namespace ConsoleRoutine.Models
{
    public class CurrencyData
    {
        public string ID_MOEDA { get; set; }
        public DateTime DATA_REF { get; set; }
    }

    public class CurrencyQuotationResult
    {
        public string ID_MOEDA { get; set; }
        public DateTime DATA_REF { get; set; }
        public string VL_COTACAO { get; set; }
    }
}
