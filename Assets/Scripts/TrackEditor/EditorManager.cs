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


    private List<string> _songList;
    private AudioSource _audio;
    private AudioClip _currentSong;
    private Track _currentTrack;
    private Coroutine _songControlLoop;
    private UISliderHandler _sliderHandler;
    private TrackEditor _editor;

	void Start () {
        _songList = GetSongList();
        _audio = Camera.main.GetComponent<AudioSource>();
        _sliderHandler = CurrentSongSlider.GetComponent<UISliderHandler>();
        _editor = GetComponent<TrackEditor>();

        RenderUI();
    }

    private void RenderUI() {
        ResetSongPlayer();
        _sliderHandler.OnInteract += SliderSeek;
        GoToBeatBtn.onClick.AddListener(BeatSeek);

        foreach (var song in _songList) {
            string ctx = song;
            Button btn = Instantiate<Button>(SongEntryPrefab);
            btn.transform.SetParent(SongsScrollView, false);
            btn.GetComponentInChildren<Text>().text = song;
            btn.onClick.AddListener(() => { LoadSong(ctx); });
        }

        LoadSong(_songList[0]);
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

        if (track == null)
            DialogWindow.Show((bpm) => {
                NewTrack(songName, _currentSong.length, bpm);
                CurrentSongBPM.text = bpm.ToString();
            });
        else {
            _currentTrack = JsonUtility.FromJson<Track>(track.text);
            CurrentSongBPM.text = _currentTrack.BPM.ToString();
        }

        _audio.clip = _currentSong;
        CurrentSongNameText.text = songName;
    }

    private void NewTrack(string name, float length, int bpm) {

        _currentTrack = _editor.NewTrack(name, bpm, length);

        string json = JsonUtility.ToJson(_currentTrack);
        File.WriteAllText(TRACKS_PATH+"/"+name+".json", json);

        Debug.Log("Song saved to: " + TRACKS_PATH + "/" + name + ".json");
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
            _audio.time = position * _currentSong.length;
            int min = (int)_audio.time / 60;
            int sec = (int)_audio.time % 60;
            int beat = _currentTrack.TimeToBeat(_audio.time);

            CurrentSongTimer.text = string.Format("{0}:{1:00}", min, sec);
            CurrentSongBeat.text = beat.ToString();
        }
    }

    private void BeatSeek() {
        if (_currentSong == null)
            return;

        int beat = int.Parse(CurrentSongBeat.text);
        _audio.time = _currentTrack.BeatToTime(beat);
        UpdateTime();
    }

    private IEnumerator SongControlLoop() {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        while (true) {
            UpdateTime();
            yield return wait;
        }
    }

    private void UpdateTime() {
        int min, sec, beat;

        min = (int)_audio.time / 60;
        sec = (int)_audio.time % 60;
        beat = _currentTrack.TimeToBeat(_audio.time);

        CurrentSongSlider.value = _audio.time / _currentSong.length;
        CurrentSongTimer.text = string.Format("{0}:{1:00}", min, sec);
        CurrentSongBeat.text = beat.ToString();
    }
}
