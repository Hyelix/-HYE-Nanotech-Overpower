using UnityEngine;
using Verse;

namespace Nanotech
{
    public class NanotechSettings : ModSettings
    {
        // ---- Nanochest  ----
        public int chestSlots = 50;

        // ---- Nanobots ----
        public int nanobotsMarketValue = 10;

        // ---- Turret ----
        public float turret_range = 25.9f;

        // ---- Mech Booster ----
        public float mech_range = 20f;
        public int mech_scanIntervalTicks = 60;
        public int mech_lingerTicks = 0;
        public bool mech_affectNonHostileAllies = false;

        // ---- Obelisk ----
        public float reg_radius = 12f;
        public int reg_tickInterval = 60;
        public bool reg_moteEnabled = true;

        public int reg_buildingHealPerPulse = 50;
        public int reg_itemRepairPerPulse = 50;
        public float reg_pawnHealPerPulse = 2f;

        public bool reg_healPawns = true;
        public bool reg_healPermanent = false;
        public bool reg_removePermanentIfLow = true;
        public float reg_permanentRemoveThreshold = 0.01f;

        public bool reg_repairBuildings = true;
        public bool reg_repairItems = true;

        public bool reg_repairColonistBuildings = true;
        public bool reg_repairAlliedBuildings = true;
        public bool reg_repairNeutralBuildings = true;
        public bool reg_repairHostileBuildings = false;
        public bool reg_repairNoFactionBuildings = true;
        public bool reg_repairNoFactionMineables = false;
        public bool reg_repairNaturalRock = false;

        public bool reg_affectColonists = true;
        public bool reg_affectPrisoners = false;
        public bool reg_affectAllies = true;
        public bool reg_affectAnimals = true;
        public bool reg_affectMechanoids = false;
        public bool reg_affectHostiles = false;

        public bool reg_healDiseases = true;
        public float reg_diseaseSeverityPerPulse = 0.03f;
        public bool reg_removeDiseaseIfLow = true;
        public float reg_removeThreshold = 0.01f;

        public bool reg_affectAnesthetic = true;
        public float reg_anestheticSeverityPerPulse = 0.20f;

        public bool reg_cleanFilth = true;
        public int reg_filthLevelsPerPulse = 2;

        public bool reg_extinguishFires = true;
        public float reg_fireSizePerPulse = 0.5f;
        public float reg_fireExtinguishThreshold = 0.1f;
        public int reg_maxFiresPerPulse = 5;

        public bool reg_regenInternalOrgans = true;
        public bool reg_regenExternalLimbs = false;
        public int reg_missingPartsPerPulse = 1;

        public bool reg_repairWornApparel = true;
        public int reg_wornApparelRepairPerPulse = 10;

        public bool reg_showInspectSummary = true;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref chestSlots, "NT_chestSlots", 50);
            Scribe_Values.Look(ref nanobotsMarketValue, "NT_nanobotsMarketValue", 10);
            Scribe_Values.Look(ref turret_range, "NT_turret_range", 25.9f);

            Scribe_Values.Look(ref mech_range, "NT_mech_range", 20f);
            Scribe_Values.Look(ref mech_scanIntervalTicks, "NT_mech_scanIntervalTicks", 60);
            Scribe_Values.Look(ref mech_lingerTicks, "NT_mech_lingerTicks", 0);
            Scribe_Values.Look(ref mech_affectNonHostileAllies, "NT_mech_affectNonHostileAllies", false);

            Scribe_Values.Look(ref reg_radius, "NT_reg_radius", 12f);
            Scribe_Values.Look(ref reg_tickInterval, "NT_reg_tickInterval", 60);
            Scribe_Values.Look(ref reg_moteEnabled, "NT_reg_moteEnabled", true);

            Scribe_Values.Look(ref reg_buildingHealPerPulse, "NT_reg_buildingHealPerPulse", 50);
            Scribe_Values.Look(ref reg_itemRepairPerPulse, "NT_reg_itemRepairPerPulse", 50);
            Scribe_Values.Look(ref reg_pawnHealPerPulse, "NT_reg_pawnHealPerPulse", 2f);

            Scribe_Values.Look(ref reg_healPawns, "NT_reg_healPawns", true);
            Scribe_Values.Look(ref reg_healPermanent, "NT_reg_healPermanent", false);
            Scribe_Values.Look(ref reg_removePermanentIfLow, "NT_reg_removePermanentIfLow", true);
            Scribe_Values.Look(ref reg_permanentRemoveThreshold, "NT_reg_permanentRemoveThreshold", 0.01f);

            Scribe_Values.Look(ref reg_repairBuildings, "NT_reg_repairBuildings", true);
            Scribe_Values.Look(ref reg_repairItems, "NT_reg_repairItems", true);

            Scribe_Values.Look(ref reg_repairColonistBuildings, "NT_reg_repairColonistBuildings", true);
            Scribe_Values.Look(ref reg_repairAlliedBuildings, "NT_reg_repairAlliedBuildings", true);
            Scribe_Values.Look(ref reg_repairNeutralBuildings, "NT_reg_repairNeutralBuildings", true);
            Scribe_Values.Look(ref reg_repairHostileBuildings, "NT_reg_repairHostileBuildings", false);
            Scribe_Values.Look(ref reg_repairNoFactionBuildings, "NT_reg_repairNoFactionBuildings", true);
            Scribe_Values.Look(ref reg_repairNoFactionMineables, "NT_reg_repairNoFactionMineables", false);
            Scribe_Values.Look(ref reg_repairNaturalRock, "NT_reg_repairNaturalRock", false);

            Scribe_Values.Look(ref reg_affectColonists, "NT_reg_affectColonists", true);
            Scribe_Values.Look(ref reg_affectPrisoners, "NT_reg_affectPrisoners", false);
            Scribe_Values.Look(ref reg_affectAllies, "NT_reg_affectAllies", true);
            Scribe_Values.Look(ref reg_affectAnimals, "NT_reg_affectAnimals", true);
            Scribe_Values.Look(ref reg_affectMechanoids, "NT_reg_affectMechanoids", false);
            Scribe_Values.Look(ref reg_affectHostiles, "NT_reg_affectHostiles", false);

            Scribe_Values.Look(ref reg_healDiseases, "NT_reg_healDiseases", true);
            Scribe_Values.Look(ref reg_diseaseSeverityPerPulse, "NT_reg_diseaseSeverityPerPulse", 0.03f);
            Scribe_Values.Look(ref reg_removeDiseaseIfLow, "NT_reg_removeDiseaseIfLow", true);
            Scribe_Values.Look(ref reg_removeThreshold, "NT_reg_removeThreshold", 0.01f);

            Scribe_Values.Look(ref reg_affectAnesthetic, "NT_reg_affectAnesthetic", true);
            Scribe_Values.Look(ref reg_anestheticSeverityPerPulse, "NT_reg_anestheticSeverityPerPulse", 0.20f);

            Scribe_Values.Look(ref reg_cleanFilth, "NT_reg_cleanFilth", true);
            Scribe_Values.Look(ref reg_filthLevelsPerPulse, "NT_reg_filthLevelsPerPulse", 2);

            Scribe_Values.Look(ref reg_extinguishFires, "NT_reg_extinguishFires", true);
            Scribe_Values.Look(ref reg_fireSizePerPulse, "NT_reg_fireSizePerPulse", 0.5f);
            Scribe_Values.Look(ref reg_fireExtinguishThreshold, "NT_reg_fireExtinguishThreshold", 0.1f);
            Scribe_Values.Look(ref reg_maxFiresPerPulse, "NT_reg_maxFiresPerPulse", 5);

            Scribe_Values.Look(ref reg_regenInternalOrgans, "NT_reg_regenInternalOrgans", true);
            Scribe_Values.Look(ref reg_regenExternalLimbs, "NT_reg_regenExternalLimbs", false);
            Scribe_Values.Look(ref reg_missingPartsPerPulse, "NT_reg_missingPartsPerPulse", 1);

            Scribe_Values.Look(ref reg_repairWornApparel, "NT_reg_repairWornApparel", true);
            Scribe_Values.Look(ref reg_wornApparelRepairPerPulse, "NT_reg_wornApparelRepairPerPulse", 10);

            Scribe_Values.Look(ref reg_showInspectSummary, "NT_reg_showInspectSummary", true);

            base.ExposeData();
        }

        public void ResetToDefaults()
        {
            chestSlots = 50;
            nanobotsMarketValue = 10;
            turret_range = 25.9f;

            mech_range = 20f;
            mech_scanIntervalTicks = 60;
            mech_lingerTicks = 0;
            mech_affectNonHostileAllies = false;

            reg_radius = 12f;
            reg_tickInterval = 60;
            reg_moteEnabled = true;

            reg_buildingHealPerPulse = 50;
            reg_itemRepairPerPulse = 50;
            reg_pawnHealPerPulse = 2f;

            reg_healPawns = true;
            reg_healPermanent = false;
            reg_removePermanentIfLow = true;
            reg_permanentRemoveThreshold = 0.01f;

            reg_repairBuildings = true;
            reg_repairItems = true;

            reg_repairColonistBuildings = true;
            reg_repairAlliedBuildings = true;
            reg_repairNeutralBuildings = true;
            reg_repairHostileBuildings = false;
            reg_repairNoFactionBuildings = true;
            reg_repairNoFactionMineables = false;
            reg_repairNaturalRock = false;

            reg_affectColonists = true;
            reg_affectPrisoners = false;
            reg_affectAllies = true;
            reg_affectAnimals = true;
            reg_affectMechanoids = false;
            reg_affectHostiles = false;

            reg_healDiseases = true;
            reg_diseaseSeverityPerPulse = 0.03f;
            reg_removeDiseaseIfLow = true;
            reg_removeThreshold = 0.01f;

            reg_affectAnesthetic = true;
            reg_anestheticSeverityPerPulse = 0.20f;

            reg_cleanFilth = true;
            reg_filthLevelsPerPulse = 2;

            reg_extinguishFires = true;
            reg_fireSizePerPulse = 0.5f;
            reg_fireExtinguishThreshold = 0.1f;
            reg_maxFiresPerPulse = 5;

            reg_regenInternalOrgans = true;
            reg_regenExternalLimbs = false;
            reg_missingPartsPerPulse = 1;

            reg_repairWornApparel = true;
            reg_wornApparelRepairPerPulse = 10;

            reg_showInspectSummary = true;
        }
    }
}
