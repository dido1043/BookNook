using System;
using System.Collections.Generic;
using System.Text;

namespace BookNook.Data.Models
{
    public enum OrderStatus
    {
        New = 1,
        Confirmed = 2,
        Fulfilled = 3,
        Rejected = 4
    }

    public enum DeliveryMethod
    {
        InStore = 1,
        Delivery = 2
    }
}
