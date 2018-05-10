using System;
using System.Collections.Generic;
using AirlinePlanner;
using MySql.Data.MySqlClient;


namespace AirlinePlanner.Models
{
  public class City
  {
    private int _id;
    private string _name;
    private string _airline;
    private static List<City> cities = new List<City> {};

    public City(string name, string airline, int id=0)
    {
      _name = name;
      _airline = airline;
      _id = id;
    }

    public string GetName()
    {
      return _name;
    }

    public string GetAirline()
    {
      return _airline;
    }

    public int GetCityId()
    {
      return _id;
    }

    public override bool Equals(System.Object otherCity)
    {
      if (!(otherCity is City))
      {
        return false;
      }
      else
      {
        City newCity = (City) otherCity;
        return this.GetName().Equals(newCity.GetName());
      }
    }

    public override int GetHashCode()
    {
         return this.GetName().GetHashCode();
    }
//Create/save new city instance to cities table
    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;

      cmd.CommandText = @"INSERT INTO cities (name, airline) VALUES (@thisName, @thisAirline);";

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@thisName";
      name.Value = this._name;
      cmd.Parameters.Add(name);

      MySqlParameter airline = new MySqlParameter();
      airline.ParameterName = "@thisAirline";
      airline.Value = this._airline;
      cmd.Parameters.Add(airline);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }
//Read/get all city instances from cities table
    public static List<City> GetAllCities()
    {
      List<City> allCities = new List<City>{};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText= @"SELECT * FROM cities;";

      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      while (rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        string airline = rdr.GetString(2);
        City newCity = new City(name, airline, id);
        allCities.Add(newCity);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allCities;
    }
//Recalls/finds city instances in cities table
    public static City Find(int citiesId)
    {
        MySqlConnection conn = DB.Connection();
        conn.Open();
        var cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"SELECT * FROM cities WHERE id = (@searchId);";

        MySqlParameter searchId = new MySqlParameter();
        searchId.ParameterName = "@searchId";
        searchId.Value = citiesId;
        cmd.Parameters.Add(searchId);

        var rdr = cmd.ExecuteReader() as MySqlDataReader;
        int id = 0;
        string name = "";
        string airline = "";

        while(rdr.Read())
        {
          id = rdr.GetInt32(0);
          name = rdr.GetString(1);
          airline = rdr.GetString(2);

        }
        City newCity = new City(name, airline, id);
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
        return newCity;
    }
//Create/save to join table
    public void AddFlightsToCity(Flight newFlight)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText= @"INSERT INTO cities_flights (cities_id, flights_id) VALUES (@CitiesId, @FlightsId);";

      MySqlParameter cities_id = new MySqlParameter();
      cities_id.ParameterName = "@CitiesId";
      cities_id.Value = _id;
      cmd.Parameters.Add(cities_id);

      MySqlParameter flights_id = new MySqlParameter();
      flights_id.ParameterName = "@FlightsId";
      flights_id.Value = newFlight.GetId();
      cmd.Parameters.Add(flights_id);
      cmd.ExecuteNonQuery();

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

    }
//Reads from join table
    public List<Flight> GetFlights()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT flights.* FROM cities
      JOIN cities_flights ON (cities.id = cities_flights.cities_id)
      JOIN flights ON (cities_flights.flights_id = flights.id)
      WHERE cities.id = @cityId;";

      MySqlParameter cityIdParameter = new MySqlParameter();
      cityIdParameter.ParameterName = "@cityId";
      cityIdParameter.Value = _id;
      cmd.Parameters.Add(cityIdParameter);

      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;

      List<Flight> flights = new List<Flight> {};
      while (rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string name = rdr.GetString(1);
        string date = rdr.GetString(2);
        string deptCity = rdr.GetString(3);
        string arrCity = rdr.GetString(4);
        string status = rdr.GetString(5);
        Flight newFlight = new Flight(name, date, deptCity, arrCity, status, id);
        flights.Add(newFlight);
      }

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return flights;


    }
//Updates single city instance in cities table
    public void UpdateCity(string newName, string newAirline)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = "@UPDATE cities SET name = @newName, airline = @newAirline WHERE id = @searchId;";

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@newName";
      name.Value = newName;
      cmd.Parameters.Add(name);
      //cmd.Parameters.Add(newMySqlParameter("@newName", _name));

      MySqlParameter airline = new MySqlParameter();
      airline.ParameterName = "@newAirline";
      airline.Value = newAirline;
      cmd.Parameters.Add(airline);
      //cmd.Parameters.Add(newMySqlParameter("@newAirline", _airline));

      cmd.ExecuteNonQuery();
      _name = newName;
      _airline = newAirline;

      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }

    }
//Deletes single city instance from cities table
    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM cities WHERE id = @Cityd;
      DELETE FROM cities_flights
      WHERE city_id = @CityId;";

      MySqlParameter cityIdParameter = new MySqlParameter();
      cityIdParameter.ParameterName = "@CityId";
      cityIdParameter.Value = this.GetCityId();
      cmd.Parameters.Add(cityIdParameter);

      cmd.ExecuteNonQuery();
      if (conn != null)
      {
        conn.Close();
      }
    }
//Deletes all city instances from cities table
    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM cities;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }

    }
  }
}
