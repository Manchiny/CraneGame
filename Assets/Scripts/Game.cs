using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }

    [SerializeField] private WindowsController _windowsController;
    public static WindowsController Windows => Instance._windowsController;

	[SerializeField] private LevelLoader _levelLoader;
	public static LevelLoader LevelLoader => Instance._levelLoader;

	[SerializeField] private LevelManager _levelManager;
	public static LevelManager LevelManager => Instance._levelManager;

	[SerializeField] private ColorManager _colors;
	public static ColorManager ColorManager => Instance._colors;

	public static Locker Locker => Instance._windowsController.Locker;

	private UserData _userData;
	public static UserData User => Instance._userData;

    private void Awake()
    {
		Instance = this;
		DontDestroyOnLoad(this);
	}
    private void Start()
    {
		Init();
	}

	private void Init()
	{
		_userData = new UserData();
		_colors.Init();
		MainMenuWindow.Of();
	}
}
