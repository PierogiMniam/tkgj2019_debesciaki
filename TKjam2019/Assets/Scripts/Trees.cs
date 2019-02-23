﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Trees : MonoBehaviour
{

    public float infectTreeAfter;

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
        InfectTree();
        InvokeRepeating("InfectTree", infectTreeAfter, infectTreeAfter);
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            ProcessRayCast((hit) =>
            {
                Tree tree = hit.collider.gameObject.GetComponent<Tree>();
                if (tree != null)
                {
                    Debug.Log(tree.name);
                    tree.MyState = Tree.State.DESTROYED;
                }
            });
        }
    }

    void InfectTree()
    {
        Tree[] trees = transform.GetComponentsInChildren<Tree>().Where(x => x.MyState == Tree.State.NORMAL).ToArray();
        Tree randomTree = trees[Random.Range(0, trees.Length)];
        randomTree.MyState = Tree.State.INFECTED;
    }
}