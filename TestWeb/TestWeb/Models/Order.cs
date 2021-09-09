using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWeb.Models
{
    public class ResponseOrder
    {
        public string ApiVersion;
        public Order[] Result;
    }
    public class DeliveryAddress
    {
       public string ZipCode;
       public string AddressLine1;
       public string AddressLine2;
       public string Description;
       public string City;
       public string Country;
       public string FullName;
       public string Phone;
    }

    public class Shipping
    {
      public  int Id;
      public  string Title;
      public  string Phone;
      public  string Email;
      public  string Type;
    }
    public class Order
    {
        public int Id;
        public int PhotolabId;
        public string CustomId;
        public string SourceOrderId;
        public string ManagerId;
        public string AssignedTold;
        public string Title;
        public string TrackingUrl;
        public string TrackingNumber;
        public string Status;
        public string RenderStatus;
        public string PaymentStatus;
        public DeliveryAddress deliveryAddress;
        public Shipping shipping;
        public int CommentsCount;
        public string DownloadLink;
        public string PreviewImageUrl;
        public float Price;
        public float DiscountPrice;
        public float DeliveryPrice;
        public float TotalPrice;
        public float PaidPrice;
        public int UserId;
        public string UserCompanyAccountId;
        public int DiscountId;
        public string DiscountTitle;
        public string DateCreated;
        public string DateModified;
        public string DatePaid;
        public string LastDownloadedPaymentDocument;
        public string PaymentSystemUniqueId;
        public string GoogleClientId;
        public string ContractorOrders;

    }
}
