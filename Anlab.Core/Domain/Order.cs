﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Anlab.Core.Models;
using Newtonsoft.Json;

namespace Anlab.Core.Domain
{
    public class Order :IDatedEntity
    {

        public int Id { get; set; }
        
        [Required]
        public string CreatorId { get; set; }

        [ForeignKey("CreatorId")]
        public User Creator { get; set; }

        [StringLength(256)]
        [Required]
        public string Project { get; set; }

        [StringLength(16)]
        public string LabId { get; set; }

        [StringLength(16)]
        public string ClientId { get; set; }
        
        public string AdditionalEmails { get; set; }
        
        public string JsonDetails { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public string Status { get; set; }

        public OrderDetails GetOrderDetails()
        {
            try
            {
                return JsonConvert.DeserializeObject<OrderDetails>(JsonDetails);
            }
            catch (JsonSerializationException)
            {
                return new OrderDetails();
            }
        }

        public void SaveDetails(OrderDetails details)
        {
            JsonDetails = JsonConvert.SerializeObject(details);
        }

    }
}
