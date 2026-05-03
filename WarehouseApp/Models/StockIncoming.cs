using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseApp.Models
{
    public class StockIncoming
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Cell { get; set; }
        public int Quantity { get; set; }
        public string Inn { get; set; }
        public DateTime ArrivalDate { get; set; }
        public int InventoryId { get; set; }
        public string CompanyName { get; set; } // для отображения
    }
}
