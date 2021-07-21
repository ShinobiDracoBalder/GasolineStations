using System;
using System.Collections.Generic;
using System.Text;

namespace GasolineStations.ClassLibrary.Models
{
    public class TblFuelPriceModel
    {
        public int FuelStationId { get; set; }
        public decimal Gas_Price { get; set; }
        public int Placegas_PriceId { get; set; }
        public string Gas_PriceType { get; set; }
    }
}
