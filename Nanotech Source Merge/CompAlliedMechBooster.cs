using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Nanotech
{
    public class CompAlliedMechBooster : ThingComp
    {
        public CompProperties_AlliedMechBooster Props => (CompProperties_AlliedMechBooster)props;

        private int _tickCounter;
        private readonly Dictionary<Pawn, int> trackedExpire = new Dictionary<Pawn, int>(64);

        private static readonly List<Pawn> tmpInRadius = new List<Pawn>(64);
        private static readonly List<Pawn> tmpToClear = new List<Pawn>(64);

        public override void CompTick()
        {
            base.CompTick();
            if (parent == null || !parent.Spawned || parent.Map == null) return;

            _tickCounter++;
            if (_tickCounter < Mathf.Max(1, Props.scanIntervalTicks)) return;
            _tickCounter = 0;

            DoScan();
        }

        private void DoScan()
        {
            var map = parent.Map;
            if (map == null) return;

            if (Props.activeSound != null)
                Props.activeSound.PlayOneShot(new TargetInfo(parent.Position, map));

            tmpInRadius.Clear();
            CollectMechsInRadius(map, tmpInRadius);

            int now = Find.TickManager.TicksGame;
            for (int i = 0; i < tmpInRadius.Count; i++)
            {
                var p = tmpInRadius[i];
                NanoMechBoostManager.Register(p, parent, Props.hediff);
                trackedExpire[p] = now + Props.scanIntervalTicks + 20 + Props.lingerTicks;
            }

            tmpToClear.Clear();
            foreach (var kv in trackedExpire)
            {
                if (!tmpInRadius.Contains(kv.Key) && now > kv.Value)
                    tmpToClear.Add(kv.Key);
            }

            for (int i = 0; i < tmpToClear.Count; i++)
            {
                var p = tmpToClear[i];
                NanoMechBoostManager.Unregister(p, parent, Props.hediff);
                trackedExpire.Remove(p);
            }

            tmpInRadius.Clear();
            tmpToClear.Clear();
        }

        private void CollectMechsInRadius(Map map, List<Pawn> buffer)
        {
            float rangeSq = Props.range * Props.range;
            var allPawns = map.mapPawns.AllPawns;
            for (int i = 0; i < allPawns.Count; i++)
            {
                var p = allPawns[i];
                if (!p.Spawned) continue;
                if (!p.RaceProps.IsMechanoid) continue;
                if ((float)p.Position.DistanceToSquared(parent.Position) > rangeSq) continue;
                if (p.Faction != Faction.OfPlayer &&
                    !(Props.alsoAffectNonHostileAllies && p.Faction != null && !p.Faction.HostileTo(Faction.OfPlayer)))
                    continue;
                if (!buffer.Contains(p)) buffer.Add(p);
            }
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            base.PostDrawExtraSelectionOverlays();
            if (Props.range > 0f)
                GenDraw.DrawRadiusRing(parent.Position, Props.range);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            NanoMechBoostManager.CleanupForSource(parent, Props.hediff);
            trackedExpire.Clear();
        }
    }
}
