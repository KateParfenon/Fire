using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace _4Lab
{
    public class Particle
    {

        public int Radius; // радиус частицы
        public float X; // X координата положения частицы в пространстве
        public float Y; // Y координата положения частицы в пространстве

        public float Direction; // направление движения
        public float Speed; // скорость перемещения
        public float Life;

        public static Random rand = new Random();

        // метод генерации частицы
        public static Particle Generate()
        {
            // я не заполняю координаты X, Y потому что хочу, чтобы все частицы возникали из одного места
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
            // рассчитываем коэффициент прозрачности по шкале от 0 до 1.0
            float k = Math.Min(1f, Life / 100);
            // рассчитываем значение альфа канала в шкале от 0 до 255
            // по аналогии с RGB, он используется для задания прозрачности
            int alpha = (int)(k * 255);

            // создаем цвет из уже существующего, но привязываем к нему еще и значение альфа канала
            var color = Color.FromArgb(alpha, Color.Black);
            var b = new SolidBrush(color);

            // остальное все так же
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
                Life = 10 + rand.Next(30),
            };
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(image, X - Radius, Y - Radius, Radius * 2, Radius * 2);
        }
    }
    public abstract class EmiterBase
    {
        List<Particle> particles = new List<Particle>();

        // количество частиц эмитера храним в переменной
        int particleCount = 0;
        // и отдельной свойство которое возвращает количество частиц
        public int ParticlesCount
        {
            get
            {
                return particleCount;
            }
            set
            {
                // при изменении этого значения
                particleCount = value;
                // удаляем лишние частицы если вдруг
                if (value < particles.Count)
                {
                    particles.RemoveRange(value, particles.Count - value);
                }
            }
        }

        // три абстрактных метода мы их переопределим позже
        public abstract void ResetParticle(Particle particle);
        public abstract void UpdateParticle(Particle particle);
        public abstract Particle CreateParticle();

        // тут общая логика обновления состояния эмитера
        // по сути копипаста
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

        public override Particle CreateParticle()
        {
            var particle = ParticleImage.Generate();
            particle.image = Properties.Resources.fire;
            particle.X = Position.X;
            particle.Y = Position.Y;
            return particle;
        }

        public override void ResetParticle(Particle particle)
        {
            particle.Life = 10 + Particle.rand.Next(30);
            particle.Speed = 1 + Particle.rand.Next(10);
            particle.Direction = Particle.rand.Next(360);
            particle.Radius = 2 + Particle.rand.Next(40);
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
        public int direction = 90; // направление частиц
        public int Spread = 40; // разброс частиц
        

        public override Particle CreateParticle()
        {
            var particle = ParticleImage.Generate();
            particle.image = Properties.Resources.fire;

            particle.Direction = this.direction + Particle.rand.Next(-Spread / 2, Spread / 2);

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

               
                particler.Direction = this.direction + Particle.rand.Next(-Spread / 2, Spread / 2);

                particler.X = Position.X;
                particler.Y = Position.Y;
            }
        }
    }
}

