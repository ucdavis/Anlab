using System;
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
        public Order()
        {
            MailMessages = new List<MailMessage>();
        }
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
        public string SavedTestDetails { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public string Status { get; set; }

        public string RequestNum { get; set; }

        public string ResultsFileIdentifier { get; set; }
        public Guid ShareIdentifier { get; set; }

        public ICollection<MailMessage> MailMessages { get; set; }

        public PaymentEvent ApprovedPayment { get; set; }
        
        public Sloth Sloth { get; set; }

        public bool Paid { get; set; } = false;

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

        public IList<TestItemModel> GetTestDetails()
        {
            return JsonConvert.DeserializeObject<IList<TestItemModel>>(SavedTestDetails);
        }

        public void SaveTestDetails(IList<TestItemModel> tests)
        {
            SavedTestDetails = JsonConvert.SerializeObject(tests);
        }

    }
}
