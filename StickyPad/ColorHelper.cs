namespace StickyPad
{
    public static class ColorHelper
    {
        private static System.Drawing.Color TempColor;

        public static System.Windows.Media.SolidColorBrush ColorShade(System.Windows.Media.Color color, bool SetLightColor)
        {
            if (SetLightColor)
            {
                TempColor = System.Windows.Forms.ControlPaint.Light(System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B));
            }
            else
            {
                TempColor = System.Windows.Forms.ControlPaint.Dark(System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B));
            }

            return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(TempColor.A, TempColor.R, TempColor.G, TempColor.B));
        }
    }
}
