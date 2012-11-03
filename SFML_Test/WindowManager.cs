using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;
using System.Runtime.InteropServices;

using Tao.OpenGl;

namespace SFML_Test
{
    public class WindowManager
    {
        [DllImport("KERNEL32")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        /// <summary>
        /// Gets the instance of the SFML Window
        /// </summary>
        public RenderWindow window { get; set; }

        private long _timerFreq;
        private long _startTime, _endTime;
        private int _fps;
        private float _deltatime;

        const int pcount = 10000;

        Text mytext, mytextShadow;

        CircleShape[] particles;
        CircleShape circletest, circletest2;

        Simulation sim;

        public WindowManager()
        {
            //Init main window shit
            window = new RenderWindow(new VideoMode(640, 480), "Super cool window title!");
            window.SetVerticalSyncEnabled(true);
            window.SetActive(true);
            window.Closed += window_Closed;

            // Open gl
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glLoadIdentity();
            Gl.glOrtho(0, 640, 480, 0, 0, 1);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);

            Gl.glDisable(Gl.GL_DEPTH_TEST);

            Gl.glLoadIdentity();
            Gl.glTranslatef(0.375f, 0.375f, 0);

            particles = new CircleShape[pcount];
            for (int i = 0; i < pcount; i++)
            {
                particles[i] = new CircleShape(4f);
                particles[i].FillColor = Color.White;
                particles[i].Position = new Vector2f(0, 0);
                particles[i].Radius = 4;
            }

            circletest = new CircleShape();
            circletest.Position = new Vector2f(100, 100);
            circletest.OutlineColor = Color.Red;
            circletest.OutlineThickness = 1;
            circletest.FillColor = Color.Black;
            circletest.Radius = 100;

            circletest2 = new CircleShape();
            circletest2.Position = new Vector2f(200, 300);
            circletest2.OutlineColor = Color.Red;
            circletest2.OutlineThickness = 1;
            circletest2.FillColor = Color.Black;
            circletest2.Radius = 100;

            mytext = new Text("test", new Font("arial.ttf"), 16);
            mytext.Position = new Vector2f(4, 4);
            mytext.Color = Color.White;
            mytext.Style = Text.Styles.Bold;

            mytextShadow = new Text("test", new Font("arial.ttf"), 16);
            mytextShadow.Position = new Vector2f(6, 6);
            mytextShadow.Color = Color.Black;

            sim = new Simulation();
            sim.InitSimulation(pcount);
        }

        public void Update()
        {
            sim.UpdateSimulation(1f / 30f);
        }

        public void circle(float x, float y, float r, float s)
        {
            if (s >= 3)
            {
                float i;

                s = (3.14f * 2 / s);

                Gl.glBegin(Gl.GL_LINE_LOOP);
                for (i = 3.14f; i >= -3.14f; i -= s)
                {
                    Gl.glVertex2f(x + (float)Math.Sin(i) * r, y + (float)Math.Cos(i) * r);
                }
                Gl.glEnd();
            }

        }


        /// <summary>
        /// Returns on window closed
        /// </summary>
        public void Draw()
        {
            while (window.IsOpen())
            {
                if (QueryPerformanceFrequency(out _timerFreq))
                {
                    QueryPerformanceCounter(out _startTime);
                    
                    //Update everything
                    Update();

                    //Main drawing loop 
                    Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);

                    window.DispatchEvents();

                    mytext.DisplayedString = "FPS: " + _fps;
                    mytextShadow.DisplayedString = "FPS: " + _fps;

                    window.Draw(circletest);
                    window.Draw(circletest2);

                    for (int i = 0; i < pcount; i++)
                    {
                        //640x480
                        //particles[i].Center = pos;                    
                        particles[i].Position = new Vector2f(sim.aParticles[i].x - 4, sim.aParticles[i].y - 4);
                        circle(particles[i].Position.X - 195, particles[i].Position.Y - 295, 4, 6);
                    }

                    window.Draw(mytextShadow);
                    window.Draw(mytext);

                    window.Display();
                    //End main drawing loop

                    QueryPerformanceCounter(out _endTime);
                    _fps = (int)(_timerFreq / ((_endTime - _startTime)));
                    _deltatime = (_endTime - _startTime) / 100000f;
                }
            }

            return;
        }

        private void window_Closed(object sender, EventArgs e)
        {
            window.Close();
        }
    }
}
