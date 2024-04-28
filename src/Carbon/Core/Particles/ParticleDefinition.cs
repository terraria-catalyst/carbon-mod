using Microsoft.Xna.Framework.Graphics;
using TeamCatalyst.Carbon.Core.Autoloading;

namespace TeamCatalyst.Carbon.Core.Particles {
    internal unsafe abstract class ParticleDefinition : Definition {

        public abstract SpritebatchSettings spritebatchSettings { get; protected set; }

        public abstract void Update(Particle* particle);
        /// <summary>
        /// Spritebatch has already begun here with your <see cref="spritebatchSettings"/>!
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="spriteBatch"></param>
        public abstract void Draw(Particle* particle, SpriteBatch spriteBatch);
    }
}
