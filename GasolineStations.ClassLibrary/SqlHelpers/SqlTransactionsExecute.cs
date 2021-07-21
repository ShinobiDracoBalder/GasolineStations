using GasolineStations.ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace GasolineStations.ClassLibrary.SqlHelpers
{
    public class SqlTransactionsExecute
    {
        public string connectionString = @"Data Source=192.168.0.2;Initial Catalog=GasStations;Persist Security Info=True;User ID=GalfordSO;Password=8ko2aujnz4;";

        public void ExecuteSqlTransactionPrice(List<PlacePrices> placePrice)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = connection.BeginTransaction("SampleTransaction");

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connection;
                command.Transaction = transaction;
                int Counts = 0;
                try{
                    
                    Console.WriteLine($" Total Register  :{placePrice.Count()}");

                    foreach (PlacePrices ItemPlace in placePrice.ToList())
                    {
                        var Result = GetFuelStationPrice(ItemPlace.place_id);

                        var _ResultPrice = GetFuelStPrices(Result.FuelStationId.ToString());

                        //if (Result.FuelStationId > 0 && _ResultPrice.Count == 0)
                        if (Result.FuelStationId > 0){
                            Console.WriteLine("******  Open Price Insert *******");

                            foreach (var itemPrice in ItemPlace.DetailGasPriceType)
                            {
                                command.CommandText =
                                $"{"Insert into ProductServices.Placegas_Prices Values(@Gas_Price, @Gas_PriceType, @FuelStationId,0, GETDATE())"}";

                                Console.WriteLine($"{"Gas_Price      :"}{itemPrice.Gas_Price}");
                                Console.WriteLine($"{"Gas_PriceType  :"}{itemPrice.Type.ToUpper()}");
                                Console.WriteLine($"{"FuelStationId  :"}{Result.FuelStationId}");
                                Console.WriteLine($"{"PermitNumber   :"}{Result.PermitNumber}");


                                command.Parameters.Clear();
                                command.Parameters.Add("@Gas_Price", SqlDbType.Money).Value = itemPrice.Gas_Price;
                                command.Parameters.Add("@Gas_PriceType", SqlDbType.VarChar, 50).Value = itemPrice.Type.ToUpper();
                                command.Parameters.Add("@FuelStationId", SqlDbType.Int).Value = Result.FuelStationId;


                                command.ExecuteNonQuery();
                            }
                        }
                        //else if (_ResultPrice.Count > 0)
                        //{
                        //    command.CommandText =
                        //     $"{" Update  [ProductServices].[Placegas_Prices] Set Gas_Price = @Gas_Price where FuelStationId = @FuelStationId and Gas_PriceType = @Gas_PriceType "}";
                        //    foreach (var itemPrice in ItemPlace.DetailGasPriceType)
                        //    {
                        //        var _prices = _ResultPrice
                        //            .Where(p => p.FuelStationId == Result.FuelStationId && p.Gas_PriceType == itemPrice.Type)
                        //            .FirstOrDefault();

                        //        Console.WriteLine($"{"Gas_Price      :"}{itemPrice.Gas_Price}");
                        //        Console.WriteLine($"{"Gas_PriceType  :"}{itemPrice.Type}");
                        //        Console.WriteLine($"{"FuelStationId  :"}{_prices.FuelStationId}");
                        //        Console.WriteLine($"{"PermitNumber   :"}{Result.PermitNumber}");


                        //        command.Parameters.Clear();
                        //        command.Parameters.Add("@Gas_Price", SqlDbType.Money).Value = _prices.Gas_Price;
                        //        command.Parameters.Add("@Gas_PriceType", SqlDbType.VarChar, 50).Value = _prices.Gas_PriceType;
                        //        command.Parameters.Add("@FuelStationId", SqlDbType.Int).Value = _prices.FuelStationId;

                        //        command.ExecuteNonQuery();
                        //    }
                        //}
                        Counts++;
                        Console.WriteLine(Counts);
                        Console.WriteLine($"{"****** Not Open *******   "}{ItemPlace.gas_pricetype}");

                    }

                    // Attempt to commit the transaction.
                    transaction.Commit();
                    Console.WriteLine("Both records are written to database.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);

                    // Attempt to roll back the transaction.
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        // This catch block will handle any errors that may have occurred
                        // on the server that would cause the rollback to fail, such as
                        // a closed connection.
                        Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                        Console.WriteLine("  Message: {0}", ex2.Message);
                    }
                }
            }
        }
        public List<TblFuelStationModel> GetFuelStations()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand($"{" Select f.FuelStationId,f.PermitNumber,f.FuelName, f.place_id, f.Latitude, f.Longitude from [AdminServices].[TblFuelStations] f with(Nolock) "}", con))
                {
                    List<TblFuelStationModel> customers = new List<TblFuelStationModel>();
                    cmd.CommandType = CommandType.Text;
                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            customers.Add(new TblFuelStationModel
                            {
                                FuelStationId = Convert.ToInt32(sdr["FuelStationId"]),
                                FuelName = sdr["FuelName"].ToString(),
                                PermitNumber = sdr["PermitNumber"].ToString(),
                                place_id = Convert.ToInt32(sdr["place_id"].ToString()),
                                Latitude = Convert.ToDecimal(sdr["Latitude"].ToString()),
                                Longitude = Convert.ToDecimal(sdr["Longitude"].ToString()),
                            });
                        }
                    }
                    con.Close();
                    return customers;
                }
            }
        }
        public List<TblFuelPriceModel> GetAllPPriceS()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand($"{" Select pp.FuelStationId, pp.Placegas_PriceId, pp.Gas_Price, pp.Gas_PriceType from [ProductServices].[Placegas_Prices]  pp With(Nolock) "}", con))
                {
                    List<TblFuelPriceModel> PriceModel = new List<TblFuelPriceModel>();
                    cmd.CommandType = CommandType.Text;
                    con.Open();

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            PriceModel.Add(new TblFuelPriceModel
                            {
                                FuelStationId = Convert.ToInt32(sdr["FuelStationId"]),
                                Gas_Price = Convert.ToDecimal(sdr["Gas_Price"]),
                                Gas_PriceType = sdr["Gas_PriceType"].ToString(),
                                Placegas_PriceId = Convert.ToInt32(sdr["Placegas_PriceId"].ToString()),
                            });
                        }
                    }
                    con.Close();
                    return PriceModel;
                }
            }
        }
        private TblFuelStationModel GetFuelStationPrice(string place_id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand($"{"Select f.FuelStationId,f.PermitNumber,f.FuelName, f.place_id, f.Latitude, f.Longitude "}{" "}{"from [AdminServices].[TblFuelStations] f with(Nolock) Where f.place_id=@place_id"}", con))
                {
                    TblFuelStationModel tblFuelStation = new TblFuelStationModel();
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    cmd.Parameters.AddWithValue("@place_id", place_id);
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {

                                tblFuelStation.FuelStationId = Convert.ToInt32(sdr["FuelStationId"]);
                                tblFuelStation.FuelName = sdr["FuelName"].ToString();
                                tblFuelStation.PermitNumber = sdr["PermitNumber"].ToString();

                                Console.WriteLine($"{"place_id >>>>>"}{sdr["place_id"].ToString()}");

                                //tblFuelStation.place_id = Convert.ToInt32(sdr["place_id"].ToString());
                                //tblFuelStation.Latitude = Convert.ToDecimal(sdr["Latitude"].ToString());
                                //tblFuelStation.Longitude = Convert.ToDecimal(sdr["Longitude"].ToString());

                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine("  Message: {0}", ex.Message);
                    }
                    con.Close();
                    return tblFuelStation;
                }
            }
        }
        private List<TblFuelPriceModel> GetFuelStPrices(string FuelStationId)
        {
            List<TblFuelPriceModel> _ListPrice = new List<TblFuelPriceModel>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand($"{" Select pp.FuelStationId, pp.Gas_Price, pp.Placegas_PriceId,pp.Gas_PriceType "}{" "}{" from [ProductServices].[Placegas_Prices] pp With(Nolock) Where pp.FuelStationId =@FuelStationId "}", con))
                {
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    cmd.Parameters.AddWithValue("@FuelStationId", FuelStationId);
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {

                                var _price = new TblFuelPriceModel
                                {
                                    FuelStationId = Convert.ToInt32(sdr["FuelStationId"]),
                                    Gas_Price = Convert.ToDecimal(sdr["Gas_Price"]),
                                    Placegas_PriceId = Convert.ToInt32(sdr["Placegas_PriceId"]),
                                    Gas_PriceType = sdr["Gas_PriceType"].ToString(),
                                };

                                _ListPrice.Add(_price);
                                Console.WriteLine($"{"***** Placegas_PriceId *******"}{sdr["Placegas_PriceId"].ToString()}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine("  Message: {0}", ex.Message);
                    }
                    con.Close();
                    return _ListPrice;
                }
            }
        }
        public void ExecuteSqlTransaction(List<Places> places)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = connection.BeginTransaction("SampleTransaction");

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connection;
                command.Transaction = transaction;
                int Counts = 0;
                try
                {
                    
                    Console.WriteLine($" Total Register  :{places.Count()}");

                    foreach (Places Items in places.ToList())
                    {
                        var Result = GetFuelStation(Items.cre_id);

                        if (Result.FuelStationId > 0)
                        {
                            command.CommandText =
                            $"{"Update[AdminServices].[TblFuelStations] Set place_id=@place_id, Latitude=@Latitude, Longitude=@Longitude  where PermitNumber=@PermitNumber"}";


                            Console.WriteLine("****** Update Open TblFuelStations *******");
                            command.Parameters.Clear();
                            command.Parameters.Add("@place_id", SqlDbType.Int).Value = Items.place_id;
                            command.Parameters.Add("@Latitude", SqlDbType.Decimal).Value = Items.y;
                            command.Parameters.Add("@Longitude", SqlDbType.Decimal).Value = Items.x;
                            command.Parameters.Add("@PermitNumber", SqlDbType.VarChar).Value = Items.cre_id;

                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            command.CommandText =
                            $"{"Insert Into [AdminServices].[TblFuelStations]Values(@PermitNumber,@FuelName,@Address,@PostalCode,@Reference,@Latitude,@Longitude,0,GetDate(),@place_id);"}";

                            Console.WriteLine("****** Insert Open TblFuelStations *******");
                            command.Parameters.Clear();

                            command.Parameters.Add("@PermitNumber", SqlDbType.VarChar).Value = Items.cre_id;
                            command.Parameters.Add("@FuelName", SqlDbType.VarChar).Value = Items.PlaceName.ToUpper();
                            command.Parameters.Add("@Address", SqlDbType.VarChar).Value = "S/D";
                            command.Parameters.Add("@PostalCode", SqlDbType.VarChar).Value = "S/D";
                            command.Parameters.Add("@Reference", SqlDbType.VarChar).Value = "S/D";
                            command.Parameters.Add("@place_id", SqlDbType.Int).Value = Items.place_id;
                            command.Parameters.Add("@Latitude", SqlDbType.Decimal).Value = Items.y;
                            command.Parameters.Add("@Longitude", SqlDbType.Decimal).Value = Items.x;

                            command.ExecuteNonQuery();
                        }
                        Counts++;
                        Console.WriteLine(Counts);
                        Console.WriteLine($"{"****** Not Open *******   "}{Items.cre_id}");

                    }

                    // Attempt to commit the transaction.
                    transaction.Commit();
                    Console.WriteLine("Both records are written to database.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);

                    // Attempt to roll back the transaction.
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        // This catch block will handle any errors that may have occurred
                        // on the server that would cause the rollback to fail, such as
                        // a closed connection.
                        Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                        Console.WriteLine("  Message: {0}", ex2.Message);
                    }
                }
            }
        }
        private TblFuelStationModel GetFuelStation(string PermitNumber)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand($"{"Select f.FuelStationId,f.PermitNumber,f.FuelName, f.place_id, f.Latitude, f.Longitude "}{" "}{"from [AdminServices].[TblFuelStations] f with(Nolock) Where f.PermitNumber=@PermitNumber"}", con))
                {
                    TblFuelStationModel tblFuelStation = new TblFuelStationModel();
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    cmd.Parameters.AddWithValue("@PermitNumber", PermitNumber);
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {

                                tblFuelStation.FuelStationId = Convert.ToInt32(sdr["FuelStationId"]);
                                tblFuelStation.FuelName = sdr["FuelName"].ToString();
                                tblFuelStation.PermitNumber = sdr["PermitNumber"].ToString();

                                Console.WriteLine($"{"place_id >>>>>"}{sdr["place_id"].ToString()}");

                                //tblFuelStation.place_id = Convert.ToInt32(sdr["place_id"].ToString());
                                //tblFuelStation.Latitude = Convert.ToDecimal(sdr["Latitude"].ToString());
                                //tblFuelStation.Longitude = Convert.ToDecimal(sdr["Longitude"].ToString());

                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine("  Message: {0}", ex.Message);
                    }
                    con.Close();
                    return tblFuelStation;
                }
            }
        }

        public void ExecuteSqlTransUpdatePrice(List<PlacePrices> placePrice){
            using (SqlConnection connection = new SqlConnection(connectionString)){
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = connection.BeginTransaction("SampleTransaction");

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connection;
                command.Transaction = transaction;
                int Counts = 0;
                try{

                    Console.WriteLine($" Total Register  :{placePrice.Count()}");

                    foreach (PlacePrices ItemPlace in placePrice.ToList())
                    {
                        var Result = GetFuelStationPrice(ItemPlace.place_id);
                        if (Result.FuelStationId > 0)
                        {
                            Console.WriteLine("******  Open Price Update *******");
                            command.CommandText =
                             $"{" Update  [ProductServices].[Placegas_Prices] Set Gas_Price = @Gas_Price where FuelStationId = @FuelStationId and Gas_PriceType = @Gas_PriceType "}";
                            foreach (var itemPrice in ItemPlace.DetailGasPriceType)
                            {
                               
                                Console.WriteLine($"{"Gas_Price      :"}{itemPrice.Gas_Price}");
                                Console.WriteLine($"{"Gas_PriceType  :"}{itemPrice.Type.ToUpper()}");
                                Console.WriteLine($"{"FuelStationId  :"}{Result.FuelStationId}");
                                Console.WriteLine($"{"PermitNumber   :"}{Result.PermitNumber}");


                                command.Parameters.Clear();
                                command.Parameters.Add("@Gas_Price", SqlDbType.Money).Value = itemPrice.Gas_Price;
                                command.Parameters.Add("@Gas_PriceType", SqlDbType.VarChar, 50).Value = itemPrice.Type.ToUpper();
                                command.Parameters.Add("@FuelStationId", SqlDbType.Int).Value = Result.FuelStationId;

                                command.ExecuteNonQuery();
                            }
                        }

                        Counts++;
                        Console.WriteLine(Counts);
                        Console.WriteLine($"{"****** Not Open *******   "}{ItemPlace.gas_pricetype}");

                    }

                    // Attempt to commit the transaction.
                    transaction.Commit();
                    Console.WriteLine("Both records are written to database.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);

                    // Attempt to roll back the transaction.
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        // This catch block will handle any errors that may have occurred
                        // on the server that would cause the rollback to fail, such as
                        // a closed connection.
                        Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                        Console.WriteLine("  Message: {0}", ex2.Message);
                    }
                }
            }
        }
    }

}
