using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RoutingFailure.Controllers
{
    public class MoviesController : Controller
    {
        public IActionResult Index(int? page, string sort = null, string country = null, string person = null, string role = null, int? year = null)
        {
            if (country != null)
            {
                ViewBag.Country = country;
            }

            if (person != null)
            {
                ViewBag.Person = person;
                ViewBag.PersonRoles = new string[] { "Actor", "Director", "Producer", "Action Director" };

                if (role != null)
                {
                    ViewBag.Role = role;
                }
            }

            if (year != null)
            {
                ViewBag.Year = year;
            }

            if (sort != null)
            {
                ViewBag.Sort = sort;
            }

            if (page != null)
            {
                ViewBag.Page = page;
            }

            return View("MoviesIndex");
        }
    }
}
