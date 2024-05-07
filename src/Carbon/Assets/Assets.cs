namespace TeamCatalyst.Carbon.Assets
{
    internal static class Assets
    {
        // god i hate doing this but tmod's asset organization sucks, so going with this

        // foundational
        internal const string AssetDirectory = "Carbon/Assets/";
        internal const string TextureDirectory = AssetDirectory + "Textures/";

        #region Main Content
        // general directories
        internal const string MainContent = TextureDirectory + "MainContent/";
        internal const string MCArmors = MainContent + "Armors/";
        internal const string MCBuffs = MainContent + "Buffs/";

        // specific directories
        internal const string BunnySet = MCArmors + "Bunny/";
        internal const string BunnyBuffs = MCBuffs + "Bunny/";
        #endregion
    }
}
