using RimWorld;
using RimWorld.Planet;
using Verse;

namespace LuciferiumExpansion
{
    public class ScarletSludgeManager : WorldComponent
    {
        private bool _shouldWarn = true;
        private float _scarletSludgeAmount = -1;

        public float ScarletSludgeAmount
        {
            get => _scarletSludgeAmount;
            set
            {
                if (value > _scarletSludgeAmount)
                    _shouldWarn = true;

                if (value <= LEUtils.WARNING_PERCENT * DefaultScarletSludgeAmount())
                    DoWarning();

                _scarletSludgeAmount = value <= 0 ? 0 : value;
            }
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
            Scribe_Values.Look(ref _shouldWarn, "USH_DidWarning", true);
        }

        private void DoWarning()
        {
            if (!_shouldWarn)
                return;

            Find.LetterStack.ReceiveLetter(
                "USH_LE_ScarletWarningLetterLabel".Translate(),
                "USH_LE_ScarletWarningLetterDesc".Translate(),
                LetterDefOf.ThreatSmall);

            _shouldWarn = false;
        }

        public float DefaultScarletSludgeAmount() => LEUtils.SUSTAIN_ONE_YEAR * 8f;
    }
}