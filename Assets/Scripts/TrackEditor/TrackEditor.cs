using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class TrackEditor : MonoBehaviour {

    public static string SONGS_FOLDER = "/Resources/Songs";
    public static string SONGS_PATH { get { return Application.dataPath + SONGS_FOLDER; }}

    [SerializeField] Button SongEntryPrefab;
    [SerializeField] Transform SongsScrollView;
    [SerializeField] Text CurrentSongNameText;
    [SerializeField] Text CurrentSongTimer;
    [SerializeField] Button CurrentSongBtn;
    [SerializeField] Slider CurrentSongSlider;


    private List<string> _songList;
    private AudioSource _audio;
    private AudioClip _currentSong;
    private Coroutine _songControlLoop;
    private UISliderHandler _sliderHandler;

	void Start () {
        _songList = GetSongList();
        _audio = Camera.main.GetComponent<AudioSource>();
        _sliderHandler = CurrentSongSlider.GetComponent<UISliderHandler>();

        RenderUI();
    }

    private void RenderUI() {
        ResetSongPlayer();
        _sliderHandler.OnInteract += SliderSeek;

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

        _audio.clip = _currentSong;
        CurrentSongNameText.text = songName;

    }

    private void ResetSongPlayer() {
        _audio.Stop();

        if (_songControlLoop != null)
            StopCoroutine(_songControlLoop);

        CurrentSongNameText.text = "";
        CurrentSongTimer.text = "0:00";
        CurrentSongSlider.value = 0;
        _audio.time = 0;
        CurrentSongBtn.GetComponentInChildren<Text>().text = "Play";
        CurrentSongBtn.onClick.RemoveAllListeners();
        CurrentSongBtn.onClick.AddListener(PlaySong);

        

    }

    private void PauseSong() {
        CurrentSongBtn.GetComponentInChildren<Text>().text = "Play";
        CurrentSongBtn.onClick.RemoveListener(PauseSong);
        CurrentSongBtn.onClick.AddListener(PlaySong);
        StopCoroutine(_songControlLoop);

        _audio.Pause();
    }

    private void PlaySong() {
        if (_currentSong == null)
            return;

        CurrentSongBtn.GetComponentInChildren<Text>().text = "Pause";
        CurrentSongBtn.onClick.RemoveListener(PlaySong);
        CurrentSongBtn.onClick.AddListener(PauseSong);
        _songControlLoop = StartCoroutine(SongControlLoop());

        _audio.Play();
    }

    private void SliderSeek(float position) {

        Debug.Log("Seek " + position);

        if (_currentSong != null && position < 1)
            _audio.time = position * _currentSong.length;
    }

    private IEnumerator SongControlLoop() {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        int min, sec;
        while (true) {
            min = (int)_audio.time / 60;
            sec = (int)_audio.time % 60;
            CurrentSongSlider.value = _audio.time / _currentSong.length;
            CurrentSongTimer.text = string.Format("{0}:{1:00}", min, sec);
            yield return wait;
        }
    }
}
