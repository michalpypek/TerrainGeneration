using UnityEngine;


public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	#region Variables

	private const string STR_TempInstanceOf = "Temp Instance of ";
	private const string STR_ProblemDuringTheCreationOf = "Problem during the creation of {0}";
	private static T m_Instance = null;

	private static bool IsShuttingDown = false;
	#endregion

	public static T get
	{
		get
		{
			if (IsShuttingDown)
				return null;

			if (m_Instance == null)
			{
				m_Instance = FindObjectOfType(typeof(T)) as T;

				if (m_Instance == null)
				{
					m_Instance = new GameObject(STR_TempInstanceOf + typeof(T).ToString(), typeof(T)).GetComponent<T>();

					if (m_Instance == null)
					{
						Debug.Log(string.Format(STR_ProblemDuringTheCreationOf, typeof(T).ToString()));
					}
				}
				DontDestroyOnLoad(m_Instance.gameObject);
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public static bool isInstance
	{
		get { return m_Instance != null; }
	}

	private void Awake()
	{
		if (m_Instance == null)
		{
			m_Instance = this as T;
			m_Instance.Init();
		}
	}

	public virtual void Init() { }

	internal virtual void OnApplicationQuit()
	{
		IsShuttingDown = true;

		m_Instance = null;
	}
}
