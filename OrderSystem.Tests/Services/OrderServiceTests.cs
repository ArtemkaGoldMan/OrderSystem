﻿using System;
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

        [Theory]
        [InlineData("", 100, "Address", PaymentMethod.Card)]
        [InlineData("Product", -1, "Address", PaymentMethod.Card)]
        [InlineData("Product", 100, "", PaymentMethod.Card)]
        [InlineData("Product", 100, "Address", (PaymentMethod)99)]
        public void CreateOrder_Should_Not_AddOrder_When_InvalidData(string productName, decimal amount, string address, PaymentMethod payment)
        {
            // Arrange
            _mockRepo.Setup(r => r.AddOrder(It.IsAny<Order>()));

            // Act
            _orderService.CreateOrder(productName, amount, CustomerType.Individual, address, payment);

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
        public void SendToWarehouse_Should_NotModify_When_OrderNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetOrderById(It.IsAny<int>())).Returns((Order)null!);

            // Act
            _orderService.SendToWarehouse(999);

            // Assert
            _mockRepo.Verify(r => r.UpdateOrder(It.IsAny<Order>()), Times.Never);
        }

        [Theory]
        [InlineData(OrderStatus.Closed)]
        [InlineData(OrderStatus.Cancelled)]
        public void SendToWarehouse_Should_NotModify_When_OrderClosedOrCancelled(OrderStatus status)
        {
            // Arrange
            var order = new Order { Id = 1, Status = status };
            _mockRepo.Setup(r => r.GetOrderById(order.Id)).Returns(order);
            _mockRepo.Setup(r => r.UpdateOrder(It.IsAny<Order>()));

            // Act
            _orderService.SendToWarehouse(order.Id);

            // Assert
            Assert.Equal(status, order.Status);
            _mockRepo.Verify(r => r.UpdateOrder(It.IsAny<Order>()), Times.Never);
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

        [Theory]
        [InlineData(OrderStatus.Closed)]
        [InlineData(OrderStatus.Cancelled)]
        public void SendToShipping_Should_NotModify_When_OrderClosedOrCancelled(OrderStatus status)
        {
            // Arrange
            var order = new Order { Id = 1, Status = status, DeliveryAddress = "Address" };
            _mockRepo.Setup(r => r.GetOrderById(order.Id)).Returns(order);
            _mockRepo.Setup(r => r.UpdateOrder(It.IsAny<Order>()));

            // Act
            _orderService.SendToShipping(order.Id);

            // Assert
            Assert.Equal(status, order.Status);
            _mockRepo.Verify(r => r.UpdateOrder(It.IsAny<Order>()), Times.Never);
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

        [Fact]
        public void ViewOrders_Should_HandleEmptyList()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetAllOrders()).Returns(new List<Order>());

            // Act
            _orderService.ViewOrders();

            // Assert
            _mockRepo.Verify(r => r.GetAllOrders(), Times.Once);
        }

        [Fact]
        public void CancelOrder_Should_NotModify_When_OrderNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetOrderById(It.IsAny<int>())).Returns((Order)null!);

            // Act
            _orderService.CancelOrder(999);

            // Assert
            _mockRepo.Verify(r => r.UpdateOrder(It.IsAny<Order>()), Times.Never);
        }

        [Theory]
        [InlineData(OrderStatus.Closed)]
        [InlineData(OrderStatus.Cancelled)]
        [InlineData(OrderStatus.InShipping)]
        public void CancelOrder_Should_NotModify_When_OrderInInvalidState(OrderStatus status)
        {
            // Arrange
            var order = new Order { Id = 1, Status = status };
            _mockRepo.Setup(r => r.GetOrderById(order.Id)).Returns(order);
            _mockRepo.Setup(r => r.UpdateOrder(It.IsAny<Order>()));

            // Act
            _orderService.CancelOrder(order.Id);

            // Assert
            Assert.Equal(status, order.Status);
            _mockRepo.Verify(r => r.UpdateOrder(It.IsAny<Order>()), Times.Never);
        }

        [Theory]
        [InlineData(OrderStatus.New)]
        [InlineData(OrderStatus.InWarehouse)]
        [InlineData(OrderStatus.Error)]
        [InlineData(OrderStatus.ReturnedToCustomer)]
        public void CancelOrder_Should_SetStatusToCancelled_When_ValidState(OrderStatus initialStatus)
        {
            // Arrange
            var order = new Order { Id = 1, Status = initialStatus };
            _mockRepo.Setup(r => r.GetOrderById(order.Id)).Returns(order);
            _mockRepo.Setup(r => r.UpdateOrder(It.IsAny<Order>()));

            // Act
            _orderService.CancelOrder(order.Id);

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order.Status);
            _mockRepo.Verify(r => r.UpdateOrder(It.IsAny<Order>()), Times.Once);
        }

        [Theory]
        [InlineData(OrderStatus.Closed)]
        [InlineData(OrderStatus.Cancelled)]
        public void DeleteOrder_Should_DeleteOrder_When_ValidState(OrderStatus status)
        {
            // Arrange
            var order = new Order { Id = 1, Status = status };
            _mockRepo.Setup(r => r.GetOrderById(order.Id)).Returns(order);
            _mockRepo.Setup(r => r.DeleteOrder(It.IsAny<Order>()));

            // Act
            _orderService.DeleteOrder(order.Id);

            // Assert
            _mockRepo.Verify(r => r.DeleteOrder(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public void DeleteOrder_Should_NotDelete_When_OrderNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetOrderById(It.IsAny<int>())).Returns((Order)null!);

            // Act
            _orderService.DeleteOrder(999);

            // Assert
            _mockRepo.Verify(r => r.DeleteOrder(It.IsAny<Order>()), Times.Never);
        }

        [Theory]
        [InlineData(OrderStatus.New)]
        [InlineData(OrderStatus.InWarehouse)]
        [InlineData(OrderStatus.Error)]
        [InlineData(OrderStatus.ReturnedToCustomer)]
        [InlineData(OrderStatus.InShipping)]
        public void DeleteOrder_Should_NotDelete_When_OrderInInvalidState(OrderStatus status)
        {
            // Arrange
            var order = new Order { Id = 1, Status = status };
            _mockRepo.Setup(r => r.GetOrderById(order.Id)).Returns(order);
            _mockRepo.Setup(r => r.DeleteOrder(It.IsAny<Order>()));

            // Act
            _orderService.DeleteOrder(order.Id);

            // Assert
            _mockRepo.Verify(r => r.DeleteOrder(It.IsAny<Order>()), Times.Never);
        }
    }
}
