using UnityEngine;
using TMPro;

public enum GameState { Menu, Playing, Paused, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("State")]
    public GameState state = GameState.Menu;
    public int score = 0;
    public int lives = 3;

    [Header("Difficulty")]
    public float baseEnemySpeed = 2.0f;
    public float maxEnemySpeed = 7.0f;
    public float speedPerScore = 0.05f;
    public float baseSpawnInterval = 1.0f;
    public float minSpawnInterval = 0.35f;
    public float spawnIntervalPerScore = 0.006f;
    public int waveEvery = 10;

    [Header("References")]
    public EnemySpawner spawner;
    public Transform centerTarget;
    public LaserFX laserFx;
    public CameraShake camShake;

    [Header("Audio")]
    public AudioSource bgmSource;
    public AudioClip blastSfx;
    public AudioClip hurtSfx;
    //public AudioClip powerupSfx;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public GameObject startPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    [Header("Controls")]
    public bool useRightClick = false; // set true if you prefer right click

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    void Start()
    {
        UpdateHUD();
        EnterMenu();
    }

    public void EnterMenu()
    {
        Time.timeScale = 0f;
        state = GameState.Menu;
        startPanel.SetActive(true);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    public void StartGame()
    {
        GameManager.I.bgmSource.volume = 0.1f;
        score = 0;
        lives = 3;
        UpdateHUD();
        state = GameState.Playing;
        Time.timeScale = 1f;
        startPanel.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        if (bgmSource && !bgmSource.isPlaying) bgmSource.Play();
        spawner.Begin();
    }

    public void PauseGame()
    {
        if (state != GameState.Playing) return;
        state = GameState.Paused;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        if (state != GameState.Paused) return;
        state = GameState.Playing;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    public void GameOver()
    {
        state = GameState.GameOver;
        Time.timeScale = 0f;
        if (finalScoreText) finalScoreText.text = $"Score: {score}";
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void AddScore(int amt)
    {
        score += amt;
        UpdateHUD();
        if (score % waveEvery == 0 && score > 0) spawner.BumpWave();
    }

    public void LoseLife()
    {
        lives--;
        if (hurtSfx) AudioSource.PlayClipAtPoint(hurtSfx, centerTarget.position, 1.0f);
        camShake?.Shake(0.2f, 0.2f);
        UpdateHUD();
        if (lives <= 0) GameOver();
    }

    public float sfxVolume = 1;

    public void PlayBlast(Vector3 at)
    {
        if (blastSfx) AudioSource.PlayClipAtPoint(blastSfx, at, sfxVolume);
    }

    /*

    public void PlayPowerup()
    {
        if (powerupSfx) AudioSource.PlayClipAtPoint(powerupSfx, centerTarget.position);
    }
    */

    public float CurrentEnemySpeed()
    {
        return Mathf.Min(baseEnemySpeed + score * speedPerScore, maxEnemySpeed);
    }

    public float CurrentSpawnInterval()
    {
        float t = baseSpawnInterval - score * spawnIntervalPerScore;
        return Mathf.Max(t, minSpawnInterval);
    }

    void UpdateHUD()
    {
        if (scoreText) scoreText.text = $"Score: {score}";
        if (livesText) livesText.text = $"Lives: {lives}";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (state == GameState.Playing) PauseGame();
            else if (state == GameState.Paused) ResumeGame();
        }
    }

    public bool FireButtonDown()
    {
        int btn = useRightClick ? 1 : 0;
        return Input.GetMouseButtonDown(btn);
    }

    public AudioClip laserSfx;

    public void PlayLaser(Vector3 at)
    {
        if (laserSfx) AudioSource.PlayClipAtPoint(laserSfx, at, 0.7f);
    }

}
