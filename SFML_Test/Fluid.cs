using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SFML_Test
{

    struct GridCell
    {
        public float m;		// mass
        public float vx;		// x-axis velocity
        public float vy;		// y-axis velocity
        public float ax;		// x-axis acceleration
        public float ay;		// y-axis acceleration
    };

    // Cell weights for performing quadratic interpolation
    struct CellWeight
    {
        private float[,] values;

        // Indices into the values array
        private const int wxInd = 0, wyInd = 1, gxInd = 2, gyInd = 3;

        /// <summary>
        /// X-axis weight
        /// </summary>
        public float wx(int i)
        {
            return values[wxInd, i];
        }

        /// <summary>
        /// Y-axis weight
        /// </summary>
        public float wy(int i)
        {
            return values[wyInd, i];
        }

        /// <summary>
        /// X-axis gradient
        /// </summary>
        public float gx(int i)
        {
            return values[gxInd, i];
        }

        /// <summary>
        /// Y-axis gradient
        /// </summary>
        public float gy(int i)
        {
            return values[gyInd, i];
        }

        public float w(int x, int y)
        {
            return values[wxInd, x] * values[wyInd, y];
        }

        public float dx(int x, int y)
        {
            return values[gxInd, x] * values[wyInd, y];
        }

        public float dy(int x, int y)
        {
            return values[wxInd, x] * values[gyInd, y];
        }

        /// <summary>
        /// Initializes the CellWeight values to zero.
        /// </summary>
        public void Init()
        {
            values = new float[4, 3];
        }

        /// <summary>
        /// Sets the cell weights for a particular cell position.
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        public void SetValues(float u, float v)
        {
            // Biquadratic interpolation weights along each axis
            values[wxInd, 0] = 0.5f * u * u + 1.5f * u + 1.125f;
            values[gxInd, 0] = u + 1.5f;
            u++;
            values[wxInd, 1] = -u * u + 0.75f;
            values[gxInd, 1] = -2f * u;
            u++;
            values[wxInd, 2] = 0.5f * u * u - 1.5f * u + 1.125f;
            values[gxInd, 2] = u - 1.5f;

            values[wyInd, 0] = 0.5f * v * v + 1.5f * v + 1.125f;
            values[gyInd, 0] = v + 1.5f;
            v++;
            values[wyInd, 1] = -v * v + 0.75f;
            values[gyInd, 1] = -2f * v;
            v++;
            values[wyInd, 2] = 0.5f * v * v - 1.5f * v + 1.125f;
            values[gyInd, 2] = v - 1.5f;
        }
    };

    class Fluid
    {
        public List<Particle> Particles = new List<Particle>();
        public List<CellWeight> Weights = new List<CellWeight>();

        public float Density;
        public float Stiffness;
        public float Viscosity;
        public GridCell[,] Grid;

        public Fluid(int gwidth, int gheight)
        {
            Grid = new GridCell[gwidth, gheight];

            Density = 1.5f;
            Stiffness = 0.1f;
            Viscosity = 0.1f;
        }

        public void AddParticle(float x, float y, float vx, float vy)
        {
            Particle p;
            p.x = x;
            p.y = y;
            p.vx = vx;
            p.vy = vy;
            p.color = Color.White;
            p.isSensor = false;
            Particles.Add(p);

            CellWeight w = new CellWeight();
            w.Init();
            Weights.Add(w);
        }
    }

    class FluidSim
    {
        DistanceField SDF;
        GridCell[,] Grid;
        public List<Fluid> Fluids = new List<Fluid>();

        Random rand = new Random();

        float GridCoeff;
        float GravityX;
        float GravityY;
        float Scale;

        public int GWidth;
        public int GHeight;

        public FluidSim(int width, int height, float scale)
        {
            Scale = 0.5f;
            GWidth = (int)(width / scale) + 1;
            GHeight = (int)(height / scale) + 1;

            Grid = new GridCell[GHeight, GWidth];

            GridCoeff = 1f;
            GravityX = 0f;
            GravityY = (9.81f / 0.5f) * (1f / 900f);

            SDF = new DistanceField();
            SDF.Create(256, GWidth, GHeight);

            SDF.SubRect(0, 0, 128, 128);
            SDF.AddCircle(64, 128, 10);

            SDF.Blur();
        }

        public void Update()
        {
            // Clear all grid 
            Array.Clear(Grid, 0, Grid.Length);
            for (int i = 0; i < Fluids.Count; i++)
                Array.Clear(Fluids[i].Grid, 0, Fluids[i].Grid.Length);

            // Fill out grid initial grid information
            for (int i = 0, lim = Fluids.Count; i < lim; i++)
            {
                Fluids[i] = InitGrid(Fluids[i]);
            }

            // Average grid velocity
            for (int y = 0, ylim = GHeight; y < ylim; y++)
            {
                for (int x = 0, xlim = GWidth; x < xlim; x++)
                {
                    float m = Grid[y, x].m;
                    if (m == 0f)
                        continue;

                    Grid[y, x].vx /= m;
                    Grid[y, x].vy /= m;
                }
            }

            // Compute particle acceleration and propagate to grid
            for (int i = 0, lim = Fluids.Count; i < lim; i++)
                CalcAccel(Fluids[i]);

            // Average grid acceleration
            for (int y = 0, ylim = GHeight; y < ylim; y++)
            {
                for (int x = 0, xlim = GWidth; x < xlim; x++)
                {
                    //GridCell cell = Grid[y, x];
                    float m = Grid[y, x].m;
                    if (m == 0f)
                        continue;

                    Grid[y, x].ax /= m;
                    Grid[y, x].ay /= m;
                }
            }

            // Update fluid velocity fields
            // Update particle positions
            for (int i = 0, lim = Fluids.Count; i < lim; i++)
            {
                CalcVelocity(Fluids[i]);
                UpdateParticles(Fluids[i]);
            }
        }

        public Fluid InitGrid(Fluid fluid)
        {
            for (int i = 0, lim = fluid.Particles.Count; i < lim; i++)
            {
                Particle p = fluid.Particles[i];

                int cx = Math.Min(GWidth - 3, Math.Max(0, (int)(p.x - 0.5f)));
                int cy = Math.Min(GHeight - 3, Math.Max(0, (int)(p.y - 0.5f)));

                float u = (float)cx - p.x;
                float v = (float)cy - p.y;

                // Biquadratic interpolation weights along each axis
                fluid.Weights[i].SetValues(u, v);

                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        float w = fluid.Weights[i].w(x, y);

                        Grid[cy + y, cx + x].m += w;
                        Grid[cy + y, cx + x].vx += p.vx * w;
                        Grid[cy + y, cx + x].vy += p.vy * w;
                    }
                }
            }

            return fluid;
        }

        private static bool IsBad(float value)
        {
            return float.IsNaN(value) || float.IsInfinity(value);
        }

        void CalcAccel(Fluid fluid)
        {
            for (int i = 0, lim = fluid.Particles.Count; i < lim; i++)
            {
                float fx = fluid.Particles[i].x;
                float fy = fluid.Particles[i].y;
                int cx = Math.Min(GWidth - 3, Math.Max(0, (int)(fx - 0.5f)));
                int cy = Math.Min(GHeight - 3, Math.Max(0, (int)(fy - 0.5f)));

                CellWeight weight = fluid.Weights[i];

                // Determine interpolated mass and velocity derivatives
                float dudx = 0f, dudy = 0f;
                float dvdx = 0f, dvdy = 0f;
                float mass = 10.0f;
                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        float w = weight.w(x, y);
                        float dx = weight.dx(x, y);
                        float dy = weight.dy(x, y);

                        GridCell cell = Grid[cy + y, cx + x];

                        dudx += cell.vx * dx;
                        dudy += cell.vx * dy;
                        dvdx += cell.vy * dx;
                        dvdy += cell.vy * dy;
                        mass += cell.m * w;
                    }
                }

                float pressure = (fluid.Stiffness / Math.Max(1f, fluid.Density)) *
                    (mass - fluid.Density);

                // Add a bit of a pushing force near the collision boundaries
                float ax = 0f, ay = 0f;
                float d = SDF.SampleDistance(fx, fy);
                if (d < 3f)
                {
                    float dirx, diry;
                    SDF.SampleGradient(fx, fy, out dirx, out diry);
                    ax += dirx * (1f - (d / 3f));
                    ay += diry * (1f - (d / 3f));
                }

                // Update grid acceleration values
                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        float w = weight.w(x, y);
                        float dx = weight.dx(x, y);
                        float dy = weight.dy(x, y);

                        Grid[cy + y, cx + x].ax += ax * w - dx * pressure - (dudx * dx + dudy * dy) * fluid.Viscosity * w;
                        Grid[cy + y, cx + x].ay += ay * w - dy * pressure - (dvdx * dx + dvdy * dy) * fluid.Viscosity * w;
                    }
                }
            }
        }

        void CalcVelocity(Fluid fluid)
        {
            for (int i = 0, lim = fluid.Particles.Count; i < lim; i++)
            {
                Particle p = fluid.Particles[i];
                float fx = p.x;
                float fy = p.y;
                int cx = Math.Min(GWidth - 3, Math.Max(0, (int)(fx - 0.5f)));
                int cy = Math.Min(GHeight - 3, Math.Max(0, (int)(fy - 0.5f)));

                // Add grid acceleration to the particle velocities
                CellWeight weight = fluid.Weights[i];
                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        GridCell cell = Grid[cy + y, cx + x];
                        float w = weight.w(x, y);

                        p.vx += w * cell.ax;
                        p.vy += w * cell.ay;
                    }
                }

                p.vx += GravityX;
                p.vy += GravityY;

                // Check new position and push away from distance field boundaries
                float nx = fx + p.vx;
                float ny = fy + p.vy;
                float d = SDF.SampleDistance(nx, ny);
                if (d < 1f)
                {
                    float dirx, diry;
                    SDF.SampleGradient(nx, ny, out dirx, out diry);
                    p.vx += (dirx) * (1f - d) * (float)(1f + rand.NextDouble() * 0.01f);
                    p.vy += (diry) * (1f - d) * (float)(1f + rand.NextDouble() * 0.01f);
                }

                // Update fluid specific velocity grid
                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        float w = weight.w(x, y);

                        fluid.Grid[cy + y, cx + x].m += w;
                        fluid.Grid[cy + y, cx + x].vx += (w * p.vx);
                        fluid.Grid[cy + y, cx + x].vy += (w * p.vy);
                    }
                }

                fluid.Particles[i] = p;
            }

            // Average out the fluid velocity grid
            for (int y = 0, ylim = GHeight; y < ylim; y++)
            {
                for (int x = 0, xlim = GWidth; x < xlim; x++)
                {
                    //GridCell & cell = fluid.Grid[y, x];
                    float m = fluid.Grid[y, x].m;
                    if (m == 0f)
                        continue;
                    fluid.Grid[y, x].vx /= m;
                    fluid.Grid[y, x].vy /= m;
                }
            }
        }

        public void UpdateParticles(Fluid fluid)
        {
            for (int i = 0, lim = fluid.Particles.Count; i < lim; i++)
            {
                Particle p = fluid.Particles[i];

                int cx = Math.Min(GWidth - 3, Math.Max(0, (int)(p.x - 0.5f)));
                int cy = Math.Min(GHeight - 3, Math.Max(0, (int)(p.y - 0.5f)));

                // Get interpolated velocity
                CellWeight weight = fluid.Weights[i];
                float vx = 0f, vy = 0f, tweight = 0f;
                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        GridCell cell = fluid.Grid[cy + y, cx + x];
                        float w = weight.w(x, y);
                        vx += w * cell.vx;
                        vy += w * cell.vy;
                    }
                }

                

                // Update particle position, velocity
                p.x += MathHelper.Clamp(vx, -1, 1);
                p.y += MathHelper.Clamp(vy, -1, 1);

                p.vx += GridCoeff * (vx - p.vx);
                p.vy += GridCoeff * (vy - p.vy);

                // Resolve collisions, clamp positions, update velocities based on this
                float x0 = p.x;
                float y0 = p.y;

                float d = SDF.SampleDistance(x0, y0);
                if (d < 0f)
                {
                    float dx, dy;
                    SDF.SampleGradient(x0, y0, out dx, out dy);
                    x0 -= dx;
                    y0 -= dy;
                }

                x0 = Math.Min(Math.Max(x0, 1f), GWidth - 2f);
                y0 = Math.Min(Math.Max(y0, 1f), GHeight - 2f);
                p.x = x0;
                p.y = y0;

                //FUCK!
                fluid.Particles[i] = p;
            }
        }
    }
}
