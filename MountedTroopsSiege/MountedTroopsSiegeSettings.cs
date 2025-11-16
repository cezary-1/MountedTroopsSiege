using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Localization;

namespace MountedTroopsSiege
{
    public sealed class MountedTroopsSiegeSettings : AttributeGlobalSettings<MountedTroopsSiegeSettings>
    {
        public override string Id => "MountedTroopsSiege";
        public override string DisplayName => new TextObject("{=MTS_MCM_TITLE}Mounted Troops Siege").ToString();
        public override string FolderName => "MountedTroopsSiege";
        public override string FormatType => "json";

        //── General Mount Settings ────────────────────────────────

        [SettingPropertyBool(
            "{=MTS_MCM_DFR}Enable Defender Mounts",
            Order = 0, RequireRestart = false,
            HintText = "{=MTS_MCM_DFR_HINT}Allow defenders to spawn with mounts in siege.")]
        [SettingPropertyGroup("{=MCM_GENERAL}General")]
        public bool Defender { get; set; } = true;

        [SettingPropertyBool(
            "{=MTS_MCM_ATK}Enable Attacker Mounts",
            Order = 1, RequireRestart = false,
            HintText = "{=MTS_MCM_ATK_HINT}Allow attackers to spawn with mounts in siege.")]
        [SettingPropertyGroup("{=MCM_GENERAL}General")]
        public bool Attacker { get; set; } = true;

        [SettingPropertyBool(
            "{=MTS_MCM_DFR_REF}Defender Reinforcements",
            Order = 2, RequireRestart = false,
            HintText = "{=MTS_MCM_DFR_REF_HINT}Allow reinforcements on defender side to use mounts.")]
        [SettingPropertyGroup("{=MCM_GENERAL}General")]
        public bool DefenderReinforcement { get; set; } = true;

        [SettingPropertyBool(
            "{=MTS_MCM_ATK_REF}Attacker Reinforcements",
            Order = 3, RequireRestart = false,
            HintText = "{=MTS_MCM_ATK_REF_HINT}Allow reinforcements on attacker side to use mounts.")]
        [SettingPropertyGroup("{=MCM_GENERAL}General")]
        public bool AttackerReinforcement { get; set; } = true;

        //── AI Overrides ─────────────────────────────────────────

        [SettingPropertyBool(
            "{=MTS_MCM_DFR_AI}Disable AI Defender Mounts",
            Order = 0, RequireRestart = false,
            HintText = "{=MTS_MCM_DFR_AI_HINT}Prevent AI (non‑player) defenders from spawning on mounts.")]
        [SettingPropertyGroup("{=MCM_GENERAL}General/{=MCM_AI_OVERRIDES}AI Overrides")]
        public bool DisableAiDefender { get; set; } = false;

        [SettingPropertyBool(
            "{=MTS_MCM_ATK_AI}Disable AI Attacker Mounts",
            Order = 1, RequireRestart = false,
            HintText = "{=MTS_MCM_ATK_AI_HINT}Prevent AI (non‑player) attackers from spawning on mounts.")]
        [SettingPropertyGroup("{=MCM_GENERAL}General/{=MCM_AI_OVERRIDES}AI Overrides")]
        public bool DisableAiAttacker { get; set; } = false;

        [SettingPropertyBool(
            "{=MTS_MCM_DFR_REF_AI}Disable AI Defender Reinforcements",
            Order = 2, RequireRestart = false,
            HintText = "{=MTS_MCM_DFR_REF_AI_HINT}Prevent AI defender reinforcements from mounting.")]
        [SettingPropertyGroup("{=MCM_GENERAL}General/{=MCM_AI_OVERRIDES}AI Overrides")]
        public bool DisableAiDefenderReinforcement { get; set; } = false;

        [SettingPropertyBool(
            "{=MTS_MCM_ATK_REF_AI}Disable AI Attacker Reinforcements",
            Order = 3, RequireRestart = false,
            HintText = "{=MTS_MCM_ATK_REF_AI_HINT}Prevent AI attacker reinforcements from mounting.")]
        [SettingPropertyGroup("{=MCM_GENERAL}General/{=MCM_AI_OVERRIDES}AI Overrides")]
        public bool DisableAiAttackerReinforcement { get; set; } = false;

        //── Mount/Dismount/Clean Behavior ───────────────────────────────

        [SettingPropertyBool(
            "{=MTS_MCM_MOUNT_DISMOUNT}Enable Mount/Dismount Logic",
            Order = 0, RequireRestart = false,
            HintText = "{=MTS_MCM_MOUNT_DISMOUNT_HINT}Automatically mount/dismount troops based on ladders, towers, and breaches.")]
        [SettingPropertyGroup("{=MCM_GENERAL}General/{=MCM_BEHAVIOR}Behavior")]
        public bool MountAndDismount { get; set; } = true;


        [SettingPropertyInteger(
            "{=MTS_MCM_REMOVAL}Unmounted horse removal delay (s)",
            0, 1000, Order = 1, RequireRestart = false,
            HintText = "{=MTS_MCM_REMOVAL_HINT}After how many seconds an unattended mount is removed from the battlefield. Set 0 to disable. (Default: 60 sec)")]
        [SettingPropertyGroup("{=MCM_GENERAL}General/{=MCM_BEHAVIOR}Behavior")]
        public int UnmountedHorseRemovalSeconds { get; set; } = 60;

        [SettingPropertyBool(
            "{=MTS_MCM_REMOVAL_PLAYER}Do Not Remove Player Mount",
            Order = 2, RequireRestart = false,
            HintText = "{=MTS_MCM_REMOVAL_PLAYER_HINT}Ignore player last dismounted mount in horse removal. (Default: true)")]
        [SettingPropertyGroup("{=MCM_GENERAL}General/{=MCM_BEHAVIOR}Behavior")]
        public bool UnmountedHorseRemovalPlayer { get; set; } = true;

        //── Troop Types ───────────────────────────────────────────

        [SettingPropertyBool(
            "{=MTS_MCM_CAV}Enable Cavalry",
            Order = 0, RequireRestart = false,
            HintText = "{=MTS_MCM_CAV_HINT}Allow cavalry units to spawn on mounts.")]
        [SettingPropertyGroup("{=MCM_GENERAL}General/{=MCM_TROOP_TYPES}Troop Types")]
        public bool Cavalry { get; set; } = true;

        [SettingPropertyBool(
            "{=MTS_MCM_HA}Enable Horse Archers",
            Order = 1, RequireRestart = false,
            HintText = "{=MTS_MCM_HA_HINT}Allow horse archer units to spawn on mounts.")]
        [SettingPropertyGroup("{=MCM_GENERAL}General/{=MCM_TROOP_TYPES}Troop Types")]
        public bool HorseArchers { get; set; } = true;

        [SettingPropertyBool(
            "{=MTS_MCM_HA_INF}Horse Archers to Infantry",
            Order = 2, RequireRestart = false,
            HintText = "{=MTS_MCM_HA_INF_HINT}Change Horse Archers formation to Infantry (they behaving little weird in Ranged).")]
        [SettingPropertyGroup("{=MCM_GENERAL}General/{=MCM_TROOP_TYPES}Troop Types")]
        public bool HorseArchersToInfantry { get; set; } = true;

        [SettingPropertyBool(
            "{=MTS_MCM_HERO}Enable Heroes",
            Order = 3, RequireRestart = false,
            HintText = "{=MTS_MCM_HERO_HINT}Allow hero units to spawn on mounts.")]
        [SettingPropertyGroup("{=MCM_GENERAL}General/{=MCM_TROOP_TYPES}Troop Types")]
        public bool Heroes { get; set; } = true;

    }
}
