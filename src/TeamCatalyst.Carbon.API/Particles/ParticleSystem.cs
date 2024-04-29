using static TeamCatalyst.Carbon.API.Core.Autoloading.Definition;
using System.Numerics;
using System.Collections.Generic;
using Terraria.ModLoader;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace TeamCatalyst.Carbon.Core.API.Particles {
    public static class ParticleSystem {
        public const int MAXPARTICLES = 10000;
        static Particle[] _particles = new Particle[MAXPARTICLES];
        static List<Particle> particlesToSort = [];

        public static ref Particle FindNextEmptyParticle() {
            ref Particle particle = ref Particle.FailParticle;

            for (int i = 0; i < _particles.Length; i++) {
                ref var partiterator = ref _particles[i];
                if (partiterator.lifeTime == 0) {
                    particle = partiterator;
                    break;
                }
            }

            return ref particle;
        }

        static Dictionary<int, ParticleDefinition> _cachedDefinitions = new();

        internal static void UpdateParticles() {
            particlesToSort.Clear(); // this probably gets rid of most of my benefits, will see about replacing it later

            for (int i = 0; i < MAXPARTICLES; i++) {
                Particle particle = _particles[i];
                int defID = particle.definition;

                if (defID == 0)
                    continue;

                if (_cachedDefinitions.TryGetValue(defID, out var definition)) {
                    definition.Update(ref particle);
                }
                else {
                    // Slow update, to be avoided
                    definition = GetDefinitionFromID(defID) as ParticleDefinition;

                    if (definition is null)
                        continue;

                    _cachedDefinitions.Add(defID, definition);
                    definition.Update(ref particle);
                }

                particlesToSort.Add(particle);
            }

            particlesToSort = particlesToSort.OrderBy(p => p.definition).ToList();
        }

        internal static void DrawParticles(SpriteBatch spriteBatch) {
            var tempSettings = new SpritebatchSettings();
            bool activeSB = false;
            for (int i = 0; i < MAXPARTICLES; i++) {
                Particle particle = _particles[i];
                ref int defID = ref particle.definition;

                if (defID == 0)
                    continue;

                if (_cachedDefinitions.TryGetValue(defID, out var definition)) {
                    if (definition.spritebatchSettings.GetHashCode() != tempSettings.GetHashCode()) { // Could maybe use a better method of comparison?
                        tempSettings = definition.spritebatchSettings;

                        if (activeSB) {
                            spriteBatch.End();
                        }

                        spriteBatch.Begin(tempSettings.sortMode, tempSettings.blendState, tempSettings.samplerState, tempSettings.depthStencilState, tempSettings.rasterizerState, tempSettings.effect, tempSettings.transformationMatrix);
                        activeSB = true;
                    }

                    definition.Draw(particle, spriteBatch);
                }
            }
        }
    }

    public class ParticleModSystem : ModSystem {
        public override void PreUpdateDusts() {
            ParticleSystem.UpdateParticles();
        }
    }

    public struct Particle {
        public static Particle FailParticle = new Particle() { definition = -1 };

        public Vector3 position;
        public Vector3 velocity;
        public uint lifeTime;
        public int definition;
    }
}
