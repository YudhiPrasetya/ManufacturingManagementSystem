using Microsoft.AspNetCore.Mvc;
using Production.Api.Contracts.ProductionOrders;
using Production.Application.UseCases.ProductionOrders.CreateProductionOrder;
using Production.Application.UseCases.ProductionOrders.GetProductionOrder;
using Production.Application.UseCases.ProductionOrders.GetProductionOrders;
using Production.Application.UseCases.ProductionOrders.ReportProduction;

namespace Production.Api.Controllers
{
    [Route("api/production/orders")]
    [ApiController]
    public class ProductionOrderController : ControllerBase
    {
        private readonly CreateProductionOrderHandler _handler;
        private readonly GetProductionOrderHandler _getHandler;
        private readonly ReportProductionHandler _reportHandler;
        private readonly GetProductionOrdersHandler _listHandler;

        public ProductionOrderController(
            CreateProductionOrderHandler handler,
            GetProductionOrderHandler getHandler,
            ReportProductionHandler reportHandler,
            GetProductionOrdersHandler listHandler)
        {
            _handler = handler;
            _getHandler = getHandler;
            _reportHandler = reportHandler;
            _listHandler = listHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductionOrderRequest request, CancellationToken ct)
        {
            try
            {
                var command = new CreateProductionOrderCommand(request.OrderNumber, request.ProductCode, request.ProductName, request.PlannedQuantity);
                var id = await _handler.Handle(command, ct);

                return CreatedAtAction(nameof(GetByOrderNumber),
                    new { orderNumber = request.OrderNumber },
                    new
                    {
                        success = true,
                        message = "Production order created successfully.",
                        data = new { id }
                    });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("{orderNumber}")]
        public async Task<IActionResult> GetByOrderNumber(string orderNumber, CancellationToken ct)
        {
            var query = new GetProductionOrderQuery(orderNumber);

            var productionOrder = await _getHandler.Handle(query, ct);
            if (productionOrder is null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"Production order '{orderNumber}' not found."
                });
            }

            return Ok(new
            {
                success = true,
                data = new
                {
                    productionOrder.Id,
                    productionOrder.OrderNumber,
                    productionOrder.ProductCode,
                    productionOrder.ProductName,
                    productionOrder.PlannedQuantity,
                    productionOrder.CompletedQuantity,
                    productionOrder.CreatedAt,
                    productionOrder.Status,
                }
            });
        }

        [HttpPost("{orderNumber}/report")]
        public async Task<IActionResult> ReportProduction(
            string orderNumber,
            [FromBody] ReportProductionRequest request,
            CancellationToken ct)
        {
            try
            {
                var command = new ReportProductionCommand(
                    orderNumber, request.Quantity
                );

                await _reportHandler.Handle(command, ct);

                return Ok(new
                {
                    success = true,
                    message = "Production reported successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            CancellationToken ct = default)
        {
            try
            {
                var query = new GetProductionOrdersQuery(
                    page, pageSize, status
                );

                var (items, totalCount) = await _listHandler.Handle(query, ct);

                var totalPages = (int)Math.Ceiling(
                    (double)totalCount / pageSize
                );

                var response = new PagedResponse<ProductionOrderListResponse>(
                    items.Select(x => new ProductionOrderListResponse(
                        x.Id, x.OrderNumber,
                        x.ProductCode, x.ProductName,
                        x.PlannedQuantity, x.CompletedQuantity,
                        x.CreatedAt, x.Status
                    )).ToList(),
                    page,
                    pageSize,
                    totalCount,
                    totalPages,
                    page > 1,
                    page < totalPages
                );

                return Ok(new
                {
                    success = true,
                    data = response
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
