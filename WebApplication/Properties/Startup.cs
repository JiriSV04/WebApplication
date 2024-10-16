﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Metoda ConfigureServices slouží k registraci služeb do kontejneru závislostí
        public void ConfigureServices(IServiceCollection services)
        {
            // Přidání podpory pro MVC
            services.AddControllersWithViews();
        }

        // Metoda Configure definuje způsob, jakým bude aplikace reagovat na HTTP požadavky
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // V režimu vývoje zobrazuj detailní vývojářské stránky s chybami
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // V produkčním prostředí použij stránku s chybami
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Použití HTTPS pro přesměrování
            app.UseHttpsRedirection();

            // Nastavení pro načítání statických souborů (např. obrázky, CSS, JS)
            app.UseStaticFiles();

            // Nastavení směrování (routing)
            app.UseRouting();

            // Nastavení pro použití autorizace
            app.UseAuthorization();

            // Definice endpointů
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


        }
    }
}
