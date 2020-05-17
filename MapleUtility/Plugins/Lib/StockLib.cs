namespace MapleUtility.Plugins.Lib
{
    public static class StockLib
    {
        public static string AddString(string origin, string value, string separator, bool isAddSpace = true)
        {
            if (isAddSpace)
                separator = " " + separator + " ";

            if (origin == null || origin == "")
                return value;
            else
                return origin + separator + value;
        }
    }
}
