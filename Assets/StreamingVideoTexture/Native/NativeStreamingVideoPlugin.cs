using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Video;

namespace StreamingVideoTexture.Native
{
#if UNITY_EDITOR || UNITY_STANDALONE
    public class NativeStreamingVideoPlugin : IStreamingVideoPlugin
    {
        [UsedImplicitly]
        private readonly VideoPlayer _videoPlayer;

        [UsedImplicitly]
        private readonly AudioSource _audioSource;

        private string _url;

        public StreamingVideoStatus Status { get; private set; }

        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                Status = StreamingVideoStatus.Unknown;
            }
        }

        public bool IsDone => Status == StreamingVideoStatus.Done;

        public float Duration => (float) _videoPlayer.length;

        public float Time => (float) _videoPlayer.time;

        public bool IsReadyToPlay => Status == StreamingVideoStatus.ReadyToPlay;

        public bool IsPlaying => Status == StreamingVideoStatus.Playing;

        public NativeStreamingVideoPlugin(string url, GameObject gameObject)
        {
            var o = gameObject;
            _url = url;

            Status = StreamingVideoStatus.Unknown;

            _audioSource = o.GetComponent<AudioSource>();
            _videoPlayer = o.GetComponent<VideoPlayer>();

            if (_audioSource == null)
            {
                _audioSource = o.AddComponent<AudioSource>();
            }

            if (_videoPlayer == null)
            {
                _videoPlayer = o.AddComponent<VideoPlayer>();
            }

            _videoPlayer.errorReceived += (source, message) => { Debug.Log(message); };

            _videoPlayer.source = VideoSource.Url;
            _videoPlayer.isLooping = true;
            _videoPlayer.playOnAwake = false;

            _videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
            _videoPlayer.targetMaterialRenderer = o.GetComponent<Renderer>();
            _videoPlayer.targetMaterialProperty = "_MainTex";

            _videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            _videoPlayer.SetTargetAudioSource(0, _audioSource);

            var fullyQualifiedUrl = _url.StartsWith("http://") ? _url : Application.streamingAssetsPath + "/" + _url;
            Debug.Log("Creating video url: " + fullyQualifiedUrl);

            _videoPlayer.url = fullyQualifiedUrl;

            _videoPlayer.Prepare();
        }

        public void Play()
        {
            if (_videoPlayer.isPrepared)
            {
                _videoPlayer.Play();
            }
            else
            {
                _videoPlayer.prepareCompleted += OnPrepared;
            }
        }

        private void OnPrepared(VideoPlayer source)
        {
            _videoPlayer.Play();
            _videoPlayer.prepareCompleted -= OnPrepared;
        }

        public void Pause()
        {
            _videoPlayer.Pause();
        }

        public void Update()
        {
            var previousStatus = Status;
            switch (previousStatus)
            {
                case StreamingVideoStatus.Unknown:
                case StreamingVideoStatus.Loading:
                    if (_videoPlayer.isPrepared) Status = StreamingVideoStatus.ReadyToPlay;
                    break;

                case StreamingVideoStatus.ReadyToPlay:
                    if (_videoPlayer.isPlaying) Status = StreamingVideoStatus.Playing;
                    break;

                case StreamingVideoStatus.Playing:
                    if (!_videoPlayer.isPlaying) Status = StreamingVideoStatus.Done;
                    break;
            }
        }
    }
#endif
}
