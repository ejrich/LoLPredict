using System;
using LoLPredict.Database.Models;
using LoLPredict.GamePipeline;
using LoLPredict.ModelPipeline;
using LoLPredict.ModelPipeline.ChampionModel;
using LoLPredict.ModelPipeline.Domain;
using LoLPredict.PatchPipeline;
using LoLPredict.Pipelines.DAL;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML;
using RiotApi;

[assembly: WebJobsStartup(typeof(LoLPredict.Pipelines.Startup))]
namespace LoLPredict.Pipelines
{
    public class Startup : IWebJobsStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<GameContext>(options =>
                options.UseSqlServer(GetEnvironmentVariable("CONNECTION_STRING")));

            services.AddTransient<IModelStorageService, ModelStorageService>();
            services.AddTransient<IModelCreationPipeline, ModelCreationPipeline>();
            services.AddTransient<IModelFactory, ChampionModelPipeline>();
            services.AddTransient<IGameRepository, GameRepository>();
            services.AddTransient<IPatchPipeline, PatchPipeline.PatchPipeline>();
            services.AddTransient<IGameRepository, GameRepository>();
            services.AddTransient<IGamePipeline, GamePipeline.GamePipeline>();
            services.AddTransient<IRestClientFactory, RestClientFactory>();
            services.AddTransient<IMatchGenerator, MatchGenerator>();
            services.AddTransient<IRiotModelMapper, RiotModelMapper>();

            services.AddScoped<MLContext>();
        }

        public void Configure(IWebJobsBuilder builder)
        {
            ConfigureServices(builder.Services);
        }

        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
