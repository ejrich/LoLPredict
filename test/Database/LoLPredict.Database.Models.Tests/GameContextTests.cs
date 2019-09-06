using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoLPredict.Database.Models.Test
{
    [TestClass]
    public class GameContextTests
    {
        private GameContext _target;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<GameContext>()
                .UseInMemoryDatabase(databaseName: "Game")
                // .UseSqlServer("TODO")
                .Options;

            _target = new GameContext(options);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _target.Dispose();
        }

        [TestMethod]
        public void AllDbSets_QueryableWithoutError()
        {
            var champions = _target.Champions.ToList();
            var results = _target.Results.ToList();
            var models = _target.Models.ToList();
            var patches = _target.Patches.ToList();
            var summoners = _target.Summoners.ToList();
        }

        [TestMethod]
        public void AllDbSets_CanAddToSet()
        {
            _target.Champions.Add(new Champion
            {
                Id = 100,
                Name = "Aatrox",
                Image = "aatrox.png",
                Patch = "9.1.1"
            });
            _target.Results.Add(new GameResult
            {
                Id = 123456789,
                Patch = "9.1.1"
            });
            _target.Models.Add(new Model
            {
                Id = 1,
                Name = "testmodel.zip",
                Patch = "9.1.1"
            });
            _target.Patches.Add(new Patch
            {
                Major = 9,
                Minor = 1,
                Version = 1
            });
            _target.Summoners.Add(new Summoner
            {
                Id = "123456789",
                Name = "EJRich44",
                AccountId = "45678945678956",
                Puuid = "98765498734"
            });
            _target.SaveChanges();
        }
    }
}
