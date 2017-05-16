using System;
using System.Collections;
using System.Collections.Generic;
using Anlab.Core.Domain;
using AnlabMvc.Controllers;

namespace AnlabMvc.Models.Order
{
    public class OrderReviewModel
    {
        public Anlab.Core.Domain.Order Order { get; set; }
        public OrderDetails OrderDetails { get; set; }
    }


}
