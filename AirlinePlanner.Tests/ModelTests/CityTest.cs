using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using AirlinePlanner.Models;
using AirlinePlanner;
using MySql.Data.MySqlClient;


namespace AirlinePlanner.Tests
{
    [TestClass]
    public class FlightTests : IDisposable
    {
      public FlightTests()
      {
          DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=airline_planner_test;";
      }

      public void Dispose()
      {
        Flight.DeleteAll();
      }
    }

    [TestClass]
    public class CityTests : IDisposable
    {
     public CityTests()
     {
         DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=airline_planner_test;";
     }

     public void Dispose()
     {
       City.DeleteAll();
     }

     [TestMethod]
     public void Save_SavesCityItemToDatabase_CityTable()
     {
       City testCity = new City("Seattle", "Delta", 0);
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
      //  Console.WriteLine(result.Count);
      //  Console.WriteLine(Flight.GetAllFlights().Count);

       //Assert
       CollectionAssert.AreEqual(result, testList);

     }

     [TestMethod]
     public void Edit_UpdateCityDataInDatabase_Strings()
     {
       //Arrange
       City testCity = new City("Seattle", "Delta");
       testCity.Save();

       //Act
       testCity.UpdateCity("New York", "United");
       City newCity = City.Find(testCity.GetCityId());

       //Assert
      //  Console.WriteLine("Name: " + newCity.GetName());
      //  Console.WriteLine("Airline: " + newCity.GetAirline());
       Assert.AreEqual("New York", newCity.GetName());
       Assert.AreEqual("United", newCity.GetAirline());
     }

    [TestMethod]
    public void Delete_CityItem_CitiesTable()
    {
      //Arrange
      City testCity = new City("Seattle", "Alaska");
      testCity.Save();

      //Act
      testCity.Delete();
      City newCity = City.Find(testCity.GetCityId());

      //Assert
      // Console.WriteLine("Name: " + newCity.GetName());
      Assert.AreEqual("", newCity.GetName());
      Assert.AreEqual("", newCity.GetAirline());

    }
  }
}
