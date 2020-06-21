var WebGlStreamingVideoTexture = {
    $videoInstances: [],
    isWebGl2: false,

    WebGlStreamingVideoTextureCreate: function (url) {
        if (!this.isWebGl2) this.isWebGl2 = GLctx.getParameter(GLctx.VERSION).startsWith('WebGL 2.0');

        var str = Pointer_stringify(url);
        var video = document.createElement('video');
        video.volume = 0;
        video.style.display = 'none';
        video.src = str;

        return videoInstances.push(video) - 1;
    },

    WebGlStreamingVideoTextureUpdate: function (video, texture) {
        if (videoInstances[video].paused) return;

        if (videoInstances[video].readyState < 3) return;

        GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[texture]);

        if (this.isWebGl2) {
            GLctx.texSubImage2D(GLctx.TEXTURE_2D, 0, 0, 0, GLctx.RGBA, GLctx.UNSIGNED_BYTE, videoInstances[video]);
        } else {
            GLctx.texImage2D(GLctx.TEXTURE_2D, 0, GLctx.RGBA8, GLctx.RGBA, GLctx.UNSIGNED_BYTE, videoInstances[video]);
        }
    },

    WebGlStreamingVideoTexturePlay: function (video) {
        videoInstances[video].play();
    },

    WebGlStreamingVideoTexturePause: function (video) {
        videoInstances[video].pause();
    },

    WebGlStreamingVideoTextureSeek: function (video, time) {
        videoInstances[video].fastSeek(time);
    },

    WebGlStreamingVideoTextureLoop: function (video, loop) {
        videoInstances[video].loop = loop;
    },

    WebGlStreamingVideoTextureHeight: function (video) {
        return videoInstances[video].videoHeight || 1;
    },

    WebGlStreamingVideoTextureWidth: function (video) {
        return videoInstances[video].videoWidth || 1;
    },

    WebGlStreamingVideoTextureTime: function (video) {
        return videoInstances[video].currentTime;
    },

    WebGlStreamingVideoTextureDuration: function (video) {
        return videoInstances[video].duration;
    },

    WebGlStreamingVideoTextureIsReady: function (video) {
        return videoInstances[video].readyState >= videoInstances[video].HAVE_CURRENT_DATA;
    }
};
autoAddDeps(WebGlStreamingVideoTexture, '$videoInstances');
mergeInto(LibraryManager.library, WebGlStreamingVideoTexture);
