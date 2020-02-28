using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoLPredict.Database.Models;
using LoLPredict.Web.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoLPredict.Web.Tests.DAL
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
        public async Task LoadPatches_ReturnsAllPatches()
        {
            var expectedPatches = new List<Patch>
            {
                CreatePatch(9, 8, 1), CreatePatch(9, 9, 1)
            };

            await _context.Patches.AddRangeAsync(expectedPatches);
            await _context.SaveChangesAsync();

            var patches = await _target.LoadPatches();

            Assert.AreEqual(expectedPatches.Count, patches.Count());
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

            var champions = await _target.LoadChampionsByPatch("9.9");

            Assert.IsFalse(champions.Any());

            champions = await _target.LoadChampionsByPatch("9.8");

            Assert.AreEqual(expectedChampions.Count, champions.Count());
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

            var model = await _target.LoadLatestModel("9.8");

            Assert.AreEqual("Challenger 2", model.Name);

            model = await _target.LoadLatestModel("9.9");

            Assert.AreEqual("LS's tier list 2", model.Name);
        }

        private Patch CreatePatch(int major, int minor, int version)
        {
            return new Patch
            {
                Major = major,
                Minor = minor,
                Version = version
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
