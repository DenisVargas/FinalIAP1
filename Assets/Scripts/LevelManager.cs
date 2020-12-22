using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum healthState
{
    daijobu,
    yameteKudasaii,
    nigerundayooo
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager ins;

    [Header("Main Settings")]
    [SerializeField] RTSCamera _cameraController = null;

    [Header("Win and Defeat Conditions")]
    [SerializeField] GameObject SurvivorWinMenu = null;
    [SerializeField] GameObject ZombieWinMenu = null;

    [Header("Initialization Menu")]  //InitializationMenu
    [SerializeField] GameObject InitializationMenu = null;

    [SerializeField] TMP_Text ActiveSpawnersAmmountText = null;

    [SerializeField] TMP_Text survivorAmmountText = null;
    [SerializeField] Button SurvivorAdd_Button = null;
    [SerializeField] Button SurvivorRemove_Button = null;

    [SerializeField] TMP_Text wavesAmmountText = null;
    [SerializeField] Button wavesAdd_Button = null;
    [SerializeField] Button wavesRemove_Button = null;

    [SerializeField] TMP_InputField Frecuency_InputField = null;
    [SerializeField] Slider ActiveSpawners_Slider = null;

    int _survivorAmmount = 1;
    int _waves = 1;
    float _frecuency = 3f;
    int _activeSpawners = 1;

    private int SurvivorAmmount
    {
        get => _survivorAmmount;
        set
        {
            _survivorAmmount = value;
            _survivorAmmount = Mathf.Clamp(_survivorAmmount, 1, 10);

            if (_survivorAmmount == 10)
            {
                SurvivorAdd_Button.interactable = false;
                SurvivorRemove_Button.interactable = true;
            }
            else if (_survivorAmmount == 1)
            {
                SurvivorRemove_Button.interactable = false;
                SurvivorAdd_Button.interactable = true;
            }
            else
            {
                SurvivorAdd_Button.interactable = true;
                SurvivorRemove_Button.interactable = true;
            }

            if (survivorAmmountText != null)
                survivorAmmountText.text = _survivorAmmount.ToString();
        }
    }
    private int Waves
    {
        get => _waves;
        set
        {
            _waves = value;
            _waves = Mathf.Clamp(_waves, 1, 10);

            if (_waves == 10)
            {
                wavesAdd_Button.interactable = false;
                wavesRemove_Button.interactable = true;
            }
            else if (_waves == 1)
            {
                wavesRemove_Button.interactable = false;
                wavesAdd_Button.interactable = true;
            }
            else
            {
                wavesAdd_Button.interactable = true;
                wavesRemove_Button.interactable = true;
            }

            if (wavesAmmountText != null)
                wavesAmmountText.text = _waves.ToString();
        }
    }
    private float Frecuency
    {
        get => _frecuency;
        set
        {
            _frecuency = value;
            _frecuency = Mathf.Clamp(_frecuency, 0f, 10f);

            if (Frecuency_InputField)
                Frecuency_InputField.text = _frecuency.ToString();
        }
    }
    private int ActiveSpawners
    {
        get => _activeSpawners;
        set
        {
            _activeSpawners = value;

            if(ActiveSpawnersAmmountText)
                ActiveSpawnersAmmountText.text = _activeSpawners.ToString();
        }
    }


    [Header("GamePlay Menu")]    //Menú Ingame/Gameplay
    public GameObject GameplayMenu = null;
    [SerializeField] TMP_Text SurvivorCount_text = null;
    [SerializeField] TMP_Text ZombieCount_text = null;

    int _survivorsLeft = 0;
    int _zombiesLeft = 0;

    public int SurvivorsCount
    {
        get => _survivorsLeft;
        private set
        {
            _survivorsLeft = value;
            if (SurvivorCount_text != null)
                SurvivorCount_text.text = $"humans Left: {_survivorsLeft}";
        }
    }
    public int ZombiesCount
    {
        get => _zombiesLeft;
        private set
        {
            _zombiesLeft = value;

            if (ZombieCount_text != null)
                ZombieCount_text.text = $"Zombies Left: {_zombiesLeft}";
        }
    }


    [SerializeField] HumanSpawner humanSpawner = null;
    public ZombieSpawner[] zombieSpawners = null;

    private void Awake()
    {
        if (ins == null)
            ins = this;

        SurvivorsCount = _survivorsLeft;
        ZombiesCount = _zombiesLeft;

        SurvivorAmmount = _survivorAmmount;
        Waves = _waves;
        Frecuency = _frecuency;

        humanSpawner = FindObjectOfType<HumanSpawner>();
        zombieSpawners = FindObjectsOfType<ZombieSpawner>();
        ActiveSpawners = zombieSpawners.Length;
        ActiveSpawners_Slider.value = ActiveSpawners;

        Frecuency_InputField.onEndEdit.AddListener(OnFrecuencyChange);
        _cameraController.lockInput = true;
    }

    public void OnAdd_Waves(bool value)
    {
        Waves += value ? 1 : -1 ;
    }
    public void OnAdd_Survivors(bool value)
    {
        SurvivorAmmount += value ? 1 : -1 ;
    }
    public void OnActiveSpawner_Slider_ValueChanged()
    {
        ActiveSpawners = (int)ActiveSpawners_Slider.value;
    }
    public void OnFrecuencyChange(string value)
    {
        Frecuency = float.Parse(value);
    }

    public void TrackHuman(Human reference)
    {
        if (reference.IsAlive)
            SurvivorsCount++;
        else
            SurvivorsCount--;

        if (SurvivorsCount <= 0)
            OnGameEnded(faction.zombie);
    }
    public void TrackZombie(Zombie reference)
    {
        if (reference.IsAlive)
            ZombiesCount++;
        else
            ZombiesCount--;

        if (ZombiesCount <= 0)
            OnGameEnded(faction.human);
    }

    public void StartGame()
    {
        //Empiezo el pinche juego Weon.
        InitializationMenu.SetActive(false);
        GameplayMenu.SetActive(true);

        _cameraController.lockInput = false;

        //Spawn De humanitos.
        humanSpawner.Spawn(_survivorAmmount);

        //Spawn de Zombies.
        StartCoroutine(SpawnZombies());
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    public void ExitApp()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying)
            UnityEditor.EditorApplication.ExitPlaymode();
        else
#endif
            Application.Quit();
    }

    enum faction
    {
        human,
        zombie
    }
    void OnGameEnded(faction winner)
    {
        InitializationMenu.SetActive(false);
        GameplayMenu.SetActive(false);

        switch (winner)
        {
            case faction.human:
                SurvivorWinMenu.SetActive(true);
                break;
            case faction.zombie:
                ZombieWinMenu.SetActive(true);
                break;
            default:
                break;
        }
    }

    IEnumerator SpawnZombies()
    {
        int wavesLeft = Waves;

        while (wavesLeft > 0)
        {
            List<ZombieSpawner> aviableSpawners = new List<ZombieSpawner>(zombieSpawners);

            for (int i = 0; i < ActiveSpawners; i++)
            {
                int randomIndex = Random.Range(0, aviableSpawners.Count);
                int randomMaxAmmount = Random.Range(1, 4);
                aviableSpawners[randomIndex].SpawnWithSettings(1, randomMaxAmmount);
                aviableSpawners.RemoveAt(randomIndex);
            }

            wavesLeft--;
            yield return new WaitForSeconds(Frecuency);
        }
    }
}
