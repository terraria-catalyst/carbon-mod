using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TeamCatalyst.Carbon.API.Particles {
    internal struct SpritebatchSettings {
        public readonly SpriteSortMode sortMode;
        public readonly BlendState blendState;
        public readonly SamplerState samplerState;
        public readonly DepthStencilState depthStencilState;
        public readonly RasterizerState rasterizerState;
        public readonly Effect effect;
        public readonly Matrix transformationMatrix;

        public override int GetHashCode() {
            return HashCode.Combine(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformationMatrix);
        }
    }
}
