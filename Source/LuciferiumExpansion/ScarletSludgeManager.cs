using RimWorld.Planet;
using Verse;

namespace LuciferiumExpansion
{
    public class ScarletSludgeManager : WorldComponent
    {
        private float _scarletSludgeAmount = -1;
        public float ScarletSludgeAmount
        {
            get => _scarletSludgeAmount;
            set => _scarletSludgeAmount = value <= 0 ? 0 : value;
        }
        public static ScarletSludgeManager Instance { get; private set; }

        public ScarletSludgeManager(World world) : base(world)
        {
            Instance = this;

            if (_scarletSludgeAmount == -1)
                _scarletSludgeAmount = DefaultScarletSludgeAmount();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _scarletSludgeAmount, "USH_ScarletSludgeAmount");
        }

        private float DefaultScarletSludgeAmount()
        {
            float amountToSustainOneYear = 1440f;
            return amountToSustainOneYear * 5f;
        }
    }
}