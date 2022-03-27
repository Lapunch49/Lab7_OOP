﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Drawing.Drawing2D;

namespace Lab7_OOP
{
    public partial class Form1 : Form
    {
        public bool ctrlPress = false; // для выделения нескольких объектов
        public bool shiftPress = false; // для увеличения размера (shift +)
        static Color c = Color.White;
        Storage storObj = new Storage(10);
        CObject[] ObjList =
            {new CCircle(0,0,c),
            new CTriangle(0,0,c),
            new CRectangle(0,0, c),
            new CSquare(0,0, c),
            new CEllipse(0,0,c),
            new CRhomb(0,0,c),
            new CTrapeze(0,0,c),
            new CPolygon(0,0,c)
        };
        string cur_select = "CCircle"; // текущий выбор фигуры, которая будет создаваться при нажатии на пустое место 
        CObject line_st = null; // точка - начало отрезка
        public int mouseX = 0;
        public int mouseY = 0;
        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            // меняю оформление концов pen для рисовании отрезков
            Brush.normPen.EndCap = LineCap.RoundAnchor;
            Brush.normPen.StartCap = LineCap.RoundAnchor;
            Brush.highlightPen.EndCap = LineCap.RoundAnchor;
            Brush.highlightPen.StartCap = LineCap.RoundAnchor;
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // удаление объектов
            if (e.KeyCode == Keys.Delete) // выделенные объекты удалятся из хранилища, и произойдет перерисовка
            {
                for (int i = 0; i < storObj.get_count(); ++i)
                {
                    if (storObj.get_el(i).get_highlighted() == true)
                    {
                        storObj.del(i);
                        i--;
                    }
                }
            }

            // увеличение-уменьшение размера объектов 
            bool changeSize = false;
            bool size_delt = false;
            // увеличение размера объектов 
            if (e.KeyCode == Keys.Oemplus && shiftPress == true)
            {
                changeSize = true;
                size_delt = true;
            }
            // уменьшение размера объектов
            if (e.KeyCode == Keys.OemMinus)
                changeSize = true;
            // применяем изменения к объектам
            if (changeSize == true)
                for (int i = 0; i < storObj.get_count(); ++i)
                    if (storObj.get_el(i).get_highlighted() == true)
                        storObj.get_el(i).resize(size_delt, pictureBox1.Width, pictureBox1.Height,10);

            // передвижение объектов вправо-влево-вверх-вниз
            int move = 0;
            switch (e.KeyCode)
            {
                case Keys.Left: { move = -1; break; }
                case Keys.Right: { move = 1; break; }
                case Keys.Up: { move = 2; break; }
                case Keys.Down: { move = -2; break; }
                default: break;
            }
            if (move != 0)
            {
                for (int i = 0; i < storObj.get_count(); ++i)
                    if (storObj.get_el(i).get_highlighted() == true)
                        storObj.get_el(i).move(move, pictureBox1.Width, pictureBox1.Height, 10);
            }

            pictureBox1.Invalidate();

            ctrlPress = e.Control;
            shiftPress = e.Shift;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            ctrlPress = e.Control;
            shiftPress = e.Shift;
        }
        private void setAllHighlightFalse()
        {
            // в хранилище меняем у выделенных объектов св. выделенности
            for (int i = 0; i < storObj.get_count(); ++i)
                if (storObj.get_el(i).get_highlighted() == true)
                    storObj.get_el(i).change_highlight();
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int ind = -1; // попадание по объекту с индексом ind

                // определяем попадание по существующему объекту 
                for (int i = 0; i < storObj.get_count(); ++i)
                    if (storObj.get_el(i).mouseClick_on_Object(e.X, e.Y))
                        ind = i;

                // не попали по объекту 
                if (ind == -1)
                {
                    // убираем все выделения
                    setAllHighlightFalse();

                    // создаем новый объект
                    //// если нов. об. не линия и не многоугольник
                    //if (cur_select != "CLine" && cur_select != "CPolygon")
                    // если нов. об. не линия
                    if (cur_select != "CLine")
                        {
                        CObject newObj = createObj();
                        newObj = newObj.new_obj(e.X, e.Y, Brush.normBrush.Color);
                        storObj.add(newObj);

                        //считаем, что мы попали по нему
                        ind = storObj.get_count() - 1;
                    }
                    else if (cur_select == "CLine")
                        if (line_st == null)
                        {
                            line_st = new CObject(e.X, e.Y, Brush.normPen.Color);
                        }
                        else
                        {
                            CObject newObj = new CLine(line_st, e.X, e.Y, Brush.normPen.Color);
                            line_st = null;
                            storObj.add(newObj);
                            ind = storObj.get_count() - 1;
                        }
                }
                else
                {
                    // попали по сущ-му объекту
                    // дорисовываем отрезок, если 1 точка отрезка уже есть 
                    if (cur_select == "CLine" && line_st != null)
                    {
                        CObject newObj = new CLine(line_st, e.X, e.Y, Brush.normPen.Color);
                        line_st = null;
                        storObj.add(newObj);
                        ind = storObj.get_count() - 1;
                    }
                    // если не дорисовываем отрезок, проверяем ctrl
                    // если ctrl не зажат - убираем остальные выделения
                    else if (ctrlPress != true)
                    {
                        setAllHighlightFalse();
                    }
                }
                // выделяем объект, по которому попали
                if (ind != -1)
                    storObj.get_el(ind).change_highlight();
                pictureBox1.Invalidate();
            }
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < storObj.get_count(); ++i)
                if (storObj.get_el(i) != null)
                    storObj.get_el(i).draw(e);
            // возвращаем цвет кистям
            Brush.normBrush.Color = Brush.Color;
            Brush.normPen.Color = Brush.Color;
            // рисуем начало отрезка, если оно есть 
            if (line_st != null)
                e.Graphics.DrawLine(Brush.normPen, line_st.get_x(), line_st.get_y(), mouseX, mouseY);
            //line_st.draw(e);
        }
        private CObject createObj()
        {
            for (int i = 0; i < ObjList.Length; ++i)
            {
                if (ObjList[i].classname() == cur_select)
                    return ObjList[i];
            }
            return new CCircle(0, 0, c);
        }
        private void btn_color_Click(object sender, EventArgs e)
        {
            Color new_color = ((Button)sender).BackColor;
            // у выделенных объектов меняем цвет
            for (int i = 0; i < storObj.get_count(); ++i)
                if (storObj.get_el(i).get_highlighted() == true)
                    storObj.get_el(i).set_color(new_color);
            // меняем текущий цвет, используемый при рисовании новых фигур
            Brush.Color = new_color;
            Brush.normBrush.Color = new_color;
            Brush.normPen.Color = new_color;
            // для отрезка
            if (line_st != null)
                line_st.set_color(new_color);
        }

        private void btn_other_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                btn_other.BackColor = colorDialog1.Color;
                btn_color_Click(btn_other, e);
            }
        }
        private void btn_shape_Click(object sender, EventArgs e)
        {
            cur_select = ((Button)sender).Name.ToString();
            line_st = null;
        }
        private void btn_clear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < storObj.get_count(); ++i)
            {
                storObj.del(i);
                i--;
            }
            pictureBox1.Invalidate();
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;
            if (cur_select == "CLine" && line_st != null)
                pictureBox1.Invalidate();
        }
        private void btn_Group_Click(object sender, EventArgs e)
        {   // добавляем все выделенные объекты в новую группу,
            // удаляем их из хранилища, группу добавляем в хранилище
            CGroup new_group = new CGroup();
            int i = 0;
            while (i < storObj.get_count())
            {
                if (storObj.get_el(i).get_highlighted() == true)
                {
                    storObj.get_el(i).change_highlight();
                    new_group.addObject(storObj.get_el(i));
                    storObj.del(i);
                }
                else i++;
            }
            storObj.add(new_group);
            // объекты внутри группы и группа(рамка) выделяются
            storObj.get_el(i).change_highlight();
            pictureBox1.Invalidate();
        }
        private void btn_DisGroup_Click(object sender, EventArgs e)
        {   // из всех выделенных групп переносим объекты в хранилище и удаляем группы из хранилища
            // цикл проходит задом наперед на случай, если группа содержала группу, а мы не хотим 
            // разгруппировывать внутр. группу
            for (int i = storObj.get_count()-1; i >=0; --i)
                if (storObj.get_el(i).get_highlighted() == true && storObj.get_el(i).classname() == "CGroup")
                {
                    for (int j = 0; j < (storObj.get_el(i) as CGroup).get_count(); ++j)
                        storObj.add((storObj.get_el(i) as CGroup).get_el(j));
                    storObj.del(i);
                    i++;
                }
            pictureBox1.Invalidate();
        }
    }
    public class Brush
    {
        public static SolidBrush normBrush = new SolidBrush(Color.LightPink);
        public static SolidBrush highlightBrush = new SolidBrush(Color.Red);
        public static Pen normPen = new Pen(Color.LightPink, 3);
        public static Pen highlightPen = new Pen(Color.Red, 4);
        public static Color Color = Color.LightPink;
    }
}