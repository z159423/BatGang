using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace MondayOFF {
    public class CPButton : MonoBehaviour {
        [SerializeField] VideoPlayer _vp;
        [SerializeField] CPVideoList _videoList;
        int _mediaIndex = 0;

        void Awake() {
            if (_videoList == null) {
                _videoList = Resources.Load("VideoList") as CPVideoList;
                Debug.Assert(_videoList != null, "Video list Scriptable Object is NULL!");
            }

            if (_vp == null) {
                _vp = GetComponent<VideoPlayer>();
            }

            for (int i = _videoList.count - 1; i >= 0; --i) {
                if (string.IsNullOrEmpty(_videoList[i].url) || _videoList[i].videoClip == null) {
                    Debug.LogWarning($"Removing {_videoList[i].name} because it does not contain store url or video");
                    _videoList.list.RemoveAt(i);
                }
            }

            _videoList.list.Sort((a, b) => Mathf.CeilToInt(b.weight - a.weight));

            if (_videoList.count <= 0) {
                Debug.LogError("No CP video and url found!");
                Destroy(this.gameObject);
                return;
            }

            GetComponentInChildren<Button>().onClick.AddListener(linkToStore);
        }

        void OnEnable() {
            shuffleVideo();
        }

        void OnDisable() {
            stopPlayback();
        }

        void OnApplicationFocus(bool focusStatus) {
            if (focusStatus) {
                shuffleVideo();
            } else {
                stopPlayback();
            }
        }

        void stopPlayback() {
            _vp.Stop();
        }

        void shuffleVideo() {
            var count = _videoList.count;
            var weight = 0f;
            int i = 0;
            for (; i < count; ++i) {
                weight += _videoList[i].weight;
            }

            var roll = Random.Range(0f, weight);

            for (i = 0; i < count; ++i) {
                weight = _videoList[i].weight;
                if (roll < weight) {
                    _mediaIndex = i;
                    break;
                }
                roll -= weight;
            }

            _vp.clip = _videoList[_mediaIndex].videoClip;
            _vp.Play();
        }

        void linkToStore() {
            Application.OpenURL(_videoList[_mediaIndex].url);
        }
    }
}