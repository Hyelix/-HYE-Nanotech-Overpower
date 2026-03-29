using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Nanotech
{
    public class RecipeWorker_NanoRecycle : RecipeWorker
    {
        public override void ConsumeIngredient(Thing ingredient, RecipeDef recipe, Map map)
        {
            Thing itemReal = ingredient.GetInnerIfMinified();
            var costs = CostListCalculator.CostListAdjusted(itemReal.def, itemReal.Stuff, true);
            var dropPos = ingredient.PositionHeld;
            int stackMultiplier = ingredient.stackCount;

            base.ConsumeIngredient(ingredient, recipe, map);

            if (costs != null && costs.Count > 0)
            {
                foreach (var cost in costs)
                {
                    int totalToSpawn = cost.count * stackMultiplier;
                    while (totalToSpawn > 0)
                    {
                        var obj = ThingMaker.MakeThing(cost.thingDef);
                        int stack = obj.stackCount = Mathf.Min(totalToSpawn, cost.thingDef.stackLimit);
                        GenPlace.TryPlaceThing(obj, dropPos, map, ThingPlaceMode.Near);
                        totalToSpawn -= stack;
                    }
                }
                return;
            }

            // Fallback: spawn nanobots if no cost list
            var fallback = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamedSilentFail("NT_Nanobots"));
            if (fallback != null)
            {
                fallback.stackCount = stackMultiplier;
                GenPlace.TryPlaceThing(fallback, dropPos, map, ThingPlaceMode.Near);
            }
        }
    }
}
