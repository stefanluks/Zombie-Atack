using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InfoJogo{
    public int id;
    public string nome;
    public string descricao;
    public List<Pontuacao> ranking;

}


[System.Serializable]
public class Ranking{
    public bool error;
    public InfoJogo content;
}