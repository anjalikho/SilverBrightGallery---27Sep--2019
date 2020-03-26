using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SilverBrightGallery.Controllers;

namespace SilverBrightGallery.Models
{
    public class RegisterUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class Product
    {
        public int ID { get; set; }
        public string ManufacturingDate { get; set; }
        public float Price { get; set; }
        public float Discount { get; set; }
        public string  Photo { get; set; }
        public string Categorey { get; set; }
        public string ProductDescription { get; set; }
        public string ProductName { get; set; }
    }

    public class AddToCart
    {
        public int UserId { get; set; }
        public int Product_Id { get; set; }
        public string Photo { get; set; }
        public string ProductName { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
    }

    public class ViewCart
    {
        public int Id { get; set; }
        public int Product_Id { get; set; }
        public string Photo { get; set; }
        public string ProductName { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public float Total { get; set; }
    }

    public class CartViewModel
    {
        public List<ViewCart> ListofCartItems { get; set; }
        public int CountCartItems { get; set; }
        public float TotalCost { get; set; }

    }

    public class CheckoutViewModel
    {
        public string UserName { get; set; }
        public List<ViewCart> ListofCartItems { get; set; }
        public float TotalCost { get; set; }
        public int CountCartIems { get; set; }
        public int Id { get; set; }
    }
}