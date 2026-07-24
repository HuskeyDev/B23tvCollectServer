namespace B23tvCollect.Common.Constants
{
    public static class DirConst
    {
        public static string ExeDir { get { return AppContext.BaseDirectory; } }
        public static string ConfigDir { get { return "./Config"; } }
        public static string LogDir { get; set; } = "./Log";
        public static string DataBaseDir { get; set; } = "./DataBase";
    }
}
