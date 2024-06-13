using NUnit.Framework;

namespace MekaruStudios.MonsterCreator.Tests.Editor
{
    public class ServiceLocatorTests
    {
        [Test]
        public void Single_Instance_Created()
        {
            var instance = ServiceLocator.Instance;

            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void DestroyInstance_DestroysSingleInstance()
        {
            var instance = ServiceLocator.Instance;

            ServiceLocator.DestroyInstance();
            var newInstance = ServiceLocator.Instance;

            Assert.That(instance, Is.Not.SameAs(newInstance));
        }

        [Test]
        public void ServiceLocator_WithValidImplementation_ProvideAndResolve()
        {
            ServiceLocator.Instance.Provide<ITest>(new Testable(5f));

            var testable = ServiceLocator.Instance.Resolve<ITest>();
            
            Assert.That(testable.Get(), Is.EqualTo(5f));
        }

        [TearDown]
        public void DestroyServiceLocatorInstance()
        {
            ServiceLocator.LoadDependencies();
        }


        interface ITest
        {
            float Get();

        }
        class Testable : ITest
        {
            readonly float _value;
            public Testable(float value)
            {
                _value = value;
            }

            public float Get() => _value;
        }


    }
}
