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

                    var config = unitOfWork.StatusTimeConfig.Get(c => true); // Obtiene el único registro
                    if (config != null)
                    {
                        var sequence = new List<string>
                        {
                            StaticValues.Status_OnTime,
                            StaticValues.Status_OverTime,
                            StaticValues.Status_Delayed
                        };

                        var minutesPerChange = config.MinutesPerStatusChange;

                        foreach (var order in activeOrders)
                        {
                            var currentIndex = sequence.IndexOf(order.Status.Name);

                            if (currentIndex >= 0 && currentIndex < sequence.Count - 1)
                            {
                                var minutesElapsed = (DateTime.Now - order.CreatedAt).TotalMinutes;

                                if (minutesElapsed >= (currentIndex + 1) * minutesPerChange)
                                {
                                    var nextStatusName = sequence[currentIndex + 1];
                                    var nextStatus = unitOfWork.Status.Get(s => s.Name == nextStatusName);

                                    if (nextStatus != null)
                                    {
                                        order.StatusId = nextStatus.Id;
                                    }
                                }
                            }
                        }

                        unitOfWork.Save();
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
