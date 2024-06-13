using NUnit.Framework;

namespace MekaruStudios.MonsterCreator.Tests.Editor
{
    public class WindowTests
    {
        TestableWindow _window;

        [SetUp]
        public void InitializeWindow()
        {
            _window = new TestableWindow(new GUIComponent());
        }

        [Test]
        public void Invoke_OnEnter()
        {
            _window.OnEnter();

            Assert.That(_window.IsEntered, Is.True);
        }

        [Test]
        public void Invoke_OnExit()
        {
            _window.OnExit();

            Assert.That(_window.IsExited, Is.True);
        }

        [Test]
        public void GetComponents_ReturnsComponent()
        {
            var result = _window.GetComponent();
            Assert.That(result, Is.TypeOf<GUIComponent>());
        }

    }

    public class TestableWindow : IWindow
    {
        public bool IsEntered;
        public bool IsExited;
        
        readonly IGUIComponent _component;
        
        public TestableWindow(IGUIComponent component)
        {
            _component = component;
        }

        public void Render() => _component.Render();
        public IGUIComponent GetComponent() => _component;
        public void OnEnter() => IsEntered = true;
        public void OnExit() => IsExited = true;
        public float WidthPercentage { get; set; }
    }
}
