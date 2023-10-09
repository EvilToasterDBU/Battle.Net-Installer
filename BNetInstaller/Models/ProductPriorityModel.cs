namespace BNetInstaller.Models;

public class ProductPriorityModel : UidModel
{
    public PriorityModel Priority { get; set; } = new();
}