using System;
using System.Collections;
using System.Collections.Generic;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using AnlabMvc.Controllers;

namespace AnlabMvc.Models.Order
{
    public class OrderReviewModel
    {
        public OrderReviewModel()
        {
            History = new List<History>();
        }
        public Anlab.Core.Domain.Order Order { get; set; }
        public OrderDetails OrderDetails { get; set; }
        public bool HideLabDetails { get; set; } = true;

        public IList<History> History { get; set; }

        public string? LabworksFinished { get; set; } //Has the initials of the person who finihed this in Labworks. May be null or empty if not finished.
        public bool WasFinalEmailSent { get; set; } = false; //Was the final email sent to the client?
        public bool WasFinalEmailSkipped { get; set; } = false; //Was the final email skipped?
    }


}
