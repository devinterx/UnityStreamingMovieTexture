namespace StreamingVideoTexture
{
    public interface IStreamingVideoPlugin
    {
        string Url { get; set; }
        bool IsReadyToPlay { get; }
        bool IsPlaying { get; }
        void Play();
        void Pause();
        bool IsDone { get; }
        StreamingVideoStatus Status { get; }
        void Update();
        float Duration { get; }
        float Time { get; }
    }

    public enum StreamingVideoStatus
    {
        Unknown,
        Error,
        Loading,
        ReadyToPlay,
        Playing,
        Paused,
        Done,
        Stopped
    }
}
