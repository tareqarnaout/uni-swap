using System.ComponentModel.DataAnnotations;

namespace uni_swap.Models
{
    public class Item
    {
        public int ItemID { get; set; } 

        [Required(ErrorMessage = "Item name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be positive")]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public string DocumentUrl { get; set; } = string.Empty;

        public int SellerID { get; set; }
    }
}
