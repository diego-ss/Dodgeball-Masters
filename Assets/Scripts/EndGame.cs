using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class EndGame : MonoBehaviour
{
    //vetor de personagens
    public List<GameObject> enemies;

    //posição do personagem atual
    public GameObject currentPlayerPositon;

    private Quaternion defaultRotation = Quaternion.Euler(0, 180, 0);
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = currentPlayerPositon.transform.position;
        player.transform.rotation = defaultRotation;

        ConfigurarPersonagens();

        // animação de movimento da câmera
        var camera = Camera.main;
        var initialPosition = camera.transform.position;

        Sequence tweenSeq = DOTween.Sequence();
        tweenSeq.PrependInterval(2);
        tweenSeq.Append(camera.transform.DOMove(new Vector3(0, 2, -3f), 5));
        tweenSeq.AppendInterval(3);
        tweenSeq.Append(camera.transform.DOMove(new Vector3(0, 5, -2.5f), 10)).Join(camera.transform.DORotate(new Vector3(50, 0, 0), 5));
        tweenSeq.AppendInterval(2);
        tweenSeq.Append(camera.transform.DOMove(new Vector3(6, 1.5f, -2f), 5)).Join(camera.transform.DORotate(new Vector3(10, -45, 0), 5));
        tweenSeq.AppendInterval(2);
        tweenSeq.Append(camera.transform.DOMove(new Vector3(0, 1.5f, -3.5f), 10)).Join(camera.transform.DORotate(new Vector3(10, 0, 0), 8));
        tweenSeq.PrependInterval(2);
        tweenSeq.Append(camera.transform.DOMove(initialPosition, 5));

        tweenSeq.SetLoops(-1);
        tweenSeq.Play();
    }

    void ConfigurarPersonagens()
    {
        player.transform.Find("Canvas").gameObject.SetActive(false);
        player.GetComponent<Animator>().Play("Dancing");
        player.GetComponent<PlayerController>().enabled = false;

        enemies.ForEach(p =>
        {
            p.transform.Find("Canvas").gameObject.SetActive(false);
            p.SetActive(true);
            p.GetComponent<Animator>().Play("Dancing");
            p.GetComponent<EnemyController>().enabled = false;
        });

    }

    void PosicionarPersonagem(GameObject character, GameObject positionRef, bool instantiate = true)
    {
        if (instantiate)
            Instantiate(character, positionRef.transform.position, defaultRotation);
        else
        {
            character.transform.position = positionRef.transform.position;
            character.transform.rotation = defaultRotation;
        }
    }
}
