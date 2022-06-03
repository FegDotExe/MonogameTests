using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace FCSG{
    public class SpriteBatchParameters{
        public SpriteSortMode sortMode;
        public BlendState blendState;
        public SamplerState samplerState;
        public DepthStencilState depthStencilState;
        public RasterizerState rasterizerState;
        public Effect effect;
        public Matrix? transformMatrix;

        public SpriteBatchParameters(
            SpriteSortMode sortMode=SpriteSortMode.Deferred,
            BlendState blendState=null,
            SamplerState samplerState=null,
            DepthStencilState depthStencilState=null,
            RasterizerState rasterizerState=null,
            Effect effect=null,
            Matrix? transformMatrix=null
        ){
            this.sortMode=sortMode;
            this.blendState=blendState;
            this.samplerState=samplerState;
            this.depthStencilState=depthStencilState;
            this.rasterizerState=rasterizerState;
            this.effect=effect;
            this.transformMatrix=transformMatrix;
        }
    }
}