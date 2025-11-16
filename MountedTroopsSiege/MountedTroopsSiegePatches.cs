using HarmonyLib;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static TaleWorlds.MountAndBlade.FormationAI;

namespace MountedTroopsSiege
{

    public static class HarmonyPatches
    {
        [HarmonyPatch(typeof(Mission), nameof(Mission.SpawnTroop))]
        public static class MountSiegePatch
        {
            static void Prefix(
                IAgentOriginBase troopOrigin,
                ref bool spawnWithHorse,
                bool isPlayerSide,
                bool isReinforcement,
                Mission __instance)
            {
                // Only touch sieges, and skip the player character
                if (!__instance.IsSiegeBattle || troopOrigin.Troop.IsPlayerCharacter)
                    return;

                // Determine which side (Attacker/Defender) this spawn belongs to
                Team team = Mission.GetAgentTeam(troopOrigin, isPlayerSide);
                var side = team.Side; // BattleSideEnum.Attacker or Defender

                var settings = MountedTroopsSiegeSettings.Instance;

                var nativeclass = troopOrigin.Troop.GetFormationClass();

                if (troopOrigin.Troop.IsHero)
                {
                    if (!settings.Heroes) return;
                }
                else
                {
                    if (nativeclass == FormationClass.HorseArcher && !settings.HorseArchers)
                        return;

                    if (nativeclass == FormationClass.Cavalry && !settings.Cavalry)
                        return;
                }



                if (isReinforcement)
                {
                    if (side == BattleSideEnum.Attacker && settings.AttackerReinforcement)
                    {
                        if (!isPlayerSide && settings.DisableAiAttackerReinforcement) return;

                        spawnWithHorse = true;
                    }

                    else if (side == BattleSideEnum.Defender && settings.DefenderReinforcement)
                    {
                        if (!isPlayerSide && settings.DisableAiDefenderReinforcement) return;

                        spawnWithHorse = true;
                    }
                }
                else
                {
                    if (side == BattleSideEnum.Attacker && settings.Attacker)
                    {
                        if (!isPlayerSide && settings.DisableAiAttacker) return;

                        spawnWithHorse = true;
                    }
                    else if (side == BattleSideEnum.Defender && settings.Defender)
                    {
                        if (!isPlayerSide && settings.DisableAiDefender) return;

                        spawnWithHorse = true;
                    }
                }
            }
        }

        
        // 2) Postfix on GetAgentTroopClass to restore cavalry/horse‐archer lines
        [HarmonyPatch(typeof(Mission), nameof(Mission.GetAgentTroopClass))]
        public static class GetAgentTroopClass_Patch
        {
            static void Postfix(
                ref FormationClass __result,
                BattleSideEnum battleSide,
                BasicCharacterObject agentCharacter,
                Mission __instance)
            {
                var settings = MountedTroopsSiegeSettings.Instance;

                if (!settings.HorseArchersToInfantry) 
                    return;
                // Only the same siege/sally‑out cases where the game normally forces dismount:
                if (!(__instance.IsSiegeBattle
                      || (__instance.IsSallyOutBattle && battleSide == BattleSideEnum.Attacker)))
                    return;

                // If this unit’s native formation class is a mounted one, restore it:
                FormationClass nativeClass = agentCharacter.GetFormationClass();
                if (nativeClass == FormationClass.HorseArcher)
                {
                    __result = FormationClass.Infantry;
                }
            }
        }

        public static Agent playerMount = null;

        // Horse CleanUp
        [HarmonyPatch(typeof(Mission), nameof(Mission.OnTick))]
        public static class UnmountedMountCleaner
        {
            static void Postfix(float dt, float realDt, bool updateCamera, bool doAsyncAITick)
            {
                var mission = Mission.Current;
                
                if (mission == null || !mission.IsSiegeBattle) return;

                var s = MountedTroopsSiegeSettings.Instance;


                if (s == null || s.UnmountedHorseRemovalSeconds <= 0) return;
                
                    var mounts = mission.MountsWithoutRiders.Where(m=>
                    {
                        var time = MathF.Floor(mission.CurrentTime) - MathF.Floor(m.Value.ToSeconds);
                        if(time <= 0 || time % s.UnmountedHorseRemovalSeconds != 0) return false;
                        if (s.UnmountedHorseRemovalPlayer && m.Key == playerMount) return false;
                        return true;
                    }).ToList();
                    foreach (var mount in mounts) 
                    {
                        mount.Key.Die(new Blow());
                    }
                


            }

        }

        //Player Horse Defend
        [HarmonyPatch(typeof(Agent), "OnDismount")]
        public static class Agent_OnDismount
        {
            [HarmonyPrefix]
            static void Prefix(Agent __instance, Agent mount)
            {
                var settings = MountedTroopsSiegeSettings.Instance;
                if (settings == null || settings.UnmountedHorseRemovalSeconds <= 0 || !settings.UnmountedHorseRemovalPlayer) return;

                var mission = Mission.Current;
                if (mission == null
                 || !mission.IsSiegeBattle
                 || !__instance.IsPlayerControlled)
                    return;

                if(playerMount != mount) playerMount = mount;

            }


        }


        public static class SelectiveDismountPatches
        {
            //Tick try patch
            [HarmonyPatch(typeof(Formation), nameof(Formation.Tick))]
            public static class Formation_Tick_MountDismount
            {

                static void Prefix(Formation __instance, float dt)
                {
                    var settings = MountedTroopsSiegeSettings.Instance;
                    if (!settings.MountAndDismount) return;

                    var mission = Mission.Current;
                    if (mission == null
                     || !mission.IsSiegeBattle
                     || !__instance.Team.HasTeamAi
                     || !__instance.IsAIControlled
                     || __instance.CountOfUnitsWithoutDetachedOnes == 0)
                        return;

                    var side = __instance.AI.Side;


                    var lane = GetLaneForSide(side);
                    if (lane == null)
                    {
                        return;
                    }

                    // 3) decide mount/dismount
                    // dismount if ladders are standing on that wall side
                    bool ladders = lane.PrimarySiegeWeapons
                        .OfType<SiegeLadder>()
                        .Any(lad => lad.State == SiegeLadder.LadderState.OnLand
                                 || lad.State == SiegeLadder.LadderState.OnWall);

                    // remount if it’s actually breached or its towers are gone
                    bool breached = lane.IsOpen;
                    bool towers = lane.PrimarySiegeWeapons
                        .OfType<SiegeTower>()
                        .Any(tower => !tower.IsDestroyed);

                    if (__instance.RidingOrder != RidingOrder.RidingOrderMount)
                    {
                        if (lane.LaneSide == BehaviorSide.Middle || (!ladders && !towers && breached)) //gate (only gate is middle) | no ladders and no towers and breached
                        {
                            __instance.RidingOrder = RidingOrder.RidingOrderMount;
                        }
                        else if (__instance.Team.Side == BattleSideEnum.Defender && !ladders && !towers && !breached) // walls where towers were destroyed get remounted
                        {
                            __instance.RidingOrder = RidingOrder.RidingOrderMount;
                        }
                    }
                    else if (__instance.RidingOrder != RidingOrder.RidingOrderDismount)
                    {
                        if (__instance.GetReadonlyMovementOrderReference() == MovementOrder.MovementOrderStop)
                        {
                            __instance.RidingOrder = RidingOrder.RidingOrderDismount;
                        }

                        else if (ladders || towers) //has ladders or towers
                        {
                            __instance.RidingOrder = RidingOrder.RidingOrderDismount;
                        }
                    }


                    // else leave whatever riding order they already have
                }
            


            // Helper: find the SiegeLane that matches a BehaviorSide
            private static SiegeLane GetLaneForSide(BehaviorSide side)
            {
                return TeamAISiegeComponent.SiegeLanes
                    .FirstOrDefault(l => l.LaneSide == side);
            }


        }
        


            // Helper: find the SiegeLane that matches a BehaviorSide
            private static SiegeLane GetLaneForSide(BehaviorSide side)
            {
                return TeamAISiegeComponent.SiegeLanes
                    .FirstOrDefault(l => l.LaneSide == side);
            }


        }


    }


}