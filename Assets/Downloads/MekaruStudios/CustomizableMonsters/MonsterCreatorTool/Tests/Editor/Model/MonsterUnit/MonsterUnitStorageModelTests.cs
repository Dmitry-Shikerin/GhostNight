using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace MekaruStudios.MonsterCreator.Tests.Editor
{
    public class MonsterUnitStorageModelTests
    {
        IUnitContainerModel m_UnitContainer;
        MockUnitContainerObserver m_MockContainerObserver;

        [SetUp]
        public void Setup()
        {
            m_UnitContainer = new UnitContainerModel();
            m_MockContainerObserver = new MockUnitContainerObserver(m_UnitContainer);
        }

        [Test]
        public void AddMonsterUnitModel_AddsAndNotify()
        {
            var newMonsterUnitModel = ScriptableObject.CreateInstance<UnitModel>();

            m_UnitContainer.AddUnit(newMonsterUnitModel);

            Assert.That(m_MockContainerObserver.NotifiedList, Has.Member(newMonsterUnitModel));
        }

        [Test]
        public void RemoveMonsterUnitModel_RemovesAndNotify()
        {
            var newMonsterUnitModel = ScriptableObject.CreateInstance<UnitModel>();
            m_UnitContainer.AddUnit(newMonsterUnitModel);
            
            m_UnitContainer.RemoveUnit(newMonsterUnitModel);

            Assert.That(m_MockContainerObserver.NotifiedList, Has.No.Member(newMonsterUnitModel));
        }

    }

    public class MockUnitContainerObserver : IUnitContainerObserver
    {
        public List<IUnitModel> NotifiedList;

        public MockUnitContainerObserver(IUnitContainerModel subject)
        {
            subject.RegisterObserver(this);
        }

        public void UpdateMonsterUnitStorage(List<IUnitModel> monsterUnitModels)
        {
            NotifiedList = monsterUnitModels;
        }
    }
}
