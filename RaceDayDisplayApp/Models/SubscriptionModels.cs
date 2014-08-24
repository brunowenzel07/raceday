using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RaceDayDisplayApp.Models
{
    public class SubscriptionData
    {
        public int SubscriptionId { get; set; }

        public string FullName { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public string Currency { get; set; }

        public int Duration { get; set; }

        public int MaxSubscriptions { get; set; }

        public bool IsSelected { get; set; }

        public int SubscriptionsCount { get; set; }

    }

    public class UserSubscription : SubscriptionData
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

    public class SubscriptionModel
    {
        public List<SubscriptionData> NewSubscriptions { get; set; }

        public List<UserSubscription> ExistingSubscriptions { get; set; }
    }
}