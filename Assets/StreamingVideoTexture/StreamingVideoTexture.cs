// ReSharper disable RedundantUsingDirective

using StreamingVideoTexture.Native;
using UnityEngine;

#if UNITY_EDITOR || UNITY_STANDALONE

#elif UNITY_WEBGL
using StreamingVideoTexture.WebGl;
#endif

namespace StreamingVideoTexture
{
    public class StreamingVideoTexture : MonoBehaviour
    {
        public string OgvUrl;

        private IStreamingVideoPlugin _videoPlugin;

        public bool PlayRequested;

        private void Start()
        {
            if (string.IsNullOrEmpty(OgvUrl)) return;

            Debug.Log("Creating WWW for ogv");

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
            _videoPlugin = new NativeStreamingVideoPlugin(OgvUrl, gameObject);
#elif UNITY_WEBGL
            _videoPlugin = new WebGlStreamingVideoPlugin(OgvUrl, gameObject);
#endif
        }

        private void Update()
        {
            if (_videoPlugin == null) return;

            _videoPlugin.Update();

            if (_videoPlugin.IsReadyToPlay && PlayRequested)
            {
                _videoPlugin.Play();
            }
        }

        private void LateUpdate()
        {
            gameObject.transform.Rotate(Time.deltaTime * 10, Time.deltaTime * 30, 0);
        }
    }
}
