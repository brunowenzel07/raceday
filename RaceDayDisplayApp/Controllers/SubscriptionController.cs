using RaceDayDisplayApp.DAL;
using RaceDayDisplayApp.Models;
using System.Web.Mvc;
using System.Collections.Generic;
using WebMatrix.WebData;
using System;

namespace RaceDayDisplayApp.Controllers
{
    public class SubscriptionController : Controller
    {
        private DBGateway entities = new DBGateway();

        public ActionResult Subscribe()
        {
            var userId = int.Parse(Session["UserId"].ToString());

            var model = new SubscriptionModel();
            model.NewSubscriptions = entities.GetAvailableSubscriptions(userId);
            model.ExistingSubscriptions = entities.GetUserSubscriptions(userId);

            return View(model);
        }
        
        [HttpPost]
        public ActionResult Subscribe(SubscriptionModel model)
        {
            var userId = int.Parse(Session["UserId"].ToString());

            if (ModelState.IsValid)
            {
                var subscriptions = new List<UserSubscription>();
                foreach (var newsubscription in model.NewSubscriptions)
                {
                    if (newsubscription.IsSelected)
                    {
                        var userSubscription = new UserSubscription
                        {
                            Currency = newsubscription.Currency,
                            Duration = newsubscription.Duration,
                            FullName = newsubscription.FullName,
                            ImageUrl = newsubscription.ImageUrl,
                            MaxSubscriptions = newsubscription.MaxSubscriptions,
                            Price = newsubscription.Price,
                            SubscriptionId = newsubscription.SubscriptionId,
                            StartDate = DateTime.Today,
                            EndDate = DateTime.Today.AddDays(newsubscription.Duration)
                        };

                        subscriptions.Add(userSubscription);
                    }
                }

                if (subscriptions.Count > 0)
                {
                    entities.SaveNewSubscriptions(subscriptions, userId);
                }

                if (model.ExistingSubscriptions != null)
                {
                    var existingSubscriptions = new List<UserSubscription>();
                    foreach (var existingSubscription in model.ExistingSubscriptions)
                    {
                        if (existingSubscription.IsSelected)
                        {
                            var subscription = new UserSubscription
                            {
                                Currency = existingSubscription.Currency,
                                Duration = existingSubscription.Duration,
                                FullName = existingSubscription.FullName,
                                ImageUrl = existingSubscription.ImageUrl,
                                MaxSubscriptions = existingSubscription.MaxSubscriptions,
                                Price = existingSubscription.Price,
                                SubscriptionsCount = existingSubscription.SubscriptionId,
                                StartDate = existingSubscription.StartDate,
                                EndDate = existingSubscription.EndDate.AddDays(existingSubscription.Duration)
                            };

                            existingSubscriptions.Add(subscription);
                        }
                    }

                    if (existingSubscriptions.Count > 0)
                    {
                        entities.SaveExistingSubscriptions(existingSubscriptions, userId);
                    }
                }

                return RedirectToAction("Confirm");
            }

            return View(model);
        }

        public ActionResult Confirm()
        {
            return View();
        }
    }
}