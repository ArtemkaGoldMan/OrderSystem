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
        void CreateOrder(string productName, decimal amount, CustomerType customer, string deliveryAddress, PaymentMethod payment);
        void SendToWarehouse(int orderId);
        void SendToShipping(int orderId);
        void ViewOrders();
        void CancelOrder(int orderId);
    }
}
