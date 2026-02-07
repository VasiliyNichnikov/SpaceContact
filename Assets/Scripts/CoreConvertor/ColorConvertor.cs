namespace CoreConvertor
{
    public static class ColorConvertor
    {
        public static Core.EngineData.Color FromUnityColor(UnityEngine.Color unityColor)
        {
            return new Core.EngineData.Color(unityColor.r, unityColor.g, unityColor.b, unityColor.a);
        }
        
        public static UnityEngine.Color FromCoreColor(Core.EngineData.Color coreColor)
        {
            return new UnityEngine.Color(coreColor.R, coreColor.G, coreColor.B, coreColor.A);
        }
    }
}