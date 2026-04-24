using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Nanotech
{
    public class PlaceWorker_NanoRegenerator : PlaceWorker
    {
        private CompProperties_NanoRegenerator _cachedProps;
        private static readonly List<IntVec3> _drawCells = new List<IntVec3>();

        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            var map = Find.CurrentMap;
            if (map == null) return;

            if (_cachedProps == null)
            {
                var comps = def.comps;
                if (comps != null)
                    for (int i = 0; i < comps.Count; i++)
                        if (comps[i] is CompProperties_NanoRegenerator p) { _cachedProps = p; break; }
            }

            float radius = _cachedProps?.radius ?? 10f;

            _drawCells.Clear();
            foreach (var c in GenRadial.RadialCellsAround(center, radius, true))
                if (c.InBounds(map)) _drawCells.Add(c);
            GenDraw.DrawFieldEdges(_drawCells);
        }
    }
}
