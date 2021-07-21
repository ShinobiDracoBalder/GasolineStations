using GasolineStations.ClassLibrary.Models;
using GasolineStations.ClassLibrary.SqlHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace GasolineStations.ClassLibrary.Helpers
{
    public class  ReadPlaceXml
    {
        SqlTransactionsExecute _executeSql = new SqlTransactionsExecute();
        public  void RDirectoryInfo() {
            DirectoryInfo directoryInfo = new DirectoryInfo(@"C:\Fueldata");
            FileInfo[] fileNames = directoryInfo.GetFiles("*.xml");
            Dictionary<int, string> fileDictionary = new Dictionary<int, string>();
            //store the files
            for (int i = 0; i < fileNames.Length; i++)
            {
                // use Name here so user doesn't need to enter full path of file for a full match
                var _Name = fileNames[i].Name;
                var _path = fileNames[i].FullName;

                if (_Name.Contains("places")){
                    Console.WriteLine($"Read Place.xml  {_Name}");
                    Console.WriteLine($"Path Place.xml  {_path}");

                   var ListPlace = ReadXmlPlaces(_path);
                    
                       _executeSql.ExecuteSqlTransaction(ListPlace);

                } else if (_Name.Contains("prices")) {
                    Console.WriteLine($"Read Price.xml {_Name}");
                    Console.WriteLine($"Path Place.xml {_path}");

                   var ListPrice = XmlDataPrices(_path);
                    var _allprices = _executeSql.GetAllPPriceS();
                    if (_allprices.Count == 0)
                    {
                        _executeSql.ExecuteSqlTransactionPrice(ListPrice);
                    }
                    else
                    {
                        _executeSql.ExecuteSqlTransUpdatePrice(ListPrice);
                    }

                }

                fileDictionary.Add(i, fileNames[i].Name);
            }
        }

        public List<Places> ReadXmlPlaces(string Path)
        {
            StringBuilder result = new StringBuilder();
            List<Places> Lplace = new List<Places>();
            foreach (XElement level1Element in XElement.Load(Path).Elements("place"))
            {
                result.AppendLine(level1Element.Attribute("place_id").Value);
                Console.WriteLine($"{"place_id  :"}{level1Element.Attribute("place_id").Value}");
                Console.WriteLine($"{"name      :"}{level1Element.Element("name").Value}");
                Console.WriteLine($"{"cre_id :"}{level1Element.Element("cre_id").Value}");

                Places _place = new Places
                {
                    place_id = level1Element.Attribute("place_id").Value,
                    PlaceName = level1Element.Element("name").Value,
                    cre_id = level1Element.Element("cre_id").Value
                };

                foreach (XElement level2Element in level1Element.Elements("location"))
                {
                    Console.WriteLine($"{"X     :"}{level2Element.Element("x").Value}");
                    Console.WriteLine($"{"Y      :"}{level2Element.Element("y").Value}");
                    _place.x = level2Element.Element("x").Value;
                    _place.y = level2Element.Element("y").Value;
                }

                Lplace.Add(_place);
            }
            Console.WriteLine(Lplace.Count);
            //Console.WriteLine(result.ToString());
            return Lplace;

        }

        public List<PlacePrices> XmlDataPrices(string Path){
            List<PlacePrices> placePrices = new List<PlacePrices>();

            StringBuilder result = new StringBuilder();

            foreach (XElement leve1Element in XElement.Load(Path).Elements("place"))
            {
                result.AppendLine(leve1Element.Attribute("place_id").Value);
                Console.WriteLine(leve1Element.Attribute("place_id").Value);

                PlacePrices _placePrice = new PlacePrices
                {
                    place_id = leve1Element.Attribute("place_id").Value,
                };
                List<GasPriceType> _GasPriceType = new List<GasPriceType>();
                foreach (XElement leve2Element in leve1Element.Elements("gas_price"))
                {

                    Console.WriteLine(leve2Element.Attribute("type").Value);
                    Console.WriteLine(leve2Element.LastNode);
                    Console.WriteLine($"{"Gas Price Type :  "}{$"{leve2Element.Attribute("type").Value}{"-"}{leve2Element.LastNode}"}");


                    _GasPriceType.Add(new GasPriceType
                    {
                        Type = leve2Element.Attribute("type").Value,
                        Gas_Price = leve2Element.LastNode.ToString(),
                    });

                    _placePrice.DetailGasPriceType = _GasPriceType;

                    _placePrice.gas_pricetype = $"{leve2Element.Attribute("type").Value}{"-"}{leve2Element.LastNode}";
                }

                placePrices.Add(_placePrice);
            }
            Console.WriteLine(placePrices.Count);

            Console.WriteLine("Exit");

            return placePrices;

        }
    }
}
