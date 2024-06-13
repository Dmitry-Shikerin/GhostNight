using System;
using NUnit.Framework;
using UnityEngine;

namespace MekaruStudios.MonsterCreator.Tests.Editor
{
    [TestFixture]
    public class WindowFactoryTests
    {
        IWindowFactory _factory;

        [SetUp]
        public void Setup()
        {
            _factory = ServiceLocator.Instance.Resolve<IWindowFactory>();
        }

        [Test]
        public void Factory_CreateWithWalidType_ReturnsIWindow()
        {
            IWindow window = _factory.GetWindow("empty");

            Assert.That(window, Is.Not.Null);
        }

        [Test]
        [TestCase(-.1f)]
        [TestCase(2f)]
        public void Factory_CreateWithInvalidWidth_ThrowsException(float widthPercentage)
        {
            Assert.That(() => _factory.GetWindow("empty", widthPercentage),
                Throws.TypeOf<ArgumentException>());
        }


        [Test]
        public void Factory_Create_WithInvalidType_ThrowsException()
        {
            Assert.That(() => _factory.GetWindow("invalid"),
                Throws.TypeOf<ArgumentException>());
        }



    }
}
