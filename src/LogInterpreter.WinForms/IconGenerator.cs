using System.Drawing;
using System.Drawing.Drawing2D;

namespace LogInterpreter.WinForms.Properties
{
    internal static class IconGenerator
    {
        public static Bitmap CreateOpenIcon()
        {
            var bitmap = new Bitmap(16, 16);
            using var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);
            
            // Draw folder icon with more visible colors
            using var brush = new SolidBrush(Color.FromArgb(255, 220, 100)); // More yellow
            using var pen = new Pen(Color.FromArgb(80, 80, 80), 1); // Darker outline
            
            // Folder body
            g.FillRectangle(brush, 2, 6, 12, 8);
            g.DrawRectangle(pen, 2, 6, 12, 8);
            
            // Folder tab
            g.FillRectangle(brush, 2, 4, 5, 3);
            g.DrawRectangle(pen, 2, 4, 5, 3);
            
            return bitmap;
        }
        
        public static Bitmap CreateRefreshIcon()
        {
            var bitmap = new Bitmap(16, 16);
            using var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);
            
            using var pen = new Pen(Color.FromArgb(0, 120, 215), 2); // Windows blue
            
            // Draw circular arrow
            g.DrawArc(pen, 3, 3, 10, 10, 45, 270);
            
            // Arrow head - make it bigger and more visible
            var points = new Point[] { new(13, 4), new(10, 1), new(10, 7) };
            using var brush = new SolidBrush(Color.FromArgb(0, 120, 215));
            g.FillPolygon(brush, points);
            
            return bitmap;
        }
        
        public static Bitmap CreateClearIcon()
        {
            var bitmap = new Bitmap(16, 16);
            using var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);
            
            using var pen = new Pen(Color.FromArgb(216, 27, 27), 3); // Thicker red X
            
            // Draw X with rounded caps
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            
            g.DrawLine(pen, 4, 4, 12, 12);
            g.DrawLine(pen, 12, 4, 4, 12);
            
            return bitmap;
        }
        
        public static Bitmap CreateSearchIcon()
        {
            var bitmap = new Bitmap(16, 16);
            using var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);
            
            using var pen = new Pen(Color.FromArgb(60, 60, 60), 2);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            
            // Draw magnifying glass
            g.DrawEllipse(pen, 2, 2, 8, 8);
            g.DrawLine(pen, 9, 9, 13, 13);
            
            return bitmap;
        }
        
        public static Bitmap CreateFilterIcon()
        {
            var bitmap = new Bitmap(16, 16);
            using var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);
            
            using var pen = new Pen(Color.FromArgb(80, 80, 80), 1);
            using var brush = new SolidBrush(Color.FromArgb(180, 180, 180));
            
            // Draw funnel with better proportions
            var points = new Point[] { 
                new(1, 2), new(15, 2), 
                new(11, 8), new(5, 8) 
            };
            g.FillPolygon(brush, points);
            g.DrawPolygon(pen, points);
            
            // Filter stem
            g.FillRectangle(brush, 7, 8, 2, 6);
            g.DrawRectangle(pen, 7, 8, 2, 6);
            
            // Add some filter lines for clarity
            using var thinPen = new Pen(Color.FromArgb(120, 120, 120), 1);
            g.DrawLine(thinPen, 4, 4, 12, 4);
            g.DrawLine(thinPen, 6, 6, 10, 6);
            
            return bitmap;
        }
    }
}