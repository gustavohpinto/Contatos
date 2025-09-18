using Contatos.Application.Services;
using Contatos.Domain.Abstractions;
using Contatos.Domain.Repositories;
using Contatos.Infrastructure.Data;
using Contatos.Infrastructure.Repositories;
using Contatos.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contatos.Infrastructure.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDBContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddSingleton<IClock, SystemClock>();
            services.AddScoped<IContactService, ContactService>();
            return services;
        }
    }
}
