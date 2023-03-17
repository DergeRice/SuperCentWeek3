using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FloatingLv : MonoBehaviour
{

    Vector3 LVPosition;
    Vector3 ConPosition;

    public GameObject ThisLvObj,LVCanvas,ThisConObj;

    public float ThisObjectLV;
    [SerializeField] float FloatingY,FloatingX,FloatingZ;
    [SerializeField] GameObject LVText,Contry;

    [SerializeField] string nickname;
    float amount=0;

    public float PlayerLV;
    [SerializeField] float TempLV=0;
    float index=1;

    Vector3 temp;
    void Awake()
    {
        LVCanvas =GameObject.Find("LVCanvas");
        LVPosition = Camera.main.WorldToScreenPoint(gameObject.transform.position+new Vector3(FloatingX,FloatingY,FloatingZ));
        ConPosition = Camera.main.WorldToScreenPoint(gameObject.transform.position)+new Vector3(FloatingX,FloatingY,FloatingZ);
        // ThisLvObj = Instantiate(LVText,LVPosition,Quaternion.identity,LVCanvas.transform);
        // ThisConObj = Instantiate(Contry,ConPosition,Quaternion.identity,LVCanvas.transform);
    }
    // Start is called before the first frame update
    void Start()
    {

         temp=Camera.main.gameObject.GetComponent<CameraFollow>()._minValues;
    }

 float d=1;
     // Update is called once per frame
    void Update()
    {
        PlayerLV= GameObject.Find("Player").GetComponent<FloatingLv>().ThisObjectLV;

        LvFloating();
    }
   
    [SerializeField] float speed;

    
    void LvFloating(){
        if(Camera.main!=null){
        LVPosition = Camera.main.WorldToScreenPoint(gameObject.transform.position+new Vector3(0,0,FloatingY));
        ConPosition = Camera.main.WorldToScreenPoint(gameObject.transform.position+new Vector3(FloatingX,0,FloatingY));
        }
        if(ThisLvObj !=null){
            ThisLvObj.transform.position = LVPosition;
            ThisConObj.transform.position = ConPosition;
            ThisLvObj.GetComponent<Text>().text = nickname;  
        }
    }
}
