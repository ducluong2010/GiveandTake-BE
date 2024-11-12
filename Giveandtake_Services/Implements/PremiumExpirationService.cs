using Giveandtake_Business;
using GiveandTake_Repo.Models;
using GiveandTake_Repo.Repository.Implements;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Giveandtake_Services.Implements
{
    public class PremiumExpirationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PremiumExpirationService> _logger;
        private const int CHECK_INTERVAL_HOURS = 12;

        public PremiumExpirationService(
            IServiceProvider serviceProvider,
            ILogger<PremiumExpirationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Premium Expiration Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    try
                    {
                        var accountBusiness = scope.ServiceProvider.GetRequiredService<AccountBusiness>();
                        await accountBusiness.CheckAndUpdateExpiredPremium();

                        _logger.LogInformation("Successfully checked and updated expired premium accounts.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error in PremiumExpirationService: {ex.Message}");
                    }
                }

                _logger.LogInformation($"Premium check complete. Next check in {CHECK_INTERVAL_HOURS} hours.");
                await Task.Delay(TimeSpan.FromHours(CHECK_INTERVAL_HOURS), stoppingToken);
            }
        }
    }
}
