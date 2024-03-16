using Verse;

namespace LuciferiumExpansion
{
    public class CompProperties_ScarlettCollector : CompProperties
    {
        public int ticksPerPortion = 60;
        public float portionSize = 0.025f;
        public SoundDef ambientSound;

        public CompProperties_ScarlettCollector() => compClass = typeof(CompScarlettCollector);
    }
}
