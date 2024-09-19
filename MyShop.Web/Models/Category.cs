﻿namespace MyShop.Web.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedTime { get; set; } = DateTime.Now;
}
