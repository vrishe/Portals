using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/PortalsRenderPipelineAsset")]
public class PortalsRenderPipelineAsset : RenderPipelineAsset
{
    /*
     * Some other options go here
     */

    protected override RenderPipeline CreatePipeline() => new PortalsRenderPipeline(this);

    private class PortalsRenderPipeline : RenderPipeline
    {
        private readonly PortalsRenderPipelineAsset _asset;

        private CommandBuffer _cb;

        public PortalsRenderPipeline(PortalsRenderPipelineAsset asset)
        {
            _asset = asset;
            _cb = new CommandBuffer();
        }

        protected override void Dispose(bool disposing)
        {
            _cb.Dispose();
            _cb = null;


            base.Dispose(disposing);
        }

        protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
        {
            RenderCameras(context, cameras);
        }

        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            RenderCameras(context, cameras);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RenderCameras(ScriptableRenderContext context, IList<Camera> cameras)
        {
            for (var i = 0; i < cameras.Count; ++i)
            {
                RenderCamera(context, cameras[i]);
            }

            context.Submit();
        }

        private void RenderCamera(ScriptableRenderContext context, Camera camera)
        {
            var cb = CommandBufferPool.Get();

            cb.ClearRenderTarget(true, true, camera.backgroundColor);

            context.ExecuteCommandBuffer(cb);
            CommandBufferPool.Release(cb);
        }
    }
}
