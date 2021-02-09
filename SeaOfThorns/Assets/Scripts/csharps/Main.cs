using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum GState{
    ATTITLE,
    GAMING,
    DEAD
}

public class InteractiveItem
{
    public InteractiveItem(int w)
    {
        this.weight = w;
    }
    public int weight = 1;
}   

public class GameingState
{
    public Main mainscript;
    private GState gstate = GState.ATTITLE;
    private static GameingState instance;
    public GState CurrentState{
        get{
            return gstate;
        }
        set{
            gstate = value;
        }
    }

    private Dictionary<string, InteractiveItem> itemsConfig;
    public static GameingState Instance
    {
        get{
            if(instance == null)
            {
                instance = new GameingState();
            }
            return instance;
        }
    }

    public GameingState()
    {
        itemsConfig = new Dictionary<string, InteractiveItem>();
        InteractiveItem item1 = new InteractiveItem(1);
        InteractiveItem item2 = new InteractiveItem(2);
        itemsConfig["Kind"] = item1;
        itemsConfig["Evil"] = item2;
    }

    public InteractiveItem GetItemConfig(string name)
    {
        return itemsConfig[name];
    }
    public GameObject NextIsland;
    public Transform SceneRoot;

    public void UpdateWeight(float value)
    {
        mainscript.SetWeightSlider(value);
        if(value <= 0)
        {
            mainscript.GameOver();
        }
    }


}

public class Main : MonoBehaviour
{
    public Transform TitleRoot;
    public Transform GameOverImg;
    public Transform SceneRoot;

    public Transform Compass;
    Transform compass;
    public GameObject tempisland;

    public Transform WeightSlider;
    
    Slider slider;
    Text sliderText;
    ColorBlock sliderColorBlock;

    Transform boat;
    BoatMove boatmove;
    // Start is called before the first frame update
    void Start()
    {
        GameingState.Instance.mainscript = this;
        GameingState.Instance.NextIsland = tempisland;
        GameingState.Instance.SceneRoot = SceneRoot;
        boat = SceneRoot.Find("Boat");

        boatmove = boat.GetComponent<BoatMove>();
        
        sliderColorBlock = new ColorBlock();
        sliderColorBlock.normalColor = Color.white;
        sliderColorBlock.highlightedColor = Color.white;
        sliderColorBlock.pressedColor = Color.white;
        sliderColorBlock.selectedColor = Color.white;
        sliderColorBlock.colorMultiplier = 1;
        sliderColorBlock.fadeDuration = .1f;
        slider = WeightSlider.GetComponent<Slider>();
        sliderText = slider.transform.Find("Handle Slide Area/Handle/Text").GetComponent<Text>();
        if(slider != null)
        {
        }

        compass = Compass.Find("Pointer");
        Compass.gameObject.SetActive(false);
        WeightSlider.gameObject.SetActive(false);
        GameOverImg.gameObject.SetActive(false);
    }


    void StartGame()
    {
        if(Input.GetMouseButtonDown(0))
        {
            boatmove.weight = 1;
            SetWeightSlider(1);
            foreach(Transform texttrans in TitleRoot)
            {
                Text text = texttrans.GetComponent<Text>();
                float dd = text.color.a;
                text.DOColor(new Color(0,0,0,0), 5f);
            }
            GameingState.Instance.CurrentState = GState.GAMING;
            Compass.gameObject.SetActive(true);
            WeightSlider.gameObject.SetActive(true);
            GameOverImg.gameObject.SetActive(false);
        }
    }

    public void GameOver()
    {
        GameingState.Instance.CurrentState = GState.ATTITLE;
        GameOverImg.gameObject.SetActive(true);
    }

    public void SetWeightSlider(float value)
    {
        if(slider != null)
        {
            slider.value = value;
            sliderColorBlock.disabledColor = Color.Lerp(new Color(.93f, .267f, .189f, 1f), new Color(.024f, 0.839f, 0.627f,1f), value);
            sliderText.color = Color.Lerp(new Color(.93f, .267f, .189f, 1f), new Color(.024f, 0.839f, 0.627f,1f), value);
            slider.colors = sliderColorBlock;
        }
    }
    public void SetCompassDir(Vector2 dir)
    {
        if(dir.y > 0)
            compass.transform.eulerAngles = Vector3.forward * Vector2.Angle(Vector2.right, dir);
        else
            compass.transform.eulerAngles = Vector3.forward * Vector2.Angle(Vector2.left, dir) + Vector3.forward * 180;
    }
    // Update is called once per frame
    void Update()
    {
        SetCompassDir(Util.V3ToV2(GameingState.Instance.NextIsland.transform.position) - Util.V3ToV2(boat.position));
        if(Input.GetMouseButtonDown(1))
        {
            //重置坐标
            Camera.main.transform.position -= Util.KeepY(GameingState.Instance.NextIsland.transform.position, 0);
            foreach(Transform child in SceneRoot)
            {
                child.localPosition -= GameingState.Instance.NextIsland.transform.localPosition;
            }
        }
        switch(GameingState.Instance.CurrentState)
        {
            case GState.ATTITLE: StartGame(); break;
            case GState.GAMING: break;
            case GState.DEAD: break;
        }
    }
}
