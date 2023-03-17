using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyCS : MonoBehaviour
{
    [SerializeField] float Radi;
    [SerializeField] GameObject Player;
    //int PlayerLayerMask;
    int PlayerMask,WallMask;
    NavMeshAgent NavMeshAgent;
    NavMeshPath NavMeshPath;

    Animator _Animator;

    
    [SerializeField] bool IAMStunned;

    bool Death;

    [SerializeField] Collider[] Col;
    Vector3 origin_Navi_Speed;
    

    [SerializeField] GameObject StunEff;
    GameObject KnockBackObject;

    int stagenum = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        stagenum = 1;
        Player = GameObject.Find("Player");
        NavMeshPath = new NavMeshPath();
        _Animator = GetComponent<Animator>();
        _Animator.SetFloat("RanPar",Random.Range(0,1f));
        PlayerMask = 1  << LayerMask.NameToLayer("Player");  
        WallMask = 1  << LayerMask.NameToLayer("Wall");  
       InvokeRepeating(nameof(SetNavMesh),0f,4f);
       NavMeshAgent = GetComponent<NavMeshAgent>();
       origin_Navi_Speed = NavMeshAgent.velocity;
      
    }

    // Update is called once per frame
    void Update()
    {
       // FindPlayer();
        SetAnimation();
       
       // AmIDead();
        //IWantToWakeUP();
        if(KnockBackObject!=null){hitKnockBack();}
    
    }
    void hitKnockBack(){
        KnockBackObject.transform.position = KnockBackObject.transform.position-((transform.position-KnockBackObject.transform.position).normalized*0.03f);
    }

    // void FindPlayer(){
    //      Col =  Physics.OverlapSphere(transform.position+((transform.position-NavMeshAgent.destination).normalized*-4),Radi);
    //    for(int i = 0;i<Col.Length;i++){
    //     if(Col[i].tag=="Player"||Col[i].tag=="Enemy"){ //싸다구 대상 발견
    //         _Animator.SetBool("AttackBool",true);
    //                  HitDamage(Col[i]);
    //     }
    //     }
        
    // }


    // void IWantToWakeUP(){
    //     if(_Animator.GetCurrentAnimatorStateInfo(0).IsName("Stunned")){
             
             
    //        // IAMStunned = true;
            
    //     }
    //     else
    //         { 
          
    //         //IAMStunned = false;
            
    // }

    void RecoveryFromStun(){
        if(!Death){
         StunEff.SetActive(false);
        _Animator.SetBool("Stunned",false);
         _Animator.SetBool("AttackBool",false);
        IAMStunned = false;
        NavMeshAgent.isStopped = false;
        NavMeshAgent.velocity = origin_Navi_Speed;
        }
    }
    // void AmIDead(){
    //     if(_Animator.GetCurrentAnimatorStateInfo(0).IsName("ImDead")){
            
    //         CancelInvoke(nameof(SetNavMesh));
    //        // NavMeshAgent =null;
    //         //GetComponent<NavMeshAgent>().enabled =false;
    //         Invoke(nameof(Killme),0.3f);
            
    //     }
    // }

    public void IamDead(){
         CancelInvoke(nameof(SetNavMesh));
         
         NavMeshAgent.isStopped = true;
         NavMeshAgent.velocity = Vector3.zero;
         _Animator.Play("ImDead");
         transform.GetChild(0).localScale = new Vector3(1.2f,0.1f,1.2f);
         Death =true;
         GetComponent<CapsuleCollider>().enabled = false;
         GetComponent<NavMeshAgent>().enabled = false;
         transform.position = new Vector3(transform.position.x,0.13f,transform.position.z);
         StunEff.SetActive(false);
         Invoke(nameof(TransMom),1f);
         Invoke(nameof(GoDown),3f);
    }
    void TransMom(){
        transform.parent = transform.parent.parent;
    }

    void GoDown(){
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        GetComponent<Rigidbody>().useGravity =true;
        Invoke(nameof(Killme),1f);

    }

    void Killme(){

        GameObject lvText = GetComponent<FloatingLv>().ThisLvObj;
        GameObject ConObj = GetComponent<FloatingLv>().ThisConObj;
            
        Destroy(lvText); 
        Destroy(gameObject);
    }

    
    
    void SetAnimation(){
        if(NavMeshAgent!=null){
        if(NavMeshAgent.velocity.magnitude>0.01f){
        _Animator.SetBool("Walking",true);
       }
       else{
        _Animator.SetBool("Walking",false);
       }}
    }

    void SetNavMesh(){
        NavMeshHit Hit;
        //int d,f;
        // for (int i = 0; i < 30; i++)
        // {
        //     d= Random.Range(0,10);
        //             if(NavMesh.SamplePosition(new Vector3(Random.Range(-100f,100f),4,Random.Range(-100f,100f)),out Hit,200f,d)){
        //                 f =d;
        //                 Debug.Log(f);
        //             }
        // }
        if(NavMesh.SamplePosition(new Vector3(Random.Range(-100f,100f),4,Random.Range(-100f,100f)),out Hit,200f,8)){//stagemanger should change value of stage num to 1>>4 not 2;
             
             if(NavMeshAgent!=null) 
                NavMeshAgent.SetDestination(Hit.position);
        }
        else
            SetNavMesh();
    }

    private void OnCollisionEnter(Collision other) {
          if(!IAMStunned){DoAttack(other.collider);}
    }
    void StopKnockBack(){
        KnockBackObject = null;

    }

    void DoAttack(Collider other){
        if(!IAMStunned){
            if(other.tag =="Player"||other.tag=="Enemy"){
                transform.LookAt(other.transform.position);
                    GameObject pre = Resources.Load("Prefabs/Particles/RedHit") as GameObject;
                    Instantiate(pre,other.transform.position+(Vector3.up*3),Quaternion.identity);
                    KnockBackObject = other.gameObject;
                    Invoke(nameof(StopKnockBack),1.5f);
                      _Animator.SetBool("AttackBool",true);
                      Invoke(nameof(HitCoolTime),0.1f);
                      HitDamage(other);
            }
        }
    }
    void HitCoolTime(){
        _Animator.SetBool("AttackBool",false);
    }
    void HitDamage(Collider other){
       if(other.gameObject.GetComponent<EnemyCS>() !=null) other.gameObject.GetComponent<EnemyCS>().IwasSlapped();
       if(other.gameObject.GetComponent<Player>() !=null) other.gameObject.GetComponent<Player>().IwasSlapped();
    }

    public void IwasSlapped(){
        
       StunEff.SetActive(true);
        IAMStunned=true;
        NavMeshAgent.isStopped = true;
         NavMeshAgent.velocity = Vector3.zero;
        _Animator.SetBool("Stunned",true);

        Invoke(nameof(RecoveryFromStun),1.5f);
    }

    // private void OnDestroy() {
    //     GameObject _uimanager,_uimanager2;
    //     UIManager _ui;

    //     // _uimanager = GameObject.FindGameObjectWithTag("UIManager");
    //     // _uimanager2 = GameObject.FindGameObjectWithTag("UIManager");
    //     _ui = _uimanager.gameObject.GetComponent<UIManager>();
    //     _ui._enemyCount++;
    //         _ui._enemyCountText.text = _ui._enemyCount.ToString() + " / " + _ui.EnemyCountMax.ToString();
    // }
}
