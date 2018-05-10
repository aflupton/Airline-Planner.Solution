using System;
using System.Collections.Generic;
using AirlinePlanner;
using MySql.Data.MySqlClient;

namespace AirlinePlanner.Models
{
  public class Flight
  {
    private int _id;
    private string _flightName;
    private string _date;
    private string _deptCity;
    private string _arrivalCity;
    private string _status;

    public Flight(string flightName, string date, string departureCity, string arrivalCity, string status, int id = 0)
    {
      _flightName = flightName;
      _date = date;
      _deptCity = departureCity;
      _arrivalCity = arrivalCity;
      _status = status;
      _id = id;
    }
    public string GetFlightName()
    {
      return _flightName;
    }

    public void SetFlightName(string newFlightName)
    {
      _flightName = newFlightName;
    }

    public string GetDate()
    {
      return _date;
    }

    public void SetDate(string newDate)
    {
      _date = newDate;
    }

    public string GetDepartureCity()
    {
      return _deptCity;
    }

    public void SetDepartureCity(string newDepartureCity)
    {
      _deptCity = newDepartureCity;
    }

    public string GetArrivalCity()
    {
      return _arrivalCity;
    }

    public void SetArrivalCity(string newArrivalCity)
    {
      _arrivalCity = newArrivalCity;
    }

    public string GetStatus()
    {
      return _status;
    }

    public void SetStatus(string newStatus)
    {
      _status = newStatus;
    }

    public int GetId()
    {
      return _id;
    }

    public override bool Equals(System.Object otherFlight)
    {
      if (!(otherFlight is Flight))
      {
        return false;
      }
      else
      {
        Flight newFlight = (Flight) otherFlight;
        return this.GetFlightName().Equals(newFlight.GetFlightName());
      }
    }

    public override int GetHashCode()
    {
         return this.GetFlightName().GetHashCode();
    }
//Create/save flight instance to flights table
    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO flights (flight_name, date, dept_city, arrival_city, status) VALUES (@thisName, @thisDate, @thisDeparture, @thisArrival, @thisStatus);";

      // MySqlParameter flightName = new MySqlParameter();
      // flightName.ParameterName = "@thisName";
      // flightName.Value = this._name;
      // cmd.Parameters.Add(flightName);
      cmd.Parameters.Add(new MySqlParameter("@thisName", _flightName));

      //MySqlParameter date = new MySqlParameter();
      // date.ParameterName = "@thisDate";
      // date.Value = this._date;
      // cmd.Parameters.Add(flightName);
      cmd.Parameters.Add(new MySqlParameter("@thisDate", _date));

      //MySqlParameter departureCity = new MySqlParameter();
      // departureCity.ParameterName = "@thisDeparture";
      // departureCity.Value = this._deptCity;
      // cmd.Parameters.Add(departureCity);
      cmd.Parameters.Add(new MySqlParameter("@thisDeparture", _deptCity));

      //MySqlParameter arrivalCity = new MySqlParameter();
      // arrivalCity.ParameterName = "@thisArrival";
      // arrivalCity.Value = this._arrivalCity;
      // cmd.Parameters.Add(arrivalCity);
      cmd.Parameters.Add(new MySqlParameter("@thisArrival", _arrivalCity));

      //MySqlParameter status = new MySqlParameter();
      // status.ParameterName = "@thisStatus";
      // status.Value = this._status;
      // cmd.Parameters.Add(status);
      cmd.Parameters.Add(new MySqlParameter("@thisStatus", _status));

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }
    }
//Read/get all flight instances from flights table
    public static List<Flight> GetAllFlights()
    {
      List<Flight> allFlights = new List<Flight> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM flights";
      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;

      while(rdr.Read())
      {
        int id = rdr.GetInt32(0);
        string flightName = rdr.GetString(1);
        string date = rdr.GetString(2);
        string departureCity = rdr.GetString(3);
        string arrivalCity = rdr.GetString(4);
        string status = rdr.GetString(5);
        Flight newFlight = new Flight(flightName, date, departureCity, arrivalCity, status, id);
        allFlights.Add(newFlight);
      }
      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }
      return allFlights;

    }
//Create/save to join table
    public void AddCitiesToFlights(City newCity)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText= @"INSERT INTO cities_flights (cities_id, flights_id) VALUES (@CitiesId, @FlightsId);";

      MySqlParameter flights_id = new MySqlParameter();
      flights_id.ParameterName = "@FlightsId";
      flights_id.Value = _id;
      cmd.Parameters.Add(flights_id);

      MySqlParameter cities_id = new MySqlParameter();
      cities_id.ParameterName = "@CitiesId";
      cities_id.Value = newCity.GetCityId();
      cmd.Parameters.Add(cities_id);
      cmd.ExecuteNonQuery();

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }

    }
//Reads from join table
    public List<City> GetCities()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT cities.* FROM flights
      JOIN cities_flights ON (flights.id = cities_flights.flights_id)
      JOIN cities ON (cities_flights.cities_id = cities.id)
      WHERE flights.id = @FlightId;";

      MySqlParameter flightIdParameter = new MySqlParameter();
      flightIdParameter.ParameterName = "@FlightId";
      flightIdParameter.Value = _id;
      cmd.Parameters.Add(flightIdParameter);

      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      List<City> cities = new List<City> {};

      while(rdr.Read())
      {
        int cityId = rdr.GetInt32(0);
        string cityName = rdr.GetString(1);
        string cityAirline = rdr.GetString(2);
        City newCity = new City(cityName, cityAirline);
        cities.Add(newCity);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return cities;
    }
//Recalls/finds flight instances in flights table
    public static Flight Find(int flightsId)
    {
        MySqlConnection conn = DB.Connection();
        conn.Open();
        var cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"SELECT * FROM flights WHERE id = (@searchId);";

        MySqlParameter searchId = new MySqlParameter();
        searchId.ParameterName = "@searchId";
        searchId.Value = flightsId;
        cmd.Parameters.Add(searchId);

        var rdr = cmd.ExecuteReader() as MySqlDataReader;
        int id = 0;
        string flight_name = "";
        string date = "";
        string dept_city = "";
        string arrival_city = "";
        string status = "";

        while(rdr.Read())
        {
          id = rdr.GetInt32(0);
          flight_name = rdr.GetString(1);
          date = rdr.GetString(2);
          dept_city = rdr.GetString(3);
          arrival_city = rdr.GetString(4);
          status = rdr.GetString(5);
        }
        Flight newFlight = new Flight(flight_name, date, dept_city, arrival_city, status, id);
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
        return newFlight;
    }
//Update single flight instance in flights table
    public void UpdateStatus(string newName, string newDate, string newDeparture, string newArrival, string newStatus)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"UPDATE flights SET flight_name = @newName, date = @newDate, dept_city = @newDept, arrival_city = @newArrival, status = @newStatus WHERE id = @searchId;";

      MySqlParameter searchId = new MySqlParameter();
      searchId.ParameterName = "@searchId";
      searchId.Value = _id;
      cmd.Parameters.Add(searchId);
      //cmd.Parameters.Add(newMySqlParameter("@searchId", _id));

      MySqlParameter flight_name = new MySqlParameter();
      flight_name.ParameterName = "@newName";
      flight_name.Value = newName;
      cmd.Parameters.Add(flight_name);
      //cmd.Parameters.Add(new MySqlParameter("@newName", _flightName));

      MySqlParameter date = new MySqlParameter();
      date.ParameterName = "@newDate";
      date.Value = newDate;
      cmd.Parameters.Add(date);
      //cmd.Parameters.Add(new MySqlParameter("@newDate", _date));

      MySqlParameter dept_city = new MySqlParameter();
      dept_city.ParameterName = "@newDept";
      dept_city.Value = newDeparture;
      cmd.Parameters.Add(dept_city);
      //cmd.Parameters.Add(new MySqlParameter("@newDept", _deptCity));

      MySqlParameter arrival_city = new MySqlParameter();
      arrival_city.ParameterName = "@newArrival";
      arrival_city.Value = newStatus;
      cmd.Parameters.Add(arrival_city);
      //cmd.Parameters.Add(new MySqlParameter("@newArrival", _arrivalCity));

      MySqlParameter status = new MySqlParameter();
      status.ParameterName = "@newStatus";
      status.Value = newStatus;
      cmd.Parameters.Add(status);
      //cmd.Parameters.Add(new MySqlParameter("@newStatus", _status));

      cmd.ExecuteNonQuery();
      _flightName = newName;
      _date = newDate;
      _deptCity = newDeparture;
      _arrivalCity = newArrival;
      _status = newStatus;

      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
    }
//Delete single flight instance from flights table
    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM flights WHERE id = @FlightId;
      DELETE FROM cities_flights
      WHERE flight_id = @FlightId;";

      MySqlParameter flightIdParameter = new MySqlParameter();
      flightIdParameter.ParameterName = "@FlightId";
      flightIdParameter.Value = this.GetId();
      cmd.Parameters.Add(flightIdParameter);

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }
//Delete all flight instances from flights table
    public static void DeleteAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM flights;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }
    }
  }
}
