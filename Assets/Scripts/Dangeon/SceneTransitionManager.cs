using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

// Керує переходом між сценами.
// Гравець, його HP/MP-полоси, інвентар і камера переносяться
// в нову сцену без створення копій.
public sealed class SceneTransitionManager : MonoBehaviour
{
    // Єдиний екземпляр менеджера
    public static SceneTransitionManager Instance { get; private set; }

    // Чи вже йде перехід
    public bool IsTransitioning { get; private set; }

    // Кореневі об'єкти, які переживають завантаження сцени
    private readonly List<GameObject> carriedRoots = new List<GameObject>();
    // Корінь гравця
    private GameObject playerRoot;
    // Корінь EventSystem
    private GameObject eventSystemRoot;
    // Чи був EventSystem увімкнений до переходу
    private bool eventSystemWasActive;
    // Чи був PlayerController увімкнений до переходу
    private bool playerControllerWasEnabled;

    // Скидаємо static-змінну перед запуском гри в редакторі
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        Instance = null;
    }

    private void Awake()
    {
        // Другий менеджер не потрібен
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        // Дізнаємось, коли нова сцена завантажилась
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    // Знайти менеджер або створити його
    public static SceneTransitionManager GetOrCreate()
    {
        if (Instance != null)
        {
            return Instance;
        }

        GameObject managerObject = new GameObject("SceneTransitionManager");
        DontDestroyOnLoad(managerObject);
        Instance = managerObject.AddComponent<SceneTransitionManager>();
        return Instance;
    }

    // Почати перехід у вказану сцену
    public bool ChangeScene(string sceneName, bool useSpawnPosition, Vector2 spawnPosition)
    {
        if (IsTransitioning)
        {
            return false;
        }

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("SceneTransitionManager: scene name is empty.", this);
            return false;
        }

        // Сцена повинна бути додана в Build Settings
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError("SceneTransitionManager: scene '" + sceneName + "' is not in Build Settings.", this);
            return false;
        }

        // Не перезавантажуємо ту саму сцену
        if (SceneManager.GetActiveScene().name == sceneName)
        {
            return false;
        }

        StartCoroutine(LoadScene(sceneName, useSpawnPosition, spawnPosition));
        return true;
    }

    // Сам перехід: збираємо об'єкти, вантажимо сцену, переносимо їх
    private IEnumerator LoadScene(string sceneName, bool useSpawnPosition, Vector2 spawnPosition)
    {
        IsTransitioning = true;

        CollectCarriedRoots();

        if (playerRoot == null)
        {
            Debug.LogError("SceneTransitionManager: Player was not found.", this);
            IsTransitioning = false;
            yield break;
        }

        // Тимчасово виносимо об'єкти з поточної сцени
        for (int i = 0; i < carriedRoots.Count; i++)
        {
            DontDestroyOnLoad(carriedRoots[i]);
        }

        PrepareForLoad();

        AsyncOperation operation = null;
        try
        {
            operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }
        catch (System.Exception exception)
        {
            Debug.LogException(exception, this);
        }

        // Якщо завантажити сцену не вдалося - повертаємо все назад
        if (operation == null)
        {
            RestoreAfterFailedLoad();
            IsTransitioning = false;
            yield break;
        }

        yield return operation;

        Scene newScene = SceneManager.GetActiveScene();

        // Переносимо збережені об'єкти в нову сцену
        for (int i = 0; i < carriedRoots.Count; i++)
        {
            GameObject root = carriedRoots[i];
            if (root == null)
            {
                continue;
            }
            if (root.scene != newScene)
            {
                SceneManager.MoveGameObjectToScene(root, newScene);
            }
        }

        // Ставимо гравця в точку входу
        if (useSpawnPosition)
        {
            PlacePlayer(spawnPosition);
        }

        // Камера нової сцени повинна стежити за перенесеним гравцем
        SetupCamera(newScene);

        RestoreAfterLoad();

        carriedRoots.Clear();
        playerRoot = null;
        eventSystemRoot = null;
        IsTransitioning = false;
    }

    // Збираємо кореневі об'єкти, які треба перенести
    private void CollectCarriedRoots()
    {
        carriedRoots.Clear();
        playerRoot = null;
        eventSystemRoot = null;

        Scene activeScene = SceneManager.GetActiveScene();
        GameObject[] roots = activeScene.GetRootGameObjects();

        for (int i = 0; i < roots.Length; i++)
        {
            GameObject root = roots[i];
            if (!IsGameplayRoot(root))
            {
                continue;
            }

            carriedRoots.Add(root);

            if (playerRoot == null)
            {
                if (root.GetComponent<PlayerController>() != null)
                {
                    playerRoot = root;
                }
            }

            if (eventSystemRoot == null)
            {
                if (root.GetComponent<EventSystem>() != null)
                {
                    eventSystemRoot = root;
                }
            }
        }
    }

    // Чи це гравець або його інтерфейс, який переходить у нову сцену
    private bool IsGameplayRoot(GameObject root)
    {
        if (root.GetComponent<PlayerController>() != null)
        {
            return true;
        }
        if (root.GetComponent<ManaHUD>() != null)
        {
            return true;
        }
        if (root.GetComponent<Canvas>() != null)
        {
            return true;
        }
        if (root.GetComponent<EventSystem>() != null)
        {
            return true;
        }
        if (root.GetComponent<BattleTransition>() != null)
        {
            return true;
        }
        if (root.GetComponent<Light2D>() != null)
        {
            return true;
        }
        return false;
    }

    // Unity викликає цей метод після завантаження сцени
    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!IsTransitioning)
        {
            return;
        }

        RemoveDuplicates(scene);
    }

    // Видаляємо копії гравця та інтерфейсу, які вже були в новій сцені
    private void RemoveDuplicates(Scene scene)
    {
        GameObject[] roots = scene.GetRootGameObjects();

        for (int i = 0; i < roots.Length; i++)
        {
            GameObject root = roots[i];
            if (root == null)
            {
                continue;
            }
            if (carriedRoots.Contains(root))
            {
                continue;
            }
            if (!IsGameplayRoot(root))
            {
                continue;
            }

            // Вимикаємо одразу, щоб два об'єкти не працювали в одному кадрі
            root.SetActive(false);
            Destroy(root);
        }
    }

    // Готуємо гравця та EventSystem до завантаження
    private void PrepareForLoad()
    {
        eventSystemWasActive = false;
        if (eventSystemRoot != null)
        {
            eventSystemWasActive = eventSystemRoot.activeSelf;
            eventSystemRoot.SetActive(false);
        }

        PlayerController controller = playerRoot.GetComponent<PlayerController>();
        if (controller != null)
        {
            playerControllerWasEnabled = controller.enabled;
            controller.enabled = false;
        }
    }

    // Повертаємо керування після завантаження
    private void RestoreAfterLoad()
    {
        if (eventSystemRoot != null)
        {
            if (eventSystemWasActive)
            {
                eventSystemRoot.SetActive(true);
            }
        }

        if (playerRoot != null)
        {
            PlayerController controller = playerRoot.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.enabled = playerControllerWasEnabled;
            }
        }
    }

    // Якщо сцена не завантажилась - повертаємо об'єкти в поточну сцену
    private void RestoreAfterFailedLoad()
    {
        Scene activeScene = SceneManager.GetActiveScene();

        for (int i = 0; i < carriedRoots.Count; i++)
        {
            GameObject root = carriedRoots[i];
            if (root == null)
            {
                continue;
            }
            if (root.scene != activeScene)
            {
                SceneManager.MoveGameObjectToScene(root, activeScene);
            }
        }

        RestoreAfterLoad();
    }

    // Ставимо гравця в точку появи
    private void PlacePlayer(Vector2 spawnPosition)
    {
        if (playerRoot == null)
        {
            return;
        }

        Vector3 position = playerRoot.transform.position;
        position.x = spawnPosition.x;
        position.y = spawnPosition.y;
        playerRoot.transform.position = position;

        Rigidbody2D body = playerRoot.GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
            body.position = spawnPosition;
        }
    }

    // Прив'язуємо камеру нової сцени до перенесеного гравця
    private void SetupCamera(Scene scene)
    {
        if (playerRoot == null)
        {
            return;
        }

        Camera sceneCamera = FindCamera(scene);
        if (sceneCamera == null)
        {
            Debug.LogWarning("SceneTransitionManager: camera was not found in scene '" + scene.name + "'.", this);
            return;
        }

        CameraFollow follow = sceneCamera.GetComponent<CameraFollow>();
        if (follow == null)
        {
            follow = sceneCamera.gameObject.AddComponent<CameraFollow>();
        }

        follow.SetTarget(playerRoot.transform);
        follow.SnapToTarget();
    }

    // Шукаємо камеру серед кореневих об'єктів сцени
    private Camera FindCamera(Scene scene)
    {
        GameObject[] roots = scene.GetRootGameObjects();

        for (int i = 0; i < roots.Length; i++)
        {
            Camera camera = roots[i].GetComponentInChildren<Camera>(true);
            if (camera != null)
            {
                return camera;
            }
        }

        return Camera.main;
    }
}
