using OrderSystem.Models;
using OrderSystem.Data.Interfaces;

namespace OrderSystem.Data.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DatabaseContext _context;

        public OrderRepository(DatabaseContext context)
        {
            _context = context;
        }

        public void AddOrder(Order order)
        {
            try
            {
                _context.Orders.Add(order);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error adding order: {ex.Message}");
            }
        }

        public Order GetOrderById(int id)
        {
            try
            {
                return _context.Orders.Find(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error fetching order by ID: {ex.Message}");
                return null;
            }
        }

        public List<Order> GetAllOrders()
        {
            try
            {
                return _context.Orders.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error fetching orders: {ex.Message}");
                return new List<Order>();
            }
        }

        public void UpdateOrder(Order order)
        {
            try
            {
                _context.Orders.Update(order);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error updating order: {ex.Message}");
            }
        }
    }
}
