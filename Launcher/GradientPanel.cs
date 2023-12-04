using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher {
    internal class GradientPanel : Panel {
        public Color TopColor { get; set; }
        public Color BottomColor { get; set; }
        protected override void OnPaint(PaintEventArgs e) {
            LinearGradientBrush gradientBrush = new LinearGradientBrush(this.ClientRectangle, TopColor, BottomColor, 90f);
            Graphics graphics = e.Graphics;
            graphics.FillRectangle(gradientBrush, this.ClientRectangle);
            base.OnPaint(e);
        }
    }
}
