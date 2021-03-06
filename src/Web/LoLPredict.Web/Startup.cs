﻿using System;
using LoLPredict.Database.Models;
using LoLPredict.Web.Domain;
using LoLPredict.Web.DAL;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML;

[assembly: WebJobsStartup(typeof(LoLPredict.Web.Startup))]
namespace LoLPredict.Web
{
    public class Startup : IWebJobsStartup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            services.Configure<Settings>(configuration);
            services.AddDbContext<GameContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Game")));

            services.AddTransient<IGameRepository, GameRepository>();
            services.AddTransient<IModelRepository, ModelRepository>();

            services.AddScoped<MLContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IWebJobsBuilder builder)
        {
            ConfigureServices(builder.Services);

            #if DEBUG
            NpmScriptRunner.CreateNodeServer("../../../ClientApp", "start", "5000");
            #endif
        }
    }
}
