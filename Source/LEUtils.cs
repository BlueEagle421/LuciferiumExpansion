namespace LuciferiumExpansion
{
    public static class LEUtils
    {

        //the amount of scarlet sludge needed to sustain one addict for a year
        //10 (luci needed per year) * 24 (mechanites per luci) * 48 (sludge per mechnite)
        public const float SUSTAIN_ONE_YEAR = 11520f;
        public const float WARNING_PERCENT = 0.1f;

        public static float ConvertToLuciferium(this float sludge) => sludge / 24f / 48f;

    }
}
