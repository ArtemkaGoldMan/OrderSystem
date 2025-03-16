using OrderSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Services.Interfaces
{
    public interface IOrderService
    {
        void CreateOrder(Order order);
        void SendToWarehouse(int orderId);
        void SendToShipping(int orderId);
        void ViewOrders();
    }
}
