using System.Linq;
using MekaruStudios.CustomizableMonsters;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace MekaruStudios.MonsterCreator.Tests.Editor
{
    [TestFixture]
    public class PacketControllerTests
    {
        IPacketListController m_ListController;
        IPacketModel _packetModel;
        IMainWindow _mainWindow;
        
        [SetUp]
        public void SetUp()
        {
            m_ListController = new PacketListController();
            _packetModel = ScriptableObject.CreateInstance<PacketModel>();
            _mainWindow = ServiceLocator.Instance.Resolve<IMainWindow>();
        }
        
        [Test]
        public void LoadCustomizationWindow_Change_CustomizationWindow_OnMainWindow()
        {
            var oldWindow = _mainWindow.GetWindow(1);
            
            m_ListController.LoadCustomizationWindow(_packetModel);

            var newWindow = _mainWindow.GetWindow(1);
            Assert.That(oldWindow, Is.Not.SameAs(newWindow));
        }

        [Test]
        public void LoadAllPacketModels_Returns_AllPackets()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(PacketModel)}");
            var allPacketModelsInProject = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<PacketModel>)
                .ToList();

            var result = m_ListController.GetAllPacketModels();

            Assert.That(allPacketModelsInProject, Is.EquivalentTo(result));
        }

    }
}
