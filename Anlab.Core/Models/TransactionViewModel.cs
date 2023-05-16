using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Anlab.Jobs.MoneyMovement
{
    public class TransactionViewModel
    {
        public TransactionViewModel()
        {
            TransactionDate = DateTime.UtcNow; //DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            Transfers = new List<TransferViewModel>(2);
        }
        public string MerchantTrackingNumber { get; set; }

        public string MerchantTrackingUrl { get; set; }

        public DateTime TransactionDate { get; set; }

        public string Source { get; set; } = "ANLAB Internal Recharge";
        public string SourceType { get; set; } = "Recharge";

        public string Description { get; set; } //If it isn't set, Sloth with use one of the transfer descriptions...

        public IList<MetadataEntry> Metadata { get; set; } = new List<MetadataEntry>();

        public void AddMetadata(string name, string value)
        {
            Metadata.Add(new MetadataEntry { Name = name, Value = value });
        }

        public class MetadataEntry
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public IList<TransferViewModel> Transfers { get; set; }
    }

    public class TransferViewModel
    {
        public Decimal Amount { get; set; }
        public string Chart { get; set; }
        public string Account { get; set; }
        public string SubAccount { get; set; }
        public string ObjectCode { get; set; }
        //Max 40 characters
        public string Description { get; set; }

        public string FinancialSegmentString { get; set; }

        public string Direction { get; set; }// Debit or Credit Code associated with the transaction. = ['Credit', 'Debit'],

        public class Directions
        {
            public const string Debit = "Debit";
            public const string Credit = "Credit";
        }
    }
}
