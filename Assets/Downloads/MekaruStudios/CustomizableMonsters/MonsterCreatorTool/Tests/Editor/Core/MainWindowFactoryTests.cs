using System;
using NUnit.Framework;

namespace MekaruStudios.MonsterCreator.Tests.Editor
{
    [TestFixture]
    public class MainWindowFactoryTests
    {
            [Test]
            public void Factory_Create_ValidType_ReturnsValidWindow()
            {
                IMainWindowFactory factory = new MainWindowFactory();

                IMainWindow mainWindow = factory.GetWindow("default");

                Assert.IsNotNull(mainWindow);
            }

            [Test]
            public void Factory_Create_InvalidType_ThrowException()
            {
                IMainWindowFactory factory = new MainWindowFactory();

                Assert.That(() => factory.GetWindow("invalid"), 
                    Throws.TypeOf<ArgumentException>());
            }
    }
}
