using RimWorld;
using Verse;

namespace LuciferiumExpansion
{
    public class HediffCompProperties_RemoveSkillXP : HediffCompProperties
    {
        public float xpToRemove;
        public SkillDef skillDef;

        public HediffCompProperties_RemoveSkillXP() => compClass = typeof(HediffCompRemoveSkillXP);
    }

    public class HediffCompRemoveSkillXP : HediffComp
    {
        public HediffCompProperties_RemoveSkillXP Props => (HediffCompProperties_RemoveSkillXP)props;

        public override void CompPostMake()
        {
            base.CompPostMake();
            RemoveSkillXP();
        }

        private void RemoveSkillXP()
        {
            SkillRecord skillRecord = Pawn.skills.GetSkill(Props.skillDef);

            if (skillRecord == null)
                return;

            if (skillRecord.levelInt == 0 & skillRecord.xpSinceLastLevel <= Props.xpToRemove)
            {
                skillRecord.xpSinceLastLevel = 0;
            }
            else
            {
                if (skillRecord.xpSinceLastLevel < Props.xpToRemove)
                {
                    float XPsinceLastLvl = skillRecord.xpSinceLastLevel;
                    skillRecord.levelInt--;
                    skillRecord.xpSinceLastLevel = skillRecord.XpRequiredForLevelUp - (Props.xpToRemove - XPsinceLastLvl);
                }
                else
                {
                    skillRecord.xpSinceLastLevel = Pawn.skills.GetSkill(SkillDefOf.Intellectual).xpSinceLastLevel - Props.xpToRemove;
                }
            }
        }
    }
}