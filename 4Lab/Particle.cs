using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace _4Lab
{
    public class Particle
    {

        public int Radius; 
        public float X; 
        public float Y; 

        public float Direction; 
        public float Speed; 
        public float Life;

        public static Random rand = new Random();

        
        public static Particle Generate()
        {
            
            return new Particle
            {
                Direction = rand.Next(360),
                Speed = 1 + rand.Next(10),
                Radius = 2 + rand.Next(50),
                Life = 20 + rand.Next(100),
            };
        }
        public virtual  void Draw(Graphics g)
        {
            
            float k = Math.Min(1f, Life / 100);
            
            int alpha = (int)(k * 255);

           
            var color = Color.FromArgb(alpha, Color.Black);
            var b = new SolidBrush(color);

           
            g.FillEllipse(b, X - Radius, Y - Radius, Radius * 2, Radius * 2);

            b.Dispose();
        }
    }
    public class ParticleImage : Particle
    {
        public Image image;

        public new static ParticleImage Generate()
        {
            return new ParticleImage
            {
                Direction = rand.Next(360),
                Speed = 1 + rand.Next(10),
                Radius = 2 + rand.Next(50),
                Life = rand.Next(100),
            };
        }

        public override void Draw(Graphics g)
        {
            float k = Math.Min(1f, Life / 100);

           
            ColorMatrix matrix = new ColorMatrix(new float[][]{
            new float[] {1F, 0, 0, 0, 0}, 
            new float[] {0, 1F, 0, 0, 0}, 
            new float[] {0, 0, 1F, 0, 0},
            new float[] {0, 0, 0, k, 0}, 
            new float[] {0, 0, 0, 0, 1F}});

          
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(matrix);

            
            g.DrawImage(image,
                
                new Rectangle((int)(X - Radius), (int)(Y - Radius), Radius * 2, Radius * 2),
               
                0, 0, image.Width, image.Height,
                GraphicsUnit.Pixel, 
                imageAttributes
               );
        }
    }
    public abstract class EmiterBase
    {
        List<Particle> particles = new List<Particle>();

      
        int particleCount = 0;
       
        public int ParticlesCount
        {
            get
            {
                return particleCount;
            }
            set
            {
               
                particleCount = value;
                
                if (value < particles.Count)
                {
                    particles.RemoveRange(value, particles.Count - value);
                }
            }
        }

      
        public abstract void ResetParticle(Particle particle);
        public abstract void UpdateParticle(Particle particle);
        public abstract Particle CreateParticle();

        
        public void UpdateState()
        {
            foreach (var particle in particles)
            {
                particle.Life -= 1;
                if (particle.Life < 0)
                {
                    ResetParticle(particle);
                }
                else
                {
                    UpdateParticle(particle);
                }
            }

            for (var i = 0; i < 10; ++i)
            {
                if (particles.Count < 500)
                {
                    particles.Add(CreateParticle());
                }
                else
                {
                    break;
                }
            }
        }

        public void Render(Graphics g)
        {
            foreach (var particle in particles)
            {
                particle.Draw(g);
            }
        }
    }
    public class PointEmiter : EmiterBase
    {
        public Point Position;
        public float Life;

        public override Particle CreateParticle()
        {
            var particle = ParticleImage.Generate();
            particle.image = Properties.Resources.fire;
            particle.Life = Life;
            particle.X = Position.X;
            particle.Y = Position.Y;
            return particle;
        }

        public override void ResetParticle(Particle particle)
        {
            
            particle.Speed = 1 + Particle.rand.Next(10);
            particle.Direction = Particle.rand.Next(360);
            particle.Radius = 2 + Particle.rand.Next(40);
            particle.Life = Life;
            particle.X = Position.X;
            particle.Y = Position.Y;
        }

        public override void UpdateParticle(Particle particle)
        {
            var directionInRadians = particle.Direction / 180 * Math.PI;
            particle.X += (float)(particle.Speed * Math.Cos(directionInRadians));
            particle.Y -= (float)(particle.Speed * Math.Sin(directionInRadians));
        }
    }
    public class DirectionEmiter : PointEmiter
    {
        public int direction = 90; 
        public int Spread = 40;
       


        public override Particle CreateParticle()
        {
            var particle = ParticleImage.Generate();
            particle.image = Properties.Resources.fire;

            particle.Direction = this.direction + Particle.rand.Next(-Spread / 2, Spread / 2);
            particle.Life = Life;
            particle.X = Position.X;
            particle.Y = Position.Y;
            return particle;
        }

        public override void ResetParticle(Particle particle)
        {
            var particler = particle as ParticleImage;
           

            if (particler != null)
            {
                particler.Life = 10 + Particle.rand.Next(30);
                particler.Speed = 1 + Particle.rand.Next(10);

                particle.Life = Life;
                particler.Direction = this.direction + Particle.rand.Next(-Spread / 2, Spread / 2);

                particler.X = Position.X;
                particler.Y = Position.Y;
            }
        }
    }
}

