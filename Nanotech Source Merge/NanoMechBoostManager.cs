using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Nanotech
{
    public static class NanoMechBoostManager
    {
        private static readonly Dictionary<Pawn, HashSet<Thing>> sourcesByPawn = new Dictionary<Pawn, HashSet<Thing>>(256);
        private static Game _lastGame;

        public static int GetCount(Pawn p)
        {
            if (p == null) return 0;
            if (sourcesByPawn.TryGetValue(p, out var set)) return set.Count;
            return 0;
        }

        public static void Register(Pawn p, Thing source, HediffDef def)
        {
            if (p == null || source == null || def == null) return;
            if (p.DestroyedOrNull() || p.Dead) return;

            // Clear state on new game to avoid ghost entries
            if (Current.Game != _lastGame)
            {
                sourcesByPawn.Clear();
                _lastGame = Current.Game;
            }

            if (!sourcesByPawn.TryGetValue(p, out var set))
            {
                set = new HashSet<Thing>();
                sourcesByPawn[p] = set;
            }

            set.Add(source);
            EnsureHediffWithSeverity(p, def, set.Count, source);
        }

        public static void Unregister(Pawn p, Thing source, HediffDef def)
        {
            if (p == null || source == null || def == null) return;
            if (!sourcesByPawn.TryGetValue(p, out var set)) return;
            if (!set.Remove(source)) return;

            int count = set.Count;
            if (count <= 0)
            {
                sourcesByPawn.Remove(p);
                var h = p.health?.hediffSet?.GetFirstHediffOfDef(def);
                if (h != null) p.health.RemoveHediff(h);
                return;
            }

            Thing any = null;
            foreach (var t in set) { any = t; break; }
            EnsureHediffWithSeverity(p, def, count, any);
        }

        public static void CleanupForSource(Thing source, HediffDef def)
        {
            if (source == null || def == null) return;
            var affectedPawns = new List<Pawn>();
            foreach (var kv in sourcesByPawn)
            {
                if (kv.Value.Contains(source)) affectedPawns.Add(kv.Key);
            }
            for (int i = 0; i < affectedPawns.Count; i++)
                Unregister(affectedPawns[i], source, def);
        }

        private static void EnsureHediffWithSeverity(Pawn p, HediffDef def, int count, Thing linkTo = null)
        {
            if (p?.health?.hediffSet == null) return;
            var h = p.health.hediffSet.GetFirstHediffOfDef(def);
            if (h == null)
            {
                h = HediffMaker.MakeHediff(def, p);
                p.health.AddHediff(h);
            }
            var link = h.TryGetComp<HediffComp_Link>();
            if (link != null && linkTo != null)
                link.other = linkTo;

            h.Severity = Mathf.Max(0f, 0.5f + count - 1);
        }
    }
}
