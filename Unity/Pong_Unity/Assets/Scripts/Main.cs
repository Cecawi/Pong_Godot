using UnityEngine;
using GameCore; // la dll youpi
using UnityEngine.UI;
using TMPro;

public class Main : MonoBehaviour
{
    private GameObject ball;
    private GameObject  _paddle1;
    private GameObject  _paddle2;
    private GameState _gameState;
    private GameObject _score;

    public GameObject BallObject;
    public GameObject Paddle1Object;
    public GameObject Paddle2Object;


    private SpriteRenderer _ballSprite;
    private SpriteRenderer _paddleSprite1;
    private SpriteRenderer _paddleSprite2;

    public TMP_Text LabelP1;
    public TMP_Text LabelP2;
    public GameObject GameOverUI;
    public TMP_Text GameOverLabel;
    public Button RestartButton;
    public bool _isGameOver;

    private AudioSource _bounceSound;
    private AudioSource _goalSound;


    private Vector2 _windowSize;

    private bool _isSplitMode = false;


    void Start()
    {
        _gameState = new GameState();

        _ballSprite = BallObject.GetComponent<SpriteRenderer>();
        _paddleSprite1 = Paddle1Object.GetComponent<SpriteRenderer>();
        _paddleSprite2 = Paddle2Object.GetComponent<SpriteRenderer>();

        _bounceSound = BallObject.GetComponent<AudioSource>(); // ou autre objet
        _goalSound = Paddle1Object.GetComponent<AudioSource>(); // à adapter

        RestartButton.onClick.AddListener(OnRestartPressed);
        GameOverUI.SetActive(false);

        _windowSize = new Vector2(Screen.width, Screen.height);

        UpdateVisuals();
        //CreateVisualWalls();
    }

    private PlayerIntention GetPlayerIntention1()
    {
        int move = PlayerIntention.Neutral;

        if (_isSplitMode)
        {
            // En mode split (vue FPS) : A et D
            if (Input.GetKey(KeyCode.A)) move = PlayerIntention.Up;      // gauche = monte
            else if (Input.GetKey(KeyCode.D)) move = PlayerIntention.Down; // droite = descend
        }
        else
        {
            // Vue normale : W et S
            if (Input.GetKey(KeyCode.W)) move = PlayerIntention.Up;
            else if (Input.GetKey(KeyCode.S)) move = PlayerIntention.Down;
        }

        return new PlayerIntention(move);
    }

    private PlayerIntention GetPlayerIntention2()
    {
        int move = PlayerIntention.Neutral;

        if (_isSplitMode)
        {
            if (Input.GetKey(KeyCode.LeftArrow)) move = PlayerIntention.Up;      // gauche = monte
            else if (Input.GetKey(KeyCode.RightArrow)) move = PlayerIntention.Down; // droite = descend
        }
        else
        {
            if (Input.GetKey(KeyCode.UpArrow)) move = PlayerIntention.Up;
            else if (Input.GetKey(KeyCode.DownArrow)) move = PlayerIntention.Down;
        }

        return new PlayerIntention(move);
    }




    void Update()
    {
        float deltaTime = Time.deltaTime;

        var p1Intention = GetPlayerIntention1();
        var p2Intention = GetPlayerIntention2();

        _gameState.Update(deltaTime, p1Intention, p2Intention);

        if (_gameState.GetBallBounceSoundFlag())
        {
            if (_bounceSound!=null)
            _bounceSound.Play();
            _gameState.SetBallBounceSoundFlag(false);
        }

        if (_gameState.GetScoreGoalSoundFlag())
        {
            if (_goalSound!=null)
            _goalSound.Play();
            _gameState.SetScoreGoalSoundFlag(false);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            _isSplitMode = !_isSplitMode;
            FindObjectOfType<Split>().SetSplitScreen(_isSplitMode);
        }

        if (!_isGameOver && _gameState.IsGameOver())
        {
            string winner = _gameState.GetScore().Item1 > _gameState.GetScore().Item2 ? "Player 1" : "Player 2";
            OnGameOver(winner);
        }

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        Vector2 ToPixels(float normX, float normY)
        {
            float x = (normX + 0.5f) * _windowSize.x;
            float y = ( 0.5f- normY) * _windowSize.y;
            return new Vector2(x, y);
        }

        var (ballX, ballY) = _gameState.GetBallPosition();
        BallObject.transform.position = ToPixels(ballX, ballY);

        var (p1X, p1Y) = _gameState.GetPaddle1Position();
        var (p2X, p2Y) = _gameState.GetPaddle2Position();
        Paddle1Object.transform.position = ToPixels(p1X, p1Y);
        Paddle2Object.transform.position = ToPixels(p2X, p2Y);

        var (s1, s2) = _gameState.GetScore();
        LabelP1.text = s1.ToString();
        LabelP2.text = s2.ToString();
    }


    private void OnGameOver(string winner)
    {
        if (_isGameOver) return;
        _isGameOver = true;
        GameOverLabel.text = $"Game Over\n{winner} Wins!";
        GameOverUI.SetActive(true);
        RestartButton.gameObject.SetActive(true);
        Time.timeScale = 0f; // pause le jeu
    }

    private void OnRestartPressed()
    {
        _gameState = new GameState();
        _isGameOver = false;
        GameOverUI.SetActive(false);
        RestartButton.gameObject.SetActive(false);
        Time.timeScale = 1f;
        UpdateVisuals();
    }

    void CreateVisualWalls()
    {
        float fieldWidth = _gameState.GetFieldWidth();
        float fieldHeight = _gameState.GetFieldHeight();
        float wallThickness = 0.1f; // epaisseur mur

        // Parent pour organiser
        GameObject arena = new GameObject("Arena");

        // Top wall
        GameObject topWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topWall.transform.parent = arena.transform;
        topWall.transform.position = new Vector3(0, fieldHeight / 2 + wallThickness / 2, 0);
        topWall.transform.localScale = new Vector3(fieldWidth, wallThickness, 1);

        // Bottom wall
        GameObject bottomWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bottomWall.transform.parent = arena.transform;
        bottomWall.transform.position = new Vector3(0, -fieldHeight / 2 - wallThickness / 2, 0);
        bottomWall.transform.localScale = new Vector3(fieldWidth, wallThickness, 1);

        // Optionnel : Left / Right
        GameObject leftWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftWall.transform.parent = arena.transform;
        leftWall.transform.position = new Vector3(-fieldWidth / 2 - wallThickness / 2, 0, 0);
        leftWall.transform.localScale = new Vector3(wallThickness, fieldHeight, 1);

        GameObject rightWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightWall.transform.parent = arena.transform;
        rightWall.transform.position = new Vector3(fieldWidth / 2 + wallThickness / 2, 0, 0);
        rightWall.transform.localScale = new Vector3(wallThickness, fieldHeight, 1);
    }





}
