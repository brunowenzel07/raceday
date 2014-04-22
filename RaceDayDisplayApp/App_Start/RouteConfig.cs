using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RaceDayDisplayApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default_Meetings",
                url: "Meetings/{action}/{id}",
                defaults: new { controller = "Meetings", action = "RaceList", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default_RaceResearch",
                url: "RaceResearch/{action}/{id}",
                defaults: new { controller = "RaceResearch", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Meetings", action = ConfigValues.DefaultAction, id = UrlParameter.Optional }
            );

        }
    }
}