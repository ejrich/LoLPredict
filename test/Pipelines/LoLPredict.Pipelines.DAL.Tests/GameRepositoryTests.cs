using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoLPredict.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoLPredict.Pipelines.DAL.Tests
{
    [TestClass]
    public class GameRepositoryTests
    {
        private IGameRepository _target;

        private GameContext _context;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<GameContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new GameContext(options);
            _target = new GameRepository(_context);
        }

        [TestMethod]
        public async Task LoadLivePatch_ReturnsSinglePatch()
        {
            var expectedPatches = new List<Patch>
            {
                CreatePatch(9, 8, 1), CreatePatch(9, 9, 1, true)
            };

            await _context.Patches.AddRangeAsync(expectedPatches);
            await _context.SaveChangesAsync();

            var livePatch = await _target.LoadLivePatch();
        }

        [TestMethod]
        public async Task LoadChampionsByPatch_ReturnsChampionsInPatch()
        {
            var expectedChampions = new List<Champion>
            {
                CreateChampion(1, "Annie", "9.8"), CreateChampion(2, "Aatrox", "9.8")
            };

            await _context.Champions.AddRangeAsync(expectedChampions);
            await _context.SaveChangesAsync();

        }

        [TestMethod]
        public async Task LoadLatestModel_WhenMultiplePatches_ReturnsMostRecentForPatch()
        {
            var models = new List<Model>
            {
                CreateModel(1, "9.8", "Challenger"), CreateModel(2, "9.8", "Challenger 2"),
                CreateModel(3, "9.9", "LS's tier list"), CreateModel(4, "9.9", "LS's tier list 2")
            };

            await _context.Models.AddRangeAsync(models);
            await _context.SaveChangesAsync();

        }

        private Patch CreatePatch(int major, int minor, int version, bool live = false)
        {
            return new Patch
            {
                Major = major,
                Minor = minor,
                Version = version,
                Live = live
            };
        }

        private Champion CreateChampion(int id, string name, string patch)
        {
            return new Champion
            {
                Id = id,
                Name = name,
                Patch = patch
            };
        }

        private Model CreateModel(int id, string patch, string name)
        {
            return new Model
            {
                Id = id,
                Patch = patch,
                Name = name
            };
        }
    }
}
