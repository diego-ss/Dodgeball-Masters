using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System.IO;

public class PlayerSelect : MonoBehaviour
{
    //vetor de personagens
    public List<GameObject> players;

    //posição do personagem atual
    public GameObject currentPlayerPositon;
    //posição do personagem anterior
    public GameObject previousPlayerPosition;
    //posição do próximo personagem
    public GameObject nextPlayerPosition;
    //texto do nome
    public TMP_Text nomeText;
    //texto da descricao
    public TMP_Text descricaoText;

    public AudioSource soundEffects;
    public AudioClip nextPreviousSound;
    public AudioClip selectedSound;

    private GameObject selectedPlayer;
    private GameObject nextPlayer;
    private GameObject previousPlayer;

    int index;
    Quaternion defaultRotation = Quaternion.Euler(0, 180, 0);

    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        //desativa o canvas de vida e stamina
        players.ForEach(p => p.transform.Find("Canvas").gameObject.SetActive(false));
    }

    // Update is called once per frame
    void Update()
    {
        // gestão da seleção de players
        if (selectedPlayer == null)
        {
            selectedPlayer = Instantiate(players[index], currentPlayerPositon.transform.position, defaultRotation);
            var charName = selectedPlayer.name.Replace("(Clone)", "");
            nomeText.text = charName;
            descricaoText.text = BuscarDescricao(charName);
            nextPlayer = Instantiate(players[(index + 1) % players.Count], nextPlayerPosition.transform.position, defaultRotation);
            previousPlayer = Instantiate(players[index == 0 ? players.Count - 1 : (index - 1) % players.Count], previousPlayerPosition.transform.position, defaultRotation);
        }
    }

    public void Next()
    {
        index++;

        if (index >= players.Count)
            index = 0;

        //limpando os players
        Destroy(selectedPlayer);
        Destroy(previousPlayer);
        Destroy(nextPlayer);
        soundEffects.PlayOneShot(nextPreviousSound);
    }

    public void Previous()
    {
        index--;

        if (index < 0)
            index = players.Count - 1;

        //limpando os players
        Destroy(selectedPlayer);
        Destroy(previousPlayer);
        Destroy(nextPlayer);
        soundEffects.PlayOneShot(nextPreviousSound);
    }

    public void Select()
    {
        soundEffects.PlayOneShot(selectedSound);

        //seta o player selecionado
        DontDestroyOnLoad(selectedPlayer);
        //carrega a cena arcade
        SceneManager.LoadScene("QuadraArcade_0", LoadSceneMode.Single);
    }

    private string BuscarDescricao(string Nome)
    {
        //Read the text from directly from the txt file
        TextAsset text = Resources.Load<TextAsset>($"TextFiles/{Nome}");
        return text.text;
    }
}
