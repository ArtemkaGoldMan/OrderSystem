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
    }
}
