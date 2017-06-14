using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnlabMvc.Models.Order
{
    public static class OrderStatusCodes
    {
        //TODO: Add as we figure out what they want/need
        public const string Created = "Created";
        public const string Confirmed = "Confirmed";
        public const string Received = "Received";

        public const string Complete = "Complete";
    }
}
