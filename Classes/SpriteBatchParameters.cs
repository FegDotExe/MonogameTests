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

        public static SpriteBatchParameters operator +(SpriteBatchParameters sbp1, SpriteBatchParameters sbp2){
            SpriteBatchParameters output=new SpriteBatchParameters();

            output.sortMode=Utilities.Choose(sbp1.sortMode, sbp2.sortMode);
            output.blendState=Utilities.Choose(sbp1.blendState, sbp2.blendState);
            output.samplerState=Utilities.Choose(sbp1.samplerState, sbp2.samplerState);
            output.depthStencilState=Utilities.Choose(sbp1.depthStencilState, sbp2.depthStencilState);
            output.rasterizerState=Utilities.Choose(sbp1.rasterizerState, sbp2.rasterizerState);
            output.effect=Utilities.Choose(sbp1.effect, sbp2.effect);
            output.transformMatrix=Utilities.Choose(sbp1.transformMatrix, sbp2.transformMatrix);

            return output;
        }
    }
}