using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Nanotech
{
    public class NanotechMod : Mod
    {
        public static NanotechSettings Settings;

        private int currentTab = 0;

        // tab names: 0=Nanochest, 1=Mech Booster, 2=Obelisk (Gen), 3=Obelisk (Tgt), 4=Obelisk (Adv), 5=Misc
        private readonly string[] tabs = new string[] { "Nanochest", "Mech Booster", "Obelisk (Gen)", "Obelisk (Tgt)", "Obelisk (Adv)", "Misc" };

        // scroll states
        private Vector2 scrollChestPos = Vector2.zero;
        private float scrollChestHeight = 500f;
        private Vector2 scrollMechPos = Vector2.zero;
        private float scrollMechHeight = 500f;
        private Vector2 scrollObGenPos = Vector2.zero;
        private float scrollObGenHeight = 600f;
        private Vector2 scrollObTgtPos = Vector2.zero;
        private float scrollObTgtHeight = 600f;
        private Vector2 scrollObAdvPos = Vector2.zero;
        private float scrollObAdvHeight = 800f;
        private Vector2 scrollMiscPos = Vector2.zero;
        private float scrollMiscHeight = 400f;

        // numeric buffers
        private string bufChestSlots;
        private string bufNanobotsVal;
        private string bufTurretRange;
        private string bufMechRange;
        private string bufMechScan;
        private string bufMechLinger;
        private string bufRegRadius;
        private string bufRegTick;
        private string bufRegBuildHeal;
        private string bufRegItemRepair;
        private string bufRegPawnHeal;
        private string bufRegPermThresh;
        private string bufRegDiseasePulse;
        private string bufRegRemoveThresh;
        private string bufRegAnesthPulse;
        private string bufRegFilthPulse;
        private string bufRegFireSize;
        private string bufRegFireThresh;
        private string bufRegMaxFires;
        private string bufRegMissParts;
        private string bufRegWornRepair;

        public NanotechMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<NanotechSettings>();
        }

        public override string SettingsCategory() => "Nanotech Overpower";

        // Initializes all numeric buffers from current settings values.
        // TextFieldNumeric mutates the buffer when the user types; if a buffer
        // is null on first paint *and* the value happens to be outside the
        // allowed [min,max] range, the call can throw inside OnGUI, breaking
        // the surrounding Listing_Standard and "freezing" the rest of the tab.
        // Calling this once before the first draw avoids that whole class of bug.
        private void EnsureBuffers()
        {
            if (bufChestSlots != null) return; // already initialized

            bufChestSlots       = Settings.chestSlots.ToString();
            bufNanobotsVal      = Settings.nanobotsMarketValue.ToString();
            bufTurretRange      = Settings.turret_range.ToString("0.##");
            bufMechRange        = Settings.mech_range.ToString("0.##");
            bufMechScan         = Settings.mech_scanIntervalTicks.ToString();
            bufMechLinger       = Settings.mech_lingerTicks.ToString();
            bufRegRadius        = Settings.reg_radius.ToString("0.##");
            bufRegTick          = Settings.reg_tickInterval.ToString();
            bufRegBuildHeal     = Settings.reg_buildingHealPerPulse.ToString();
            bufRegItemRepair    = Settings.reg_itemRepairPerPulse.ToString();
            bufRegPawnHeal      = Settings.reg_pawnHealPerPulse.ToString("0.##");
            bufRegPermThresh    = Settings.reg_permanentRemoveThreshold.ToString("0.###");
            bufRegDiseasePulse  = Settings.reg_diseaseSeverityPerPulse.ToString("0.###");
            bufRegRemoveThresh  = Settings.reg_removeThreshold.ToString("0.###");
            bufRegAnesthPulse   = Settings.reg_anestheticSeverityPerPulse.ToString("0.##");
            bufRegFilthPulse    = Settings.reg_filthLevelsPerPulse.ToString();
            bufRegFireSize      = Settings.reg_fireSizePerPulse.ToString("0.##");
            bufRegFireThresh    = Settings.reg_fireExtinguishThreshold.ToString("0.##");
            bufRegMaxFires      = Settings.reg_maxFiresPerPulse.ToString();
            bufRegMissParts     = Settings.reg_missingPartsPerPulse.ToString();
            bufRegWornRepair    = Settings.reg_wornApparelRepairPerPulse.ToString();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            EnsureBuffers();

            Rect titleRect = GenUI.TopPartPixels(inRect, 35f);
            Text.Font = GameFont.Medium;
            Widgets.Label(titleRect, "Settings");
            Text.Font = GameFont.Small;

            Rect tabsRect = new Rect(inRect.x, titleRect.yMax + 5f, inRect.width, 32f);
            float tabWidth = inRect.width / tabs.Length;
            for (int i = 0; i < tabs.Length; i++)
            {
                var tabRect = new Rect(tabsRect.x + i * tabWidth, tabsRect.y, tabWidth, 30f);
                Color old = GUI.color;
                if (currentTab == i) GUI.color = Color.yellow;
                if (Widgets.ButtonText(tabRect, tabs[i])) currentTab = i;
                GUI.color = old;
            }

            float footerHeight = 40f;
            float footerY = inRect.yMax - footerHeight;
            Rect footerRect = new Rect(inRect.x, footerY, inRect.width, footerHeight);
            Rect resetRect = new Rect(footerRect.x, footerRect.y, 140f, footerRect.height);
            Rect applyRect = new Rect(resetRect.xMax + 10f, footerRect.y, footerRect.width - 150f, footerRect.height);

            float contentY = tabsRect.yMax + 10f;
            float contentHeight = footerY - contentY - 10f;
            Rect contentRect = new Rect(inRect.x, contentY, inRect.width, contentHeight);
            Rect viewRect = new Rect(0f, 0f, contentRect.width - 16f, 0f);

            if (currentTab == 0)
                DrawScrollableTab(contentRect, viewRect, ref scrollChestPos, ref scrollChestHeight, DrawChest);
            else if (currentTab == 1)
                DrawScrollableTab(contentRect, viewRect, ref scrollMechPos, ref scrollMechHeight, DrawMechBooster);
            else if (currentTab == 2)
                DrawScrollableTab(contentRect, viewRect, ref scrollObGenPos, ref scrollObGenHeight, DrawObeliskGeneral);
            else if (currentTab == 3)
                DrawScrollableTab(contentRect, viewRect, ref scrollObTgtPos, ref scrollObTgtHeight, DrawObeliskTargets);
            else if (currentTab == 4)
                DrawScrollableTab(contentRect, viewRect, ref scrollObAdvPos, ref scrollObAdvHeight, DrawObeliskAdvanced);
            else if (currentTab == 5)
                DrawScrollableTab(contentRect, viewRect, ref scrollMiscPos, ref scrollMiscHeight, DrawMisc);

            Widgets.DrawLineHorizontal(footerRect.x, footerRect.y - 5f, footerRect.width);

            if (Widgets.ButtonText(resetRect, "Reset Defaults"))
            {
                Settings.ResetToDefaults();
                ClearBuffers();
                EnsureBuffers(); // repopulate from the freshly-defaulted settings
                WriteSettings();
                SoundStarter.PlayOneShotOnCamera(SoundDefOf.Tick_Low);
                Messages.Message("All settings reset to defaults.", MessageTypeDefOf.TaskCompletion, false);
            }

            if (Widgets.ButtonText(applyRect, "Apply Changes (Update Defs)"))
            {
                WriteSettings();
                SoundStarter.PlayOneShotOnCamera(SoundDefOf.Click);
                Messages.Message("Settings Applied to Defs!", MessageTypeDefOf.PositiveEvent, false);
            }
        }

        // Renders one tab inside its own scroll view + Listing_Standard.
        // The try/finally guarantees that End() and EndScrollView() always
        // run, even if a child draw throws — without this, an exception
        // mid-listing leaves the GUI in an inconsistent state and the rest
        // of the tab disappears (and stays gone, frame after frame).
        private void DrawScrollableTab(
            Rect contentRect,
            Rect viewRect,
            ref Vector2 scrollPos,
            ref float scrollHeight,
            System.Action<Listing_Standard> drawAction)
        {
            viewRect.height = scrollHeight;
            Widgets.BeginScrollView(contentRect, ref scrollPos, viewRect, true);
            var ls = new Listing_Standard();
            // IMPORTANT: pass a *very large* height to Begin, NOT viewRect.height.
            // Listing_Standard auto-wraps into a "new column" when curY exceeds the
            // rect height passed here, which manifests as the cursor snapping back
            // to ~0 mid-draw and later controls being drawn on top of earlier ones.
            // We want a plain vertical layout; the scroll view handles clipping.
            ls.Begin(new Rect(0f, 0f, viewRect.width, 100000f));
            try
            {
                drawAction(ls);
                scrollHeight = ls.CurHeight + 50f;
            }
            catch (System.Exception e)
            {
                Log.ErrorOnce("[Nanotech] Settings draw threw: " + e, 0x4E414E54);
            }
            finally
            {
                ls.End();
                Widgets.EndScrollView();
            }
        }

        private void DrawChest(Listing_Standard ls)
        {
            ls.Label("Nanochest Storage");
            ls.GapLine();
            IntField(ls, "Max Items per Cell (1-9999):", ref Settings.chestSlots, 1, 9999, ref bufChestSlots);
            ls.Label("If it doesn't work right away, try saving and loading your game.");
        }

        private void DrawMechBooster(Listing_Standard ls)
        {
            ls.Label("Mech Booster Settings");
            ls.GapLine();
            FloatField(ls, "Range (Max 79.8):", ref Settings.mech_range, 1f, 79.8f, ref bufMechRange);
            IntField(ls, "Scan Interval (Ticks):", ref Settings.mech_scanIntervalTicks, 1, 600, ref bufMechScan);
            IntField(ls, "Linger Ticks:", ref Settings.mech_lingerTicks, 0, 1000, ref bufMechLinger);
            ls.CheckboxLabeled("Affect Non-Hostile Allies", ref Settings.mech_affectNonHostileAllies);
        }

        private void DrawObeliskGeneral(Listing_Standard ls)
        {
            ls.Label("The Obelisk (Base Stats)");
            ls.GapLine();
            FloatField(ls, "Radius (Max 79.8):", ref Settings.reg_radius, 1f, 79.8f, ref bufRegRadius);
            IntField(ls, "Pulse Interval (Ticks):", ref Settings.reg_tickInterval, 10, 6000, ref bufRegTick);
            ls.CheckboxLabeled("Enable Visual Mote Effect", ref Settings.reg_moteEnabled);
            ls.CheckboxLabeled("Show Inspect Summary", ref Settings.reg_showInspectSummary);
            ls.Gap();
            ls.Label("Heal Amounts per Pulse");
            IntField(ls, "Building HP:", ref Settings.reg_buildingHealPerPulse, 1, 5000, ref bufRegBuildHeal);
            IntField(ls, "Item/Apparel HP:", ref Settings.reg_itemRepairPerPulse, 1, 5000, ref bufRegItemRepair);
            FloatField(ls, "Pawn HP (Distributed):", ref Settings.reg_pawnHealPerPulse, 0.1f, 500f, ref bufRegPawnHeal);
        }

        private void DrawObeliskTargets(Listing_Standard ls)
        {
            ls.Label("Targets - Who gets healed/repaired?");
            ls.GapLine();
            ls.Label("<b>Pawns (Creatures/Mechs)</b>");
            DrawTwoChecksRow(ls, "Colonists", ref Settings.reg_affectColonists, "Animals", ref Settings.reg_affectAnimals);
            DrawTwoChecksRow(ls, "Allies", ref Settings.reg_affectAllies, "Hostiles", ref Settings.reg_affectHostiles);
            DrawTwoChecksRow(ls, "Mechanoids", ref Settings.reg_affectMechanoids, "Prisoners", ref Settings.reg_affectPrisoners);
            ls.Gap();
            ls.Label("<b>Buildings</b>");
            ls.CheckboxLabeled("Repair Buildings (Master Switch)", ref Settings.reg_repairBuildings);
            if (Settings.reg_repairBuildings)
            {
                DrawTwoChecksRow(ls, "Colonist Building", ref Settings.reg_repairColonistBuildings, "Allied Building", ref Settings.reg_repairAlliedBuildings);
                DrawTwoChecksRow(ls, "Neutral Building", ref Settings.reg_repairNeutralBuildings, "Hostile Building", ref Settings.reg_repairHostileBuildings);
                DrawTwoChecksRow(ls, "Ruins (No Faction)", ref Settings.reg_repairNoFactionBuildings, "Mineables (Ores)", ref Settings.reg_repairNoFactionMineables);
                ls.CheckboxLabeled("Natural Rocks (Walls)", ref Settings.reg_repairNaturalRock);
            }
            ls.Gap();
            ls.Label("<b>Items</b>");
            ls.CheckboxLabeled("Repair Items on Ground", ref Settings.reg_repairItems);
        }

        private void DrawObeliskAdvanced(Listing_Standard ls)
        {
            ls.Label("Advanced Logic");
            ls.GapLine();
            ls.CheckboxLabeled("Heal Pawns (Master Switch)", ref Settings.reg_healPawns);
            if (Settings.reg_healPawns)
            {
                ls.CheckboxLabeled("Heal Permanent (Scars)", ref Settings.reg_healPermanent);
                if (Settings.reg_healPermanent)
                {
                    ls.CheckboxLabeled("Remove Scar if Severity Low", ref Settings.reg_removePermanentIfLow);
                    FloatField(ls, "Remove Thresh:", ref Settings.reg_permanentRemoveThreshold, 0f, 1f, ref bufRegPermThresh);
                }
                ls.Gap();
                ls.CheckboxLabeled("Heal Diseases", ref Settings.reg_healDiseases);
                if (Settings.reg_healDiseases)
                {
                    FloatField(ls, "Severity reduction:", ref Settings.reg_diseaseSeverityPerPulse, 0.001f, 1f, ref bufRegDiseasePulse);
                    ls.CheckboxLabeled("Cure if Severity Low", ref Settings.reg_removeDiseaseIfLow);
                    FloatField(ls, "Cure Thresh:", ref Settings.reg_removeThreshold, 0f, 1f, ref bufRegRemoveThresh);
                }
                ls.Gap();
                ls.CheckboxLabeled("Reduce Anesthetic", ref Settings.reg_affectAnesthetic);
                if (Settings.reg_affectAnesthetic)
                {
                    FloatField(ls, "Anesth. red.:", ref Settings.reg_anestheticSeverityPerPulse, 0.01f, 1f, ref bufRegAnesthPulse);
                }
                ls.Gap();
                ls.Label("<b>Regenerate Missing Parts</b>");
                DrawTwoChecksRow(ls, "Internal Organs", ref Settings.reg_regenInternalOrgans, "External Limbs", ref Settings.reg_regenExternalLimbs);
                IntField(ls, "Parts per Pulse:", ref Settings.reg_missingPartsPerPulse, 1, 10, ref bufRegMissParts);
            }
            ls.Gap();
            ls.Label("<b>Worn Apparel</b>");
            ls.CheckboxLabeled("Repair Worn Apparel", ref Settings.reg_repairWornApparel);
            if (Settings.reg_repairWornApparel)
            {
                IntField(ls, "HP per pulse:", ref Settings.reg_wornApparelRepairPerPulse, 1, 500, ref bufRegWornRepair);
            }
            ls.Gap();
            ls.Label("<b>Environment</b>");
            ls.CheckboxLabeled("Clean Filth", ref Settings.reg_cleanFilth);
            if (Settings.reg_cleanFilth)
            {
                IntField(ls, "Stacks per Pulse:", ref Settings.reg_filthLevelsPerPulse, 1, 50, ref bufRegFilthPulse);
            }
            ls.CheckboxLabeled("Extinguish Fires", ref Settings.reg_extinguishFires);
            if (Settings.reg_extinguishFires)
            {
                FloatField(ls, "Size reduction:", ref Settings.reg_fireSizePerPulse, 0.1f, 5f, ref bufRegFireSize);
                FloatField(ls, "Extinguish Thresh:", ref Settings.reg_fireExtinguishThreshold, 0.01f, 1f, ref bufRegFireThresh);
                IntField(ls, "Max Fires/Pulse:", ref Settings.reg_maxFiresPerPulse, 1, 100, ref bufRegMaxFires);
            }
        }

        private void DrawMisc(Listing_Standard ls)
        {
            ls.Label("Misc Settings");
            ls.GapLine();
            IntField(ls, "Nanobots Market Value:", ref Settings.nanobotsMarketValue, 1, 100000, ref bufNanobotsVal);
            ls.Label("Choose between 1 and 100000. Recommend around 40 to 60. If the value is too low enemies/bases will spawn with nanotech stuff, if it's too high might break the wealth system in your game. Choose carefully.");
            ls.Gap();
            ls.Label("Nano Turret");
            FloatField(ls, "Turret Range:", ref Settings.turret_range, 1f, 100f, ref bufTurretRange);
            ls.Label("Some changes might require restart/reload if Apply fails.");
        }

        public override void WriteSettings()
        {
            // Clamps
            Settings.chestSlots = Mathf.Clamp(Settings.chestSlots, 1, 9999);
            Settings.nanobotsMarketValue = Mathf.Clamp(Settings.nanobotsMarketValue, 1, 100000);
            Settings.turret_range = Mathf.Clamp(Settings.turret_range, 1f, 100f);

            Settings.mech_range = Mathf.Clamp(Settings.mech_range, 1f, 79.8f);
            Settings.mech_scanIntervalTicks = Mathf.Clamp(Settings.mech_scanIntervalTicks, 1, 600);
            Settings.mech_lingerTicks = Mathf.Clamp(Settings.mech_lingerTicks, 0, 1000);

            Settings.reg_radius = Mathf.Clamp(Settings.reg_radius, 1f, 79.8f);
            Settings.reg_tickInterval = Mathf.Clamp(Settings.reg_tickInterval, 10, 6000);
            Settings.reg_buildingHealPerPulse = Mathf.Clamp(Settings.reg_buildingHealPerPulse, 1, 5000);
            Settings.reg_itemRepairPerPulse = Mathf.Clamp(Settings.reg_itemRepairPerPulse, 1, 5000);
            Settings.reg_pawnHealPerPulse = Mathf.Clamp(Settings.reg_pawnHealPerPulse, 0.1f, 500f);
            Settings.reg_permanentRemoveThreshold = Mathf.Clamp01(Settings.reg_permanentRemoveThreshold);
            Settings.reg_diseaseSeverityPerPulse = Mathf.Clamp(Settings.reg_diseaseSeverityPerPulse, 0.001f, 1f);
            Settings.reg_removeThreshold = Mathf.Clamp(Settings.reg_removeThreshold, 0f, 1f);
            Settings.reg_anestheticSeverityPerPulse = Mathf.Clamp(Settings.reg_anestheticSeverityPerPulse, 0.01f, 1f);
            Settings.reg_filthLevelsPerPulse = Mathf.Clamp(Settings.reg_filthLevelsPerPulse, 1, 50);
            Settings.reg_fireSizePerPulse = Mathf.Clamp(Settings.reg_fireSizePerPulse, 0.1f, 5f);
            Settings.reg_fireExtinguishThreshold = Mathf.Clamp(Settings.reg_fireExtinguishThreshold, 0.01f, 1f);
            Settings.reg_maxFiresPerPulse = Mathf.Clamp(Settings.reg_maxFiresPerPulse, 1, 100);
            Settings.reg_missingPartsPerPulse = Mathf.Clamp(Settings.reg_missingPartsPerPulse, 1, 10);
            Settings.reg_wornApparelRepairPerPulse = Mathf.Clamp(Settings.reg_wornApparelRepairPerPulse, 1, 500);

            base.WriteSettings();
            ApplySettingsToDefs.Apply();
        }

        private void ClearBuffers()
        {
            bufChestSlots = bufNanobotsVal = bufTurretRange = null;
            bufMechRange = bufMechScan = bufMechLinger = null;
            bufRegRadius = bufRegTick = bufRegBuildHeal = bufRegItemRepair = null;
            bufRegPawnHeal = bufRegPermThresh = bufRegDiseasePulse = bufRegRemoveThresh = null;
            bufRegAnesthPulse = bufRegFilthPulse = bufRegFireSize = bufRegFireThresh = null;
            bufRegMaxFires = bufRegMissParts = bufRegWornRepair = null;
        }

        private void DrawTwoChecksRow(Listing_Standard ls, string labelLeft, ref bool valueLeft, string labelRight, ref bool valueRight)
        {
            var r = ls.GetRect(28f);
            var left = GenUI.ContractedBy(GenUI.LeftHalf(r), 4f);
            var right = GenUI.ContractedBy(GenUI.RightHalf(r), 4f);
            Widgets.CheckboxLabeled(left, labelLeft ?? string.Empty, ref valueLeft, placeCheckboxNearText: true);
            if (!string.IsNullOrEmpty(labelRight))
                Widgets.CheckboxLabeled(right, labelRight, ref valueRight, placeCheckboxNearText: true);
        }

        private void IntField(Listing_Standard ls, string label, ref int value, int min, int max, ref string buffer)
        {
            var r = ls.GetRect(28f);
            var left = GenUI.ContractedBy(GenUI.LeftPart(r, 0.6f), 4f);
            var right = GenUI.ContractedBy(GenUI.RightPart(r, 0.4f), 4f);
            Widgets.Label(left, label);
            Widgets.TextFieldNumeric(right, ref value, ref buffer, min, max);
        }

        private void FloatField(Listing_Standard ls, string label, ref float value, float min, float max, ref string buffer)
        {
            var r = ls.GetRect(28f);
            var left = GenUI.ContractedBy(GenUI.LeftPart(r, 0.6f), 4f);
            var right = GenUI.ContractedBy(GenUI.RightPart(r, 0.4f), 4f);
            Widgets.Label(left, label);
            Widgets.TextFieldNumeric(right, ref value, ref buffer, min, max);
        }
    }
}