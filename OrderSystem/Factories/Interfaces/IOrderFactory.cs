using OrderSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Factories.Interfaces
{
    public interface IOrderFactory
    {
        Order CreateOrder(string productName, decimal amount, CustomerType customer, string deliveryAddress, PaymentMethod payment);
    }
}
