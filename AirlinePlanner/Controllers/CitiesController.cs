using System;
using System.Collections.Generic;
using AirlinePlanner.Models;
using Microsoft.AspNetCore.Mvc;

namespace AirlinePlanner.Controllers
{
  public class CitiesController : Controller
  {
    [HttpGet("/cities")]
    public ActionResult Index()
    {
      List<City> allCities = City.GetAllCities();
      return View(allCities);
    }

    [HttpGet("/cities/new")]
    public ActionResult CreateForm()
    {
        return View();
    }

    [HttpPost("/cities")]
    public ActionResult Create()
    {
        City newCity = new City(Request.Form["city-name"], Request.Form["airline-name"]);
        newCity.Save();
        return RedirectToAction("Success", "Home");
    }

    [HttpGet("/cities/{id}")]
    public ActionResult Details(int cityId)
    {
        Dictionary<string, object> model = new Dictionary<string, object>();
        City selectedCity = City.Find(cityId);
        List<Flight> cityFlights = selectedCity.GetFlights();
        List<Flight> allFlights = Flight.GetAllFlights();
        model.Add("selectedCity", selectedCity);
        model.Add("cityFlights", cityFlights);
        model.Add("allFlights", allFlights);
        return View("Details", model);
    }

    [HttpPost("/cities/{cityId}/flights/new")]
    public ActionResult AddFlight(int cityId)
    {
       City city = City.Find(cityId);
       Flight flight = Flight.Find(Int32.Parse(Request.Form["flight-id"]));
       city.AddFlightsToCity(flight);
       return RedirectToAction("Details",  new { id = cityId });
    }

    [HttpPost("/cities/{id}/update")]
    public ActionResult UpdateCity(int cityId)
    {
      City updateCity = City.Find(cityId);
      return RedirectToAction("Details", updateCity);
    }

    // [HttpPost("/cities/{id}/updated")]
    // public ActionResult UpdatedCity(int cityId)
    // {
    //     string newName = Request.Form["city-name"];
    //     string newAirline = Request.Form["airline-name"];
    //     City newCity = new City(newName, newAirline, cityId);
    //     newCity.UpdateCity(newCity);
    //     return RedirectToAction("Details");
    // }

    [HttpGet("/cities/{id}/delete")]
    public ActionResult Delete(int cityId)
    {
      City selectedCity = City.Find(cityId);
      selectedCity.Delete();
      return RedirectToAction("Index");
    }

    [HttpGet("/cities/deleteall")]
    public ActionResult DeleteAll()
    {
      City.DeleteAll();
      return RedirectToAction("Index");
    }
  }
}
