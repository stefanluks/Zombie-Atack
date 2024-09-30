using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.Mathematics;

public class GameController : MonoBehaviour
{
    public static GameController _instancia {private set; get;}

    [Header("CONFIG's")]
    [SerializeField] private int JOGO_ID = 14;
    [SerializeField] private List<GameObject> telas;
    [SerializeField] private bool jogando = false;
    [SerializeField] private bool pause = false;
    [SerializeField] private bool muteMusica = false;
    [SerializeField] private bool carregando = false;
    [SerializeField] private Button btnSom;
    [SerializeField] private Sprite somLigado;
    [SerializeField] private Sprite somDesligado;
    [SerializeField] private GameObject spawn;
    [SerializeField] private int pontos = 0;
    [SerializeField] private int nivel = 1;
    [SerializeField] private int balas = 5;
    [SerializeField] private int Maxbalas = 5;
    [SerializeField] private int cont = 0;
    [SerializeField] private int tempo = 500;


    [Header(" SOM CONTROL ")]
    [SerializeField] private AudioSource trilhaSonora;
    [SerializeField] private List<AudioSource> EfeitosSonoros;


    [Header("HUD")]
    [SerializeField] private Slider _barraDeVida;
    [SerializeField] private Text pontuacao;
    [SerializeField] private Text qtdBalas;
    [SerializeField] private InputField entradaNome;
    [SerializeField] private GameObject msg_sucesso;
    [SerializeField] private GameObject msg_falha;
    [SerializeField] private GameObject msg_carregando;

    
    [Header("-- RANKING --")]
    [SerializeField] private List<Text> _fieldsRanking;


    [Header("Zumbi controller")]

    [SerializeField] private GameObject zumbi;
    [SerializeField] private List<GameObject> zumbis;
    
    void Awake(){
        if (_instancia != null && _instancia != this)
        {
            Destroy(this.gameObject);
        } else {
            _instancia = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        SelecionarTela(0);
        _barraDeVida.minValue = 0;
        _barraDeVida.maxValue = 100;
        _barraDeVida.value = 100;
        msg_falha.SetActive(false);
        msg_sucesso.SetActive(false);
    }

    void SelecionarTela(int indice){
        foreach (GameObject tela in telas){ tela.SetActive(false); }
        telas[indice].SetActive(true);
    }

    public void SomControle(){
        muteMusica = !muteMusica;
        if(muteMusica) btnSom.image.sprite = somDesligado;
        else btnSom.image.sprite = somLigado;
    }

    string AjustePontos(int pontos){
        string saida = "";
        if(pontos < 10) saida = "000"+pontos;
        else if(pontos < 100) saida = "00"+pontos;
        else if(pontos < 1000) saida = "0"+pontos;
        else saida = ""+pontos;
        return saida;
    }

    void Update()
    {
        SomController();
        if(jogando && !pause){
            ControleDeVidas();
            ControleZumbis();
            ControlePontosNivel();
            ControleBalas();
            pontuacao.text = "Pontos: "+AjustePontos(pontos)+"\nNível: "+nivel;
            qtdBalas.text = "X "+balas+"/"+Maxbalas;
        }
    }

    public bool idPaused(){
        return pause;
    }

    public int GetNivel(){
        return nivel;
    }

    public void Pausar(){
        pause = !pause;
        if(pause) SelecionarTela(3);
        else SelecionarTela(2);
    }

    void ControleBalas(){
        if(nivel * 5 < 50) Maxbalas = (nivel * 5);         
        else Maxbalas = 50;
        if(balas == Maxbalas) carregando = false;
    }

    void ControlePontosNivel(){
        if(pontos/100 == nivel){
            nivel++;
            EfeitosSonoros[3].Play();
            if(tempo > 30) tempo -= 20;
        }
    }

    void AddZumbi(){
        GameObject z = Instantiate(zumbi, spawn.transform.position, Quaternion.identity);
        z.GetComponent<Zombie>().velocidade = nivel + 1;
        zumbis.Add(z);
    }

    void ControleZumbis(){
        if(zumbis.Count < 1) AddZumbi();
        if(nivel > 1){
            if(cont < tempo){
                cont++;
            }else{
                cont = 0;
                AddZumbi();
            }
        }
    }

    public void ZumbiDead(GameObject zumbi){
        zumbis.Remove(zumbi);
        pontos += 5;
    }

    public void RankingPage(){
        SelecionarTela(1);
        StartCoroutine(RedeController._instancia.GetRanking(JOGO_ID, ConfigurarRaking));
    }

    public void Tela(int indice){
        SelecionarTela(indice);
    }

    void ConfigurarRaking(Ranking lista){
        if(lista != null){
            for(int i=0;i< 10; i++)
            {
                if(i >= lista.content.ranking.Count) _fieldsRanking[i].gameObject.SetActive(false);
                else _fieldsRanking[i].text = lista.content.ranking[i].nome +"\n"+"Pontos:"+lista.content.ranking[i].pontos;
            }
        }
    }

    void SomController(){
        trilhaSonora.mute = muteMusica;
        foreach (AudioSource som in EfeitosSonoros) som.mute = muteMusica;
        foreach (GameObject zombie in zumbis) zombie.GetComponent<Zombie>().Som(muteMusica);
    }

    void ControleDeVidas(){
        int vida = Player._instancia.GetVida();
        _barraDeVida.value = vida;
        if(vida <= 0){
            jogando = false;
            pause = true;
            SelecionarTela(5);
        }
    }

    public void Jogar(){
        jogando = true;
        SelecionarTela(2);
    }

    public void Reiniciar(){
        jogando = true;
        pause = true;
        pontos = 0;
        Maxbalas = 5;
        balas = 5;
        nivel = 1;
        cont = 0;
        tempo = 500;
        Player._instancia.SetVida(100);
        foreach (GameObject zumbi in zumbis) { Destroy(zumbi); }
        zumbis = new List<GameObject>();
        SelecionarTela(0);
    }

    public void SalvarPontuacao(){
        Pontuacao jogador = new Pontuacao();
        jogador.nome = entradaNome.text;
        jogador.pontos = ""+pontos;
        jogador.jogo = JOGO_ID;
        entradaNome.gameObject.SetActive(false);
        StartCoroutine(RedeController._instancia.SavePontuacao(jogador, respostaSalvar));
    }

    void respostaSalvar(bool resposta){
        if(resposta) msg_sucesso.SetActive(true);
        else msg_falha.SetActive(false);
        Invoke("FecharALerta",1f);
    }

    void FecharALerta(){
        msg_falha.SetActive(false);
        msg_sucesso.SetActive(false);
    }

    public void Atirar(){
        if(balas > 0 && !carregando){
            balas--;
            Player._instancia.disparar();
            EfeitosSonoros[1].Play();
        }else if(balas < Maxbalas){
            carregando = true;
            Player._instancia.Recarregar();
            EfeitosSonoros[2].Play(); //Click sem bala
            balas++;
        }
    }

    public void ToqueBotao(){
        EfeitosSonoros[0].Play();
    }

    public void VoltarParaMenu(){
        jogando = false;
        pause = false;
        pontos = 0;
        Maxbalas = 5;
        balas = 5;
        nivel = 1;
        cont = 0;
        tempo = 500;
        Player._instancia.SetVida(100);
        foreach (GameObject zumbi in zumbis) { Destroy(zumbi); }
        zumbis = new List<GameObject>();
        SelecionarTela(0);
    }
}
