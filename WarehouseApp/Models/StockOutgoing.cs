using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseApp.Models
{
    public class StockOutgoing
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int QuantityBefore { get; set; }
        public int QuantityOut { get; set; }
        public string Reason { get; set; }
        public DateTime OutgoingDate { get; set; }
        public int InventoryId { get; set; }
    }
}
