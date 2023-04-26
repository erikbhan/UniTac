#nullable enable
using NUnit.Framework;
using UniTac.Models;


namespace UniTac.Tests
{
    public class SensorTests
    {
        private Sensor Sensor = new();

        [SetUp]
        public void SetUp()
        {
            int[] one = { 1, 1, 1 };
            int[] two = { 2, 2, 2 };
            int[] three = { 3, 3, 3 };
            Entity[] entities = new Entity[3];
            entities[0] = new Entity
            {
                Id = 1,
                Speed = 1,
                X = one,
                Y = one,
            };
            entities[1] = new Entity
            {
                Id = 2,
                Speed = 2,
                X = two,
                Y = two,
            };
            entities[2] = new Entity
            {
                Id = 3,
                Speed = 3,
                X = three,
                Y = three,
            };
            foreach (var entity in entities) Sensor.Entities.Add(entity.Id, entity);
        }

        [TearDown]
        public void TearDown()
        {
            Sensor = new();
        }

        [Test]
        public void GetClosestEntity_WithEntities_ReturnsClosest()
        {
            Assert.AreEqual(1, Sensor.GetClosestEntity()?.Id);
        }

        [Test]
        public void GetClosestEntity_WithoutEntities_ReturnsNull()
        {
            Sensor.Entities.Clear();
            Assert.IsNull(Sensor.GetClosestEntity());
        }

        [Test]
        public void GetEntity_ExistingId_ReturnsEntity()
        {
            var entity = new Entity
            {
                Id = 3,
                Speed = 1
            };

            Assert.AreEqual(entity, Sensor.GetEntity(3));
        }

        [Test]
        public void GetEntity_OutOfScopeId_ReturnsNull()
        {
            Assert.IsNull(Sensor.GetEntity(4));
        }
    }
}
