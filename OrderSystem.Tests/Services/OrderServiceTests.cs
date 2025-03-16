using System;
using System.Collections.Generic;
using Moq;
using OrderSystem.Data.Interfaces;
using OrderSystem.Factories.Interfaces;
using OrderSystem.Models;
using OrderSystem.Services.Implementations;
using OrderSystem.Services.Interfaces;
using Xunit;

namespace OrderSystem.Tests.Services
{
    public class OrderServiceTests : IDisposable
    {
        private readonly Mock<IOrderRepository> _mockRepo;
        private readonly Mock<IOrderFactory> _mockFactory;  
        private readonly IOrderService _orderService;

        public OrderServiceTests()
        {
            _mockRepo = new Mock<IOrderRepository>(MockBehavior.Strict);
            _mockFactory = new Mock<IOrderFactory>();
            _orderService = new OrderService(_mockRepo.Object, _mockFactory.Object); 
        }

        public void Dispose()
        {
            _mockRepo.Reset();
            _mockFactory.Reset();
        }

        [Fact]
        public void CreateOrder_Should_AddOrder_When_ValidData()
        {
            // Arrange
            var order = new Order
            {
                ProductName = "Laptop",
                Amount = 3000,
                Customer = CustomerType.Company,
                DeliveryAddress = "Tech Street 5",
                Payment = PaymentMethod.Card,
                Status = OrderStatus.New
            };

            _mockFactory.Setup(f => f.CreateOrder(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<CustomerType>(), It.IsAny<string>(), It.IsAny<PaymentMethod>()))
                        .Returns(order); 

            _mockRepo.Setup(r => r.AddOrder(It.IsAny<Order>()));

            // Act
            _orderService.CreateOrder(order.ProductName, order.Amount, order.Customer, order.DeliveryAddress, order.Payment);

            // Assert
            _mockRepo.Verify(r => r.AddOrder(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public void CreateOrder_Should_Not_AddOrder_When_InvalidData()
        {
            // Arrange
            var invalidOrder = new Order
            {
                ProductName = "", // ❌ Missing product name
                Amount = -500, // ❌ Invalid amount
                DeliveryAddress = null, // ❌ Missing address
                Payment = (PaymentMethod)99 // ❌ Invalid payment method
            };

            _mockFactory.Setup(f => f.CreateOrder(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<CustomerType>(), It.IsAny<string>(), It.IsAny<PaymentMethod>()))
                        .Returns(invalidOrder);

            _mockRepo.Setup(r => r.AddOrder(It.IsAny<Order>()));

            // Act
            _orderService.CreateOrder(invalidOrder.ProductName, invalidOrder.Amount, invalidOrder.Customer, invalidOrder.DeliveryAddress, invalidOrder.Payment);

            // Assert
            _mockRepo.Verify(r => r.AddOrder(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public void SendToWarehouse_Should_SetStatusToInWarehouse_When_Valid()
        {
            // Arrange
            var order = new Order { Id = 1, Amount = 2000, Payment = PaymentMethod.Card, Status = OrderStatus.New };
            _mockRepo.Setup(r => r.GetOrderById(order.Id)).Returns(order);
            _mockRepo.Setup(r => r.UpdateOrder(It.IsAny<Order>()));

            // Act
            _orderService.SendToWarehouse(order.Id);

            // Assert
            Assert.Equal(OrderStatus.InWarehouse, order.Status);
            _mockRepo.Verify(r => r.UpdateOrder(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public void SendToWarehouse_Should_ReturnOrder_When_CashOnDelivery_And_HighAmount()
        {
            // Arrange
            var expensiveOrder = new Order { Id = 2, Amount = 3000, Payment = PaymentMethod.CashOnDelivery, Status = OrderStatus.New };
            _mockRepo.Setup(r => r.GetOrderById(expensiveOrder.Id)).Returns(expensiveOrder);
            _mockRepo.Setup(r => r.UpdateOrder(It.IsAny<Order>()));

            // Act
            _orderService.SendToWarehouse(expensiveOrder.Id);

            // Assert
            Assert.Equal(OrderStatus.ReturnedToCustomer, expensiveOrder.Status);
            _mockRepo.Verify(r => r.UpdateOrder(It.IsAny<Order>()), Times.Once);
        }


        [Fact]
        public void SendToShipping_Should_SetStatusToClosed_After_5_Seconds() //does not work due to me
        {
            // Arrange
            var order = new Order { Id = 3, DeliveryAddress = "Shipping Address", Status = OrderStatus.InWarehouse };
            _mockRepo.Setup(r => r.GetOrderById(order.Id)).Returns(order);
            _mockRepo.Setup(r => r.UpdateOrder(It.IsAny<Order>())).Callback<Order>(o => order.Status = o.Status);

            // Act
            var startTime = DateTime.Now;
            _orderService.SendToShipping(order.Id);
            var elapsedTime = DateTime.Now - startTime;

            // Assert
            _mockRepo.Verify(r => r.UpdateOrder(It.Is<Order>(o => o.Status == OrderStatus.InShipping)), Times.Once);
            _mockRepo.Verify(r => r.UpdateOrder(It.Is<Order>(o => o.Status == OrderStatus.Closed)), Times.Once);

            Assert.Equal(OrderStatus.Closed, order.Status);
            Assert.True(elapsedTime.TotalSeconds >= 5, "Shipping delay should be at least 5 seconds.");
        }


        [Fact]
        public void SendToShipping_Should_SetStatusToError_When_MissingAddress()
        {
            // Arrange
            var invalidOrder = new Order { Id = 4, DeliveryAddress = "", Status = OrderStatus.InWarehouse };
            _mockRepo.Setup(r => r.GetOrderById(invalidOrder.Id)).Returns(invalidOrder);
            _mockRepo.Setup(r => r.UpdateOrder(It.IsAny<Order>()));

            // Act
            _orderService.SendToShipping(invalidOrder.Id);

            // Assert
            Assert.Equal(OrderStatus.Error, invalidOrder.Status);
            _mockRepo.Verify(r => r.UpdateOrder(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public void ViewOrders_Should_ListAllOrders()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { Id = 1, ProductName = "Phone", Status = OrderStatus.New },
                new Order { Id = 2, ProductName = "Tablet", Status = OrderStatus.InWarehouse }
            };

            _mockRepo.Setup(r => r.GetAllOrders()).Returns(orders);

            // Act
            _orderService.ViewOrders();

            // Assert
            _mockRepo.Verify(r => r.GetAllOrders(), Times.Once);
        }
    }
}
