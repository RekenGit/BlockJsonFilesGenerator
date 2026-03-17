using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MinecraftJsonGenerator
{
    public class PixelArtPictureBox : PictureBox
    {
        public PixelArtPictureBox()
        {
            SizeMode = PictureBoxSizeMode.Normal;
            BorderStyle = BorderStyle.FixedSingle;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (Image == null)
            {
                base.OnPaint(pe);
                return;
            }

            pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            pe.Graphics.SmoothingMode = SmoothingMode.None;
            pe.Graphics.CompositingQuality = CompositingQuality.HighQuality;

            pe.Graphics.Clear(BackColor);
            pe.Graphics.DrawImage(Image, ClientRectangle);
        }
    }
}