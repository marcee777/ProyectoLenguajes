using Microsoft.EntityFrameworkCore;
using ProyectoLenguajes.Data;
using ProyectoLenguajes.Utilities;

namespace ProyectoLenguajes.Services
{
    public class OrderStatusUpdaterService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<OrderStatusUpdaterService> _logger;

        public OrderStatusUpdaterService(IServiceScopeFactory serviceScopeFactory, ILogger<OrderStatusUpdaterService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OrderStatusUpdaterService iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        // Solo traer órdenes activas: OnTime y OverTime
                        var orders = await dbContext.Orders
                            .Include(o => o.Status)
                            .Where(o => o.Status.Name == StaticValues.Status_OnTime ||
                                        o.Status.Name == StaticValues.Status_OverTime)
                            .ToListAsync();

                        foreach (var order in orders)
                        {
                            if (order.Status.Name == StaticValues.Status_OnTime &&
                                order.Status.TimeToNextStatus.HasValue)
                            {
                                var minutosPasados = (DateTime.Now - order.LastStatusChange).TotalMinutes;
                                if (minutosPasados >= order.Status.TimeToNextStatus.Value)
                                {
                                    var overTimeStatus = await dbContext.Status
                                        .FirstOrDefaultAsync(s => s.Name == StaticValues.Status_OverTime);

                                    if (overTimeStatus != null)
                                    {
                                        order.StatusId = overTimeStatus.Id;
                                        order.LastStatusChange = DateTime.Now;
                                        _logger.LogInformation($"Order {order.Id} changed from On Time to Over Time.");
                                    }
                                }
                            }
                            else if (order.Status.Name == StaticValues.Status_OverTime &&
                                     order.Status.TimeToNextStatus.HasValue)
                            {
                                var minutosPasados = (DateTime.Now - order.LastStatusChange).TotalMinutes;
                                if (minutosPasados >= order.Status.TimeToNextStatus.Value)
                                {
                                    var delayedStatus = await dbContext.Status
                                        .FirstOrDefaultAsync(s => s.Name == StaticValues.Status_Delayed);

                                    if (delayedStatus != null)
                                    {
                                        order.StatusId = delayedStatus.Id;
                                        order.LastStatusChange = DateTime.Now;
                                        _logger.LogInformation($"Order {order.Id} changed from Over Time to Delayed.");
                                    }
                                }
                            }
                        }

                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in OrderStatusUpdaterService");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
