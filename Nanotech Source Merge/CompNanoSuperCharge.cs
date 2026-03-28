using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Nanotech
{
    public class CompNanoSuperCharge : ThingComp
    {
        public CompProperties_NanoSuperCharge Props => (CompProperties_NanoSuperCharge)props;

        private readonly List<Pawn> _pawns = new List<Pawn>(8);

        public override void CompTick()
        {
            if (parent.IsHashIntervalTick(60))
                Work(60f);
        }

        public override void CompTickRare() => Work(250f);

        private void Work(float ticksThisCall)
        {
            if (parent == null || !parent.Spawned) return;
            var map = parent.Map;
            if (map == null) return;

            float perTick = (Props?.energyPerSecond ?? 0f) / 60f;
            if (perTick <= 0f) return;
            float delta = perTick * ticksThisCall;

            _pawns.Clear();

            foreach (var c in parent.OccupiedRect().ExpandedBy(1).Cells)
                CollectMechsAt(map, c);

            var ic = parent.InteractionCell;
            if (ic.IsValid) CollectMechsAt(map, ic);

            if (Props.scanRadius > 0f)
            {
                foreach (var c in GenRadial.RadialCellsAround(parent.Position, Props.scanRadius, true))
                    CollectMechsAt(map, c);
            }

            for (int i = 0; i < _pawns.Count; i++)
            {
                var pawn = _pawns[i];

                if (Props.onlyWhenChargingJob)
                {
                    bool chargingHere = false;
                    var job = pawn.CurJob;
                    var jname = pawn.CurJobDef?.defName ?? string.Empty;

                    if (jname.IndexOf("Recharge", StringComparison.OrdinalIgnoreCase) >= 0) chargingHere = true;
                    if (job != null && (job.targetA.Thing == parent || job.targetB.Thing == parent)) chargingHere = true;

                    if (!chargingHere) continue;
                }

                var need = pawn.needs?.TryGetNeed<Need_MechEnergy>();
                if (need == null) continue;

                need.CurLevel = Mathf.Min(need.CurLevel + delta, need.MaxLevel);
            }

            _pawns.Clear();
            parent.GetComp<CompThingContainer>()?.innerContainer?.ClearAndDestroyContents(DestroyMode.Vanish);
        }

        private void CollectMechsAt(Map map, IntVec3 c)
        {
            if (!c.InBounds(map)) return;
            var list = map.thingGrid.ThingsListAtFast(c);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] is Pawn p && p.RaceProps?.IsMechanoid == true && !_pawns.Contains(p))
                    _pawns.Add(p);
            }
        }

        public override string CompInspectStringExtra()
        {
            if (Props == null) return null;
            return $"Nano charge rate: +{Props.energyPerSecond:0.#}/s" +
                   (Props.onlyWhenChargingJob ? " (charging job only)" : " (any mech on pad)");
        }
    }
}
