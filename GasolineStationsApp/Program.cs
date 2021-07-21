using GasolineStations.ClassLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.IO;

namespace GasolineStationsApp
{
    public class Program
    {
        
        static void Main(string[] args)
        {
            ReadPlaceXml _readXml = new ReadPlaceXml();
           
            Console.WriteLine("Hello World!");

            _readXml.RDirectoryInfo();

            Console.ReadKey();
        }
    }
}
