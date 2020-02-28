using LoLPredict.Database.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoLPredict.Pipelines.DAL.Tests
{
    [TestClass]
    public class ModelExtensionsTests
    {
        [TestMethod]
        public void Patch_Number_FormatsThePatchNumber()
        {
            var patch = new Patch
            {
                Major = 9,
                Minor = 8,
                Version = 1
            };

            var result = patch.Number();

            Assert.AreEqual("9.8.1", result);
        }
    }
}
