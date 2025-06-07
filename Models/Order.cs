using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeaShop.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        public List<OrderItem> Items { get; set; }

        public string Status { get; set; } = "Pending";
    }
} 