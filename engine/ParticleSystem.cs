using SFML.Graphics;
using SFML.System;
using SilverRaven.SFML.Tools;
using static SilverRaven.SFML.Engine;

namespace SilverRaven.SFML
{
    public class ParticleSystem : GameObject
    {
        /// <summary>
        /// Particle info used by ParticleSystem
        /// </summary>
        public struct Particle
        {
            public readonly float Progress => 1f - MathTools.Clamp01(lifetime / startLifetime);

            public Vector2f position;
            public float lifetime;

            public Vector2f velocity = new ();
            public Color color = Color.White;

            private float startLifetime;

            public Particle(Vector2f position, float lifetime)
            {
                this.position = position;
                startLifetime = lifetime;
                this.lifetime = startLifetime;
            }
        }

#region Emission

        private enum EmitterShape { Point, Circle, Rect }

        private EmitterShape shape = EmitterShape.Point;
        private Vector2f emitSize = new ();
        private bool emitOverTime = false;
        private (bool random, float min, float max) emitTime = new (false, 1f, 0f);
        private bool emitOverDistance = false;
        private (bool random, float min, float max) emitDistance = new (false, 1f, 0f);

        private float emitTimer;
        private Vector2f emitPoint;
        private float currentEmitDistance;

#endregion

#region Physics

        public enum BurstShape { Angle, Random, Star }

        private BurstShape burstShape = BurstShape.Angle;
        private (int current, int all) starPoints = new (0, 1);
        private (bool random, float min, float max) angle = new (false, 0f, 0f);
        private float startVelocity = 0f;
        private Vector2f acceleration = new (0f, 0f);
        private float drag = 1f;

#endregion

#region Display

        /// <summary>
        /// The shape used to display the particles.
        /// </summary>
        public Shape particleDisplay;
        /// <summary>
        /// The size of the particles.
        /// </summary>
        public Vector2f particleSize = new (1f, 1f);
        /// <summary>
        /// A function to determine the size of the particles over their lifetime.
        /// </summary>
        public Func<float, float> sizeOverLifetime = null;
        /// <summary>
        /// A function to determine the color of the particles over their lifetime.
        /// </summary>
        public Func<float, Color> tintOverLifetime = null;
        /// <summary>
        /// A function to determine the initial color of the particles.
        /// </summary>
        public Func<Color> getParticleColor = null;

#endregion

        private List<Particle> particles;
        private bool isPlaying;
        private (bool random, float min, float max) lifetime;

        public ParticleSystem(bool playOnAwake = false, float? sortOrder = null) : base(sortOrder)
        {
            particles = new ();
            SetLifetime(1f);
            SetEmitTime(1f);
            SetEmitterShapePoint();
            particleDisplay = new CircleShape(5f);

            if (playOnAwake) Play();
        }

#region setup commands

        /// <summary>
        /// Sets the emitter shape to a point.
        /// </summary>
        public void SetEmitterShapePoint() => shape = EmitterShape.Point;

        /// <summary>
        /// Sets the emitter shape to a circle with the specified radius.
        /// </summary>
        /// <param name="radius">The radius of the circle.</param>
        public void SetEmitterShapeCircle(float radius)
        {
            shape = EmitterShape.Circle;
            emitSize.X = radius;
        }
        
        /// <summary>
        /// Sets the emitter shape to a rectangle with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public void SetEmitterShapeRect(float width, float height) => SetEmitterShapeRect(new (width, height));
        
        /// <summary>
        /// Sets the emitter shape to a rectangle with the specified size.
        /// </summary>
        /// <param name="size">The size of the rectangle.</param>
        public void SetEmitterShapeRect(Vector2f size)
        {
            shape = EmitterShape.Rect;
            emitSize = size;
        }

        /// <summary>
        /// Sets the lifetime of the particles.
        /// </summary>
        public void SetLifetime(float lifetime) => this.lifetime = (false, lifetime, 0f);
        /// <summary>
        /// Sets the lifetime of the particles within a range.
        /// </summary>
        /// <param name="lifetimeMin">The minimum lifetime of the particles.</param>
        /// <param name="lifetimeMax">The maximum lifetime of the particles.</param>
        public void SetLifetime(float lifetimeMin, float lifetimeMax) => lifetime = (true, lifetimeMin, lifetimeMax);

        /// <summary>
        /// Sets the time between emissions of the particles.
        /// </summary>
        /// <param name="emitTime">The time between emissions.</param>
        public void SetEmitTime(float emitTime)
        {
            if (emitTime <= 0f) 
            { DisableEmitTime(); return; }

            this.emitTime = (false, emitTime, 0f);
            emitOverTime = true;
        }

        /// <summary>
        /// Sets the time between emissions of the particles within a range.
        /// </summary>
        /// <param name="emitTimeMin">The minimum time between emissions.</param>
        /// <param name="emitTimeMax">The maximum time between emissions.</param>
        public void SetEmitTime(float emitTimeMin, float emitTimeMax)
        {
            emitTime = (true, emitTimeMin, emitTimeMax);
            emitOverTime = true;
        }

        /// <summary>
        /// Disables the time limitation of particle emission.
        /// </summary>
        public void DisableEmitTime() => emitOverTime = false;

        /// <summary>
        /// Sets the minimum-distance limit of emitting a new particle.
        /// </summary>
        /// <param name="distance">The minimum distance traveled before emitting a new particle.</param>
        public void SetEmitDistance(float distance)
        {
            if (distance <= 0f) 
            { DisableEmitDistance(); return; }

            emitOverDistance = true;
            emitDistance = (false, distance, 0f);
        }
        
        /// <summary>
        /// Sets the minimum-distance limit of emitting a new particle within a range.
        /// </summary>
        /// <param name="distanceMin">The minimum range of the distance limit.</param>
        /// <param name="distanceMax">The maximum range of the distance limit.</param>
        public void SetEmitDistance(float distanceMin, float distanceMax)
        {
            emitOverDistance = true;
            emitDistance = (true, distanceMin, distanceMax);
        }

        /// <summary>
        /// Disables the minimum-distance limit of emitting new particles.
        /// </summary>
        public void DisableEmitDistance() => emitOverDistance = false;

        /// <summary>
        /// Sets the initial velocity of the particles.
        /// </summary>
        /// <param name="velocity">The desired initial velocity of the particles.</param>
        public void SetInitialVelocity(Vector2f velocity)
        {
            startVelocity = velocity.Magnitude();
            angle = (false, velocity.Angle(), 0f);
            burstShape = BurstShape.Angle;
        }

        /// <summary>
        /// Sets the physics parameters for the particles.
        /// </summary>
        /// <param name="gravity">The gravity to apply to the particles.</param>
        /// <param name="drag">The drag to apply to the particles.</param>
        public void SetPhysicsParameters(float? gravity = null, float? drag = null) 
        {
            if (gravity != null)
                acceleration = new Vector2f(0f, -gravity.Value);

            if (drag != null)
                this.drag = drag.Value;
        }

        /// <summary>
        /// Sets the burst shape to an angle within the specified range and initial velocity for the particles.
        /// </summary>
        /// <param name="minAngle">The minimum angle for the burst.</param>
        /// <param name="maxAngle">The maximum angle for the burst.</param>
        /// <param name="velocity">The initial velocity for the particles.</param>
        public void SetBurstShapeAngle(float minAngle, float maxAngle, float velocity)
        {
            burstShape = BurstShape.Angle;
            angle = (true, minAngle, maxAngle);
            startVelocity = velocity;
        }

        /// <summary>
        /// Sets the burst shape to random and specifies the initial velocity for the particles.
        /// </summary>
        /// <param name="velocity">The initial velocity for the particles.</param>
        public void SetBurstShapeRandom(float velocity)
        {
            burstShape = BurstShape.Random;
            startVelocity = velocity;
        }

        /// <summary>
        /// Sets the burst shape to a star and specifies the number of points and initial velocity for the particles.
        /// </summary>
        /// <param name="points">The number of points for the star.</param>
        /// <param name="velocity">The initial velocity for the particles.</param>
        public void SetBurstShapeStar(int points, float velocity)
        {
            starPoints.all = points;
            burstShape = BurstShape.Star;
            startVelocity = velocity;
        }

#endregion

#region public commands

        /// <summary>
        /// Starts the particle system.
        /// </summary>
        public void Play()
        {
            isPlaying = true;
            emitTimer = TIME + (emitTime.random ? RANDOM.Range(emitTime.min, emitTime.max) : emitTime.min);
        }

        /// <summary>
        /// Stops the particle system. Optionally destroys all particles.
        /// </summary>
        /// <param name="destroyParticles">Whether to destroy all particles when stopping.</param>
        public void Stop(bool destroyParticles = false) 
        {
            isPlaying = false;
            if (destroyParticles) particles.Clear();
        }

        /// <summary>
        /// Emits particles at the specified position. If no position is specified, particles are emitted at the system's position.
        /// </summary>
        /// <param name="position">The position to emit particles at.</param>
        /// <param name="amount">The number of particles to emit.</param>
        public void Emit(Vector2f? position = null, int amount = 1)
        {
            position ??= Position;
            position += GetPositionOnShape();

            Vector2f GetPositionOnShape()
            {
                return shape switch {
                    EmitterShape.Rect => new (RANDOM.Range(-.5f, .5f) * emitSize.X, RANDOM.Range(-.5f, .5f) * emitSize.Y),
                    EmitterShape.Circle => RANDOM.InUnitCircle() * emitSize.X,
                    /*Shape.Point*/ _ => new ()
                };
            }

            for (int i = 0; i < amount; i++)
                SpawnParticle(position.Value);
        }

#endregion

        private void SpawnParticle(Vector2f position)
        {
            float particleLifetime = lifetime.random ? RANDOM.Range(lifetime.min, lifetime.max) : lifetime.min;

            Particle p = new (position, particleLifetime);
            p.velocity = GetStartVelocity();
            p.color = getParticleColor?.Invoke() ?? Color.White;
            particles.Add(p);

            Vector2f GetStartVelocity()
            {
                return burstShape switch {
                    BurstShape.Random => RANDOM.InUnitCircle() * startVelocity,
                    BurstShape.Star => GetFromAngle(GetStarPoint() * (2f * MathF.PI) - MathF.PI) * startVelocity,
                    /*BurstShape.Angle*/ _ => GetFromAngle((angle.random ? RANDOM.Range(angle.min, angle.max) : angle.min) * MathTools.DegToRad()) * MathF.Sqrt(RANDOM.NextFloat()) * startVelocity
                };

                Vector2f GetFromAngle(float angle) => new (MathF.Cos(angle), MathF.Sin(angle));
                float GetStarPoint()
                {
                    starPoints.current ++;
                    if (starPoints.current >= starPoints.all) starPoints.current = 0;
                    return starPoints.current / (float)starPoints.all;
                }
            }
        }

        protected override void Update()
        {
            bool emitTimerCondition = (emitOverTime && emitTimer <= TIME) || !emitOverTime;
            bool emitDistanceCondition = (emitOverDistance && emitPoint.Distance(Position) >= currentEmitDistance) || !emitOverDistance;
            
            if (emitTimerCondition && emitDistanceCondition && isPlaying)
            {
                Emit();
                emitTimer += emitTime.random ? RANDOM.Range(emitTime.min, emitTime.max) : emitTime.min;
                emitPoint = Position;
                currentEmitDistance = emitDistance.random ? RANDOM.Range(emitDistance.min, emitDistance.max) : emitDistance.min;
            }
            
            for (int i = particles.Count - 1; i >= 0 ; i--)
            {
                particles[i] = UpdateParticle(particles[i]);
                if (particles[i].lifetime <= 0f) particles.RemoveAt(i);
            }
        }

        private Particle UpdateParticle(Particle p)
        {
            p.lifetime -= DELTA_TIME;

            p.velocity += acceleration * DELTA_TIME;
            p.velocity *= drag;
            p.position += p.velocity * DELTA_TIME;
            return p;
        }

        protected override void Draw(RenderWindow window)
        {
            foreach (Particle p in particles)
                DrawParticle(p, window);
        }

        private void DrawParticle(Particle p, RenderWindow window)
        {
            particleDisplay.Position = p.position;
            particleDisplay.Scale = particleSize;

            particleDisplay.Scale *= sizeOverLifetime?.Invoke(p.Progress) ?? 1f;
            particleDisplay.FillColor = p.color.Tint(tintOverLifetime?.Invoke(p.Progress) ?? Color.White);

            window.Draw(particleDisplay);
        }
    }
}