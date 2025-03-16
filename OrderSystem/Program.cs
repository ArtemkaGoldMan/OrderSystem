using System;
using Microsoft.Extensions.DependencyInjection;
using OrderSystem.Data;
using OrderSystem.Data.Implementations;
using OrderSystem.Data.Interfaces;
using OrderSystem.Factories;
using OrderSystem.Factories.Implementations;
using OrderSystem.Factories.Interfaces;
using OrderSystem.Models;
using OrderSystem.Services.Implementations;
using OrderSystem.Services.Interfaces;

class Program
{
    private static IOrderService _orderService;

    static void Main()
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<DatabaseContext>()
            .AddScoped<IOrderRepository, OrderRepository>()
            .AddScoped<IOrderFactory, OrderFactory>()
            .AddScoped<IOrderService, OrderService>()
            .BuildServiceProvider();

        _orderService = serviceProvider.GetRequiredService<IOrderService>();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Order Management System");
            Console.WriteLine("1 Create Order");
            Console.WriteLine("2 Send Order to Warehouse");
            Console.WriteLine("3 Send Order to Shipping");
            Console.WriteLine("4 View Orders");
            Console.WriteLine("5 Exit");
            Console.Write("Choose an option: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    CreateOrder();
                    break;
                case "2":
                    SendToWarehouse();
                    break;
                case "3":
                    SendToShipping();
                    break;
                case "4":
                    _orderService.ViewOrders();
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("❌ Invalid choice, try again.");
                    break;
            }
        }
    }

    static void CreateOrder()
    {
        Console.Write("Enter product name: ");
        string productName = Console.ReadLine();

        Console.Write("Enter amount: ");
        decimal amount;
        while (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
        {
            Console.Write("❌ Invalid amount. Enter a positive number: ");
        }

        Console.Write("Enter customer type (1 - Individual, 2 - Company): ");
        CustomerType customerType;
        while (!Enum.TryParse(Console.ReadLine(), out customerType) || !Enum.IsDefined(typeof(CustomerType), customerType))
        {
            Console.Write("❌ Invalid choice. Enter 1 for Individual or 2 for Company: ");
        }

        Console.Write("Enter delivery address: ");
        string address = Console.ReadLine();

        Console.Write("Enter payment method (1 - Card, 2 - Cash on Delivery): ");
        PaymentMethod paymentMethod;
        while (!Enum.TryParse(Console.ReadLine(), out paymentMethod) || !Enum.IsDefined(typeof(PaymentMethod), paymentMethod))
        {
            Console.Write("❌ Invalid choice. Enter 1 for Card or 2 for Cash on Delivery: ");
        }

        _orderService.CreateOrder(productName, amount, customerType, address, paymentMethod);
        Console.WriteLine("✔ Order created successfully! Press any key to continue...");
        Console.ReadKey();
    }

    static void SendToWarehouse()
    {
        Console.Write("Enter order ID: ");
        int orderId;
        while (!int.TryParse(Console.ReadLine(), out orderId))
        {
            Console.Write("❌ Invalid order ID. Enter a valid number: ");
        }
        _orderService.SendToWarehouse(orderId);
        Console.WriteLine("✔ Process completed. Press any key to continue...");
        Console.ReadKey();
    }

    static void SendToShipping()
    {
        Console.Write("Enter order ID: ");
        int orderId;
        while (!int.TryParse(Console.ReadLine(), out orderId))
        {
            Console.Write("❌ Invalid order ID. Enter a valid number: ");
        }
        _orderService.SendToShipping(orderId);
        Console.WriteLine("✔ Process completed. Press any key to continue...");
        Console.ReadKey();
    }
}
