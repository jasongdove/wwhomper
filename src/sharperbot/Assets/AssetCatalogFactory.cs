namespace sharperbot.Assets
{
    public static class AssetCatalogFactory
    {
        public static IAssetCatalog CreatePakCatalog(string pakFileName)
        {
            return new PakCatalog(pakFileName);
        }
    }
}