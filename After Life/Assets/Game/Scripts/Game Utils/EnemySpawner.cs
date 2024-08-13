using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{

    #region VS757


    [SerializeField] bool shouldSpawnVS757;
    [SerializeField] GameObject vs757;


    [SerializeField] List<Transform> vs757SpawnPoints = new List<Transform>();

    GameObject vs757sp;

    [SerializeField] RuntimeAnimatorController vsController;
    [SerializeField] RuntimeAnimatorController vfxController;

    #endregion




    // Start is called before the first frame update
    void Start()
    {
        if(shouldSpawnVS757){
            vs757sp = GameObject.FindGameObjectWithTag("vs757sp");

            vs757SpawnPoints = new List<Transform>();
            foreach(Transform child in vs757sp.transform){

                vs757SpawnPoints.Add(child);
            }

            for(int i = 0; i < vs757SpawnPoints.Count; i++){

                GameObject vsInstance = GameObject.Instantiate(vs757, vs757SpawnPoints[i].position, vs757SpawnPoints[i].rotation);
                vsInstance.transform.parent = GameObject.FindGameObjectWithTag("Enemies List").transform;
                VS757 vs757Comp = vsInstance.GetComponentInChildren<VS757>();
                GameObject flamer = vsInstance.transform.GetChild(1).gameObject;
                Animator flamerAnim = flamer.GetComponent<Animator>();

                vs757Comp.ID = i;
                vs757Comp.vsa.runtimeAnimatorController = vsController;
                flamerAnim.runtimeAnimatorController = vfxController;

                vs757Comp.vsa.GetBehaviour<VS757IdleScript>().ID = i;
                vs757Comp.vsa.GetBehaviour<VS757DeathScript>().ID = i;
                vs757Comp.vsa.GetBehaviour<VS757HitScript>().ID = i;
                vs757Comp.vsa.GetBehaviour<VS757RunScript>().ID = i;
                vs757Comp.vsa.GetBehaviour<VS757SurpriseScript>().ID = i;
                vs757Comp.vsa.GetBehaviour<AtkBScript>().ID = i;
                vs757Comp.vsa.GetBehaviour<AtkMScript>().ID = i;
            }
        }
    }
}
