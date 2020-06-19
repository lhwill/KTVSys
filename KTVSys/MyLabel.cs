using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KTVSys
{
    class MyLabel:Control
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams para = base.CreateParams;
                para.ExStyle |= 0x00000020; 
                return para;
            }
        }
        protected override void OnPaintBackground(PaintEventArgs e) //不画背景
        {
            
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawString(Text, Font, System.Drawing.Brushes.White, new System.Drawing.PointF(10, 10)); //自己绘制文本
            base.OnPaint(e);
        }
    }
}
