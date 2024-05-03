using Microsoft.Xna.Framework.Graphics;
using TeamCatalyst.Carbon.API.Autoloading;

namespace TeamCatalyst.Carbon.API.Particles {
    internal abstract class ParticleDefinition : Definition {

        public abstract SpritebatchSettings spritebatchSettings { get; protected set; }

        public abstract void Update(ref Particle particle);

        /// <summary>
        /// Spritebatch has already begun here with your <see cref="spritebatchSettings"/>!
        /// </summary>
        /// <param name="particle"></param>
        /// <param name="spriteBatch"></param>
        public abstract void Draw(in Particle particle, SpriteBatch spriteBatch);
    }
}
