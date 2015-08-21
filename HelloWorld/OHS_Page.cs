using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace HelloWorld
{
    class OHS_Page
    {
        PdfPage page;
        ProfileInfo userProfile;
        Dictionary<string, Parameter> userParameters = new Dictionary<string, Parameter>();
        XUnit quaterWidth;
        public OHS_Page(PdfPage page, ProfileInfo userProfile, List<Parameter> userParameters)
        {
            this.page = page;
            this.userProfile = userProfile;

            foreach (Parameter p in userParameters)
                this.userParameters.Add(p.Name, p);

            quaterWidth = page.Width / 4;
        }

        public void DrawHeader(XGraphics gfx)
        {
            XRect userDetailsRect = new XRect(20, page.Height * 0.05, quaterWidth, page.Height * 0.075);
            gfx.DrawRoundedRectangle(new XSolidBrush(XColor.FromKnownColor(XKnownColor.Gray)),
                userDetailsRect,
                new XSize(20, 20));

            XRect innerRect = new XRect(userDetailsRect.X + 10, userDetailsRect.Y + 10,
                userDetailsRect.Height - 20, userDetailsRect.Height - 20);
            gfx.DrawRoundedRectangle(new XSolidBrush(XColor.FromKnownColor(XKnownColor.LightGray)),
                innerRect,
                new XSize(10, 10));

            gfx.DrawString("Name : " + userProfile.Name, new XFont("Arial", 10), XBrushes.Black, 35, innerRect.Y + 10);
            gfx.DrawString("RB ID : " + userProfile.RBID.Replace(",", ""), new XFont("Arial", 10), XBrushes.Black, 35, innerRect.Y + 30);

            XImage headerImage = XImage.FromFile(@"C:\Users\kevin\Desktop\PDFsharp\samples\Samples C#\Based on WPF\HelloWorld\Content\Redback 3D logo.png");
            gfx.DrawImage(headerImage, new XPoint((quaterWidth * 2.5) - (headerImage.PixelWidth / 2), page.Height * 0.075));

            XSolidBrush brush = new XSolidBrush();
            double score = Convert.ToDouble(userProfile.Score);

            if (score <= 33)
                brush = new XSolidBrush(XColors.Red);
            else if (score > 33 && score <= 66)
                brush = new XSolidBrush(XColor.FromArgb(255, 255, 190, 0));
            else
                brush = new XSolidBrush(XColors.Green);

            XRect scoreRect = new XRect((quaterWidth * 3) - 20, page.Height * 0.05, quaterWidth, page.Height * 0.075);
            gfx.DrawRoundedRectangle(new XSolidBrush(XColor.FromKnownColor(XKnownColor.Gray)),
                scoreRect,
                new XSize(20, 20));

            XRect innerRectScore = new XRect(scoreRect.X + (scoreRect.Width - 50), scoreRect.Y + 10,
                scoreRect.Height - 20, scoreRect.Height - 20);
            gfx.DrawRoundedRectangle(brush,
                innerRectScore,
                new XSize(10, 10));

            gfx.DrawString("Overhead Squat : ", new XFont("Arial", 10),
                XBrushes.Black, scoreRect.X + 10, scoreRect.Y + 35);

            gfx.DrawString(userProfile.Score + "%", new XFont("Arial", 10),
                XBrushes.Black, innerRectScore.X + 5, innerRectScore.Y + 25);
        }

        public void DrawPentagon(XGraphics gfx)
        {
            XRect backgroundRect = new XRect(20, page.Height * 0.15, page.Width - 40, page.Height * 0.425);
            gfx.DrawRoundedRectangle(new XSolidBrush(XColor.FromKnownColor(XKnownColor.Gray)),
                backgroundRect,
                new XSize(40, 40));

            double c1 = Math.Cos((2 * Math.PI) / 5);
            double c2 = Math.Cos(Math.PI / 5);
            double s1 = Math.Sin((2 * Math.PI) / 5);
            double s2 = Math.Sin((4 * Math.PI) / 5);

            XPoint[] pentatPoints = new XPoint[5];

            XPoint center = new XPoint(page.Width * 0.5, page.Height * 0.425);
            XPoint top = new XPoint(page.Width * 0.5, page.Height * 0.25);

            double lengthOfLine = Distance(top, center);
            XPoint bottomLeft = new XPoint(center.X + (lengthOfLine * -s2), center.Y - (lengthOfLine * -c2));
            XPoint bottomRight = new XPoint(center.X + (lengthOfLine * s2), center.Y - (lengthOfLine * -c2));
            XPoint topLeft = new XPoint(center.X + (lengthOfLine * -s1), center.Y - (lengthOfLine * c1));
            XPoint topRight = new XPoint(center.X + (lengthOfLine * s1), center.Y - (lengthOfLine * c1));

            pentatPoints[0] = top;
            pentatPoints[1] = topRight;
            pentatPoints[2] = bottomRight;
            pentatPoints[3] = bottomLeft;
            pentatPoints[4] = topLeft;

            gfx.DrawPolygon(XBrushes.DimGray, pentatPoints, XFillMode.Winding);

            XPen scorePen = XPens.Yellow;
            XPen gaugePen = XPens.Green;
            XPen perimeterPen = XPens.LightGray;
       
            //center out
            gfx.DrawLine(gaugePen, center, top);
            gfx.DrawLine(gaugePen, center, bottomLeft);
            gfx.DrawLine(gaugePen, center, bottomRight);
            gfx.DrawLine(gaugePen, center, topLeft);
            gfx.DrawLine(gaugePen, center, topRight);

            XBrush brush = XBrushes.Black;
            XSize size = new XSize(50, 50);
            XSize ellipseSize = new XSize(10, 10);
            //Info Boxes
            XImage image = XImage.FromFile(@"C:\Users\kevin\Desktop\PDFsharp\samples\Samples C#\Based on WPF\HelloWorld\Content\Left Knee Stability.png");

            DrawPentaInfoBox(gfx, top + new XPoint(-50, -75), image, userParameters["Tibia / Spine Angle"]);
            DrawPentaInfoBox(gfx, topLeft + new XPoint(-100,-35), image, userParameters["LEFT Knee Stability"]);
            DrawPentaInfoBox(gfx, topRight + new XPoint(25, -35), image, userParameters["RIGHT Knee Stability"]);
            DrawPentaInfoBox(gfx, bottomRight + new XPoint(25, -60), image, userParameters["Pelvic Stability"]);
            DrawPentaInfoBox(gfx, bottomLeft + new XPoint(-125, -60), image, userParameters["Depth of Squat"]);

            //percentage Lines
            gfx.DrawString(0 + "%", new XFont("Arial", 10), XBrushes.Black, center);
            for (int i = 10; i > 0; i--)
            {
                float increment = -i * 0.1f;

                gfx.DrawString((i * 10).ToString() + "%", new XFont("Arial", 8), XBrushes.Black, Interpolate(center, top, increment));

                gfx.DrawLine(perimeterPen, Interpolate(center,topLeft, increment), Interpolate(center, top, increment));
                gfx.DrawLine(perimeterPen, Interpolate(center, top, increment), Interpolate(center, topRight, increment));
                gfx.DrawLine(perimeterPen, Interpolate(center, topRight, increment), Interpolate(center, bottomRight, increment));
                gfx.DrawLine(perimeterPen, Interpolate(center, bottomRight, increment), Interpolate(center, bottomLeft, increment));
                gfx.DrawLine(perimeterPen, Interpolate(center, bottomLeft, increment), Interpolate(center, topLeft, increment));
            }


            gfx.DrawLine(scorePen, center, Interpolate(center, top, -0.5f));
        }

        void DrawPentaInfoBox(XGraphics gfx, XPoint point, XImage image, Parameter parameter)
        {
            double val = Convert.ToDouble(parameter.Value);
            string str = parameter.Name;
            XBrush brush = ChooseBrushColor(Convert.ToDouble(parameter.Percentage),
                Convert.ToDouble(parameter.RedVal),
                Convert.ToDouble(parameter.AmberVal));
            XSize ellipseSize = new XSize(10, 10);

            gfx.DrawRoundedRectangle(brush, new XRect(point.X, point.Y, 50, 50), ellipseSize);
            gfx.DrawString(str, new XFont("Arial", 12), XBrushes.White, point + new XPoint(0,60));
            gfx.DrawString(val.ToString() + "%", new XFont("Arial", 12), XBrushes.White, point + new XPoint(10,30));
            gfx.DrawImage(image, new XRect(point + new XPoint(50,0),new XSize(50,50)));
        }

        XPoint Interpolate(XPoint pt1, XPoint pt2, float amount)
        {
            return pt1 + amount * (pt1 - pt2); 
        }

        double pointLength(XPoint p1)
        {
            return Math.Sqrt((p1.X * p1.X) + (p1.Y * p1.Y));
        }

        double Distance(XPoint p1, XPoint p2)
        {
            double exes = p1.X - p2.X;
            double whys = p1.Y - p2.Y;
            return Math.Sqrt((exes * exes) + (whys * whys));
        }

        XBrush ChooseBrushColor(double value, double redVal, double amberVal)
        {
            if (value <= redVal)
                return XBrushes.Red;
            else if (value > redVal && value <= amberVal)
                return XBrushes.Gold;

            return XBrushes.Green;

        }
    }
}
