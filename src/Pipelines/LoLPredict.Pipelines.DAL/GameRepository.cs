using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoLPredict.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace LoLPredict.Pipelines.DAL
{
    public interface IGameRepository
    {
        Task<Patch> LoadLivePatch();
        Task InsertPatch(string number, bool live);
        Task UpdatePatch(Patch patch);
        Task<IEnumerable<Champion>> LoadChampions(string patch);
        Task InsertChampions(IEnumerable<Champion> champions);
        Task<bool> GameResultExists(long id);
        Task<IEnumerable<GameResult>> LoadGameResults(string patch);
        Task InsertGameResult(GameResult result);
        Task<Summoner> GetSummonerById(string id);
        Task InsertSummoner(Summoner summoner);
        Task InsertModel(Model model);
    }

    public class GameRepository : IGameRepository
    {
        private readonly GameContext _context;

        public GameRepository(GameContext context)
        {
            _context = context;
        }

        public async Task<Patch> LoadLivePatch()
        {
            var livePatch = await _context.Patches.SingleOrDefaultAsync(_ => _.Live);

            return livePatch;
        }

        public async Task InsertPatch(string number, bool live)
        {
            var patchComponents = number.Split('.')
                .Select(_ => Convert.ToInt32(_))
                .ToList();

            _context.Patches.Add(new Patch
            {
                Major = patchComponents[0],
                Minor = patchComponents[1],
                Version = patchComponents[2],
                Live = live
            });
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePatch(Patch patch)
        {
            _context.Patches.Update(patch);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Champion>> LoadChampions(string patch)
        {
            var champions = await _context.Champions.Where(_ => _.Patch == patch).ToListAsync();

            return champions;
        }

        public async Task InsertChampions(IEnumerable<Champion> champions)
        {
            await _context.AddRangeAsync(champions);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> GameResultExists(long id)
        {
            var gameResult = await _context.Results.FirstOrDefaultAsync(_ => _.Id == id);

            return gameResult != null;
        }

        public async Task<IEnumerable<GameResult>> LoadGameResults(string patch)
        {
            var gameResults = await _context.Results.Where(_ => _.Patch == patch).ToListAsync();

            return gameResults;
        }

        public async Task InsertGameResult(GameResult result)
        {
            await _context.Results.AddAsync(result);
            await _context.SaveChangesAsync();
        }

        public async Task<Summoner> GetSummonerById(string id)
        {
            var summoner = await _context.Summoners.FirstOrDefaultAsync(_ => _.Id == id);

            return summoner;
        }

        public async Task InsertSummoner(Summoner summoner)
        {
            await _context.Summoners.AddAsync(summoner);
            await _context.SaveChangesAsync();
        }

        public async Task InsertModel(Model model)
        {
            await _context.Models.AddAsync(model);
            await _context.SaveChangesAsync();
        }
    }
}
