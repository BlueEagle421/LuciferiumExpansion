using RimWorld;
using UnityEngine;
using Verse;

namespace LuciferiumExpansion
{
    public class LuciferiumExpansionMod : Mod
    {
        public static LuciferiumExpansionSettings Settings { get; private set; }
        public LuciferiumExpansionMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<LuciferiumExpansionSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            //Begin
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            //CollectionSpeedMultiplier
            listingStandard.Label("USH_LE_CollectionSpeedLabel".Translate());
            float severitySliderValue = listingStandard.Slider(Settings.CollectionSpeedMultiplier, 0.01f, 2.5f);
            listingStandard.Label("USH_LE_CollectionSpeedDesc".Translate(severitySliderValue.ToStringPercent()));
            Settings.CollectionSpeedMultiplier = severitySliderValue;

            //End
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory() => "Luciferium Expansion";
    }
    public class LuciferiumExpansionSettings : ModSettings
    {
        public float CollectionSpeedMultiplier = 1f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref CollectionSpeedMultiplier, "USH_CollectionSpeedMultiplier", 1f);
        }
    }
}