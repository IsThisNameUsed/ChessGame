using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using UnityEngine;

public static class BoardManagment
{

    public static GameObject[,] tabPosition = new GameObject[9, 9];
    public static GameObject GraveYardTile;
    public static GraveyardManagment graveyard = new GraveyardManagment();
    
    //Range les pièces à leurs positions de départ
    public static void startPosition()
    {
        int j = 1;
        GameObject Wpawn, Bpawn, piece;

        for (j = 1; j <= 8; j++)
        {
            Wpawn = GameObject.Find("Wpawn" + j);
            toPlacePiece(Wpawn, 2, j);
            Bpawn = GameObject.Find("Bpawn" + j);
            toPlacePiece(Bpawn, 7, j);
        }

        //placer les pièces blanches
        piece = GameObject.Find("Wknight2");
        toPlacePiece(piece, 1, 2);
        piece = GameObject.Find("Wknight1");
        toPlacePiece(piece, 1, 7);
        piece = GameObject.Find("Wrook1");
        toPlacePiece(piece, 1, 8);
        piece = GameObject.Find("Wrook2");
        toPlacePiece(piece, 1, 1);
        piece = GameObject.Find("Wking");
        toPlacePiece(piece, 1, 5);
        piece = GameObject.Find("Wqueen");
        toPlacePiece(piece, 1, 4);
        piece = GameObject.Find("Wbishop1");
        toPlacePiece(piece, 1, 6);
        piece = GameObject.Find("Wbishop2");
        toPlacePiece(piece, 1, 3);

        //placer les pièce noires
        piece = GameObject.Find("Bknight2");
        toPlacePiece(piece, 8, 2);
        piece = GameObject.Find("Bknight1");
        toPlacePiece(piece, 8, 7);
        piece = GameObject.Find("Brook1");
        toPlacePiece(piece, 8, 8);
        piece = GameObject.Find("Brook2");
        toPlacePiece(piece, 8, 1);
        piece = GameObject.Find("Bking");
        toPlacePiece(piece, 8, 5);
        piece = GameObject.Find("Bqueen");
        toPlacePiece(piece, 8, 4);
        piece = GameObject.Find("Bbishop1");
        toPlacePiece(piece, 8, 6);
        piece = GameObject.Find("Bbishop2");
        toPlacePiece(piece, 8, 3);
        BoardStateManagment.initializeControlArea();
        BoardStateManagment.initializeControlArea();
        graveyard.initialisationTab();
    }

    public static void toPlacePiece(GameObject piece, int i, int j)
    {
        Move.freeMove(GameObject.Find("Tile" + i + j), piece);
        tabPosition[i, j] = piece;
    }
    //Gestion du visuel de la pièce selectionnée
    public static void pieceLooseFocus(GameObject piece, string color)
    {
        if (color == "White") piece.transform.GetComponent<MeshRenderer>().material.color = Color.white;
        else if (color == "Black") piece.transform.GetComponent<MeshRenderer>().material.color = Color.black;

        GameObject selectionLight = GameObject.Find(piece.name + "_Spotlight");
        selectionLight.GetComponent<Light>().enabled = !selectionLight.GetComponent<Light>().enabled;

    }

    //Renvoi l'index i de piece dans tabPosition (colonnes)
    public static int searchIndexIOfPiece(GameObject piece)
    {
        int i, j;
        for (i = 1; i <= 8; i++)
        {
            for (j = 1; j <= 8; j++)
            {
                if (tabPosition[i, j] == piece) return i;
            }
        }
        Debug.Log("searchIndexIofPiece erreur");
        return -1;
    }

    //Renvoi l'index j de piece dans tabPosition (lignes)
    public static int searchIndexJOfPiece(GameObject piece)
    {
        int i, j;
        for (i = 1; i <= 8; i++)
        {
            for (j = 1; j <= 8; j++)
            {
                if (tabPosition[i, j] == piece) return j;
            }
        }
        Debug.Log("searchIndexJofPiece erreur");
        return -1;
    }

    //Test si le déplacement est autorisé, si oui met à jour TabPosition
    public static bool majTabPosition(GameObject piece, GameObject target)
    {
        String playerColor = (piece.name.Substring(0,1)=="W")? "White" : "Black";
        int iTarget, jTarget, iPiece, jPiece;
        bool moveAuthorized;
        iPiece = searchIndexIOfPiece(piece);
        jPiece = searchIndexJOfPiece(piece);
        //Si target est un tile
        if (target.transform.tag == "Tile")
        {
            //Récupération des coordonnées du tile dans le nom caseTarget
            iTarget = Convert.ToInt32(target.name.Substring(4, 1));
            jTarget = Convert.ToInt32(target.name.Substring(5, 1));
            moveAuthorized = Move.moveAuthorized(iPiece, jPiece, iTarget, jTarget, piece.name, target);
            if (moveAuthorized)
            {
                tabPosition[iTarget, jTarget] = piece;
                Debug.Log(piece.name + " case" + iTarget + jTarget);
                //Suppression de piece à son ancien emplacement dans tabposition
                tabPosition[iPiece, jPiece] = null;
                BoardStateManagment.eraseControlAreaOf(piece);
                BoardStateManagment.majBoard(iPiece, jPiece, iTarget, jTarget, piece, target);
                //Si le joueur se met lui même en échec annulation du déplacement
                if(VictoryConditions.kingInCheck(playerColor))
                {
                    tabPosition[iPiece, jPiece] = piece;
                    tabPosition[iTarget, jTarget] = null;
                    BoardStateManagment.eraseControlAreaOf(piece);
                    BoardStateManagment.majBoard(iPiece, jPiece, iTarget, jTarget, piece, target);
                    return false;
                }
                return true;
            }
            else return false;
        }

        //Si target est une piece
        else
        {
            iTarget = searchIndexIOfPiece(target);
            jTarget = searchIndexJOfPiece(target);

            moveAuthorized = Move.moveAuthorized(iPiece, jPiece, iTarget, jTarget, piece.name, target);
            if (moveAuthorized)
            {
                
                //Supression de piece à son ancien emplacement dans tabposition
                tabPosition[iPiece, jPiece] = null;
                //Remplacement dans tabPosition de target par piece
                tabPosition[iTarget, jTarget] = piece;
                Debug.Log(piece.name + " case" + iTarget + jTarget);
                BoardStateManagment.eraseControlAreaOf(piece);
                BoardStateManagment.majBoard(iPiece, jPiece, iTarget, jTarget, piece,target);
                if (VictoryConditions.kingInCheck(playerColor))
                {
                    tabPosition[iPiece, jPiece] = piece;
                    tabPosition[iTarget, jTarget] = target;
                    BoardStateManagment.eraseControlAreaOf(piece);
                    BoardStateManagment.majBoard(iPiece, jPiece, iTarget, jTarget, piece, target);
                    return false;
                }
                return true;
            }
            else return false;
        }
    }


    public static int movePiece(GameObject target, GameObject pieceSelected)
    {
        int iTarget, jTarget;
        bool moveAuthorized;
        if (target.transform.tag == "Tile")
        {
            iTarget = Convert.ToInt32(target.name.Substring(4, 1));
            jTarget = Convert.ToInt32(target.name.Substring(5, 1));
            if (tabPosition[iTarget, jTarget] != null) return -1;
            else
            {
                moveAuthorized = majTabPosition(pieceSelected, target);
                if (moveAuthorized)
                {
                    Move.freeMove(target, pieceSelected);
                    return 0;
                }
                else return -1;
            }
        }

        //Déplacement de pieceSelected à la place de target qui est déplacé dans le cimetière (si target != king)
        else
        {
            if (target.name.Substring(1, 4) == "king") return -1;
            //Recherche des coordonnées de target et déplacement de pieceSelected
            GameObject tileTarget;
            iTarget = searchIndexIOfPiece(target);
            jTarget = searchIndexJOfPiece(target);
            tileTarget = GameObject.Find("Tile" + iTarget + jTarget);
            moveAuthorized = majTabPosition(pieceSelected, target);

            if (moveAuthorized)
            {
                Move.freeMove(tileTarget, pieceSelected);
                //Recherche d'un tile libre dans le graveyard
                string color; color = target.name.Substring(0, 1);
                if (color == "W") color = "White";
                else color = "Black";
                int T = graveyard.addPiece(color) + 1;
                GraveYardTile = GameObject.Find(color + "GraveyardTile" + T);
                //déplacement de target dans Graveyard                                
                Move.freeMove(GraveYardTile, target);
                return 0;
            }
            else return -1;
        }

    }
}
