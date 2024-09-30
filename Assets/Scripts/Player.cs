using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player _instancia {private set; get;}
    [SerializeField] private int vida = 100;
    [SerializeField] private GameObject arma;
    [SerializeField] private GameObject efeito;
    [SerializeField] private GameObject Disparo;

     private void Awake() {
        if (_instancia != null && _instancia != this)
        {
            Destroy(this.gameObject);
        } else {
            _instancia = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    public int GetVida(){
        return vida;
    }

    public void SetVida(int valor){
        vida = valor;
    }

    void Update()
    {
        if(!GameController._instancia.idPaused()) ControleVida();
    }

    public void Recarregar(){
        GetComponent<Animator>().SetTrigger("recarregar");
    }

    public void disparar(){
        if(!GameController._instancia.idPaused()){
            efeito.SetActive(true);
            GetComponent<Animator>().SetTrigger("atirar");
            Invoke("DesativarEfeito",.2f);
            Instantiate(Disparo, arma.transform.position, Quaternion.identity);
        }
    }

    void DesativarEfeito(){
        efeito.SetActive(false);
    }

    void ControleVida(){
    }

    public void Dano(){
        vida -= 5;
    }
}
