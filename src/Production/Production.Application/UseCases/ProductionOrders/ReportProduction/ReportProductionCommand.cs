namespace Production.Application.UseCases.ProductionOrders.ReportProduction;

public record ReportProductionCommand(string OrderNumber, int Quantity);
