using Verse;

namespace LuciferiumExpansion
{
    public class CompProperties_ScarlettCollector : CompProperties
    {
        public int TicksPerPortion = 60;
        public SoundDef AmbientSound;

        public CompProperties_ScarlettCollector() => compClass = typeof(CompScarlettCollector);
    }
}
