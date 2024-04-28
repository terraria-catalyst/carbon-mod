using Microsoft.Xna.Framework.Graphics;
using TeamCatalyst.Carbon.Core.Autoloading;

namespace TeamCatalyst.Carbon.Core.Particles {
    internal abstract class ParticleDefinition : Definition {
        public abstract void Update(ref Particle particle);
        public abstract void Draw(ref Particle particle, SpriteBatch spriteBatch);
    }
}
