using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private WindowsController _windowsController;
	[SerializeField] private LevelLoader _levelLoader;
	[SerializeField] private LevelManager _levelManager;
	[SerializeField] private ColorManager _colors;
	[SerializeField] private GameSound _gameSound;

	private UserData _userData;

    public static Game Instance { get; private set; }

    public static WindowsController Windows => Instance._windowsController;
	public static LevelLoader LevelLoader => Instance._levelLoader;
	public static LevelManager LevelManager => Instance._levelManager;
	public static ColorManager ColorManager => Instance._colors;
	public static Locker Locker => Instance._windowsController.Locker;
	public static UserData User => Instance._userData;
	public static GameSound Sound => Instance._gameSound;

    private void Awake()
    {
		if(Instance == null)
        {
			Instance = this;
			DontDestroyOnLoad(this);
			return;
        }

		Destroy(this);
	}

    private void Start()
    {
		Init();
	}

#if UNITY_EDITOR
	private void Update()
    {
		if (Input.GetKeyDown(KeyCode.F1) == true)
			NoMoreLevelsWindow.Show();
    }
#endif

	private void Init()
	{
		Utils.SetMainContainer(this);
		Sound.Init(LevelLoader);

		_userData = new UserData();
		_colors.Init();
		MainMenuWindow.Show();
	}
}
