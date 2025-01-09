using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using CargoHub.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestProject1
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class DocksDBTest
    {
        private DatabaseContext db;

        [TestInitialize]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            db = new DatabaseContext(options);
        }

        public static IEnumerable<object[]> DocksTestData => new List<object[]>
        {
            new object[] { new List<Dock> {} },
            new object[] { new List<Dock> { new Dock { Id = 1, LocationId = 1, isDeleted = false } } },
            new object[] { new List<Dock> { new Dock { Id = 1, LocationId = 1, isDeleted = false }, new Dock { Id = 2, LocationId = 2, isDeleted = false } } }
        };

        [TestMethod]
        [DynamicData(nameof(DocksTestData), DynamicDataSourceType.Property)]
        public void TestGetAll(List<Dock> docks)
        {
            // Arrange
            foreach (Dock dock in docks)
            {
                db.Docks.Add(dock);
            }
            db.SaveChanges();
            DocksDBStorage storage = new(db);

            // Act
            List<Dock> result = storage.GetAllDocksAsync().Result.ToList();

            // Assert
            Assert.IsTrue(result.Count == docks.Count);
            for (int dockIterator = 0; dockIterator < result.Count; dockIterator++)
            {
                Assert.IsTrue(result[dockIterator].Equals(docks[dockIterator]));
            }
        }

        public static IEnumerable<object[]> SpecificDockTestData => new List<object[]>
        {
            new object[] { new List<Dock> {}, 1, false },
            new object[] { new List<Dock> { new Dock { Id = 1, LocationId = 1, isDeleted = false } }, 2, false },
            new object[] { new List<Dock> { new Dock { Id = 1, LocationId = 1, isDeleted = false } }, 1, true },
            new object[] { new List<Dock> { new Dock { Id = 1, LocationId = 1, isDeleted = false }, new Dock { Id = 2, LocationId = 2, isDeleted = false } }, 2, true }
        };

        [TestMethod]
        [DynamicData(nameof(SpecificDockTestData), DynamicDataSourceType.Property)]
        public void TestGetSpecific(List<Dock> docks, int soughtId, bool expectedResult)
        {
            // Arrange
            foreach (Dock dock in docks)
            {
                db.Docks.Add(dock);
                db.SaveChanges();
            }
            DocksDBStorage storage = new(db);

            // Act
            Dock? foundDock = storage.GetDockByIdAsync(soughtId).Result;

            // Assert
            bool actualResult = foundDock != null;
            Assert.IsTrue(actualResult == expectedResult);
        }

         public static IEnumerable<object[]> AddDockTestData => new List<object[]>
        {
            new object[] { null, false },
            new object[] { new Dock { Id = 0, LocationId = 1, isDeleted = false }, false },
            new object[] { new Dock { Id = 1, LocationId = 1, isDeleted = false }, true }
        };

        [TestMethod]
        [DynamicData(nameof(AddDockTestData), DynamicDataSourceType.Property)]
        public void TestAdd(Dock dock, bool expectedResult)
        {
            // Arrange
            DocksDBStorage storage = new(db);

            // Act
            bool actualResult = false;
            if (dock != null)
            {
                actualResult = storage.CreateDockAsync(dock).Result;
            }

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
            if (expectedResult)
            {
                Assert.IsTrue(db.Docks.Contains(dock));
            }
            else
            {
                if (dock != null)
                {
                    Assert.IsFalse(db.Docks.Contains(dock));
                }
            }
        }


        [TestMethod]
        public void TestAddSameIdTwice()
        {
            // Arrange
            Dock d1 = new() { Id = 1, LocationId = 1, isDeleted = false };
            Dock d2 = new() { Id = 1, LocationId = 2, isDeleted = false };
            DocksDBStorage storage = new(db);

            // Act
            bool firstAdd = storage.CreateDockAsync(d1).Result;
            bool secondAdd = storage.CreateDockAsync(d2).Result;

            // Assert
            Assert.IsTrue(firstAdd == true);
            Assert.IsTrue(secondAdd == false);
        }

        public static IEnumerable<object[]> RemoveDockTestData => new List<object[]>
        {
            new object[] { new List<Dock> {}, 1, false },
            new object[] { new List<Dock> { new Dock { Id = 1, LocationId = 1, isDeleted = false } }, 0, false },
            new object[] { new List<Dock> { new Dock { Id = 1, LocationId = 1, isDeleted = false } }, 2, false },
            new object[] { new List<Dock> { new Dock { Id = 1, LocationId = 1, isDeleted = false } }, 1, true },
            new object[] { new List<Dock> { new Dock { Id = 1, LocationId = 1, isDeleted = false }, new Dock { Id = 2, LocationId = 2, isDeleted = false } }, 2, true }
        };

        [TestMethod]
        [DynamicData(nameof(RemoveDockTestData), DynamicDataSourceType.Property)]
        public async Task TestRemove(List<Dock> docks, int idToRemove, bool expectedResult)
        {
            int oldCount = docks.Where(_ => !_.isDeleted).Count();
            // Arrange
            foreach (Dock dock in docks)
            {
                db.Docks.Add(dock);
                db.SaveChanges();
            }
            DocksDBStorage storage = new(db);

            // Act
            bool actualResult = await storage.SoftDeleteDockAsync(idToRemove);

            // Assert
            Assert.IsTrue(actualResult == expectedResult);
            if (expectedResult == true)
                Assert.IsTrue(db.Docks.Where(_ => !_.isDeleted).Count() == oldCount - 1);
            if (expectedResult == false)
                Assert.IsTrue(db.Docks.Where(_ => !_.isDeleted).Count() == oldCount);
        }

        [TestMethod]
        public void TestRemoveSameTwice()
        {
            // Arrange
            Dock d1 = new() { Id = 1, LocationId = 1, isDeleted = false };
            db.Docks.Add(d1);
            db.SaveChanges();
            DocksDBStorage storage = new(db);

            // Act
            bool firstRemove = storage.SoftDeleteDockAsync(d1.Id).Result;
            bool secondRemove = storage.SoftDeleteDockAsync(d1.Id).Result;

            // Assert
            Assert.IsTrue(firstRemove == true);
            Assert.IsTrue(secondRemove == false);
        }
    }
}