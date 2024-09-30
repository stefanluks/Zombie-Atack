using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Disparo : MonoBehaviour
{
    [SerializeField] private int velocidade;

    // Update is called once per frame
    void Update()
    {
        if(!GameController._instancia.idPaused()){
            if(transform.position.x < 20) transform.position += new Vector3(1f, 0f, 0f) * velocidade * Time.deltaTime;
            else Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D obj){
        if(obj.gameObject.tag == "zumbi"){
            obj.gameObject.GetComponent<Zombie>().Dano();
            Destroy(gameObject);
        }
    }
}
