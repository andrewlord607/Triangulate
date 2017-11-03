using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Geometry;

namespace AffineTransformations
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            EdgeIsBeingDrawn = false;
            PoligonIsBeingDrawn = false;
            EdgeCount = 0;
            PoligonVertexesCount = 0;
            CurrPoligon = new List<Point>();
			CurrPoligon2 = new Polygon();
            PoligonHasDrawn = false;
            EdgeHasDrawn = false;
			PointIsBeingChosen = false;
			EdgesIsBeingCrossed = false;
            Classify = false;
            pointIn = false;
            pointInNotConvex = false;
            b_triangulate = false;
            InitialImg = new Bitmap(730, 510);
            Clear();
        }

        // Координаты текужего отрезка
        private Point EdgeFirst;
        private Point EdgeSecond;

        

        // Флаги выбора
        private bool EdgeIsBeingDrawn;
        private bool PoligonIsBeingDrawn;
        private bool PointIsBeingChosen;
        private bool EdgesIsBeingCrossed;
        private bool pointIn;
        private bool pointInNotConvex;
        private bool Classify;
        private bool b_triangulate;

        // Флаги нарисованных фигур
        private bool PoligonHasDrawn;
        private bool EdgeHasDrawn;

        // Счётчики текущего числа выбранных вершин 
        private int EdgeCount;
        private int PoligonVertexesCount;

        // Текущие рёбра        
		private Edge CurrEdge;
		private Edge CurrEdge2;

        // Точка вращения
        private Point RotatePoint;

        // Текущие полигоны
        private List<Point> CurrPoligon;
		private Polygon CurrPoligon2;
        
		
        private Image InitialImg;
        private Bitmap CurrentImage;

        private void DrawEdge_Click(object sender, EventArgs e)
        {
            Clear();
            pictureBox1.Cursor = Cursors.Cross;
            StatusLabel.Text = "Выберете две точки";
            EdgeIsBeingDrawn = true;
        }

        private void DrawCross(Point center, int wight)
        {
			Graphics gr = pictureBox1.CreateGraphics();
			gr.DrawLine(Pens.Red, new Point(center.X + wight, center.Y), new Point(center.X - wight, center.Y));
            gr.DrawLine(Pens.Red, new Point(center.X, center.Y + wight), new Point(center.X, center.Y - wight));

        }
        private void DrawEdge(Point p)
        {
            if (EdgeCount == 0)
                CurrentImage = new Bitmap(pictureBox1.Image);

            EdgeCount++;
            if (EdgeCount == 2)
            {
                pictureBox1.Image = CurrentImage;
                EdgeSecond = p;
                CurrEdge = new Edge(new MyPoint(EdgeFirst.X, EdgeFirst.Y), new MyPoint(EdgeSecond.X, EdgeSecond.Y));
                Graphics gr = Graphics.FromImage(CurrentImage);
                //Graphics gr = pictureBox1.CreateGraphics();
                gr.DrawLine(Pens.Black, EdgeFirst, EdgeSecond);
                EdgeIsBeingDrawn = false;
                EdgeCount = 0;
                StatusLabel.Text = "Действие не выбрано";
                pictureBox1.Cursor = Cursors.Arrow;
                EdgeHasDrawn = true;
                PoligonHasDrawn = false;
            }
            else
            {
                EdgeFirst = p;
                DrawCross(p, 10);
            }
        }

        private void DrawPoligon(Point p)
        {
            if (PoligonVertexesCount == 0)
            {

                for (int i = 0; i < CurrPoligon2.size(); ++i)
                    CurrPoligon2.Remove();
                CurrPoligon.Clear();
                CurrentImage = new Bitmap(pictureBox1.Image);
            }
            PoligonVertexesCount++;
            if (PoligonVertexesCount == numericUpDown1.Value)
            {
                pictureBox1.Image = CurrentImage;
                CurrPoligon2.insert(new MyPoint(p.X, pictureBox1.Height - p.Y));
                CurrPoligon.Add(p);
                Graphics gr = Graphics.FromImage(CurrentImage);
                gr.DrawLines(Pens.Black, CurrPoligon.ToArray());
                gr.DrawLine(Pens.Black, CurrPoligon.First(), CurrPoligon.Last());
                PoligonIsBeingDrawn = false;
                PoligonVertexesCount = 0;
                StatusLabel.Text = "Действие не выбрано";
                pictureBox1.Cursor = Cursors.Arrow;
                EdgeHasDrawn = false;
                PoligonHasDrawn = true;
            }
            else
            {
                CurrPoligon2.insert(new MyPoint(p.X, pictureBox1.Height - p.Y));
                CurrPoligon.Add(p);
                DrawCross(p, 10);
            }
        }

        private void CrossEdges()
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs mArgs = (MouseEventArgs)e;
            Point p = new Point(mArgs.X, mArgs.Y);
            if (EdgeIsBeingDrawn)
                DrawEdge(p);
            if (PoligonIsBeingDrawn)
                DrawPoligon(p);
            if (PointIsBeingChosen)
            {
                RotatePoint = new Point(mArgs.X, mArgs.Y);
                RotationAroundPoint();
                pictureBox1.Cursor = Cursors.Arrow;
                PointIsBeingChosen = false;
            }
            else if (EdgesIsBeingCrossed)
            {
                if (EdgeCount == 0)
                    CurrentImage = new Bitmap(pictureBox1.Image);
                EdgeCount++;
                if (EdgeCount == 2)
                {
                    pictureBox1.Image = CurrentImage;
                    EdgeSecond = p;
                    CurrEdge2 = new Edge(new MyPoint(EdgeFirst.X, EdgeFirst.Y), new MyPoint(EdgeSecond.X, EdgeSecond.Y));
                    DrawEdge(p);
                    var cp = Edge.crossPoint(CurrEdge, CurrEdge2);
                    if (cp.x != 0 && cp.y != 0)
                    {
                        //DrawCross(new Point((int)cp.x, (int)cp.y), 10);
                        Graphics gr = Graphics.FromImage(pictureBox1.Image);
                        gr.DrawLine(Pens.Red, new Point((int)cp.x + 10, (int)cp.y), new Point((int)cp.x - 10, (int)cp.y));
                        gr.DrawLine(Pens.Red, new Point((int)cp.x, (int)cp.y + 10), new Point((int)cp.x, (int)cp.y - 10));
                    }

                    EdgeIsBeingDrawn = false;
                    EdgeCount = 0;
                    StatusLabel.Text = "Действие не выбрано";
                    pictureBox1.Cursor = Cursors.Arrow;
                    EdgeHasDrawn = true;
                    PoligonHasDrawn = false;
                    EdgesIsBeingCrossed = false;
                }
                else
                {
                    EdgeFirst = p;
                    DrawCross(p, 10);
                }
            }
            else if (pointIn)
            {
                if (PoligonHasDrawn)
                {
                    DrawCross(p, 10);
                    MessageBox.Show(CurrPoligon2.pointInConvexPolygon(new MyPoint(p.X, pictureBox1.Height - p.Y), CurrPoligon2) ? "true" : "false");
                }
                pointIn = false;
            }
            else if (pointInNotConvex)
            {
                if (PoligonHasDrawn)
                {
                    DrawCross(p, 10);
                    MessageBox.Show(CurrPoligon2.pointInPolygon(new MyPoint(p.X, pictureBox1.Height - p.Y), CurrPoligon2) ? "true" : "false");
                }
                pointInNotConvex = false;
            }
            else if(Classify)
            {
                if(EdgeHasDrawn)
                {
                    DrawCross(p, 10);
                    MyPoint temp = new MyPoint(p.X, pictureBox1.Height - p.Y);
                    MyPoint t1 = new MyPoint(CurrEdge.org.x, pictureBox1.Height - CurrEdge.org.y);
                    MyPoint t2 = new MyPoint(CurrEdge.dest.x, pictureBox1.Height - CurrEdge.dest.y);
                    var pos = temp.classify(new Edge(t1, t2));
                    if (pos == Position.LEFT)
                        MessageBox.Show("Слева от отрезка");//Слева от отрезка
                    if (pos == Position.RIGHT)
                        MessageBox.Show("Справа от отрезка");//Справа от отрезка
                    if (pos == Position.BEHIND)
                        MessageBox.Show("На прямой со стороны начала отрезка");
                    if (pos == Position.BEYOND)
                        MessageBox.Show("На прямой со стороны конца отрезка");
                    if (pos == Position.ORIGIN)
                        MessageBox.Show("Совпадает с началом отрезка");
                    if (pos == Position.DESTINATION)
                        MessageBox.Show("Совпадает с концом отрезка");
                    if (pos == Position.BETWEEN)
                        MessageBox.Show("Находится на прямой между началом и концом отрезка");
                }
                Classify = false;
            }
        }

        private void DrawPoligon_Click(object sender, EventArgs e)
        {
            Clear();
            pictureBox1.Cursor = Cursors.Cross;
            StatusLabel.Text = "Выберете вершины многоугольника";
            PoligonIsBeingDrawn = true;
        }
        private void Clear()
        {
            CurrPoligon2 = new Polygon();
            PoligonHasDrawn = false;
            pictureBox1.Image = InitialImg;
        }

        private void ClearForm_Click(object sender, EventArgs e)
        {
            Clear();
        }        

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private void Rotation_Click(object sender, EventArgs e)
        {
            double angle = (double)numericUpDown3.Value;
            double cos = Math.Cos(DegreeToRadian(angle));
            double sin = Math.Sin(DegreeToRadian(angle));
            Matrix m1 = new Matrix((float)cos, (float)sin,
                -(float)sin, (float)cos, 0, 0);
            Graphics gr = pictureBox1.CreateGraphics();
            gr.Transform = m1;
            if (PoligonHasDrawn)
            {
                gr.DrawLines(Pens.Blue, CurrPoligon.ToArray());
                gr.DrawLine(Pens.Blue, CurrPoligon.First(), CurrPoligon.Last());
            }
            if (EdgeHasDrawn)
                gr.DrawLine(Pens.Blue, EdgeFirst, EdgeSecond);
        }

        private void Rotate90Degrees_Click(object sender, EventArgs e)
        {
            if (!EdgeHasDrawn)
                return;
            double angle = 90.0;
            Point a = new Point((EdgeFirst.X + EdgeSecond.X) / 2,
                (EdgeFirst.Y + EdgeSecond.Y) / 2);
            double cos = Math.Cos(DegreeToRadian(angle));
            double sin = Math.Sin(DegreeToRadian(angle));
            Matrix m = new Matrix((float)cos, (float)sin,
                -(float)sin, (float)cos, (float)(-a.X*cos + a.Y*sin + a.X),
                (float)(-a.X * sin - a.Y * cos + a.Y));
            Graphics gr = pictureBox1.CreateGraphics();
            gr.Transform = m;
            gr.DrawLine(Pens.Blue, EdgeFirst, EdgeSecond);
        }

        private void Scaling_Click(object sender, EventArgs e)
        {
            Matrix m = new Matrix((float)numericUpDown4.Value, 0, 0, (float)numericUpDown5.Value, 0, 0);
            Graphics gr = pictureBox1.CreateGraphics();
            gr.Transform = m;
            if (PoligonHasDrawn)
            {
                gr.DrawLines(Pens.Blue, CurrPoligon.ToArray());
                gr.DrawLine(Pens.Blue, CurrPoligon.First(), CurrPoligon.Last());
            }
            if (EdgeHasDrawn)
                gr.DrawLine(Pens.Blue, EdgeFirst, EdgeSecond);
        }

        private void Translation_Click(object sender, EventArgs e)
        {
            Matrix m = new Matrix(1, 0, 0, 1, (float)numericUpDown6.Value, (float)numericUpDown7.Value);
            Graphics gr = pictureBox1.CreateGraphics();
            gr.Transform = m;
            if (PoligonHasDrawn)
            {
                gr.DrawLines(Pens.Blue, CurrPoligon.ToArray());
                gr.DrawLine(Pens.Blue, CurrPoligon.First(), CurrPoligon.Last());
            }
            if (EdgeHasDrawn)
                gr.DrawLine(Pens.Blue, EdgeFirst, EdgeSecond);
        }

       
		private void RotationAroundPoint()
		{
			double angle = (double)numericUpDown2.Value;
			double cos = Math.Cos(DegreeToRadian(angle));
			double sin = Math.Sin(DegreeToRadian(angle));
			Matrix m = new Matrix();
			m.RotateAt((float)angle, RotatePoint, MatrixOrder.Append);				
			Graphics gr = pictureBox1.CreateGraphics();
			gr.Transform = m;
			if (PoligonHasDrawn)
			{
				gr.DrawLines(Pens.Blue, CurrPoligon.ToArray());
				gr.DrawLine(Pens.Blue, CurrPoligon.First(), CurrPoligon.Last());
			}
			if (EdgeHasDrawn)
				gr.DrawLine(Pens.Blue, EdgeFirst, EdgeSecond);
		}

        

		private void button9_Click(object sender, EventArgs e)
		{
			EdgesIsBeingCrossed = true;
			pictureBox1.Cursor = Cursors.Cross;
		}

		private void button10_Click(object sender, EventArgs e)
		{
			pointIn = true;
			pictureBox1.Cursor = Cursors.Cross;
		}

        private void RotationAroundPoint_Click(object sender, EventArgs e)
        {
            PointIsBeingChosen = true;
            pictureBox1.Cursor = Cursors.Cross;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Classify = true;
            pictureBox1.Cursor = Cursors.Cross;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            pointInNotConvex = true;
            pictureBox1.Cursor = Cursors.Cross;
        }

        private bool pointInTriangle(MyPoint p, MyPoint a, MyPoint b, MyPoint c)
        {
            return ((p.classify(a, b) != Position.LEFT) &&
                  (p.classify(b, c) != Position.LEFT) &&
                  (p.classify(c, a) != Position.LEFT));
        }

        private Vertex findIntrudingVertex(Polygon p)
        {
            Vertex a = p.neighbor(Rotation.COUNTER_CLOCKWISE);
            Vertex b = p.v();
            Vertex c = p.advance(Rotation.CLOCKWISE);
            Vertex d = null;     // лучший кандидат на данный момент
            double bestD = -1.0;  // расстояние до лучшего кандидата
            Edge ca = new Edge(c.point(), a.point());
            Vertex v = p.advance(Rotation.CLOCKWISE);
            while (v != a)
            {
                if (pointInTriangle(v, a, b, c))
                {
                    double dist = v.distance(ca);
                    if (dist > bestD)
                    {
                        d = v;
                        bestD = dist;
                    }
                }
                v = p.advance(Rotation.CLOCKWISE);
            }
            p.setV(b);
            return d;
        }

        void findConvexVertex(Polygon p)
        {
            Vertex a = p.neighbor(Rotation.COUNTER_CLOCKWISE);
            Vertex b = p.v();
            Vertex c = p.neighbor(Rotation.CLOCKWISE);
            while (c.classify(a, b) != Position.RIGHT)
            {
                a = b;
                b = p.advance(Rotation.CLOCKWISE);
                c = p.neighbor(Rotation.CLOCKWISE);
            }
        }


        private List<Polygon> triangulate(Polygon p)
        {
            List<Polygon> triangles = new List<Polygon>();
            if (p.size() == 3)
                triangles.Add(p);
            else
            {
                findConvexVertex(p);
                Vertex d = findIntrudingVertex(p);
                if (d == null)
                {        // нет вторгающихся вершин
                    Vertex c = p.neighbor(Rotation.CLOCKWISE);
                    p.advance(Rotation.COUNTER_CLOCKWISE);
                    Polygon q = p.split(c);
                    triangles.AddRange(triangulate(p));
                    triangles.Add(q);
                }
                else
                {                  // d - вторгающаяся вершина
                    Polygon q = p.split(d);
                    triangles.AddRange(triangulate(q));
                    triangles.AddRange(triangulate(p));
                }
            }
            return triangles;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (PoligonHasDrawn)
            {
                pictureBox1.Image = InitialImg;
                CurrentImage = new Bitmap(pictureBox1.Image);
                Graphics gr = Graphics.FromImage(CurrentImage);
                List<Polygon> triangles = triangulate(CurrPoligon2);
                foreach (var triangle in triangles)
                {
                    List<Point> to_draw = new List<Point>();
                    to_draw.Add(new Point((int)triangle.v().point().x, pictureBox1.Height - (int)triangle.v().point().y));
                    Vertex v = triangle.next();
                    
                    for (; v != triangle.v(); v = v.next())
                    {
                        to_draw.Add(new Point((int)v.point().x, pictureBox1.Height - (int)v.point().y));
                    }
                    
                    gr.DrawLines(Pens.Black, to_draw.ToArray());
                    gr.DrawLine(Pens.Black, to_draw.First(), to_draw.Last());
                }
                pictureBox1.Image = CurrentImage;
            }
        }
    }
}
