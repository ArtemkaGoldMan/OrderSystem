using OrderSystem.Factories.Interfaces;
using OrderSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Factories.Implementations
{
    public class OrderFactory : IOrderFactory
    {
        public Order CreateOrder(string productName, decimal amount, CustomerType customer, string deliveryAddress, PaymentMethod payment)
        {
            return new Order
            {
                ProductName = productName,
                Amount = amount,
                Customer = customer,
                DeliveryAddress = deliveryAddress,
                Payment = payment,
                Status = OrderStatus.New
            };
        }
    }
}
