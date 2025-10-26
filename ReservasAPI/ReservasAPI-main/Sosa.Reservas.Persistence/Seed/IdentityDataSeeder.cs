﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sosa.Reservas.Persistence.Seed
{
    public class IdentityDataSeeder
    {
        public static async Task SeedRolesAsync(IHost app)
        {
            // 'scope' para obtener los servicios necesarios
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

                // Creacion Roles
                if (!await roleManager.RoleExistsAsync("Cliente"))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>("Cliente"));
                }

                if (!await roleManager.RoleExistsAsync("Administrador"))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>("Administrador"));
                }
            }
        }
    }
}
