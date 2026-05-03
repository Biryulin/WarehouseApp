using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseApp.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string Cell { get; set; }
        public string Inn { get; set; }
        public DateTime LastUpdated { get; set; }
        public int WhoAdded { get; set; }
        public DateTime? DateLeft { get; set; }
        public string WhoAddedName { get; set; } // для отображения
        public string CompanyName { get; set; }  // для отображения
    }
}
