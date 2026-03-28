using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Nanotech
{
    public class CompNanoRegenerator : ThingComp
    {
        public CompProperties_NanoRegenerator Props => (CompProperties_NanoRegenerator)props;

        private CompPowerTrader power;
        private List<IntVec3> cachedCells = new List<IntVec3>();

        private static readonly string[] FertilityKeywords =
        {
            "vasect", "vasectomy",
            "tubal", "ligation",
            "steril", "sterile", "sterilized", "sterilisation", "sterilization",
            "iud", "contracept", "birthcontrol", "birth_control", "lactating",
        };

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            power = parent.GetComp<CompPowerTrader>();
            RebuildCellCache();
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            base.PostDrawExtraSelectionOverlays();
            if (power == null || power.PowerOn)
                GenDraw.DrawRadiusRing(parent.Position, Props.radius);
        }

        public override void CompTick()
        {
            if (!parent.IsHashIntervalTick(Props.tickInterval)) return;
            if (power != null && !power.PowerOn) return;

            if (cachedCells == null || cachedCells.Count == 0)
                RebuildCellCache();

            DoPulse();
        }

        public void InvalidateCellCache()
        {
            RebuildCellCache();
        }

        private void RebuildCellCache()
        {
            var map = parent.Map;
            if (map == null) { cachedCells = new List<IntVec3>(); return; }

            cachedCells = GenRadial.RadialCellsAround(parent.Position, Props.radius, true)
                .Where(c => c.InBounds(map)).ToList();
        }

        private void DoPulse()
        {
            var map = parent.Map;
            if (map == null) return;

            int firesLeft = Mathf.Max(0, Props.maxFiresPerPulse);

            foreach (var cell in cachedCells)
            {
                // Fire
                if (Props.extinguishFires && firesLeft > 0 && Props.fireSizePerPulse > 0f)
                {
                    var fires = cell.GetThingList(map).OfType<Fire>().ToList();
                    foreach (var f in fires)
                    {
                        if (firesLeft <= 0) break;
                        float newSize = Mathf.Max(0f, f.fireSize - Props.fireSizePerPulse);
                        if (newSize <= Props.fireExtinguishThreshold) f.Destroy(DestroyMode.Vanish);
                        else f.fireSize = newSize;
                        firesLeft--;
                    }
                }

                // Filth
                if (Props.cleanFilth && Props.filthLevelsPerPulse > 0)
                {
                    var filths = cell.GetThingList(map).OfType<Filth>().ToList();
                    foreach (var f in filths)
                    {
                        int n = Props.filthLevelsPerPulse;
                        while (n-- > 0 && !f.Destroyed)
                            f.ThinFilth();
                    }
                }

                var things = cell.GetThingList(map);
                for (int i = 0; i < things.Count; i++)
                {
                    var t = things[i];
                    if (t.Destroyed) continue;

                    // Pawns
                    if (Props.healPawns && t is Pawn p)
                    {
                        if (ShouldAffectPawn(p))
                        {
                            HealPawn(p, Props.pawnHealPerPulse, Props.healPermanent);
                            if (Props.healDiseases)
                                HealDiseases(p, Props.diseaseSeverityPerPulse, Props.affectAnesthetic, Props.anestheticSeverityPerPulse);
                            TryRegenerateMissingParts(p);
                            RepairWornApparel(p);
                        }
                        continue;
                    }

                    // Buildings
                    if (Props.repairBuildings && t is Building b && b.def.useHitPoints)
                    {
                        if (ShouldRepairBuilding(b) && b.HitPoints < b.MaxHitPoints)
                            b.HitPoints = Mathf.Min(b.MaxHitPoints, b.HitPoints + Props.buildingHealPerPulse);
                        continue;
                    }

                    // Items
                    if (Props.repairItems && !(t is Building) && !(t is Pawn) && t.def.useHitPoints && t.HitPoints < t.MaxHitPoints)
                        t.HitPoints = Mathf.Min(t.MaxHitPoints, t.HitPoints + Props.itemRepairPerPulse);
                }
            }
        }

        private bool ShouldAffectPawn(Pawn p)
        {
            if (p.Dead) return false;

            if (p.IsPrisonerOfColony)
                return Props.affectPrisoners;

            if (p.Faction == parent.Faction)
            {
                if (p.RaceProps.Humanlike && Props.affectColonists) return true;
                if (p.RaceProps.Animal && Props.affectAnimals) return true;
                if (p.RaceProps.IsMechanoid && Props.affectMechanoids) return true;
                return false;
            }

            if (p.Faction != null && parent.Faction != null && p.Faction.AllyOrNeutralTo(parent.Faction))
                return Props.affectAllies;

            if (GenHostility.HostileTo(p, parent.Faction))
                return Props.affectHostiles;

            return false;
        }

        private bool ShouldRepairBuilding(Building b)
        {
            if (!Props.repairBuildings) return false;

            var myFaction = parent.Faction;
            var theirFaction = b.Faction;

            bool isNaturalRock = b.def?.building?.isNaturalRock == true;
            bool isMineableClass = b is Mineable;

            if (theirFaction == null)
            {
                if (isNaturalRock) return Props.repairNaturalRock;
                if (isMineableClass) return Props.repairNoFactionMineables;
                return Props.repairNoFactionBuildings;
            }

            if (myFaction == null) return false;

            if (theirFaction == myFaction) return Props.repairColonistBuildings;

            if (GenHostility.HostileTo(b, myFaction)) return Props.repairHostileBuildings;

            var rel = theirFaction.RelationKindWith(myFaction);
            if (rel == FactionRelationKind.Ally) return Props.repairAlliedBuildings;
            if (rel == FactionRelationKind.Neutral) return Props.repairNeutralBuildings;

            return Props.repairNeutralBuildings;
        }

        private void HealPawn(Pawn pawn, float totalHeal, bool healPermanent)
        {
            if (totalHeal <= 0f) return;

            var injuries = pawn.health.hediffSet.hediffs.OfType<Hediff_Injury>().ToList();
            if (injuries.Count == 0) return;

            float perInjury = totalHeal / injuries.Count;
            foreach (var inj in injuries)
            {
                if (inj.IsPermanent())
                {
                    if (healPermanent)
                    {
                        inj.Severity = Mathf.Max(0f, inj.Severity - perInjury);
                        if (Props.removePermanentIfLow && inj.Severity <= Props.permanentRemoveThreshold)
                            pawn.health.RemoveHediff(inj);
                    }
                }
                else
                {
                    inj.Heal(perInjury);
                }
            }

            foreach (var h in injuries.Where(h => h.Bleeding))
                h.Severity = Mathf.Max(0f, h.Severity - 0.01f * totalHeal);
        }

        private void HealDiseases(Pawn pawn, float perPulse, bool affectAnesthetic, float anesthPerPulse)
        {
            if (perPulse > 0f)
            {
                var toProcess = pawn.health.hediffSet.hediffs
                    .Where(h =>
                        h?.def != null &&
                        h.def.isBad == true &&
                        !(h is Hediff_Injury) &&
                        !(h is Hediff_MissingPart) &&
                        !(h is Hediff_Addiction) &&
                        !(h is Hediff_AddedPart) &&
                        !(h is Hediff_Implant) &&
                        !HediffMatchesKeywords(h, FertilityKeywords))
                    .ToList();

                foreach (var h in toProcess)
                {
                    float newSev = Mathf.Max(0f, h.Severity - perPulse);
                    h.Severity = newSev;
                    if (Props.removeDiseaseIfLow && newSev <= Props.removeThreshold)
                        pawn.health.RemoveHediff(h);
                }
            }

            if (affectAnesthetic && anesthPerPulse > 0f)
            {
                var anest = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Anesthetic);
                if (anest != null)
                {
                    anest.Severity = Mathf.Max(0f, anest.Severity - anesthPerPulse);
                    if (Props.removeDiseaseIfLow && anest.Severity <= Props.removeThreshold)
                        pawn.health.RemoveHediff(anest);
                }
            }
        }

        private static int AncestorsCount(BodyPartRecord p)
        {
            int n = 0;
            for (var q = p.parent; q != null; q = q.parent) n++;
            return n;
        }

        private static bool HasImplantOnOrAbove(Pawn pw, BodyPartRecord part)
        {
            if (part == null) return false;
            foreach (var h in pw.health.hediffSet.hediffs)
            {
                if (!(h is Hediff_AddedPart) && !(h is Hediff_Implant)) continue;
                var hp = h.Part;
                if (hp == null) continue;
                bool sameOrAncestor = false;
                for (var p = part; p != null; p = p.parent)
                    if (p == hp) { sameOrAncestor = true; break; }
                if (sameOrAncestor || IsInSubtree(part, hp))
                    return true;
            }
            return false;
        }

        private static bool HediffMatchesKeywords(Hediff h, string[] keywords)
        {
            if (h?.def == null || keywords == null || keywords.Length == 0) return false;
            string dn = h.def.defName ?? string.Empty;
            string lbl = h.LabelBaseCap ?? h.def.label ?? string.Empty;
            foreach (var k in keywords)
            {
                if (dn.IndexOf(k, System.StringComparison.OrdinalIgnoreCase) >= 0) return true;
                if (!string.IsNullOrEmpty(lbl) && lbl.IndexOf(k, System.StringComparison.OrdinalIgnoreCase) >= 0) return true;
            }
            return false;
        }

        private static bool IsInSubtree(BodyPartRecord root, BodyPartRecord node)
        {
            if (root == null || node == null) return false;
            for (var p = node; p != null; p = p.parent)
                if (p == root) return true;
            return false;
        }

        private static bool HasKeywordHediffInSubtreeOrAncestors(Pawn pawn, BodyPartRecord part, string[] keywords)
        {
            if (pawn == null || part == null || keywords == null || keywords.Length == 0) return false;
            foreach (var h in pawn.health.hediffSet.hediffs)
            {
                if (h?.def == null) continue;
                var hp = h.Part;
                if (hp == null) continue;
                bool sameOrAncestor = false;
                for (var p = part; p != null; p = p.parent)
                    if (p == hp) { sameOrAncestor = true; break; }
                if (!(sameOrAncestor || IsInSubtree(part, hp))) continue;
                string dn = h.def.defName ?? string.Empty;
                string lbl = h.LabelBaseCap ?? h.def.label ?? string.Empty;
                foreach (var k in keywords)
                {
                    if (dn.IndexOf(k, System.StringComparison.OrdinalIgnoreCase) >= 0) return true;
                    if (!string.IsNullOrEmpty(lbl) && lbl.IndexOf(k, System.StringComparison.OrdinalIgnoreCase) >= 0) return true;
                }
            }
            return false;
        }

        private static bool HasNonBadHediffOnOrAbove(Pawn pawn, BodyPartRecord part)
        {
            if (pawn == null || part == null) return false;
            foreach (var h in pawn.health.hediffSet.hediffs)
            {
                if (h?.def == null || h is Hediff_MissingPart) continue;
                var hp = h.Part;
                if (hp == null) continue;
                bool sameOrAncestor = false;
                for (var p = part; p != null; p = p.parent)
                    if (p == hp) { sameOrAncestor = true; break; }
                bool inSubtree = IsInSubtree(part, hp);
                if ((sameOrAncestor || inSubtree) && h.def.isBad == false)
                    return true;
            }
            return false;
        }

        private void TryRegenerateMissingParts(Pawn pawn)
        {
            int remaining = Mathf.Max(0, Props.missingPartsPerPulse);
            if (remaining == 0) return;

            var missingParts = pawn.health.hediffSet.hediffs
                .OfType<Hediff_MissingPart>()
                .Select(h => h.Part)
                .Where(part => part != null && part.def != null && part.def.defName != "Brain")
                .Distinct()
                .OrderBy(AncestorsCount)
                .ToList();

            foreach (var part in missingParts)
            {
                if (remaining <= 0) break;

                bool isOrgan = part.depth == BodyPartDepth.Inside;
                bool isLimb = part.depth != BodyPartDepth.Inside;

                if ((isOrgan && Props.regenInternalOrgans) || (isLimb && Props.regenExternalLimbs))
                {
                    if (HasImplantOnOrAbove(pawn, part)) continue;
                    if (HasNonBadHediffOnOrAbove(pawn, part)) continue;
                    if (HasKeywordHediffInSubtreeOrAncestors(pawn, part, FertilityKeywords)) continue;

                    pawn.health.RestorePart(part);
                    remaining--;
                }
            }
        }

        private void RepairWornApparel(Pawn pawn)
        {
            if (!Props.repairWornApparel) return;
            var tracker = pawn.apparel;
            if (tracker == null) return;
            var worn = tracker.WornApparel;
            if (worn == null || worn.Count == 0) return;

            int amt = Mathf.Max(0, Props.wornApparelRepairPerPulse);
            if (amt == 0) return;

            for (int i = 0; i < worn.Count; i++)
            {
                var app = worn[i];
                if (app?.def == null || !app.def.useHitPoints || app.HitPoints >= app.MaxHitPoints) continue;
                app.HitPoints = Mathf.Min(app.MaxHitPoints, app.HitPoints + amt);
            }
        }

        public override string CompInspectStringExtra()
        {
            if (!Props.showInspectSummary) return null;

            string s = $"Emits a restorative nano-field (radius {Props.radius}). Pulses every {Props.tickInterval / 60f:0.#}s.";
            if (Props.healPawns) s += " Heals injuries";
            if (Props.healPermanent) s += " (incl. scars)";
            if (Props.healDiseases) s += ", cures diseases";
            if (Props.affectAnesthetic) s += ", eases anesthesia";
            if (Props.repairBuildings || Props.repairItems) s += ". Repairs structures/items";
            if (Props.repairWornApparel) s += " (incl. worn apparel)";
            if (Props.cleanFilth) s += ". Cleans filth";
            if (Props.extinguishFires) s += ". Extinguishes fires";
            if (Props.regenInternalOrgans || Props.regenExternalLimbs)
                s += $". Regenerates missing {(Props.regenInternalOrgans ? "organs" : "")}{(Props.regenInternalOrgans && Props.regenExternalLimbs ? " & " : "")}{(Props.regenExternalLimbs ? "limbs" : "")}";
            s += ".";
            return s;
        }
    }
}
