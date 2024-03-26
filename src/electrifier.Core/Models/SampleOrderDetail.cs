namespace electrifier.Core.Models;

[Obsolete("This class is for demonstration purposes only.")]
public class SampleOrderDetail
{
    public long ProductID
    {
        get; set;
    }

    public string ProductName
    {
        get; set;
    }

    public int Quantity
    {
        get; set;
    }

    public double Discount
    {
        get; set;
    }

    public string QuantityPerUnit
    {
        get; set;
    }

    public double UnitPrice
    {
        get; set;
    }

    public string CategoryName
    {
        get; set;
    }

    public string CategoryDescription
    {
        get; set;
    }

    public double Total
    {
        get; set;
    }

    public string ShortDescription => $"Product ID: {ProductID} - {ProductName}";
}
