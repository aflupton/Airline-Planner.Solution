using System;
using System.Collections.Generic;
using AirlinePlanner.Models;
using Microsoft.AspNetCore.Mvc;

namespace AirlinePlanner.Controllers
{
  public class FlightsController : Controller
  {
    [HttpGet("/flights")]
    public ActionResult Index()
    {
      List<Flight> allFlights = Flight.GetAllFlights();
      return View(allFlights);
    }

    [HttpGet("/flights/new")]
    public ActionResult CreateForm()
    {
        return View();
    }

    [HttpPost("/flights")]
    public ActionResult Create()
    {
        Flight newFlight = new Flight(Request.Form["flight-name"], Request.Form["date"], Request.Form["dept-city"], Request.Form["arrival-city"], Request.Form["status"]);
        newFlight.Save();
        return RedirectToAction("Success", "Home");
    }

    [HttpGet("/flights/{id}")]
    public ActionResult Details(int id)
    {
      Dictionary<string, object> model = new Dictionary<string, object>();
      Flight selectedFlight = Flight.Find(id);
      List<City> flightCities = selectedFlight.GetCities();
      List<City> allCities = City.GetAllCities();
      model.Add("selectedFlight", selectedFlight);
      model.Add("flightCities", flightCities);
      model.Add("allCities", allCities);
      return View("Details", model);
    }

    [HttpPost("/flights/{flightId}/cities/new")]
    public ActionResult AddCity(int flightId)
    {
      Flight flight = Flight.Find(flightId);
      City city = City.Find(Int32.Parse(Request.Form["city-id"]));
      flight.AddCitiesToFlights(city);
      return RedirectToAction("Details",  new { id = flightId });
    }

    [HttpPost("/flights/{id}/update")]
    public ActionResult UpdateFlight(int id)
    {
      Flight updateFlight = Flight.Find(id);
      return RedirectToAction("Details", updateFlight);
    }

    [HttpGet("flights/{id}/delete")]
    public ActionResult Delete(int id)
    {
      Flight selectedFlight = Flight.Find(id);
      selectedFlight.Delete();
      return RedirectToAction("Index");
    }

    [HttpGet("/flights/deleteall")]
    public ActionResult DeleteAll()
    {
      Flight.DeleteAll();
      return RedirectToAction("Index");
    }
  }
}
