using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FCSG{
    public static class SpriteBatchExtension{

        /// <summary>
        /// Begins a new sprite batch with the specified parameters.
        /// </summary>
        public static void Begin(this SpriteBatch spriteBatch, SpriteBatchParameters parameters){
            spriteBatch.Begin(
                sortMode:parameters.sortMode,
                blendState:parameters.blendState,
                samplerState:parameters.samplerState,
                depthStencilState:parameters.depthStencilState,
                rasterizerState:parameters.rasterizerState,
                effect:parameters.effect,
                transformMatrix:parameters.transformMatrix
            );
        }
    }
}