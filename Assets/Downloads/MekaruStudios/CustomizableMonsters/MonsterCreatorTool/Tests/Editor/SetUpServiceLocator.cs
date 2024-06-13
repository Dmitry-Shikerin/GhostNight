using NUnit.Framework;

namespace MekaruStudios.MonsterCreator.Tests.Editor
{
    [SetUpFixture]
    public class SetUpServiceLocator
    {
        [OneTimeSetUp]
        public void LoadDependencies()
        {
            ServiceLocator.LoadDependencies();
        }

        [OneTimeTearDown]
        public void DestroyServiceLocator()
        {
            ServiceLocator.DestroyInstance();
        }
        
    }
}
