using System;
using System.Drawing;
using System.Windows.Forms;

namespace BookFlip
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}
		Point creaseOrigin, bottomRight;
		int pageWidth, pageHeight;
		bool isBendingPaper = false;

		private void panel1_Resize(object sender, EventArgs e)
		{
			Panel mainPanel = (Panel)sender;
			redrawPanel(mainPanel);
		}

		private void redrawPanel(Panel mainPanel)
		{
			pageWidth = mainPanel.Width / 2;
			pageHeight = mainPanel.Height;
			bottomRight = new Point(mainPanel.Width, pageHeight);
			creaseOrigin = new Point(pageWidth, pageHeight);

			drawPages(mainPanel);
		}

		private void panel1_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				isBendingPaper = true;
		}

		private void panel1_MouseMove(object sender, MouseEventArgs e)
		{
			if (isBendingPaper && e.Button == MouseButtons.Left)
			{
				Point mousePoint = new Point(e.X, e.Y);
				double distance = getDistance(mousePoint, creaseOrigin);
				if (distance > pageWidth)
					mousePoint = getClosestPointOnCircle(mousePoint);

				Point midpoint = getMidpoint(mousePoint, bottomRight);

				Panel mainPanel = (Panel)sender;
				drawPages(mainPanel);

				Graphics g = mainPanel.CreateGraphics();

				double m = -(double)(mousePoint.X - bottomRight.X) / (mousePoint.Y - bottomRight.Y);
				double b = -((m * midpoint.X) - midpoint.Y);
				double x = (pageHeight - b) / m;

				if (double.IsInfinity(m) || double.IsInfinity(b) || double.IsNaN(m) || double.IsNaN(b))
					return;

				int yForEdgePoint = (int)(m * (pageWidth * 2) + b);
				
				Point bottomPoint = new Point((int)x, pageHeight);
				Point rightEdgePoint = new Point(pageWidth * 2, yForEdgePoint);

				g.DrawLine(new Pen(Color.DarkRed), rightEdgePoint, bottomPoint);
				g.DrawLine(new Pen(Color.DarkRed), mousePoint, bottomPoint);
				g.DrawLine(new Pen(Color.DarkRed), mousePoint, rightEdgePoint);

				g.DrawLine(new Pen(Color.DarkGreen), mousePoint, bottomRight);
			}
		}

		private void panel1_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				isBendingPaper = false;

				Panel mainPanel = (Panel)sender;
				drawPages(mainPanel);
			}
		}

		private void drawPages(Panel mainPanel)
		{
			Graphics g = mainPanel.CreateGraphics();
			g.Clear(mainPanel.BackColor);
			g.FillRectangle(new SolidBrush(Color.LightBlue), new Rectangle(new Point(0, 0), new Size(pageWidth, pageHeight)));
			g.FillRectangle(new SolidBrush(Color.LightSkyBlue), new Rectangle(new Point(pageWidth, 0), new Size(pageWidth, pageHeight)));

			//g.DrawEllipse(new Pen(Color.DarkOliveGreen), 0, pageHeight - pageWidth, pageWidth * 2, pageWidth * 2);
		}

		private Point getMidpoint(Point p1, Point p2)
		{
			int x = (p1.X + p2.X) / 2;
			int y = (p1.Y + p2.Y) / 2;
			return new Point(x, y);
		}

		private double getDistance(Point p1, Point p2)
		{
			double x = Math.Pow(p2.X - p1.X, 2);
			double y = Math.Pow(p2.Y - p1.Y, 2);
			return Math.Sqrt(x + y);
		}

		private Point getClosestPointOnCircle(Point p)
		{
			// http://stackoverflow.com/questions/300871/best-way-to-find-a-point-on-a-circle-closest-to-a-given-point
			double vX = p.X - creaseOrigin.X;
			double vY = p.Y - creaseOrigin.Y;
			double magV = Math.Sqrt(vX * vX + vY * vY);
			double aX = creaseOrigin.X + vX / magV * pageWidth;
			double aY = creaseOrigin.Y + vY / magV * pageWidth;

			return new Point((int)aX, (int)aY);
		}

		private void Form1_Shown(object sender, EventArgs e)
		{
			redrawPanel(panel1);
		}
	}
}
