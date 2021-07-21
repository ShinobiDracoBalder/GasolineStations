using System;
using System.Collections.Generic;
using System.Text;

namespace GasolineStations.ClassLibrary.Models
{
     public class TblFuelStationModel
     {
        public int FuelStationId { get; set; }
        public string PermitNumber { get; set; }
        public string FuelName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string Reference { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int place_id { get; set; }
     }
}
