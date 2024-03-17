
using PipeSystem;
using RimWorld;
using System.Text;
using Verse;


namespace LuciferiumExpansion
{
    [StaticConstructorOnStartup]
    public class CompLuciferiumSynthesizer : CompResourceProcessor
    {
        public override void CompTick()
        {
            base.CompTick();
            Props.results[0].countNeeded = AmountNeeded();
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine();

            stringBuilder.AppendLine((string)"USH_LE_LuciStored".Translate(Resource.name) + ": " + AmountStored().ToString());

            stringBuilder.AppendLine((string)"USH_LE_LuciNeeded".Translate(Resource.name) + ": " + AmountNeeded().ToString());

            return base.CompInspectStringExtra() + stringBuilder.ToString().TrimEnd();
        }

        private int AmountNeeded()
        {
            return (int)parent.GetStatValue(StatDef.Named("USH_ScarletMechanitesOffset"));
        }

        private int AmountStored()
        {
            return (int)Storage;
        }


    }
}
