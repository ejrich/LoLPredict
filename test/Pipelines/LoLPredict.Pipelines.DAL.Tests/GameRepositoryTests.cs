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

            Assert.AreEqual(9, livePatch.Minor);
        }

        [TestMethod]
        public async Task InsertPatch_InsertsNonLivePatch()
        {
            await _target.InsertPatch("9.8.1", false);

            var patch = _context.Patches.First();

            Assert.AreEqual(9, patch.Major);
            Assert.AreEqual(8, patch.Minor);
            Assert.AreEqual(1, patch.Version);
            Assert.IsFalse(patch.Live);
        }

        [TestMethod]
        public async Task InsertPatch_InsertsLivePatch()
        {
            await _target.InsertPatch("10.4.1", true);

            var patch = _context.Patches.First();

            Assert.AreEqual(10, patch.Major);
            Assert.AreEqual(4, patch.Minor);
            Assert.AreEqual(1, patch.Version);
            Assert.IsTrue(patch.Live);
        }

        [TestMethod]
        public async Task UpdatePatch_SetsPatchDetails()
        {
            var patch = CreatePatch(9, 8, 1);

            await _context.Patches.AddAsync(patch);
            await _context.SaveChangesAsync();

            patch.Live = true;

            await _target.UpdatePatch(patch);

            var actual = await _context.Patches.FirstAsync();

            Assert.AreEqual(9, patch.Major);
            Assert.AreEqual(8, patch.Minor);
            Assert.AreEqual(1, patch.Version);
            Assert.IsTrue(patch.Live);
        }

        [TestMethod]
        public async Task LoadChampions_ReturnsChampionsInPatch()
        {
            var expectedChampions = new List<Champion>
            {
                CreateChampion(1, "Annie", 9, 8), CreateChampion(2, "Aatrox", 9, 8)
            };

            await _context.Champions.AddRangeAsync(expectedChampions);
            await _context.SaveChangesAsync();

            var champions = await _target.LoadChampions("9.7");

            Assert.IsFalse(champions.Any());

            champions = await _target.LoadChampions("9.8");

            Assert.AreEqual(expectedChampions.Count, champions.Count());
        }

        [TestMethod]
        public async Task InsertChampions_LoadsChampionsIntoDatabase()
        {
            var expectedChampions = new List<Champion>
            {
                CreateChampion(1, "Annie", 9, 8), CreateChampion(2, "Aatrox", 9, 8)
            };

            await _target.InsertChampions(expectedChampions);

            var champions = await _context.Champions.ToListAsync();

            Assert.AreEqual(expectedChampions.Count, champions.Count);
        }

        [TestMethod]
        public async Task GameResultExists_ReturnsIfGameInDatabase()
        {
            await _context.Results.AddAsync(CreateGameResult(123456, "9.8.1"));
            await _context.SaveChangesAsync();

            var result = await _target.GameResultExists(123456);

            Assert.IsTrue(result);

            result = await _target.GameResultExists(1234567);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task LoadGameResults_ReturnsGamesInPatch()
        {
            var expectedResults = new List<GameResult>
            {
                CreateGameResult(123456, "9.8.1"), CreateGameResult(1234567, "9.8.1")
            };
            await _context.Results.AddRangeAsync(expectedResults);
            await _context.SaveChangesAsync();

            var results = await _target.LoadGameResults("9.8.1");

            Assert.AreEqual(expectedResults.Count, results.Count());
        }

        [TestMethod]
        public async Task InsertGameResult_InsertsDatabaseRecord()
        {
            await _target.InsertGameResult(CreateGameResult(123456, "9.8.1"));

            var result = _context.Results.First();

            Assert.AreEqual(123456, result.Id);
            Assert.AreEqual("9.8.1", result.Patch);
        }

        [TestMethod]
        public async Task GetSummonerById_ReturnsSummonerIfExists()
        {
            var expectedSummoner = CreateSummoner("1234", "TSM BB");

            await _context.Summoners.AddAsync(expectedSummoner);
            await _context.SaveChangesAsync();

            var summoner = await _target.GetSummonerById("1234");

            Assert.AreEqual("1234", summoner.Id);
            Assert.AreEqual("TSM BB", summoner.Name);

            summoner = await _target.GetSummonerById("12345");

            Assert.IsNull(summoner);
        }

        [TestMethod]
        public async Task InsertSummoner_InsertsDatabaseRecord()
        {
            await _target.InsertSummoner(CreateSummoner("1234", "TSM BB"));

            var summoner = _context.Summoners.First();

            Assert.AreEqual("1234", summoner.Id);
            Assert.AreEqual("TSM BB", summoner.Name);
        }

        [TestMethod]
        public async Task InsertModel_InsertsDatabaseRecord()
        {
            await _target.InsertModel(CreateModel(123 ,"9.8.1", "LS's tier list"));

            var model = _context.Models.First();

            Assert.AreEqual("9.8.1", model.Patch);
            Assert.AreEqual("LS's tier list", model.Name);
        }

        private static Patch CreatePatch(int major, int minor, int version, bool live = false)
        {
            return new Patch
            {
                Major = major,
                Minor = minor,
                Version = version,
                Live = live
            };
        }

        private static Champion CreateChampion(int id, string name, int major, int minor)
        {
            return new Champion
            {
                Id = id,
                Name = name,
                Major = major,
                Minor = minor
            };
        }

        private static Model CreateModel(int id, string patch, string name)
        {
            return new Model
            {
                Id = id,
                Patch = patch,
                Name = name
            };
        }

        private static GameResult CreateGameResult(long id, string patch)
        {
            return new GameResult
            {
                Id = id,
                Patch = patch
            };
        }

        private static Summoner CreateSummoner(string id, string name)
        {
            return new Summoner
            {
                Id = id,
                Name = name
            };
        }
    }
}
