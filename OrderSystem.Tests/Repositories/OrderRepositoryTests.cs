using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OrderSystem.Data;
using OrderSystem.Data.Implementations;
using OrderSystem.Data.Interfaces;
using OrderSystem.Models;
using Xunit;

namespace OrderSystem.Tests.Repositories
{
    public class OrderRepositoryTests : IDisposable
    {
        private readonly DatabaseContext _context;
        private readonly IOrderRepository _repository;

        public OrderRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DatabaseContext(options);
            _repository = new OrderRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public void AddOrder_ShouldAddOrderToDatabase()
        {
            // Arrange
            var order = new Order
            {
                ProductName = "Test Product",
                Amount = 1000,
                Customer = CustomerType.Individual,
                DeliveryAddress = "Test Address",
                Payment = PaymentMethod.Card,
                Status = OrderStatus.New
            };

            // Act
            _repository.AddOrder(order);
            var result = _repository.GetAllOrders();

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Product", result.First().ProductName);
        }

        [Fact]
        public void AddOrder_ShouldHandleNullOrder()
        {
            // Arrange
            Order order = null!;

            // Act
            _repository.AddOrder(order!);
            var result = _repository.GetAllOrders();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetOrderById_ShouldReturnCorrectOrder()
        {
            // Arrange
            var order = new Order
            {
                ProductName = "Laptop",
                Amount = 2500,
                Customer = CustomerType.Company,
                DeliveryAddress = "Business Street 1",
                Payment = PaymentMethod.CashOnDelivery,
                Status = OrderStatus.New
            };

            _repository.AddOrder(order);

            // Act
            var result = _repository.GetOrderById(order.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Laptop", result.ProductName);
        }

        [Fact]
        public void GetOrderById_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            // Act
            var result = _repository.GetOrderById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetAllOrders_ShouldReturnAllOrders()
        {
            // Arrange
            var order1 = new Order
            {
                ProductName = "Phone",
                Amount = 1500,
                Customer = CustomerType.Individual,
                DeliveryAddress = "Address 1",
                Payment = PaymentMethod.Card
            };
            var order2 = new Order
            {
                ProductName = "Tablet",
                Amount = 2000,
                Customer = CustomerType.Company,
                DeliveryAddress = "Address 2",
                Payment = PaymentMethod.CashOnDelivery
            };

            _repository.AddOrder(order1);
            _repository.AddOrder(order2);

            // Act
            var result = _repository.GetAllOrders();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetAllOrders_ShouldReturnEmptyList_WhenNoOrders()
        {
            // Act
            var result = _repository.GetAllOrders();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void UpdateOrder_ShouldModifyExistingOrder()
        {
            // Arrange
            var order = new Order
            {
                ProductName = "Monitor",
                Amount = 1200,
                Customer = CustomerType.Individual,
                DeliveryAddress = "Tech Street 5",
                Payment = PaymentMethod.Card,
                Status = OrderStatus.New
            };

            _repository.AddOrder(order);
            var existingOrder = _repository.GetOrderById(order.Id);

            // Act
            existingOrder.Status = OrderStatus.InWarehouse;
            _repository.UpdateOrder(existingOrder);

            var updatedOrder = _repository.GetOrderById(order.Id);

            // Assert
            Assert.NotNull(updatedOrder);
            Assert.Equal(OrderStatus.InWarehouse, updatedOrder.Status);
        }

        [Fact]
        public void UpdateOrder_ShouldHandleNullOrder()
        {
            // Arrange
            Order order = null!;

            // Act & Assert
            _repository.UpdateOrder(order); // Should not throw exception
        }

        [Fact]
        public void UpdateOrder_ShouldHandleNonExistentOrder()
        {
            // Arrange
            var order = new Order
            {
                Id = 999,
                ProductName = "Non-existent",
                Status = OrderStatus.New
            };

            // Act & Assert
            _repository.UpdateOrder(order); // Should not throw exception
        }

        [Fact]
        public void DeleteOrder_ShouldRemoveOrderFromDatabase()
        {
            // Arrange
            var order = new Order
            {
                ProductName = "Test Product",
                Amount = 1000,
                Customer = CustomerType.Individual,
                DeliveryAddress = "Test Address",
                Payment = PaymentMethod.Card,
                Status = OrderStatus.Closed
            };

            _repository.AddOrder(order);
            var initialCount = _repository.GetAllOrders().Count;

            // Act
            _repository.DeleteOrder(order);
            var finalCount = _repository.GetAllOrders().Count;

            // Assert
            Assert.Equal(1, initialCount);
            Assert.Equal(0, finalCount);
            Assert.Null(_repository.GetOrderById(order.Id));
        }

        [Fact]
        public void DeleteOrder_ShouldHandleNullOrder()
        {
            // Arrange
            Order order = null!;

            // Act & Assert
            _repository.DeleteOrder(order); // Should not throw exception
        }

        [Fact]
        public void DeleteOrder_ShouldHandleNonExistentOrder()
        {
            // Arrange
            var order = new Order
            {
                Id = 999,
                ProductName = "Non-existent",
                Status = OrderStatus.Closed
            };

            // Act & Assert
            _repository.DeleteOrder(order); // Should not throw exception
        }

        [Fact]
        public void DeleteOrder_ShouldNotAffectOtherOrders()
        {
            // Arrange
            var order1 = new Order
            {
                ProductName = "First Product",
                Amount = 1000,
                Customer = CustomerType.Individual,
                DeliveryAddress = "Address 1",
                Payment = PaymentMethod.Card,
                Status = OrderStatus.Closed
            };
            var order2 = new Order
            {
                ProductName = "Second Product",
                Amount = 2000,
                Customer = CustomerType.Company,
                DeliveryAddress = "Address 2",
                Payment = PaymentMethod.CashOnDelivery,
                Status = OrderStatus.Closed
            };

            _repository.AddOrder(order1);
            _repository.AddOrder(order2);

            // Act
            _repository.DeleteOrder(order1);
            var remainingOrders = _repository.GetAllOrders();

            // Assert
            Assert.Single(remainingOrders);
            Assert.Equal(order2.Id, remainingOrders.First().Id);
        }
    }
}
