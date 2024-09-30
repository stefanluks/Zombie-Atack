using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using Unity.VisualScripting;

public class RedeController : MonoBehaviour
{
    public static RedeController _instancia { get; private set; }
    public string _url = "https://thousand-sunny-api.onrender.com/";

    private void Awake()
    {
        if (_instancia != null && _instancia != this)
        {
            Destroy(this.gameObject);
        } else {
            _instancia = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    [Obsolete]
    public IEnumerator SavePontuacao(Pontuacao Jogador, Action<bool> callback){
        string url_montado = _url + "jogador";
        string json_string = JsonUtility.ToJson(Jogador);
        UnityWebRequest req = UnityWebRequest.Post(_url+"jogador", json_string);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json_string);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        // req.downloadHandler = new DownloadHandlerBuffer();

        yield return req.SendWebRequest();

        if(req.isHttpError || req.isNetworkError){
            callback(false);
        }
        callback(true);

    }

    [Obsolete]
    public IEnumerator GetRanking(int JOGOID, Action<Ranking> callback){
        string url_montado = _url+"rankingGeral/"+JOGOID;
        UnityWebRequest www = UnityWebRequest.Get(url_montado);
        www.downloadHandler = new DownloadHandlerBuffer();

        Ranking rank = new Ranking();

        yield return www.SendWebRequest();

        if(www.isHttpError || www.isNetworkError){
            rank = null;
        }else{
            string text = www.downloadHandler.text;
            rank = JsonUtility.FromJson<Ranking>(text);
        }
        callback(rank);
    }

}
