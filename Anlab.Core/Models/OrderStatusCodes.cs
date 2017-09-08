namespace Anlab.Core.Models
{
    public static class OrderStatusCodes
    {
        //TODO: Add as we figure out what they want/need
        public const string Created         = "Created";          //Created by user, not sent and is still editable
        public const string Confirmed       = "Confirmed";        //Confirmed tests, sent to Anlab. Not editable by user. Editable by Anlab
        public const string Received        = "Received";         //Received by Anlab, imported into labworks, no longer editable by Anlab
        public const string Finalized       = "Finalized";        //Results and semi final prices uploaded from anlab
        public const string Accepted        = "Accepted";         //User/Client has accepted the final results and payment amount. No more changes will be done by anlab for tests/prices Ok to move money/pay by CC, etc.        
        public const string Complete        = "Complete";         //Money has moved
    }
}
