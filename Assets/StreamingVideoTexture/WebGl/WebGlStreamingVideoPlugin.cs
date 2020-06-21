// ReSharper disable RedundantUsingDirective

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace StreamingVideoTexture.WebGl
{
#if UNITY_WEBGL && !UNITY_EDITOR
    public class WebGlStreamingVideoPlugin : IStreamingVideoPlugin
    {
        [DllImport("__Internal")]
        private static extern int WebGlStreamingVideoTextureCreate(string url);

        [DllImport("__Internal")]
        private static extern void WebGlStreamingVideoTextureUpdate(int video, int texture);

        [DllImport("__Internal")]
        private static extern void WebGlStreamingVideoTexturePlay(int video);

        [DllImport("__Internal")]
        private static extern void WebGlStreamingVideoTexturePause(int video);

        [DllImport("__Internal")]
        private static extern void WebGlStreamingVideoTextureSeek(int video, float time);

        [DllImport("__Internal")]
        private static extern void WebGlStreamingVideoTextureLoop(int video, bool loop);

        [DllImport("__Internal")]
        private static extern int WebGlStreamingVideoTextureWidth(int video);

        [DllImport("__Internal")]
        private static extern int WebGlStreamingVideoTextureHeight(int video);

        [DllImport("__Internal")]
        private static extern bool WebGlStreamingVideoTextureIsReady(int video);

        [DllImport("__Internal")]
        private static extern float WebGlStreamingVideoTextureTime(int video);

        [DllImport("__Internal")]
        private static extern float WebGlStreamingVideoTextureDuration(int video);

        private readonly int _instance;
        private readonly Texture2D _videoTexture;

        public string Url { get; set; }

        public bool IsReadyToPlay => WebGlStreamingVideoTextureIsReady(_instance);

        public bool IsPlaying => Status == StreamingVideoStatus.Playing;

        public bool IsDone => Status == StreamingVideoStatus.Done;

        public StreamingVideoStatus Status { get; private set; }

        public float Duration => WebGlStreamingVideoTextureDuration(_instance);

        public float Time => WebGlStreamingVideoTextureTime(_instance);

        public WebGlStreamingVideoPlugin(string url, GameObject gameObject)
        {
            Status = StreamingVideoStatus.Unknown;
            Url = url;

            _instance = WebGlStreamingVideoTextureCreate(url);

            _videoTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false) {wrapMode = TextureWrapMode.Clamp};
            _videoTexture.SetPixel(0, 0, Color.black);
            _videoTexture.Apply();

            gameObject.GetComponent<Renderer>().material.mainTexture = _videoTexture;
        }

        public void Play()
        {
            if (!IsReadyToPlay) return;

            WebGlStreamingVideoTexturePlay(_instance);
            Status = StreamingVideoStatus.Playing;
        }

        public void Pause()
        {
            WebGlStreamingVideoTexturePause(_instance);
            Status = StreamingVideoStatus.Paused;
        }

        public void Update()
        {
            if (Status < StreamingVideoStatus.ReadyToPlay && IsReadyToPlay)
            {
                Status = StreamingVideoStatus.ReadyToPlay;
            }

            if (Status == StreamingVideoStatus.Playing && Math.Abs(Time - Duration) < 0.1f)
            {
                Status = StreamingVideoStatus.Done;
            }

            if (_videoTexture == null || !_videoTexture.isReadable) return;

            var width = WebGlStreamingVideoTextureWidth(_instance);
            var height = WebGlStreamingVideoTextureHeight(_instance);

            if (width != _videoTexture.width || height != _videoTexture.height)
            {
                _videoTexture.Resize(width, height, TextureFormat.RGBA32, false);
                _videoTexture.Apply();
            }

            WebGlStreamingVideoTextureUpdate(_instance, _videoTexture.GetNativeTexturePtr().ToInt32());
        }
    }
    #endif
}
