using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MountedTroopsSiege
{
    public class MountedTroopsSiegeModule : MBSubModuleBase
    {
        private Harmony _harmony;
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            InformationManager.DisplayMessage(
                new InformationMessage("MountedTroopsSiege Mod loaded successfully."));

            _harmony = new Harmony("MountedTroopsSiege");
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            _harmony.PatchAll();
        }

        public override void OnGameEnd(Game game)
        {
            base.OnGameEnd(game);
            _harmony.UnpatchAll("MountedTroopsSiege");
        }
    }
}
