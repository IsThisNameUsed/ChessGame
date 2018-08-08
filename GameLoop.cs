using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{

    public Camera CurrentCamera;
    public GameObject pieceSelected, target;
    public string color, vscolor, temp;
    int moveOK;

    void Start()
    {
        if (CurrentCamera == null)
        {
            CurrentCamera = Camera.main;
        }

        //initialisation du graveyard (toutes cases libres) et des positions des pièces
        BoardManagment.startPosition();
        VictoryConditions.initializeCheck();
        //Color sert a déterminer quel joueur à le focus
        color = "Black";
        vscolor = "White";
    }


    void Update()
    {
        //Si clique gauche
        if (Input.GetMouseButtonDown(0) && CurrentCamera != null)
        {
            RaycastHit hit;
            //Lance un rayon de la caméra vers le curseur souris
            Ray ray = CurrentCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 500))
            {
                MeshRenderer meshRenderer = hit.transform.GetComponent<MeshRenderer>();
                //Si l'objet touché possède un Box collider et le tag "WhitePiece" ou "BlackPiece
                if (meshRenderer != null && hit.collider.transform.tag == color + "Piece")
                {
                    //Si une piece a déjà été sélectionnée elle est désélectionné
                    if (pieceSelected != null)
                    {
                        BoardStateManagment.eraseControlAreaOf(pieceSelected);
                        BoardManagment.pieceLooseFocus(pieceSelected, color);
                    }
                    //La nouvelle pièce ciblé devient sélectionnée             
                    pieceSelected = GameObject.Find(hit.collider.transform.name);
                    GameObject selectionLight = GameObject.Find(pieceSelected.name + "_Spotlight");
                    selectionLight.GetComponent<Light>().enabled = !selectionLight.GetComponent<Light>().enabled;
                    //Animation de sélection
                    if (pieceSelected.GetComponent<Animator>() != null)
                    {
                        pieceSelected.GetComponent<Animation>().Play("pawnSelectionAnimation");
                    }
                    BoardStateManagment.displayControlAreaOf(pieceSelected);
                }

            }
        }
        //Si clique gauche
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = CurrentCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 500) && pieceSelected != null)
            {
                MeshRenderer meshRenderer = hit.transform.GetComponent<MeshRenderer>();
                target = GameObject.Find(hit.transform.name);
                if (meshRenderer != null && (hit.transform.tag == "Tile") || (hit.transform.tag == vscolor + "Piece"))
                {
                    moveOK = BoardManagment.movePiece(target, pieceSelected);
                }
            }
            if (moveOK == 0)
            {
                BoardManagment.pieceLooseFocus(pieceSelected, color);
                Debug.Log("GameLoop: verif echec");
                if (VictoryConditions.kingInCheck(vscolor))
                {
                    if (vscolor == "White")
                    {
                        VictoryConditions.whiteKingInCheck = true;
                    }
                    else
                    {
                        VictoryConditions.blackKingInCheck = true;  
                    }
                    Debug.Log("VictoryConditions.searchMovementAuthorized");
                    VictoryConditions.searchMovementAuthorized(vscolor);
                }
                temp = vscolor;
                vscolor = color;
                color = temp;
                pieceSelected = null;
                moveOK = 1;
            }

        }
    }
}
