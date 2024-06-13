using NUnit.Framework;

namespace MekaruStudios.MonsterCreator.Tests.Editor
{
    public class MainWindowBuilderTests
    {
        [Test]
        public void Build_ReturnsMainWindow()
        {
            var builder = new MainWindowBuilder();

            IMainWindow mainWindow = builder.Build();

            Assert.That(mainWindow, Is.Not.Null);
        }

        [Test]
        public void With_IWindow_ReturnsMainWindow()
        {
            var builder = new MainWindowBuilder();
            IWindow window = (Window)A.Window();

            var mainWindow = builder.With(window).Build();

            Assert.That(mainWindow, Is.Not.Null);
        }

        [Test]
        public void With_IWindow_AppendNewWindow()
        {
            IWindow window = (Window)A.Window();

            MainWindow mainWindow = A.MainWindow().With(window);

            Assert.That(mainWindow.GetWindow(0), Is.Not.Null);
        }

        [Test]
        public void With_Multiple_IWindow_AppendMultipleWindows()
        {
            IWindow window1 = (Window)A.Window();
            IWindow window2 = (Window)A.Window();
            IWindow window3 = (Window)A.Window();

            MainWindow mainWindow = A.MainWindow()
                .With(window1)
                .With(window2)
                .With(window3);


            bool isAllThreeWindowExist = mainWindow.GetWindow(0) != null &&
                                         mainWindow.GetWindow(1) != null &&
                                         mainWindow.GetWindow(2) != null;

            Assert.That(isAllThreeWindowExist, Is.True);
        }
    }
}
