using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Anlab.Core.Models;
using Newtonsoft.Json;
using Serilog;

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
        [StringLength(450)]
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

        [StringLength(512)]
        public string ClientName { get; set; }
        
        public string AdditionalEmails { get; set; }
        
        public string JsonDetails { get; set; }
        public string SavedTestDetails { get; set; }
        /// <summary>
        /// This a a copy of any SavedTestDetails that may have had prices set to zero because of discounted groups
        /// </summary>
        public string BackedupTestDetails { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        [StringLength(50)]
        public string Status { get; set; }
        
        [StringLength(50)]
        public string RequestNum { get; set; }

        public string ResultsFileIdentifier { get; set; }
        public Guid ShareIdentifier { get; set; }

        public ICollection<MailMessage> MailMessages { get; set; }

        public PaymentEvent ApprovedPayment { get; set; }
        [StringLength(20)] //It is 10 in sloth, but just in case...
        public string KfsTrackingNumber { get; set; }
        public Guid? SlothTransactionId { get; set; } 

        public bool Paid { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        [StringLength(50)]
        public string PaymentType { get; set; }

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
            try
            {
                return JsonConvert.DeserializeObject<IList<TestItemModel>>(SavedTestDetails);
            }
            catch (JsonSerializationException)
            {
                Log.Error("Get Test Details error");
                return new List<TestItemModel>();
            }
        }

        public void SaveTestDetails(IList<TestItemModel> tests)
        {
            SavedTestDetails = JsonConvert.SerializeObject(tests);
        }

        public IList<TestItemModel> GetBackedupTestDetails()
        {
            if (BackedupTestDetails == null)
            {
                return new List<TestItemModel>();
            }
            try
            {
                return JsonConvert.DeserializeObject<IList<TestItemModel>>(BackedupTestDetails);
            }
            catch (JsonSerializationException)
            {
                Log.Error("Get Test Details error");
                return new List<TestItemModel>();
            }
        }

        public void SaveBackedupTestDetails(IList<TestItemModel> tests)
        {
            if (tests == null || !tests.Any())
            {
                BackedupTestDetails = null;
            }
            else
            {
                BackedupTestDetails = JsonConvert.SerializeObject(tests);
            }
        }

    }
}
