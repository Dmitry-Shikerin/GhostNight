using System;
using NUnit.Framework;

namespace MekaruStudios.MonsterCreator.Tests.Editor
{
    [TestFixture]
    public class MainWindowTests
    {

        IMainWindowFactory _factory; 

        [SetUp]
        public void Setup()
        {
            _factory = new MainWindowFactory();
        }
        
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void ReplaceWindow_WithValidWindow_And_WithValidIndex_ShouldReplace(int index)
        {
            var mainWindow = _factory.GetWindow("default");
            Window window = A.Window().WithComponent(new GUIComponent());

            mainWindow.ReplaceWindow(window, index);

            Assert.That(mainWindow.GetWindow(index), Is.SameAs(window));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(3)]
        [TestCase(6)]
        public void Replace_WithInvalidIndexNumber_ThrowArgumentException(int index)
        {
            var mainWindow = _factory.GetWindow("default");
            Window window = A.Window().WithComponent(new GUIComponent());

            Assert.That(() => mainWindow.ReplaceWindow(window, index),
                Throws.TypeOf<ArgumentException>());
        }

    }
}
