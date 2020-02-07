using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Consumer.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ILogger<DbInitializer> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DbInitializer(ILogger<DbInitializer> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Initialize()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

                try
                {
                    if (context.Database.GetPendingMigrations().Count() > 0)
                    {
                        context.Database.Migrate();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error on db init", ex);
                    return;
                }
            }

        }
    }
}
