using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum LoadMode
{
    Res,
    DownLoad,
}

public class LoadPanel : MonoBehaviour
{
    static LoadPanel _instance;
    public static LoadPanel instance
    {
        get
        {
            if (_instance == null)
            {
                var go = Resources.Load("LoadView/loading");
                if (go != null)
                {
                    _instance = (Instantiate(go) as GameObject).GetComponent<LoadPanel>();
                }
            }
            return _instance;
        }
    }

    public Slider slider;
    public Image  imgFill;

    public Text title;
    public Text speed;
    public Text all;

    public GameObject[] gos;
    public Button downBt;

   

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }


    public void Init()
    {
        this.all  .gameObject.SetActive(false);
        this.speed.gameObject.SetActive(false);
        this.downBt.gameObject.SetActive(false);
    }

    public void Init(UnityAction downCall)
    {
        all    . gameObject . SetActive(true);
        speed  . gameObject . SetActive(true);
        downBt . gameObject . SetActive(true);

        downBt.onClick.AddListener(() =>
        {
            downBt    . gameObject . SetActive(false);
            downCall? . Invoke();
            downCall  = null;
        });

    }

    public void HideSpeed()
    { 
        speed.gameObject.SetActive(false);
        all  .gameObject.SetActive(false);
    }

    public void Speed(string speed)
    {
        this.speed.text = speed;
    }

    public void All(string all)
    {
        this.all.text = all;
    }

    public void Title(string all)
    {
        this.title.text = all;
    }


    public void Radio(float progress)
    {
        slider.value = progress;
        title.text   = Mathf.RoundToInt(progress * 100.0f).ToString() + "%";
        imgFill.fillAmount = progress;

        foreach (var item in gos)
        {
            item.gameObject.SetActive(progress>0);
        }

        if (progress >= 1.0f)
        {
            title.text = "×ÊÔ´½âÎö";
        }
    }

    public void Done()
    {
        GameObject.Destroy(this.gameObject);
    }


}
