namespace AnlabMvc.Models.Order
{
    public static class OrderStatusCodes
    {
        //TODO: Add as we figure out what they want/need
        public const string Created         = "Created";          //Created by user, not sent and is still editable
        public const string Confirmed       = "Confirmed";        //Confirmed tests, sent to Anlab. Not editable by user. Editable by Anlab
        public const string Received        = "Received";         //Received by Anlab, imported into labworks, no longer editable by Anlab
        public const string AwaitingPayment = "Awaiting Payment"; //Tests completed by anlab, results uploaded and any adjustments to cost done.
        public const string Paid            = "Paid";             //Paided by creator. Either internal UC, CreditCard, or some external payment like External UC accout, check, etc.
        public const string Complete        = "Complete";         //Money has moved
    }
}
