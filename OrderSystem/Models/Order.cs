using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)] 
        public string ProductName { get; set; }

        [Required]
        [Range(0, 999999.99)]
        public decimal Amount { get; set; }

        [Required]
        public CustomerType Customer { get; set; }

        [MaxLength(255)] 
        public string DeliveryAddress { get; set; }

        [Required]
        public PaymentMethod Payment { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.New; // Default status
    }
}
