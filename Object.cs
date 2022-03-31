using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;

namespace Lab7_OOP
{
    interface IObject
    {
        public void save(StreamWriter file);
        public void load(StreamReader file);
    }
    public class CObject: IObject
    {
        protected bool highlighted = false;
        protected Color color;
        protected int x, y;
        public CObject() { }
        public CObject(int x, int y, Color color)
        {
            this.x = x;
            this.y = y;
            this.color = color;
        }
        public virtual void draw(PaintEventArgs e)
        {
            e.Graphics.DrawLine(Brush.highlightPen, x, y, x + 1, y + 1);
        }
        public virtual bool mouseClick_on_Object(int x_, int y_) { return false; }
        public virtual void change_highlight()
        {
            if (highlighted)
                highlighted = false;
            else highlighted = true;
        }
        public bool get_highlighted()
        {
            return highlighted;
        }
        public virtual void set_color(Color new_color)
        {
            color = new_color;
        }
        public virtual string classname() { return "Cbject"; }
        public virtual CObject new_obj(int x, int y, Color c)
        {
            return new CObject();
        }
        public virtual void resize(bool inc, int pbW, int pbH,int d) { }
        public virtual int check_move(int move, int pbW, int pbH, int d)
        {
            switch (move)
            {
                case 1: return Math.Min(pbW - x, d);
                case -1: return Math.Min(x, d);
                case 2: return Math.Min(y, d);
                case -2: return Math.Min(pbH - y, d);
                default: return 0;
            }
        }
        public virtual void move(int move, int pbW, int pbH, int d)
        {
            d = check_move(move, pbW, pbH, d);
            if (move != 0)
                switch (move)
                {
                    case 1: { x += d; break; }
                    case -1: { x -= d; break; }
                    case 2: { y -= d; break; }
                    case -2: { y += d; break; }
                    default: break;
                }
        }
        public virtual int check_resize(bool inc, int pbW, int pbH) // возвращает значение, на которое увеличится объект
        {
            return 10;
        }
        public int get_x()
        {
            return x;
        }
        public int get_y()
        {
            return y;
        }
        public void set_x(int new_x)
        {
            x = new_x;
        }
        public void set_y(int new_y)
        {
            y = new_y;
        }
        public void move_x(int dx)
        {
            x += dx;
        }
        public void move_y(int dy)
        {
            y += dy;
        }
        public virtual int get_leftrightX(bool left) { return x; }
        public virtual int get_updownY(bool up) { return y; }
        public virtual void save(StreamWriter file)
        {
            file.WriteLine("x, y:");
            file.WriteLine(x.ToString());
            file.WriteLine(y.ToString());
            //.WriteLine("%s", color);
        }
        public virtual void load(StreamReader file)
        {
            file.ReadLine();
            x = Int32.Parse(file.ReadLine());
            y = Int32.Parse(file.ReadLine());
            //color = file.ReadLine().ToCharArray() as Color;
            //highlighted = Int32.Parse(file.ReadLine());
        }
    }
    public class CRectangle : CObject
    {
        protected int h = 50, w = 70;
        public CRectangle(int x, int y, Color col)
        {
            this.x = x;
            this.y = y;
            color = col;
        }
        public override void draw(PaintEventArgs e)
        {
            Brush.normBrush.Color = color;
            if (highlighted == false)
                e.Graphics.FillRectangle(Brush.normBrush, x - w / 2, y - h / 2, w, h);
            else e.Graphics.FillRectangle(Brush.highlightBrush, x - w / 2, y - h / 2, w, h);
        }
        public override bool mouseClick_on_Object(int x_, int y_)
        {
            if ((x + w / 2) >= x_ && (x - w / 2) <= x_ && (y + h / 2) >= y_ && (y - h / 2) <= y_)
                return true;
            else return false;
        }
        public override string classname() { return "CRectangle"; }
        public override CObject new_obj(int x, int y, Color color)
        {
            return new CRectangle(x, y, color);
        }
        public override int check_move(int move, int pbW, int pbH, int d)
        {
            switch (move)
            {
                case 1:
                    {
                        if (x + w / 2 + d > pbW)
                            return pbW - (x + w / 2);
                        else return d;
                    }
                case -1:
                    {
                        if (x - w / 2 - d < 0)
                            return x - w / 2;
                        else return d;
                    }
                case 2:
                    {
                        if (y - h / 2 - d < 0)
                            return y - h / 2;
                        else return d;
                    }
                case -2:
                    {
                        if (y + h / 2 + d > pbH)
                            return pbH - (y + h / 2);
                        else return d;
                    }
                default: return 0;
            }
        }
        public override int check_resize(bool inc, int pbW, int pbH)
        {
            int i = 10;
            if (inc == true)
            {
                if (x + w / 2 + 10 > pbW) i = pbW - x - w / 2;
                if (x - w / 2 - 10 < 0 && x - w / 2 < i) i = x - w / 2;
                if (y + h / 2 + 10 > pbH && pbH - y - h / 2 < i) i = pbH - y - h / 2;
                if (y - h / 2 - 10 < 0 && y - h / 2 < i) i = y - h / 2;
            }
            return i;
        }
        public override void resize(bool inc, int pbW, int pbH, int d)
        {
            if (inc == true)
            {
                int delt_size = check_resize(inc, pbW, pbH);
                delt_size = Math.Min(delt_size, d);
                if (w != h)
                    h += (int)((float)delt_size * h / ((float)w));
                else h += delt_size;
                w += delt_size;
            }
            else if (h > 20 && w > 20)
            {
                if (w != h)
                {
                    w -= 10; h = h - (int)((float)10 * h / ((float)w));
                }
                else { w -= 10; h -= 10; }
            }
        }
        public override int get_leftrightX(bool left) {
            if (left == true)
                return x - w / 2;
            else return x + w / 2;
        }
        public override int get_updownY(bool up) {
            if (up == true)
                return y - h / 2;
            else return y + h / 2;
        }
        public override void save(StreamWriter file)
        {
            file.WriteLine('R');
            base.save(file);
            file.WriteLine("width, hight:");
            file.WriteLine(w.ToString());
            file.WriteLine(h.ToString());
        }
        public override void load(StreamReader file)
        {
            base.load(file);
            file.ReadLine();
            w = Int32.Parse(file.ReadLine());
            h = Int32.Parse(file.ReadLine());
        }
    }
    public class CSquare : CRectangle
    {
        public CSquare(int x, int y, Color col) : base(x, y, col)
        {
            w = 50;
        }
        public override string classname() { return "CSquare"; }
        public override CObject new_obj(int x, int y, Color color)
        {
            return new CSquare(x, y, color);
        }
        public override void save(StreamWriter file)
        {
            file.WriteLine('S');
            file.WriteLine("x, y:");
            file.WriteLine(x.ToString());
            file.WriteLine(y.ToString());
            file.WriteLine("length:");
            file.WriteLine(w.ToString());
        }
        public override void load(StreamReader file)
        {
            file.ReadLine();
            x = Int32.Parse(file.ReadLine());
            y = Int32.Parse(file.ReadLine());
            file.ReadLine();
            w = Int32.Parse(file.ReadLine());
            h = w;
        }
    }
    public class CEllipse : CRectangle
    {
        public CEllipse(int x, int y, Color color) : base(x, y, color) { }
        public override void draw(PaintEventArgs e)
        {
            Brush.normBrush.Color = color;
            if (highlighted == false)
                e.Graphics.FillEllipse(Brush.normBrush, x - w / 2, y - h / 2, w, h);
            else e.Graphics.FillEllipse(Brush.highlightBrush, x - w / 2, y - h / 2, w, h);
        }
        public override bool mouseClick_on_Object(int x_, int y_)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(x-w/2, y-h/2, w, h);
            Region rgn = new Region(path);
            if (rgn.IsVisible(x_, y_)) 
                return true;
            else return false;
        }
        public override string classname() { return "CEllipse"; }
        public override CObject new_obj(int x, int y, Color color)
        {
            return new CEllipse(x, y, color);
        }
        public override void save(StreamWriter file)
        {
            file.WriteLine('E');
            file.WriteLine("x, y:");
            file.WriteLine(x.ToString());
            file.WriteLine(y.ToString());
            file.WriteLine("width, hight:");
            file.WriteLine(w.ToString());
            file.WriteLine(h.ToString());
        }
        public override void load(StreamReader file)
        {
            file.ReadLine();
            x = Int32.Parse(file.ReadLine());
            y = Int32.Parse(file.ReadLine());
            file.ReadLine();
            w = Int32.Parse(file.ReadLine());
            h = Int32.Parse(file.ReadLine());
        }
    }
    public class CCircle : CEllipse
    {
        public CCircle(int x, int y, Color color) : base(x, y, color)
        {
            w = 50;
        }
        public override string classname() { return "CCircle"; }
        public override CObject new_obj(int x, int y, Color color)
        {
            return new CCircle(x, y, color);
        }
    };
    public class CTriangle : CSquare
    {
        public CTriangle(int x, int y, Color color) : base(x, y, color) { }

        public override void draw(PaintEventArgs e)
        {
            Brush.normBrush.Color = color;
            Point[] arrPoints = { new Point(x, y - w / 2), new Point(x + w / 2, y + w / 2), new Point(x - w / 2, y + w / 2) };
            if (highlighted == false)
                e.Graphics.FillPolygon(Brush.normBrush, arrPoints);
            else e.Graphics.FillPolygon(Brush.highlightBrush, arrPoints);
        }
        public override bool mouseClick_on_Object(int x_, int y_)
        {
            int dy = y_ - y;
            int dx = x_ - x;
            if (dy <= w / 2 && dy >= 2 * dx - w / 2 && dy >= -2 * dx - w / 2)
                return true;
            else return false;
        }
        public override string classname() { return "CTriangle"; }
        public override CObject new_obj(int x, int y, Color color)
        {
            return new CTriangle(x, y, color);
        }
    };
    public class CRhomb : CRectangle
    {
        public CRhomb(int x, int y, Color color) : base(x, y, color)
        {
            h = 70; w = 35;
        }
        public override void draw(PaintEventArgs e)
        {
            Brush.normBrush.Color = color;
            Point[] arrPoints = { new Point(x - w / 2, y), new Point(x, y - h / 2), new Point(x + w / 2, y), new Point(x, y + h / 2) };
            if (highlighted == false)
                e.Graphics.FillPolygon(Brush.normBrush, arrPoints);
            else e.Graphics.FillPolygon(Brush.highlightBrush, arrPoints);
        }
        public override bool mouseClick_on_Object(int x_, int y_)
        {
            int dy = y_ - y;
            int dx = x_ - x;
            if (dy >= 2 * dx - h / 2 && dy >= -2 * dx - h / 2 && dy <= 2 * dx + h / 2 && dy <= -2 * dx + h / 2)
                return true;
            else return false;
        }
        public override void resize(bool inc, int pbW, int pbH, int d)
        {
            if (inc == true)
            {
                int delt_size = check_resize(inc, pbW, pbH);
                //delt_size = Math.Min(delt_size, d);
                if (w != h)
                    w += delt_size / 2;
                else w += delt_size;
                h += delt_size;
            }
            else if (h > 40)
            {
                w -= 5; h -= 10;
            }
        }
        public override string classname() { return "CRhomb"; }
        public override CObject new_obj(int x, int y, Color color)
        {
            return new CRhomb(x, y, color);
        }
    }
    public class CTrapeze : CRectangle
    {
        public CTrapeze(int x, int y, Color color) : base(x, y, color) { }
        Point[] get_arrPoints()
        {
            return new Point[] {
                new Point(x - w / 2, y + h / 2), new Point(x - w / 4, y - h / 2),
                new Point(x + w / 4, y - h / 2), new Point(x + w / 2, y + h / 2) };
        }
        public override void draw(PaintEventArgs e)
        {
            Brush.normBrush.Color = color;
            if (highlighted == false)
            {
                e.Graphics.FillPolygon(Brush.normBrush, get_arrPoints());
            }
            else
            {
                e.Graphics.FillPolygon(Brush.highlightBrush, get_arrPoints());
            }
        }
        public override bool mouseClick_on_Object(int x_, int y_)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddPolygon(get_arrPoints());
            Region rgn = new Region(path);
            return (rgn.IsVisible(x_, y_) == true);
        }
        public override string classname() { return "CTrapeze"; }
        public override CObject new_obj(int x, int y, Color color)
        {
            return new CTrapeze(x, y, color);
        }
    }
    public class CLine : CObject
    {
        private CObject Point1;
        public CLine(CObject point_st, int x, int y, Color color) : base(x, y, color)
        {
            Point1 = point_st;
            Point1.set_color(color);
        }
        public override string classname() { return "CLine"; }
        public override void draw(PaintEventArgs e)
        {
            Brush.normPen.Color = color;
            if (highlighted == false)
                e.Graphics.DrawLine(Brush.normPen, Point1.get_x(), Point1.get_y(), x, y);
            else e.Graphics.DrawLine(Brush.highlightPen, Point1.get_x(), Point1.get_y(), x, y);
        }
        public override bool mouseClick_on_Object(int x_, int y_)
        {
            int x1 = Point1.get_x();
            int y1 = Point1.get_y();
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddPolygon(new Point[] { new Point(x1 + 2, y1 - 2), new Point(x1 - 2, y1 + 2), new Point(x - 2, y + 2), new Point(x + 2, y - 2) });
            Region rgn = new Region(path);
            return (rgn.IsVisible(x_, y_) == true);
        }
        public override void move(int move, int pbW, int pbH, int dd)
        {
            int d = check_move(move, pbW, pbH, 10);
            int d1 = Point1.check_move(move, pbW, pbH, 10);
            if (d >= 0 && d1 >= 0)
            {
                int d_ = Math.Min(d, d1);
                d_ = Math.Min(d_, dd);
                switch (move)
                {
                    case 1: { x += d_; Point1.move_x(d_); break; }
                    case -1: { x -= d_; Point1.move_x(-d_); break; }
                    case 2: { y -= d_; Point1.move_y(-d_); break; }
                    case -2: { y += d_; Point1.move_y(d_); break; }
                    default: break;
                }
            }
        }
        public override int check_resize(bool inc, int pbW, int pbH)
        {
            if (x == 0 || Point1.get_x() == 0 || x == pbW || Point1.get_x() == pbW
                || y == 0 || Point1.get_y() == 0 || y == pbH || Point1.get_y() == pbH)
                return 0;
            else return 1;
        }
        public override void resize(bool inc, int pbW, int pbH, int dd)
        {
            // для простоты представляем отрезок диагональю прямоугольника, который хотим увеличить
            // его параметры:
            int w = Math.Abs(x - Point1.get_x());
            int h = Math.Abs(y - Point1.get_y());
            int x0 = Math.Min(x, Point1.get_x());
            int y0 = Math.Min(y, Point1.get_y());
            Rectangle rect = new Rectangle(x0, y0, w, h);
            // пропорционально "увеличиваем" или "уменьшаем" прямоугольник
            Size inflateSize = new Size();
            if (inc == true && check_resize(inc, pbW, pbH) == 1) // увеличение
            {
                int d; // на сколько увеличим по x
                // находим допустимое увеличение по x и по y
                //int dx = 40;
                int dx = dd;
                if (x0 + w + dx > pbW) dx = pbW - x0 - w;
                if (x0 - dx < 0) dx = x0;
                //int dy = 40; 
                int dy = dd;
                if (y0 + h + dy > pbH) dy = pbH - y0 - h;
                if (y0 - h < 0) dy = y0;
                d = Math.Min(dx, dy);
                // увеличиваем прям-к
                if (h < w){inflateSize = new Size(d, d * h/w);}
                else {inflateSize = new Size(d * w/h, d); }
                }
            else if (inc == false) // уменьшение
            {
                if (Math.Max(h,w)>45)
                    if (h < w ) { inflateSize = new Size(-dd, -dd * h / w); }
                    else { inflateSize = new Size(-dd * w / h, -dd); }
            }
            rect.Inflate(inflateSize);
            // присваиваем новые значения координатам нашего отрезка
            if (x == x0) { x = Math.Max(0, rect.X); Point1.set_x(Math.Min(pbW, rect.Right)); }
            else { x = Math.Min(pbW, rect.Right); Point1.set_x(Math.Max(0, rect.X)); }
            if (y == y0) { y = Math.Max(0, rect.Y); Point1.set_y(Math.Min(pbH, rect.Bottom)); }
            else { y = Math.Min(pbH, rect.Bottom); Point1.set_y(Math.Max(0, rect.Y)); }
        }
        public override int get_leftrightX(bool left)
        {
            if (left == true)
                return Math.Min(x, Point1.get_x());
            else return Math.Max(x, Point1.get_x());
        }
        public override int get_updownY(bool up)
        {
            if (up == true)
                return Math.Min(y, Point1.get_y());
            else return Math.Max(y, Point1.get_y());
        }
        public override void save(StreamWriter file)
        {
            base.save(file);
            file.WriteLine("x1, y1:");
            file.WriteLine(Point1.get_x().ToString(), "\n", Point1.get_y().ToString());
        }
        public override void load(StreamReader file)
        {
            base.load(file);
            file.ReadLine();
            Point1.set_x(Int32.Parse(file.ReadLine()));
            Point1.set_y(Int32.Parse(file.ReadLine()));
        }
    }
    public class CPolygon : CSquare
    {
        public CPolygon(int x, int y, Color color) : base(x, y, color) { }
        Point[] get_arrPoints()
        {
            return new Point[]{
                new Point(x - w / 2, y), new Point(x - w / 4, y - h / 2), new Point(x + w / 4, y - h / 2),
                new Point(x + w / 2, y), new Point(x + w / 4, y + h / 2), new Point(x - w / 4, y + h / 2)};
        }
        public override void draw(PaintEventArgs e)
        {
            Brush.normBrush.Color = color;
            if (highlighted == false)
                e.Graphics.FillPolygon(Brush.normBrush, get_arrPoints());
            else e.Graphics.FillPolygon(Brush.highlightBrush, get_arrPoints());
        }
        public override bool mouseClick_on_Object(int x_, int y_)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddPolygon(get_arrPoints());
            Region rgn = new Region(path);
            if (rgn.IsVisible(x_, y_))
                return true;
            else return false;
        }
        public override string classname() { return "CPolygon"; }
        public override CObject new_obj(int x, int y, Color color)
        {
            return new CPolygon(x, y, color);
        }
    }
    public class CGroup: CRectangle
    {
        int max_count;
        int count;
        CObject[] objects;
        public CGroup():base(0,0,Color.Black)
        {
            w = 0;
            h = 0;
            max_count = 1;
            count = 0;
            objects = new CObject[max_count];
            for (int i = 0; i < max_count; ++i)
                objects[i] = null;
        }
        public void addObject(CObject new_obj)
        {
            if (count == max_count)
            {
                max_count = max_count * 2;
                CObject[] new_objects = new CObject[max_count];
                for (int i = 0; i < count; ++i)
                    new_objects[i] = objects[i];
                objects = new_objects;
            }
            objects[count] = new_obj;
            count++;
        }
        private Rectangle get_rect()
        {
            if (objects[0] != null)
            {
                // находим самые крайние координаты - углы прямоугольника-рамки
                int x0 = objects[0].get_leftrightX(true), y0 = objects[0].get_updownY(true);
                int x1 = 0, y1 = 0;
                for (int i = 0; i < count; ++i)
                {
                    int tmp_xy = objects[i].get_leftrightX(true);
                    if (tmp_xy < x0) x0 = tmp_xy;
                    tmp_xy = objects[i].get_leftrightX(false);
                    if (tmp_xy > x1) x1 = tmp_xy;
                    tmp_xy = objects[i].get_updownY(true);
                    if (tmp_xy < y0) y0 = tmp_xy;
                    tmp_xy = objects[i].get_updownY(false);
                    if (tmp_xy > y1) y1 = tmp_xy;
                }
                // обновляем значения свойств x,y,w,h
                w = x1 - x0; h = y1 - y0;
                x = x0 + w / 2; y = y0 + h / 2;

                return new Rectangle(x0, y0, w, h);
            }
            else return default;
        }
        public override void move(int move, int pbW, int pbH, int d)
        {
            // вычисляем возможную для всей группы велечину перемещения
            d = check_move(move, pbW, pbH, d);
            for (int i = 0; i < count; ++i)
            {
                objects[i].move(move, pbW, pbH, d);
            }
        }
        public override void draw(PaintEventArgs e)
        {
            // рисуем объекты внутри группы
            for (int i = 0; i < count; ++i)
                objects[i].draw(e);
            // рисуем рамку
            Rectangle rect = get_rect();
            Brush.normPen.Color = color;
            if (highlighted == false)
                e.Graphics.DrawRectangle(Brush.normPen, rect);
            else e.Graphics.DrawRectangle(Brush.highlightPen, rect);
        }
        public override void resize(bool inc, int pbW, int pbH, int d)
        {   // находим возможную для всей группы величину увел/умен
            int tmp_d;
            for (int i = 0; i < count; ++i)
            {
                tmp_d = objects[i].check_resize(inc, pbW, pbH);
                d = Math.Min(tmp_d, d);
            }
            // поочередно изменяем размер каждого объекта внутри группы
            for (int i = 0; i < count; ++i)
                objects[i].resize(inc, pbW, pbH, d);
        }
        public override void change_highlight()
        {
            base.change_highlight();
            for (int i = 0; i < count; ++i)
                objects[i].change_highlight();
        }
        public override void set_color(Color new_color)
        {
            base.set_color(new_color);
            for (int i = 0; i < count; i++)
                objects[i].set_color(new_color);
        }
        public int get_count()
        {
            return count;
        }
        public CObject get_el(int ind)
        {
            if (ind < count)
                return objects[ind];
            else return null;
        }
        public override string classname() { return "CGroup"; }
    }
}