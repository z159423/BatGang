#pragma warning disable CS0414

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace MondayOFF {
    // [CreateAssetMenu]
    public class CPVideoList : ScriptableObject {
        public List<VideoAndUrl> list;
        public VideoAndUrl this[int i] => list[i];
        public int count => list.Count;
    }

    [System.Serializable]
    public class VideoAndUrl {
        public string name = default;
        public VideoClip videoClip = default;
        public float weight = 1f;
        [SerializeField] string iosUrl = default;
        [SerializeField] string androidUrl = default;

        public string url =>
#if UNITY_IOS
            iosUrl;
#else
            androidUrl;
#endif

#if UNITY_EDITOR
        public void SetData(string name, string iosUrl, string androidUrl) {
            this.name = name;
            this.iosUrl = iosUrl;
            this.androidUrl = androidUrl;
        }
#endif

    }
}

#pragma warning restore CS0414