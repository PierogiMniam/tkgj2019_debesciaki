﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;


public class Trees : MonoBehaviour
{

    public float infectTreeAfter;

    public AudioClip infectTreeClip;

    AudioSource audioSource;

    GameUi gameUi;

    delegate void ProcessHit(RaycastHit hit);

    void ProcessRayCast(ProcessHit processHit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            processHit(hit);
        }
    }


    void Start()
    {
        InitAudio();
        GameObject gameUiObject = GameObject.Find("GameUi");
        if (gameUiObject != null)
        {
            gameUi = gameUiObject.GetComponent<GameUi>();
        }
            InfectTree();
        InvokeRepeating("InfectTree", infectTreeAfter, infectTreeAfter);
    }

    void InitAudio()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu");
        }

        if (Input.GetMouseButtonUp(0))
        {
            ProcessRayCast((hit) =>
            {
                Tree tree = hit.collider.gameObject.GetComponent<Tree>();
                if (tree != null)
                {
                    Debug.Log(tree.name);
                    tree.SetDestroyed();
                }
            });
        }

        if (Input.GetMouseButtonUp(1))
        {
            ProcessRayCast((hit) =>
            {
                Tree tree = hit.collider.gameObject.GetComponent<Tree>();
                if (tree != null)
                {
                    Debug.Log(tree.name);
                    tree.SetMagical();
                }
            });
        }
    }

    void InfectTree()
    {
        Tree[] trees = transform.GetComponentsInChildren<Tree>().Where(x => x.MyState == Tree.State.NORMAL).ToArray();
        if (trees.Length > 0)
        {
            Tree randomTree = trees[Random.Range(0, trees.Length)];
            randomTree.SetInfected();
            audioSource.PlayOneShot(infectTreeClip);
        }
    }

    public void HealMetalTrees(int number)
    {
        List<Tree> trees = 
        new List<Tree>(transform.GetComponentsInChildren<Tree>().Where(x => x.MyState == Tree.State.METAL).ToArray());
        int count = number;
        while(trees.Count > 0 && count > 0)
        {
            Tree tree = trees[Random.Range(0, trees.Count)];
            tree.SetNormal();
            trees.Remove(tree);
            count--;
        }

    }

    public void OnTreeStateChanged()
    {
        Tree[] goodTrees =
            transform.GetComponentsInChildren<Tree>().Where(x => 
                x.MyState == Tree.State.GROWING ||
                x.MyState == Tree.State.NORMAL ||
                x.MyState == Tree.State.INFECTED ||
                x.MyState == Tree.State.MAGICAL
            ).ToArray();
        if (gameUi != null)
        {
            gameUi.UpdateTrees(goodTrees.Length);
            if (goodTrees.Length <= 0)
            {
                gameUi.SetGameOver();
            }
        }

    }


}
