using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using AirlinePlanner.Models;
using AirlinePlanner;
using MySql.Data.MySqlClient;


namespace AirlinePlanner.Tests
{

   [TestClass]
   public class ItemTests : IDisposable
   {
       public ItemTests()
       {
           DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=airline_planner_test;";
       }

       public void Dispose()
       {
         City.DeleteAll();
         Flight.DeleteAll();
       }

       [TestMethod]
       public void Save_SavesItemToDatabase_CityList()
       {
         City testCity = new City("Seattle", "Delta");
         testCity.Save();

         List<City> testResult = City.GetAllCities();
         List<City> allCities = new List<City>{testCity};

         CollectionAssert.AreEqual(allCities, testResult);

       }

       [TestMethod]
       public void Add_FlightsToACity_JoinTable()
       {
         //Arrange
         City testCity = new City("Boston", "United");
         testCity.Save();

         Flight testFlight = new Flight("UA412", "2018-12-31", "Boston", "Seattle", "On-time");
         testFlight.Save();

         //Act
         testCity.AddFlightsToCity(testFlight);

         List<Flight> result = testCity.GetFlights();
         List<Flight> testList = new List<Flight> {testFlight};
         Console.WriteLine(result.Count);
         Console.WriteLine(Flight.GetAllFlights().Count);
         //Assert
         CollectionAssert.AreEqual(result, testList);

       }

      //  [TestMethod]
      //  public void Edit_UpdateCityDataInDatabase_Strings()
      //  {
      //    //Arrange
      //    string firstCity = "Seattle";
      //    City testCity = new City(firstCity);
      //    testCity.Save();
      //    string secondCity = "New York";
       //
      //    //Act
      //    testCity.Update(secondCity);
      //    string result = City.Find(testCity.GetCityId()).GetName();
       //
      //    //Assert
      //    Assert.AreEqual(secondCity, result);
      //  }
    }
}
