using NUnit.Framework;

namespace MekaruStudios.MonsterCreator.Tests.Editor
{
    public class WindowBuilderTests
    {

        [Test]
        public static void Build_Should_ReturnIWindow()
        {
            Window window = A.Window();

            Assert.That(window, Is.Not.Null);
        }

        [Test]
        [TestCase(50f)]
        [TestCase(0f)]
        [TestCase(100f)]
        public static void Build_WithWithPercentage_ReturnsExactWidth(float percentage)
        {
            Window window = A.Window().WithWidthPercentage(percentage);

            Assert.That(window.GetWidthPercentage(), Is.EqualTo(percentage));
        }


        [Test]
        public static void WithComponent_ShouldAddComponent()
        {
            IGUIComponent component1 = new GUIComponent();

            Window window = A.Window()
                .WithWidthPercentage(50f)
                .WithComponent(component1);

            Assert.That(window.GetComponent(), Is.EqualTo(component1));
        }
    }
}
