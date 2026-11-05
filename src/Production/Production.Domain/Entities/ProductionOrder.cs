namespace Production.Domain.Entities;

public class ProductionOrder
{
    public Guid Id { get; private set; }
    public string OrderNumber { get; private set; } = string.Empty;
    public string ProductCode { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public int PlannedQuantity { get; private set; }
    public int CompletedQuantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string Status { get; private set; } = "Planned";

    private ProductionOrder()
    {

    }

    public ProductionOrder(
        string orderNumber, string productCode,
        string productName, int plannedQuantity
        )
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new ArgumentException("Order number is required.");

        if (string.IsNullOrWhiteSpace(productCode))
            throw new ArgumentException("Product code is required.");

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name is required.");

        if (plannedQuantity <= 0)
            throw new ArgumentException("Planned quantity must be greater than 0 (zero).");

        Id = Guid.CreateVersion7();
        OrderNumber = orderNumber;
        ProductCode = productCode;
        ProductName = productName;
        PlannedQuantity = plannedQuantity;
        CompletedQuantity = 0;
        CreatedAt = DateTime.UtcNow;
        Status = "Planned";
    }

    public void ReportProduction(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        if (CompletedQuantity + quantity > PlannedQuantity)
            throw new InvalidOperationException(
                "Completed quantity cannot exceed planned quantity.");

        CompletedQuantity += quantity;

        Status = CompletedQuantity == PlannedQuantity
            ? "Completed"
            : "InProgress";
    }

}
