
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System;

using Player;
using Dialog;
using Inventory;
using Skill;
using Crafting;
using Rock;
using AnilTools;
using AnilTools.Update;
using MiddleGames.Misc;
using Assets.Resources.Scripts.UI;
using static GameConstants;

public class GameMenu : Singleton<GameMenu> {
    
    #region structs
    [Serializable]
    private struct Menus {
        public GameObject PauseMenu;
        public GameObject playerMenu;
        public GameObject OptionsMenu;
        public GameObject Interface;
    }
    [Serializable]
    private struct InterfacePanels {
        public CanvasGroup Missions;
        public CanvasGroup Map;
        public CanvasGroup Crafting;
        public CanvasGroup Enhancing;
        public CanvasGroup Skills;
    }
    [Serializable]
    private struct Panels {
        public GameObject grafikPanel;
        public GameObject sesPanel;
        public GameObject oynanışPanel;
    }
    [Serializable]
    private struct Buttons {
        public GameObject AttackButton;
    }
    [Serializable]
    public struct ButtonGroups{
        // pause Menu
        public ButtonGroup pauseMenu;
        // game menu
        public ButtonGroup interfaceUpMenu;
        // options
        public ButtonGroup optionsLeft;
        public ButtonGroup graphicMain;
        public ButtonGroup soundMain;
        public ButtonGroup gameplayMain;
    }

    [SerializeField]
    private InterfacePanels interfacePanels;
    private CanvasGroup CurrentInteface;
    [SerializeField]
    private Buttons buttons;
    [SerializeField] private Menus menus;
    [SerializeField] private Panels panels;
    public CanvasController canvases;
    public ButtonGroups buttonGroups;

    #endregion

    #region Static Veriables
    private const string YouDead = "You Dead";

    private static bool _playerOnMenu;
    public static bool PlayerOnMenu
    {
        get => _playerOnMenu;
        set {
            Time.fixedDeltaTime = value ? fixedDeltaTimeStart : Mathmatic.FloatMaxValue; // fixed update hızı
            _playerOnMenu = value;
        }
    }

    public static bool PlayerOnSkill;

    private static GameObject acıkPanel;

    private static void GetAçıkPanel() {
        if (acıkPanel != null)
            acıkPanel.SetActive(false);
    }

    private GameObject Kelebekler => GameObject.FindGameObjectWithTag(Tags.Kelebek);
    public static Slider LoadingPercentSlider { get; private set; }
    #endregion

    [NonSerialized]
    public byte Dil;

    //classlar 
    [SerializeField] private MerchantMenu[] merchantMenus;
    public Languages languages;
    public Texts texts;

    [SerializeField] private CanvasGroup inventory;
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private Image FullScreenFade;
    [SerializeField] private Text PlayerDeadText;
    [SerializeField] private CanvasGroup InterfaceGroup;
                     public Slider volume;
                     public Slider Sens;

    [SerializeField] private GameObject LoadingScreenMenu;
    [SerializeField] private Slider LoadSceneSlider;

    [SerializeField] private Slider LevelSlider;
    [SerializeField] private Text HealthText;
    [SerializeField] private Text ArmorTxt;
    [SerializeField] private Text AttackText;

    public Language GetCurrentLanguage {
        get {
            if (languages.currentLanguage == AvibleLanguages.none) {
                if (Application.systemLanguage == SystemLanguage.Turkish)
                    languages.currentLanguage = AvibleLanguages.Turkish;
                if (Application.systemLanguage == SystemLanguage.English)
                    languages.currentLanguage = AvibleLanguages.English;
            }
            return languages.languages[(short)languages.currentLanguage];
        }
    }

    private VideoPlayer[] VideoPlayers;
    private ScrollRect[] scrollRects;

    private static float fixedDeltaTimeStart;

    private void Awake() {
        fixedDeltaTimeStart = Time.fixedDeltaTime;
        Shader.globalMaximumLOD = 200;
        LoadingPercentSlider = LoadSceneSlider;
    }

    private void Start() {
        //Helper.ShowToast("Hello");
        VideoPlayers = FindObjectsOfType<VideoPlayer>();
        scrollRects = FindObjectsOfType<ScrollRect>();
        SetRectsActive(false);
        SetVideosActive(false);

        SetCanvasScale();
        LoadVeriables();

        InputListenner.Add(KeyCode.I, EnableInventory);
        InputListenner.Add(KeyCode.Escape, Pause);
        InputListenner.Add(KeyCode.M, OpenInterface);
    }

#if UNITY_EDITOR
    private void OnDisable() {
        //UnityShortCuts.ClearConsole();
    }
#endif
    //menü olayları
    #region menü thing

    public void Quit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SetRectsActive(bool value) => scrollRects.Foreach(x => x.enabled = value);

    public void Resume() {
        PlayerOnMenu = false;
        SetRectsActive(false);
        canvases.MenuCanvas.SetActive(false);
        canvases.BaseCanvas.SetActive(true);
        menus.playerMenu.SetActive(true);
        menus.PauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameManager.instance.audioSource.PlayOneShot(buttonClick);
        PlayerOnMenu = false;

        InputListenner.Set(KeyCode.Escape, Pause);// buradaki amaç esc tuşuna event eklemek
    }

    public void LoadNextScene() {
        LoadingScreenMenu.SetActive(true);
        //SceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public static void SetCursor(bool value){
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = value;
    }

    // open pause Menu
    public void Pause() {
        SetCursor(true);
        PlayerOnMenu = true;
        SetRectsActive(true);
        menus.playerMenu.SetActive(false);
        menus.PauseMenu.SetActive(true);
        canvases.MenuCanvas.SetActive(true);
        canvases.BaseCanvas.SetActive(false);
        Time.timeScale = 0f;
        GameManager.instance.audioSource.PlayOneShot(buttonClick);
        GC.Collect();
        PlayerOnMenu = true;

        ButtonGroupManager.SetCurrentGroup(buttonGroups.pauseMenu);
        InputListenner.Set(KeyCode.Escape, Resume);
    }

    public void Options() {
        menus.PauseMenu.SetActive(false);
        menus.OptionsMenu.SetActive(true);
        //önceki verileri ayarlar menüsüne yükler
        volume.value = XMLmanager.instance.list.ses;
        Sens.value   = XMLmanager.instance.list.Senstivity;
        GameManager.instance.audioSource.PlayOneShot(buttonClick);

        InputListenner.Set(KeyCode.Escape, Back);
        ButtonGroupManager.SetCurrentGroup(buttonGroups.optionsLeft);
    }

    public void CloseOptions()
    {
        menus.PauseMenu.SetActive(true);
        menus.OptionsMenu.SetActive(false);
        //önceki verileri ayarlar menüsüne yükler
        GameManager.instance.audioSource.PlayOneShot(buttonClick);

        InputListenner.Set(KeyCode.Escape, Resume);
    }

    public void Graphics() => OpenInSettings(panels.grafikPanel);
    public void Sounds()   => OpenInSettings(panels.sesPanel);
    public void Gameplay() => OpenInSettings(panels.oynanışPanel);

    public void OpenInSettings(GameObject @object){
        GetAçıkPanel();
        acıkPanel = @object;
        acıkPanel.SetActive(true);
        GameManager.instance.audioSource.PlayOneShot(buttonClick);
        InputListenner.Set(KeyCode.Escape, ExitSettingPage);
        
        if (@object == panels.grafikPanel)        ButtonGroupManager.SetCurrentGroup(buttonGroups.graphicMain);
        else if (@object == panels.sesPanel)      ButtonGroupManager.SetCurrentGroup(buttonGroups.soundMain);
        else if (@object == panels.oynanışPanel)  ButtonGroupManager.SetCurrentGroup(buttonGroups.gameplayMain);
    }

    private void ExitSettingPage() => ButtonGroupManager.SetCurrentGroup(buttonGroups.optionsLeft);

    public void Back()// options tan çıkar
    {   //ses ve senstivity ayarını kaydeder
        GameManager.instance.audioSource.volume = volume.value <= 0.02f ? 0: volume.value/6 ;
        CharacterMove.instance.lookSpeed = Sens.value;
        //xml managere yazar
        XMLmanager.instance.list.ses = volume.value / 6;
        XMLmanager.instance.list.Senstivity = Sens.value;
        menus.PauseMenu.SetActive(true);
        menus.OptionsMenu.SetActive(false);
        GameManager.instance.audioSource.PlayOneShot(buttonClick);
    }

    #region grafik ayarları
    private void SetGraphic(QualityDATA quality){
        QualitySettings.SetQualityLevel(quality.QualityLvl);
        RenderSettings.fog = quality.Fog;
#if UNITY_EDITOR
        Application.targetFrameRate = 60;
#else
        Application.targetFrameRate = quality.Fps;
#endif
        XMLmanager.graphic = quality.Id;

        if(quality.QualityLvl < 2 ) Shader.SetGlobalInt("PERFORMANCE_ON", 1);
        else                        Shader.SetGlobalInt("PERFORMANCE_ON", 0);

        if (SceneManager.GetActiveScene().buildIndex < 1)
            return;

        Kelebekler.SetActive(quality.Kelebekler);
        GameManager.instance.audioSource.PlayOneShot(buttonClick);

        GrassHandeller.DisableAllGrasses = !quality.grass;

    }

    #region qualityDATA
    internal struct QualityDATA{
        internal short QualityLvl;
        internal float Dpi;
        internal bool Fog;
        internal byte Id;
        internal bool Kelebekler;
        internal short Fps;
        internal bool grass;
        internal int GrassDenisty;

        public QualityDATA(short qualityLvl, float dpi, bool fog, byte id, bool kelebekler, short fps, bool grass, int grassDenisty)
        {
            QualityLvl = qualityLvl;
            Dpi = dpi;
            Fog = fog;
            Id = id;
            Kelebekler = kelebekler;
            Fps = fps;
            this.grass = grass;
            GrassDenisty = grassDenisty;
        }
    }
    #endregion

    private readonly QualityDATA HighData = new QualityDATA(5, 1, false, 3, true, 900,true,1023 );
    private readonly QualityDATA Middata  = new QualityDATA(2, .75f, false, 2, true, 900,true,512);
    private readonly QualityDATA LowData  = new QualityDATA(0, .5f, false, 1, false, 900,false,0 );
    
    public void  Low() => SetGraphic(LowData);
    public void  Mid() => SetGraphic(Middata);
    public void High() => SetGraphic(HighData);

    public void SetGraphic(uint value) {
        switch (value) {
            case 1: Low();  break;
            case 2: Mid();  break;
            case 3: High(); break;
        }
    }
    #endregion 
    
    // ınventory
    public void EnableInventory() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;        
        CharacterMove.SetPlayer(false);
        StopAllCoroutines();
        SetCanvasGroupFade(inventory,true);
        canvases.InventoryCanvas.SetActive(true);
        menus.playerMenu.SetActive(false);
        UpdateStats();
        DialogControl.SetinEquipment(true);
        PlayerInventoryCamera.canLook = true;
        PlayerOnMenu = true;

        InputListenner.Set(KeyCode.Escape, DisableInventory);
    }

    public void UpdateStats() { 
        HealthText.text = "Health " + CharacterHealth.Health.ToString();
        ArmorTxt.text = "Armor " + PlayerInfo.DamageReducing.ToString();
        AttackText.text = "Attack " + PlayerInfo.AttackDamage.ToString();

        LevelSlider.maxValue = PlayerInfo.RequiredEXP + PlayerInfo.Exp;
        LevelSlider.value = PlayerInfo.Exp;
    }

    public void DisableInventory() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CharacterMove.SetPlayer(true);

        StopAllCoroutines();
        SetCanvasGroupFade(inventory, false);

        canvases.InventoryCanvas.SetVisuality(false);

        menus.playerMenu.SetActive(true);
        PlayerInventoryCamera.canLook = false;
        PlayerOnMenu = false;
 
        InputListenner.Set(KeyCode.Escape, Pause);
    }
    
    public void PlayerDead() {
        menus.playerMenu.SetActive(false);
        DarkFade();
    }

    #region interface

    private void SetVideosActive(bool value) => VideoPlayers.Foreach(x => x.enabled = value);
    
    public void OpenInterface(){
        SetCursor(true);
        SetRectsActive(true);
        SetVideosActive(true);

        canvases.MenuCanvas.SetActive(true);

        Debug2.Log("open interface");

        Time.timeScale = 0;
        menus.playerMenu.SetActive(false);
        menus.Interface.SetActive(true);

        StopAllCoroutines();
        SetCanvasGroupFade(InterfaceGroup, true);
        PlayerOnSkill = true;
        CraftDatabase.instance.UpdateCounts();

        InputListenner.Set(KeyCode.Escape, CloseInterface);
        ButtonGroupManager.SetCurrentGroup(buttonGroups.interfaceUpMenu);
    }

    public void CloseInterface() {
        SetCursor(false);
        SetRectsActive(false);
        SetVideosActive(false);

        canvases.MenuCanvas.SetActive(true);

        Time.timeScale = 1;
        menus.playerMenu.SetActive(true);
        menus.Interface.SetActive(false);
        StopAllCoroutines();
        SetCanvasGroupFade( InterfaceGroup, false);
        PlayerOnSkill = false;
        InputListenner.Set(KeyCode.Escape, Pause);
    }

    ////////////////////

    public void OpenMissions()  => Open(interfacePanels.Missions);
    public void OpenMap()       => Open(interfacePanels.Map);
    public void OpenCrafting()  => Open(interfacePanels.Crafting);
    public void OpenSkills()    => Open(interfacePanels.Skills);

    public void OpenEnhancing(){
        RockDatabase.instance.UpdateUI();
        Open(interfacePanels.Enhancing);
    }

    private void Open(CanvasGroup group){

        InputListenner.Set(KeyCode.Escape, GetInterfaceButtons);

        // todo: set buttongroupmanager's new group

        if (CurrentInteface != group) {
            if (CurrentInteface){
                CurrentInteface.SetVisible(false);
                CurrentInteface = group;
                group.SetVisible(true);
            }
            else{
                CurrentInteface = group;
                group.SetVisible(true);
            }
        }
    }

    private void GetInterfaceButtons() => ButtonGroupManager.SetCurrentGroup(buttonGroups.interfaceUpMenu);
    

    #endregion // Interface

    private void DarkFade(){
        RegisterUpdate.UpdateWhile(
        action: () => FullScreenFade.color = Color.Lerp(FullScreenFade.color, Color.black, Time.deltaTime),
        endCnd: () => FullScreenFade.color.a < .9f,
        then: () => new Timer(2f, delegate { RestartScene(); PlayerDeadText.text = YouDead; }));
    }

    public void RestartScene(){ 
        CharacterHealth.isDead = false;
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ---- Horse ----
    public void MountHorse() {
        //buttons.AttackButton.SetActive(false);
    }
    public void DismountHorse(){
        //buttons.AttackButton.SetActive(true);
    }
    #endregion //nenu thing 

    public void LoadVeriables() {
        FullScreenFade.gameObject.SetActive(true);
        //ChangeLanguage((byte)languages.currentLanguage);
    }

    public void ChangeLanguage(int Dil) {
        
        languages.currentLanguage = (AvibleLanguages)Dil;

        if (Dil == 2) { // none
            if      (Application.systemLanguage == SystemLanguage.English) Dil = 1;
            else if (Application.systemLanguage == SystemLanguage.Turkish) Dil = 0;
        }

        CharacterHealth.instance.ChangeLanguage(languages.languages[Dil].health);

        //item kelimeleri
        var uIItemObjects = GameObject.FindGameObjectsWithTag(Tags.item);

        //UI VE SETTİNGS KELİMELERİ
        texts.medium.text     = languages.languages[Dil].medium;
        texts.lowSetting.text = languages.languages[Dil].low;
        texts.high.text       = languages.languages[Dil].high;
        texts.look.text       = languages.languages[Dil].look;

        texts.mainSound.text  = languages.languages[Dil].mainSound;
        texts.sounds.text     = languages.languages[Dil].sounds;
        texts.sounds1.text    = languages.languages[Dil].sounds;
        texts.gameplay.text   = languages.languages[Dil].gameplay;
        texts.gameplay1.text  = languages.languages[Dil].gameplay;
        texts.graphics.text   = languages.languages[Dil].graphics;
        texts.graphics1.text  = languages.languages[Dil].graphics;
                              
        texts.exit.text       = languages.languages[Dil].exit;
        texts.load.text       = languages.languages[Dil].load;
        texts.options.text    = languages.languages[Dil].options;
        texts.save.text       = languages.languages[Dil].save;
        texts.resume.text     = languages.languages[Dil].resume;

        // dialog kelimeleri
        DialogControl.instance.ChangeLanguage(languages.languages[Dil].words, (AvibleLanguages)Dil);

        texts.trade.text = languages.languages[Dil].trade;

        // item kelimeleri
        for (short i = 0; i < ItemDatabase.instance.itemNRs.Count; i++) {
            var item = ItemDatabase.instance.itemNRs[i];
            item.ChangeLanguage(languages.languages[Dil].itemStrings.Find(x => x.item.Id == item.id()));
        }
        
        for (short i = 0; i < ItemDatabase.instance.itemSCs.Count; i++) {
            var item = ItemDatabase.instance.itemSCs[i];
            item.ChangeLanguage(languages.languages[Dil].itemStrings.Find(x => x.item.Id == item.id()));
        }

        // ticaret kelimeleri
        for (short i = 0; i < merchantMenus.Length; i++) {
            MerchantMenu item = merchantMenus[i];
            item.UpdateLanguage(Dil);
        }
        
        SkillMenuHandeller.instance.UpdateLanguage();
        CraftingController.instance.UpdateLanguage(Dil);
        
        // tooltip kelimeleri
        ToolTip.instance.ChangeLanguage(languages.languages[Dil].tooltipWords);

        // mission kelimeleri

        MissionHandeller.instance.SetLanguage(languages.languages[Dil].killCount, languages.languages[Dil].ItemCount);

        Debug2.Log("Dil Değiştirildi ");
    }

    public void FirstChange(byte dil) {
        ChangeLanguage(dil);
        GameManager.instance.audioSource.PlayOneShot(buttonClick);
    }

    public void Vibrate(byte value) => Vibration.Vibrate(value);

    public static void SetCanvasGroupFade(CanvasGroup group, bool activity , Action then = null) {
        if (group){
            
            void endaction(){ 
                group.interactable = activity;
                group.blocksRaycasts = activity;
                then?.Invoke();
            }

            if (activity) {
                RegisterUpdate.UpdateWhile(
                action: () => group.alpha += .85f * Time.unscaledDeltaTime,
                endCnd: () => group.alpha < 1f,
                then: endaction);
            }
            else {
                RegisterUpdate.UpdateWhile(
                action: () => group.alpha -= .90f * Time.unscaledDeltaTime,
                endCnd: () => group.alpha < 0f,
                then: endaction);
            }
        }
    }

    #region SetCanvasScale
    
    private static CanvasScaler[] canvasScalers => FindObjectsOfType<CanvasScaler>();
    
    public static void SetCanvasScale(){
        Vector2 screenScale = new Vector2(Screen.width, Screen.height);

        if      (screenScale == new Vector2(1920, 1080))  ChangeCanvasScales(1f);
        else if (screenScale == new Vector2(1440, 720))   ChangeCanvasScales(.7f);
        else if (screenScale == new Vector2(2560, 1440))  ChangeCanvasScales(1.1f);
        else if (screenScale == new Vector2(1280, 720) || screenScale == new Vector2(1280, 869))  ChangeCanvasScales(.6f);
        else if (screenScale == new Vector2(2560, 1080)|| screenScale == new Vector2(2220, 1080)) ChangeCanvasScales(1.02f);
        else if (screenScale == new Vector2(2960, 1440)|| screenScale == new Vector2(3040, 1440)) ChangeCanvasScales(1.35f);
        else
            foreach (var item in canvasScalers)
                item.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    }

    private static void ChangeCanvasScales(float value){
        foreach (var item in canvasScalers)
            item.scaleFactor = value;
    }
    #endregion
}

public enum AvibleLanguages{
    Turkish, English , none
}

[Serializable]
public class Languages {
    public AvibleLanguages currentLanguage;
    public Language[] languages = new Language[2];
}

[Serializable]
public struct Texts {
    public Text medium, high, lowSetting, look, next, exitDialog, trade;
    public Text mainSound, sounds, sounds1, graphics, graphics1, gameplay, gameplay1, exit, resume, save, load, options;
}
