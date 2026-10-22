using System;
using System.Collections.Generic;

namespace Pract7.Model;

public partial class Furniture
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Material { get; set; } = null!;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public bool IsAvailable { get; set; }
}
