using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class EditorManager : MonoBehaviour {

    public static string SONGS_FOLDER = "/Resources/Songs";
    public static string TRACKS_FOLDER = "/Resources/Tracks";
    public static string SONGS_PATH { get { return Application.dataPath + SONGS_FOLDER; } }
    public static string TRACKS_PATH { get { return Application.dataPath + TRACKS_FOLDER; } }

    [SerializeField] Button SongEntryPrefab;
    [SerializeField] Transform SongsScrollView;
    [SerializeField] Text CurrentSongNameText;
    [SerializeField] Text CurrentSongTimer;
    [SerializeField] Text CurrentSongBPM;
    [SerializeField] Button PlayPauseBtn;
    [SerializeField] Slider CurrentSongSlider;
    [SerializeField] InputField CurrentSongBeat;
    [SerializeField] Button GoToBeatBtn;
    [SerializeField] Dialog DialogWindow;
    [SerializeField] Button SaveBtn;

    private List<string> _songList;
    private AudioSource _audio;
    private AudioClip _currentSong;
    private Coroutine _songControlLoop;
    private UISliderHandler _sliderHandler;
    private EditorState _state;
    private TrackEditor _editor;
    private TrackVisualizer _visualizer;

	void Start () {
        _songList = GetSongList();
        _audio = Camera.main.GetComponent<AudioSource>();
        _sliderHandler = CurrentSongSlider.GetComponent<UISliderHandler>();

        _state = new EditorState();
        _editor = GetComponentInChildren<TrackEditor>();
        _visualizer = GetComponentInChildren<TrackVisualizer>();

        _editor.Init(_state);
        _visualizer.Init(_state);

        RenderUI();
    }

    private void RenderUI() {
        ResetSongPlayer();

        _sliderHandler.OnInteract += SliderSeek;

        GoToBeatBtn.onClick.AddListener(BeatSeek);
        SaveBtn.onClick.AddListener(SaveCurrentTrack);

        foreach (var song in _songList) {
            string ctx = song;
            Button btn = Instantiate<Button>(SongEntryPrefab);
            btn.transform.SetParent(SongsScrollView, false);
            btn.GetComponentInChildren<Text>().text = song;
            btn.onClick.AddListener(() => { LoadSong(ctx); });
        }

        LoadSong(_songList[0]);
    }

    private void SaveCurrentTrack() {
        string json = JsonUtility.ToJson(_state.track);
        File.WriteAllText(TRACKS_PATH + "/" + _state.track.songName + ".json", json);
        Debug.Log("Song saved to: " + TRACKS_PATH + "/" + _state.track.songName + ".json");
    }

    private List<string> GetSongList() {
        string[] files = Directory.GetFiles(SONGS_PATH, "*.mp3");
        List<string> songs = new List<string>();
        foreach(string path in files) {
            string[] parts = path.Split('\\');
            string name = parts[parts.Length - 1];
            name = name.Substring(0, name.Length - 4);
            songs.Add(name);
        }
        return songs;
    }

    private void LoadSong(string songName) {
        ResetSongPlayer();

        _currentSong = Resources.Load<AudioClip>("Songs/"+ songName);
        TextAsset track = Resources.Load<TextAsset>("Tracks/" + songName);

        if (track != null)
            LoadTrack(JsonUtility.FromJson<Track>(track.text));
        else {
            DialogWindow.Show((bpm) => {
                LoadTrack(_editor.NewTrack(songName, bpm, _currentSong.length));
                SaveCurrentTrack();
            });
        }

        _audio.clip = _currentSong;
    }

    private void LoadTrack(Track track) {
        _state.track = track;
        CurrentSongBPM.text = track.BPM.ToString();
        CurrentSongNameText.text = track.songName;

    }

    private void ResetSongPlayer() {
        _audio.Stop();

        if (_songControlLoop != null)
            StopCoroutine(_songControlLoop);

        CurrentSongNameText.text = "";
        CurrentSongBPM.text = "---";
        CurrentSongTimer.text = "0:00";
        CurrentSongBeat.text = "0";
        CurrentSongSlider.value = 0;
        _audio.time = 0;
        PlayPauseBtn.GetComponentInChildren<Text>().text = "Play";
        PlayPauseBtn.onClick.RemoveAllListeners();
        PlayPauseBtn.onClick.AddListener(PlaySong);
    }

    private void PauseSong() {
        PlayPauseBtn.GetComponentInChildren<Text>().text = "Play";
        PlayPauseBtn.onClick.RemoveListener(PauseSong);
        PlayPauseBtn.onClick.AddListener(PlaySong);
        StopCoroutine(_songControlLoop);
        
        _audio.Pause();
    }

    private void PlaySong() {
        if (_currentSong == null)
            return;

        PlayPauseBtn.GetComponentInChildren<Text>().text = "Pause";
        PlayPauseBtn.onClick.RemoveListener(PlaySong);
        PlayPauseBtn.onClick.AddListener(PauseSong);
        _songControlLoop = StartCoroutine(SongControlLoop());

        _audio.Play();
    }

    private void SliderSeek(float position) {

        if (_currentSong != null && position < 1) {  
            _audio.time    = position * _currentSong.length;
            _state.time    = _audio.time;

            int min = (int)_state.time / 60;
            int sec = (int)_state.time % 60;

            CurrentSongTimer.text = string.Format("{0}:{1:00}", min, sec);
            CurrentSongBeat.text = _state.beat.ToString();
        }
    }

    private void BeatSeek() {
        if (_currentSong == null)
            return;

        _state.beat = int.Parse(CurrentSongBeat.text);
        _audio.time = _state.time;
        UpdateTime();
    }

    private IEnumerator SongControlLoop() {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        while (true) {
            _state.time = _audio.time;
            UpdateTime();
            yield return wait;
        }
    }

    private void UpdateTime() {
        int min, sec;

        min = (int)_state.time / 60;
        sec = (int)_state.time % 60;

        CurrentSongSlider.value = _audio.time / _currentSong.length;
        CurrentSongTimer.text = string.Format("{0}:{1:00}", min, sec);
        CurrentSongBeat.text = _state.beat.ToString();
    }
}
