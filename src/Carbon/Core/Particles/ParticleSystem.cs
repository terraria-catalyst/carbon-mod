using static TeamCatalyst.Carbon.Core.Autoloading.Definition;
using System.Numerics;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace TeamCatalyst.Carbon.Core.Particles {
    public static class ParticleSystem {
        public const int MAXPARTICLES = 10000;
        static Particle[] _particles = new Particle[MAXPARTICLES];

        public static unsafe Particle* FindNextEmptyParticle() {
            Particle* particle = null;

            for (int i = 0; i < _particles.Length; i++) {
                var partiterator = _particles[i];
                if (partiterator.lifeTime == 0) {
                    particle = &partiterator;
                    break;
                }
            }

            return particle;
        }

        static Dictionary<int, ParticleDefinition> _cachedDefinitions = new();

        internal static void UpdateParticles() {
            for(int i = 0; i <  MAXPARTICLES; i++) { 
                ref var particle = ref _particles[i];
                ref readonly int defID = ref particle.definition;

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
            }
        }
    }

    public class ParticleModSystem : ModSystem {
        public override void PreUpdateDusts() {
            ParticleSystem.UpdateParticles();
        }
    }

    public struct Particle {
        public Vector3 position;
        public Vector3 velocity;
        public uint lifeTime;
        public int definition;
    }
}
