using MekaruStudios.CustomizableMonsters;
using NUnit.Framework;

namespace MekaruStudios.MonsterCreator.Tests.Editor
{
    [TestFixture]
    public class PacketListGUITests
    {
        PacketListGUI _packetListGUI;

        [SetUp]
        public void SetUp()
        {
            var packetController = ServiceLocator.Instance.Resolve<IPacketListController>();
            _packetListGUI = new PacketListGUI(packetController);
        }


        [Test]
        public void PacketModelsProperty_Loads_AllPackets()
        {
            var packets = AssetDatabaseExtension.GetAllAssets<PacketModel>();

            var result = _packetListGUI.PacketList;
            
            Assert.That(result, Is.EquivalentTo(packets));
        }

       
        
    }
}
