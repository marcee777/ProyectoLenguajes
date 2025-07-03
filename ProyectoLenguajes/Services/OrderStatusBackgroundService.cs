using ProyectoLenguajes.Data.Repository.Interfaces;
using ProyectoLenguajes.Utilities;

namespace ProyectoLenguajes.Services
{
    public class OrderStatusBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public OrderStatusBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var activeOrders = unitOfWork.Order
                        .GetAll(includeProperties: "Status")
                        .Where(o => o.Status.Name != StaticValues.Status_Canceled && o.Status.Name != StaticValues.Status_Delivered)
                        .ToList();

                    var configs = unitOfWork.StatusTimeConfig.GetAll().ToList();

                    foreach (var order in activeOrders)
                    {
                        var minutesElapsed = (DateTime.Now - order.CreatedAt).TotalMinutes;
                        var config = configs.FirstOrDefault(c => c.StatusName == order.Status.Name);

                        if (config != null && minutesElapsed >= config.MinutesToNextState)
                        {
                            var nextStatus = unitOfWork.Status.Get(s => s.Name == config.NextStatusName);
                            if (nextStatus != null)
                            {
                                order.StatusId = nextStatus.Id;
                            }
                        }
                    }

                    unitOfWork.Save();
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
