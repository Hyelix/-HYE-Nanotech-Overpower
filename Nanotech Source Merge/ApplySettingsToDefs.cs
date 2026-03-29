using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Nanotech
{
    public static class ApplySettingsToDefs
    {
        public static void Apply()
        {
            // --- Nanochest ---
            var chest = DefDatabase<ThingDef>.GetNamedSilentFail("Nanochest");
            if (chest?.building != null)
                chest.building.maxItemsInCell = Mathf.Clamp(NanotechMod.Settings.chestSlots, 1, 9999);

            // --- NanoTurret range ---
            var nanoTurretGun = DefDatabase<ThingDef>.GetNamedSilentFail("NanoTurretGun");
            if (nanoTurretGun != null)
            {
                var verbsList = nanoTurretGun.verbs;

                if (verbsList == null || verbsList.Count == 0)
                {
                    var turretDef = DefDatabase<ThingDef>.GetNamedSilentFail("NanoTurret");
                    var gunDef = turretDef?.building?.turretGunDef;
                    if (gunDef != null)
                        verbsList = gunDef.verbs;
                }

                if (verbsList != null && verbsList.Count > 0)
                {
                    for (int i = 0; i < verbsList.Count; i++)
                        if (verbsList[i] != null)
                            verbsList[i].range = NanotechMod.Settings.turret_range;
                }

                if (Current.ProgramState == ProgramState.Playing)
                {
                    foreach (var map in Find.Maps)
                    {
                        var turrets = map.listerBuildings
                            .AllBuildingsColonistOfClass<Building_TurretGun>()
                            .ToList();
                        for (int i = 0; i < turrets.Count; i++)
                        {
                            var t = turrets[i];
                            if (t.def?.defName != "NanoTurret") continue;
                            var v = t.GunCompEq?.PrimaryVerb;
                            if (v?.verbProps != null)
                                v.verbProps.range = NanotechMod.Settings.turret_range;
                        }
                    }
                }
            }

            // --- Nano Regenerator ---
            var regDef = DefDatabase<ThingDef>.GetNamedSilentFail("NanoRegenerator");
            if (regDef != null && regDef.comps != null)
            {
                var props = regDef.comps.OfType<CompProperties_NanoRegenerator>().FirstOrDefault();
                if (props != null)
                {
                    props.radius = NanotechMod.Settings.reg_radius;
                    props.tickInterval = NanotechMod.Settings.reg_tickInterval;
                    props.buildingHealPerPulse = NanotechMod.Settings.reg_buildingHealPerPulse;
                    props.itemRepairPerPulse = NanotechMod.Settings.reg_itemRepairPerPulse;
                    props.pawnHealPerPulse = NanotechMod.Settings.reg_pawnHealPerPulse;

                    props.healPawns = NanotechMod.Settings.reg_healPawns;
                    props.healPermanent = NanotechMod.Settings.reg_healPermanent;
                    props.removePermanentIfLow = NanotechMod.Settings.reg_removePermanentIfLow;
                    props.permanentRemoveThreshold = NanotechMod.Settings.reg_permanentRemoveThreshold;

                    props.repairBuildings = NanotechMod.Settings.reg_repairBuildings;
                    props.repairItems = NanotechMod.Settings.reg_repairItems;

                    props.repairColonistBuildings = NanotechMod.Settings.reg_repairColonistBuildings;
                    props.repairAlliedBuildings = NanotechMod.Settings.reg_repairAlliedBuildings;
                    props.repairNeutralBuildings = NanotechMod.Settings.reg_repairNeutralBuildings;
                    props.repairHostileBuildings = NanotechMod.Settings.reg_repairHostileBuildings;
                    props.repairNoFactionBuildings = NanotechMod.Settings.reg_repairNoFactionBuildings;
                    props.repairNoFactionMineables = NanotechMod.Settings.reg_repairNoFactionMineables;
                    props.repairNaturalRock = NanotechMod.Settings.reg_repairNaturalRock;

                    props.affectColonists = NanotechMod.Settings.reg_affectColonists;
                    props.affectPrisoners = NanotechMod.Settings.reg_affectPrisoners;
                    props.affectAllies = NanotechMod.Settings.reg_affectAllies;
                    props.affectAnimals = NanotechMod.Settings.reg_affectAnimals;
                    props.affectMechanoids = NanotechMod.Settings.reg_affectMechanoids;
                    props.affectHostiles = NanotechMod.Settings.reg_affectHostiles;

                    props.healDiseases = NanotechMod.Settings.reg_healDiseases;
                    props.diseaseSeverityPerPulse = NanotechMod.Settings.reg_diseaseSeverityPerPulse;
                    props.removeDiseaseIfLow = NanotechMod.Settings.reg_removeDiseaseIfLow;
                    props.removeThreshold = NanotechMod.Settings.reg_removeThreshold;

                    props.affectAnesthetic = NanotechMod.Settings.reg_affectAnesthetic;
                    props.anestheticSeverityPerPulse = NanotechMod.Settings.reg_anestheticSeverityPerPulse;

                    props.cleanFilth = NanotechMod.Settings.reg_cleanFilth;
                    props.filthLevelsPerPulse = NanotechMod.Settings.reg_filthLevelsPerPulse;

                    props.extinguishFires = NanotechMod.Settings.reg_extinguishFires;
                    props.fireSizePerPulse = NanotechMod.Settings.reg_fireSizePerPulse;
                    props.fireExtinguishThreshold = NanotechMod.Settings.reg_fireExtinguishThreshold;
                    props.maxFiresPerPulse = NanotechMod.Settings.reg_maxFiresPerPulse;

                    props.repairWornApparel = NanotechMod.Settings.reg_repairWornApparel;
                    props.wornApparelRepairPerPulse = NanotechMod.Settings.reg_wornApparelRepairPerPulse;

                    props.regenInternalOrgans = NanotechMod.Settings.reg_regenInternalOrgans;
                    props.regenExternalLimbs = NanotechMod.Settings.reg_regenExternalLimbs;
                    props.missingPartsPerPulse = NanotechMod.Settings.reg_missingPartsPerPulse;

                    props.showInspectSummary = NanotechMod.Settings.reg_showInspectSummary;

                    // Invalidate cached cells on all spawned regenerators so the new radius takes effect immediately
                    if (Current.ProgramState == ProgramState.Playing)
                    {
                        foreach (var map in Find.Maps)
                        {
                            foreach (var thing in map.listerThings.ThingsOfDef(regDef))
                            {
                                if (thing is ThingWithComps twc)
                                    twc.GetComp<CompNanoRegenerator>()?.InvalidateCellCache();
                            }
                        }
                    }
                }
            }

            // --- Nano Mech Booster ---
            NanotechMod.Settings.mech_range = Mathf.Min(NanotechMod.Settings.mech_range, 79.81f);

            var all = DefDatabase<ThingDef>.AllDefsListForReading;
            for (int i = 0; i < all.Count; i++)
            {
                var td = all[i];
                if (td.comps == null) continue;
                var mb = td.comps.OfType<CompProperties_AlliedMechBooster>().FirstOrDefault();
                if (mb == null) continue;

                mb.range = NanotechMod.Settings.mech_range;
                mb.scanIntervalTicks = NanotechMod.Settings.mech_scanIntervalTicks;
                mb.lingerTicks = NanotechMod.Settings.mech_lingerTicks;
                mb.alsoAffectNonHostileAllies = NanotechMod.Settings.mech_affectNonHostileAllies;
            }

            // --- Regenerator mote toggle ---
            if (regDef != null && regDef.comps != null)
            {
                var moteComps = regDef.comps.OfType<CompProperties_MoteEmitter>().ToList();
                if (!NanotechMod.Settings.reg_moteEnabled)
                {
                    if (moteComps.Count > 0)
                        regDef.comps.RemoveAll(c => c is CompProperties_MoteEmitter);
                }
                else
                {
                    if (moteComps.Count == 0)
                    {
                        var moteDef = DefDatabase<ThingDef>.GetNamedSilentFail("Mote_PsychicEmanatorEffect");
                        var newProps = new CompProperties_MoteEmitter
                        {
                            mote = moteDef,
                            emissionInterval = 75
                        };
                        regDef.comps.Add(newProps);
                    }
                }
            }

            // --- Nanobots market value ---
            var nanobots = DefDatabase<ThingDef>.GetNamedSilentFail("NT_Nanobots");
            if (nanobots != null)
            {
                if (nanobots.statBases == null)
                    nanobots.statBases = new List<StatModifier>();

                var marketStat = nanobots.statBases.FirstOrDefault(s => s.stat == StatDefOf.MarketValue);
                if (marketStat != null)
                {
                    marketStat.value = NanotechMod.Settings.nanobotsMarketValue;
                }
                else
                {
                    nanobots.statBases.Add(new StatModifier
                    {
                        stat = StatDefOf.MarketValue,
                        value = NanotechMod.Settings.nanobotsMarketValue
                    });
                }
            }
        }
    }
}
