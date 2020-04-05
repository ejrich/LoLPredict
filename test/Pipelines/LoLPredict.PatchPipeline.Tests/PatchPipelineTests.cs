using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoLPredict.Database.Models;
using LoLPredict.Pipelines.DAL;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RiotApi;
using RiotApi.Models;

namespace LoLPredict.PatchPipeline.Tests
{
    [TestClass]
    public class PatchPipelineTests
    {
        private IPatchPipeline _target;

        private Mock<IGameRepository> _mockGameRepository;
        private Mock<IRestClientWrapper> _mockRestClient;
        private Mock<ILogger<PatchPipeline>> _mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockGameRepository = new Mock<IGameRepository>(MockBehavior.Strict);
            _mockRestClient = new Mock<IRestClientWrapper>(MockBehavior.Strict);
            _mockLogger = new Mock<ILogger<PatchPipeline>>();

            var mockRestClientFactory = new Mock<IRestClientFactory>(MockBehavior.Strict);
            mockRestClientFactory.Setup(_ => _.CreateRestClient(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(_mockRestClient.Object);

            var mockOptions = new Mock<IOptionsMonitor<Settings>>();
            mockOptions.SetupGet(_ => _.CurrentValue).Returns(new Settings());

            _target = new PatchPipeline(_mockGameRepository.Object, mockRestClientFactory.Object, mockOptions.Object, _mockLogger.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mockGameRepository.VerifyAll();
            _mockRestClient.VerifyAll();
            _mockLogger.VerifyAll();
        }

        [TestMethod]
        public async Task UpdatePatch_WhenCurrentPatchIsLive_ReturnsWithoutUpdating()
        {
            _mockGameRepository.Setup(_ => _.LoadLivePatch()).ReturnsAsync(new Patch
            {
                Major = 9,
                Minor = 8,
                Version = 1
            });
            _mockRestClient.Setup(_ => _.GetAsync<Realm>("realms/na.json")).ReturnsAsync(new Realm
            {
                V = "9.8.1"
            });

            var result = await _target.UpdatePatch();

            Assert.IsFalse(result.Updated);
        }

        [TestMethod]
        public async Task UpdatePatch_WhenNoNewChampions_InsertsNone()
        {
            _mockGameRepository.Setup(_ => _.LoadLivePatch()).ReturnsAsync(new Patch
            {
                Major = 9,
                Minor = 8,
                Version = 1
            });
            _mockGameRepository.Setup(_ => _.InsertPatch("9.9.1", true)).Returns(Task.CompletedTask);
            _mockGameRepository.Setup(_ => _.UpdatePatch(It.IsAny<Patch>())).Returns(Task.CompletedTask);
            _mockGameRepository.Setup(_ => _.LoadAllChampions()).ReturnsAsync(new List<Champion>
                {
                    new Champion { Id = 123 }, new Champion { Id = 1234 }
                });
            _mockGameRepository.Setup(_ => _.InsertChampions(It.Is<IEnumerable<Champion>>(l => !l.Any()))).Returns(Task.CompletedTask);

            _mockRestClient.Setup(_ => _.GetAsync<Realm>("realms/na.json")).ReturnsAsync(new Realm
            {
                V = "9.9.1"
            });
            _mockRestClient.Setup(_ => _.GetAsync<ChampionList>("cdn/9.9.1/data/en_US/champion.json")).ReturnsAsync(new ChampionList
            {
                Data = new Dictionary<string, ChampionData>
                {
                    {
                        "Aatrox", new ChampionData
                        {
                            Key = "123",
                            Name = "Aatrox",
                            Version = "9.9.1",
                            Image = new ImageData { Full = "aatrox.png" }
                        }
                    },
                    {
                        "Annie", new ChampionData
                        {
                            Key = "1234",
                            Name = "Annie",
                            Version = "9.9.1",
                            Image = new ImageData { Full = "annie.png" }
                        }
                    }
                }
            });

            var result = await _target.UpdatePatch();

            Assert.IsTrue(result.Updated);
            Assert.AreEqual("9.9.1", result.Version);
        }

        [TestMethod]
        public async Task UpdatePatch_WhenNewChampions_InsertsChampions()
        {
            _mockGameRepository.Setup(_ => _.LoadLivePatch()).ReturnsAsync(new Patch
            {
                Major = 9,
                Minor = 8,
                Version = 1
            });
            _mockGameRepository.Setup(_ => _.InsertPatch("9.9.1", true)).Returns(Task.CompletedTask);
            _mockGameRepository.Setup(_ => _.UpdatePatch(It.IsAny<Patch>())).Returns(Task.CompletedTask);
            _mockGameRepository.Setup(_ => _.LoadAllChampions()).ReturnsAsync(new List<Champion>
                {
                    new Champion { Id = 123 }
                });
            _mockGameRepository.Setup(_ => _.InsertChampions(It.Is<IEnumerable<Champion>>(l => l.Any()))).Returns(Task.CompletedTask);

            _mockRestClient.Setup(_ => _.GetAsync<Realm>("realms/na.json")).ReturnsAsync(new Realm
            {
                V = "9.9.1"
            });
            _mockRestClient.Setup(_ => _.GetAsync<ChampionList>("cdn/9.9.1/data/en_US/champion.json")).ReturnsAsync(new ChampionList
            {
                Data = new Dictionary<string, ChampionData>
                {
                    {
                        "Aatrox", new ChampionData
                        {
                            Key = "123",
                            Name = "Aatrox",
                            Version = "9.9.1",
                            Image = new ImageData { Full = "aatrox.png" }
                        }
                    },
                    {
                        "Annie", new ChampionData
                        {
                            Key = "1234",
                            Name = "Annie",
                            Version = "9.9.1",
                            Image = new ImageData { Full = "annie.png" }
                        }
                    }
                }
            });

            var result = await _target.UpdatePatch();

            Assert.IsTrue(result.Updated);
            Assert.AreEqual("9.9.1", result.Version);
        }
    }
}
