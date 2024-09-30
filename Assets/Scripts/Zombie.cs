using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Zombie : MonoBehaviour
{

    [SerializeField] private GameObject Player;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource somPassos;
    [SerializeField] private AudioSource Barulho;
    [SerializeField] public int vida = 20;
    [SerializeField] public int MaxLife = 20;
    [SerializeField] public int velocidade = 2;
    [SerializeField] private Transform barraVida;
    [SerializeField] private Transform barraVidaBG;

    private int cont = 0, tempo = 50;
    private bool atacando = false, tocouPlayer = false, vivo = true;
    void Start()
    {
        animator = GetComponent<Animator>();
        Player = GameObject.Find("Player");
    }

    void Update()
    {
        if(!GameController._instancia.idPaused()){
            Vector3 movimento = new Vector3(0, 0, 0);
            if(!tocouPlayer){
                movimento.x = -1f;
                animator.SetInteger("controle", 1);
            }else{
                movimento.x = 0f;
                if(cont < tempo && !atacando){
                    cont++;
                    animator.SetInteger("controle", 0);
                }else{
                    Atack();
                    cont = 0;
                    atacando = true;
                }
            }
            transform.position += movimento * velocidade * Time.deltaTime;
            controleVida();
        }
    }

    void controleVida(){
        if(vida <= 0 && vivo) {
            animator.SetTrigger("morreu");
            GameController._instancia.ZumbiDead(gameObject);
            vivo = false;
        }
        MaxLife = GameController._instancia.GetNivel() * 20;
        float percent = (vida * 100) / MaxLife;
        float percentBG = (percent * barraVidaBG.localScale.x) / 100;
        Debug.Log("%: "+percent+" | f: "+percentBG);
        Vector3 atual = barraVida.localScale;
        barraVida.localScale = new Vector3(percentBG, atual.y, atual.z);
    }

    public void Destruir(){
        Destroy(gameObject);
    }

    void Atack(){
        if(!atacando) animator.SetInteger("controle", 2);
    }

    public void FimAtaque(){
        Invoke("ParaAtaque",.1f);
        if(tocouPlayer) Player.GetComponent<Player>().Dano();
    }

    public void Som(bool mute){
        somPassos.mute = mute;
        Barulho.mute = mute;
    }

    public void Dano(){
        if(vivo){
            vida -= 5;
            animator.SetTrigger("dano");
        }
    }

    void ParaAtaque(){
        atacando = false;
    }

    void OnCollisionEnter2D(Collision2D col){
        if(col.gameObject == Player){
            tocouPlayer = true;
        }
    }
}
