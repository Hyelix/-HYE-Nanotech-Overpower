using Verse;

namespace Nanotech
{
    public class CompProperties_NanoRegenerator : CompProperties
    {
        public float radius = 12f;
        public int tickInterval = 60;

        public int buildingHealPerPulse = 50;
        public int itemRepairPerPulse = 50;
        public float pawnHealPerPulse = 2f;

        public bool healPawns = true;
        public bool healPermanent = false;
        public bool removePermanentIfLow = true;
        public float permanentRemoveThreshold = 0.01f;

        public bool repairBuildings = true;
        public bool repairItems = true;

        public bool repairColonistBuildings = true;
        public bool repairAlliedBuildings = true;
        public bool repairNeutralBuildings = true;
        public bool repairHostileBuildings = false;
        public bool repairNoFactionBuildings = true;
        public bool repairNoFactionMineables = false;
        public bool repairNaturalRock = false;

        public bool affectColonists = true;
        public bool affectAllies = true;
        public bool affectAnimals = true;
        public bool affectMechanoids = false;
        public bool affectHostiles = false;
        public bool affectPrisoners = false;

        public bool healDiseases = true;
        public float diseaseSeverityPerPulse = 0.03f;
        public bool removeDiseaseIfLow = true;
        public float removeThreshold = 0.01f;

        public bool affectAnesthetic = true;
        public float anestheticSeverityPerPulse = 0.20f;

        public bool cleanFilth = true;
        public int filthLevelsPerPulse = 2;

        public bool extinguishFires = true;
        public float fireSizePerPulse = 0.5f;
        public float fireExtinguishThreshold = 0.1f;
        public int maxFiresPerPulse = 5;

        public bool regenInternalOrgans = true;
        public bool regenExternalLimbs = false;
        public int missingPartsPerPulse = 1;

        public bool repairWornApparel = true;
        public int wornApparelRepairPerPulse = 10;

        public bool showInspectSummary = true;

        public CompProperties_NanoRegenerator()
        {
            compClass = typeof(CompNanoRegenerator);
        }
    }
}
