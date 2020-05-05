using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace _4Lab
{
    public partial class Form1 : Form
    {
        DirectionEmiter emiter;

        public Form1()
        {
            InitializeComponent();
            picDisplay.Image = new Bitmap(picDisplay.Width, picDisplay.Height);
            emiter = new DirectionEmiter
            {
                ParticlesCount = 500,
                // позиция из центра
                Position = new Point(picDisplay.Width / 2, picDisplay.Height )
            };
        }
        
        

        private void Timer1_Tick(object sender, EventArgs e)
        {
            UpdateState(); // каждый тик обновляем систему

            using (var g = Graphics.FromImage(picDisplay.Image))
            {
                g.Clear(Color.Black);
                Render(g); // рендерим систему
            }

            picDisplay.Invalidate();
           
        }
        private void UpdateState()
        {
            emiter.UpdateState();
        }

        
        // функция рендеринга
        private void Render(Graphics g)
        {
            emiter.Render(g);
        }
    }
}
