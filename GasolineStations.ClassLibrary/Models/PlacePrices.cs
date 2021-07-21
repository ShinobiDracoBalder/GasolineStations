using System;
using System.Collections.Generic;
using System.Text;

namespace GasolineStations.ClassLibrary.Models
{
    public class PlacePrices
    {
        public string place_id { get; set; }
        public string gas_pricetype { get; set; }
        public List<GasPriceType> DetailGasPriceType { get; set; }
    }
}
