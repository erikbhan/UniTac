#nullable enable
using NUnit.Framework;
using Unity.Plastic.Newtonsoft.Json;
using UniTac;

public class EntityTests
{
    [Test]
    public void EntityDeserializesCorrectly()
    {
        var json = @"{""id"":76,""speed"":0.0,""x"":[-75,-93,0,0,0,0,0,0,0,0],""y"":[40,51,0,0,0,0,0,0,0,0]}";
        int[] expectedX = { -75, -93, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] expectedY = { 40, 51, 0, 0, 0, 0, 0, 0, 0, 0 };

        Entity? entity = JsonConvert.DeserializeObject<Entity>(json);

        Assert.IsNotNull(entity);
        Assert.AreEqual(entity?.Id, 76);
        Assert.AreEqual(entity?.Speed, 0.0f);
        Assert.AreEqual(entity?.X, expectedX);
        Assert.AreEqual(entity?.Y, expectedY);
    }

    [Test]
    public void Equals_EqualEntities_ReturnsTrue()
    {
        var entity1 = new Entity {
            Id = 1,
            Speed = 2.4f,
        };
        var entity2 = new Entity
        {
            Id = 1,
            Speed = 5f,
        };
        Assert.AreEqual(entity1, entity2);
    }

    [Test]
    public void Equals_NotEqualEntities_ReturnsFalse()
    {
        var entity1 = new Entity
        {
            Id = 1,
            Speed = 2.4f,
        };
        var entity2 = new Entity
        {
            Id = 2,
            Speed = 5f,
        };
        Assert.AreNotEqual(entity1, entity2);
        Assert.AreNotEqual(entity1, null);
    }

    [Test]
    public void DistanceFromParent_CalculatesAsExpected()
    {
        var entity = new Entity();
        entity.X[0] = 4;
        entity.Y[0] = 3;
        Assert.AreEqual(entity.DistanceFromParent(), 5);
    }
}
