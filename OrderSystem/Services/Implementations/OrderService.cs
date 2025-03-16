using OrderSystem.Data.Interfaces;
using OrderSystem.Models;
using OrderSystem.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public void CreateOrder(Order order)
        {
            if (string.IsNullOrWhiteSpace(order.ProductName))
            {
                Console.WriteLine("❌ Error: Product name is required.");
                return;
            }

            if (order.Amount <= 0)
            {
                Console.WriteLine("❌ Error: Amount must be greater than 0.");
                return;
            }

            if (string.IsNullOrWhiteSpace(order.DeliveryAddress))
            {
                Console.WriteLine("❌ Error: Delivery address is required.");
                return;
            }

            if (!Enum.IsDefined(typeof(PaymentMethod), order.Payment))
            {
                Console.WriteLine("❌ Error: Invalid payment method.");
                return;
            }

            _orderRepository.AddOrder(order);
            Console.WriteLine("✔ Order successfully created!");
        }

        public void SendToWarehouse(int orderId)
        {
            var order = _orderRepository.GetOrderById(orderId);
            if (order == null)
            {
                Console.WriteLine("❌ Error: Order not found.");
                return;
            }

            if (order.Amount >= 2500 && order.Payment == PaymentMethod.CashOnDelivery)
            {
                order.Status = OrderStatus.ReturnedToCustomer;
                Console.WriteLine("⚠ Order returned to customer due to payment policy.");
            }
            else
            {
                order.Status = OrderStatus.InWarehouse;
                Console.WriteLine("✔ Order moved to warehouse.");
            }

            _orderRepository.UpdateOrder(order);
        }

        public void SendToShipping(int orderId)
        {
            var order = _orderRepository.GetOrderById(orderId);
            if (order == null)
            {
                Console.WriteLine("❌ Error: Order not found.");
                return;
            }

            if (string.IsNullOrWhiteSpace(order.DeliveryAddress))
            {
                order.Status = OrderStatus.Error;
                Console.WriteLine("❌ Error: Missing delivery address.");
                _orderRepository.UpdateOrder(order);
                return;
            }

            order.Status = OrderStatus.InShipping;
            Console.WriteLine("📦 Order is being shipped...");

            Thread.Sleep(5000); //shipping

            order.Status = OrderStatus.Closed;
            Console.WriteLine("✔ Order shipped successfully!");

            _orderRepository.UpdateOrder(order);
        }

        public void ViewOrders()
        {
            var orders = _orderRepository.GetAllOrders();
            foreach (var order in orders)
            {
                Console.WriteLine($"📦 {order.Id}: {order.ProductName} - {order.Status}");
            }
        }


    }
}
