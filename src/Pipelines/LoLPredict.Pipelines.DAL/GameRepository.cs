using System;
using System.Collections.Generic;
using System.Linq;
using LoLPredict.Database.Models;

namespace LoLPredict.Pipelines.DAL
{
    public interface IGameRepository
    {
        Patch LoadLivePatch();
        void InsertPatch(string number, bool live);
        void UpdatePatch(Patch patch);
        IEnumerable<Champion> LoadChampions(string patch);
        void InsertChampions(IEnumerable<Champion> champions);
        IEnumerable<GameResult> LoadGameResults(string patch);
    }

    public class GameRepository : IGameRepository
    {
        private readonly GameContext _context;

        public GameRepository(GameContext context)
        {
            _context = context;
        }

        public Patch LoadLivePatch()
        {
            var livePatch = _context.Patches.SingleOrDefault(_ => _.Live);

            return livePatch;
        }

        public void InsertPatch(string number, bool live)
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
            _context.SaveChangesAsync();
        }

        public void UpdatePatch(Patch patch)
        {
            _context.Patches.Update(patch);
            _context.SaveChangesAsync();
        }

        public IEnumerable<Champion> LoadChampions(string patch)
        {
            var champions = _context.Champions.Where(_ => _.Patch == patch).ToList();

            return champions;
        }

        public void InsertChampions(IEnumerable<Champion> champions)
        {
            _context.AddRange(champions);
            _context.SaveChangesAsync();
        }

        public IEnumerable<GameResult> LoadGameResults(string patch)
        {
            var gameResults = _context.Results.Where(_ => _.Patch == patch).ToList();

            return gameResults;
        }
    }
}
