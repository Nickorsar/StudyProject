using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace SealApp
{
    [XmlRoot("return")]
    public class Response
    {
        [XmlElement(ElementName = "storage")]
        public List<Storage> Storages { get; set; }
        public Response()
        {
            Storages = new List<Storage>();
        }
    }

    public class Stock
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public float Mass { get; set; }
        public void ComputeMass(float mass) { this.Mass = this.Quantity * mass; }
    }
    public class Storage
    {
        [XmlElement("storage_name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "product", IsNullable = false)]
        public List<Product> Products { get; set; }
        public Storage() { Products = new List<Product>(); }
    }
    public class Product
    {
        [XmlElement("product_name")]
        public string Name { get; set; }
        [XmlElement("count")]
        public int Quantity { get; set; }
        [XmlElement("m")]
        public float Mass { get; set; }
        [XmlElement("fragile")]
        public string IsFragile { get; set; }
        [XmlElement("date")]
        public string Date { get; set; }
        public string StorageName { get; set; }
    }
}
