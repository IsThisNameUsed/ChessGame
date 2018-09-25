using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VictoryConditions {

    private static List<List<GameObject>> movementAuthorized = new List<List<GameObject>>();
    public static bool whiteKingInCheck, blackKingInCheck;

    public static void initializeCheck()
    {
        whiteKingInCheck = false;
        blackKingInCheck = false;
    }

    public static void kingNoLongerInCheck()
    {
        movementAuthorized.Clear();
        whiteKingInCheck = false;
        blackKingInCheck = false;
    }

    public static bool kingIsCheckMate()
    {
        bool noMooveExistAtLeast = true;
        //private static List<List<GameObject>> movementAuthorized = new List<List<GameObject>>();
        foreach (List<GameObject> list in movementAuthorized)
        {
            if (list.Count > 1) return noMooveExistAtLeast = false;
        }
        Debug.Log("Fin kingIsCheckMate");
        return noMooveExistAtLeast;
    }

    
    public static bool kingInCheck(string color)
    {
        GameObject king;
        if (color == "White")
        {
            king = GameObject.Find("Wking");
            int iKing = BoardManagment.searchIndexIOfPiece(king);
            int jKing = BoardManagment.searchIndexJOfPiece(king);
            if (BoardStateManagment.blackControlArea[iKing, jKing].Count > 0)
            {
                Debug.Log("whiteking in check");
                return true;
            }
            else
            {
                Debug.Log("WhiteKing not in check");
                return false;
            }
        }
        else
        {
            king = GameObject.Find("Bking");
            int iKing = BoardManagment.searchIndexIOfPiece(king);
            int jKing = BoardManagment.searchIndexJOfPiece(king);
            if (BoardStateManagment.whiteControlArea[iKing, jKing].Count > 0)
            {
                Debug.Log("blackKing in check");
                return true;
            }
            else
            {
                Debug.Log("BlackKing not in check");
                return false;
            }
        }
  
    }
    //Renvoie une liste des tiles autorisés pour le déplacement du roi
    private static List<GameObject> kingMovementAuthorized(string color)
    {
        GameObject king;
        List<GameObject>[,] opponentControlArea = new List<GameObject>[9, 9];
        List<GameObject> kingMovement = new List<GameObject>();
        List<int> tileNotAllowedByLongRange = new List<int>();
        String playerColor, opponentColor;
        if (color == "White")
        {
            playerColor = "W";
            opponentColor = "B";
            opponentControlArea = BoardStateManagment.blackControlArea;
            king = GameObject.Find("Wking");
        }
        else
        {
            playerColor = "B";
            opponentColor = "W";
            king = GameObject.Find("Bking");
            opponentControlArea = BoardStateManagment.whiteControlArea;
        }
        int iKing = BoardManagment.searchIndexIOfPiece(king);
        int jKing = BoardManagment.searchIndexJOfPiece(king);
        kingMovement.Add(king);

        //traitement du cas ou la pièce menaçante est de longue porté
        BoardManagment.tabPosition[iKing, jKing] = null;
        foreach(GameObject pieceThreateningKing in opponentControlArea[iKing,jKing])
        {
            String pieceThreateningKingName = pieceThreateningKing.name.Substring(1, 4);
            int iPieceThreateningKing = BoardManagment.searchIndexIOfPiece(pieceThreateningKing);
            int jPieceThreateningKing = BoardManagment.searchIndexJOfPiece(pieceThreateningKing);
            if (pieceThreateningKingName=="quee")
            {
                for (int i = iKing - 1; i <= iKing + 1; i++)
                {
                    for (int j = jKing - 1; j <= jKing + 1; j++)
                    {
                        if (Move.queenMoveAuthorized(iPieceThreateningKing, jPieceThreateningKing, i, j)) tileNotAllowedByLongRange.Add(i * 10 + j);

                    }
                }
            }
            else if(pieceThreateningKingName=="bish")
            {

                for (int i = iKing - 1; i <= iKing + 1; i++)
                {
                    for (int j = jKing - 1; j <= jKing + 1; j++)
                    {
                        if (Move.bishopMoveAuthorized(iPieceThreateningKing, jPieceThreateningKing, i, j)) tileNotAllowedByLongRange.Add(i * 10 + j);

                    }
                }
            }
            else if (pieceThreateningKingName == "rook")
            {

                for (int i = iKing - 1; i <= iKing + 1; i++)
                {
                    for (int j = jKing - 1; j <= jKing + 1; j++)
                    {
                        if (Move.rookMoveAuthorized(iPieceThreateningKing, jPieceThreateningKing, i, j)) tileNotAllowedByLongRange.Add(i * 10 + j);

                    }
                }
            }
        }
        BoardManagment.tabPosition[iKing, jKing] = king;
        for (int i = iKing - 1; i <= iKing + 1; i++)
        {
            for (int j = jKing - 1; j <= jKing + 1; j++)
            {
                if (i >= 1 && i <= 8 && j >= 1 && j <= 8 && opponentControlArea[i, j].Count == 0)
                {
                    GameObject isItAnAlly = BoardManagment.tabPosition[i, j];
                    if (isItAnAlly == null)
                    {
                        if (tileNotAllowedByLongRange.IndexOf(i * 10 + j) >= 0) continue;
                        else kingMovement.Add(GameObject.Find("Tile" + i + j));
                    }
                    else
                    {
                        if (isItAnAlly.name.Substring(0, 1) != playerColor)
                        {
                            if (tileNotAllowedByLongRange.IndexOf(i * 10 + j) >= 0) continue;
                            else kingMovement.Add(GameObject.Find("Tile" + i + j));
                        }
                    }
                }
               
            }
        }
        return kingMovement;
    }

    //return une liste de case, chemin de la pièce au roi mis en échec
    private static List<GameObject> searchPathToKing(GameObject piece, string playerColor, int iKing, int jKing)
    {
        List<GameObject> pathToKing = new List<GameObject>();
        int iPiece = BoardManagment.searchIndexIOfPiece(piece);
        int jPiece = BoardManagment.searchIndexJOfPiece(piece);
        int jIncrement, iIncrement;

        if (iKing - iPiece > 0) iIncrement = 1;
        else if (iKing - iPiece < 0) iIncrement = -1;
        else iIncrement = 0;
        if (jKing - jPiece > 0) jIncrement = 1;
        else if (jKing - jPiece < 0) jIncrement = -1;
        else jIncrement = 0;

        int[,] path = Move.pathToTarget(iPiece, jPiece, iKing, jKing, iIncrement, jIncrement);
        int pathIndex = path[0, 8];
        int i=0;
        GameObject tile;
        while(i!=pathIndex)
        {
            tile = GameObject.Find("Tile" + path[0, i] + path[1, i]);
            pathToKing.Add(tile);
            i++;
        }
        return pathToKing;
    }

    public static void searchMovementAuthorized(string playerColor)
    {
        List<GameObject>[,] opponentControlArea = new List<GameObject>[9, 9], playerControlArea = new List<GameObject>[9, 9];
        String opponentColor;
        GameObject king = GameObject.Find(playerColor.Substring(0, 1) + "king");
        int iKing = BoardManagment.searchIndexIOfPiece(king);
        int jKing = BoardManagment.searchIndexJOfPiece(king);

        if (playerColor == "Black")
        {
            opponentControlArea = BoardStateManagment.whiteControlArea;
            playerControlArea  = BoardStateManagment.blackControlArea;
            opponentColor = "W";
        }
        else
        {
            opponentControlArea = BoardStateManagment.blackControlArea;
            playerControlArea = BoardStateManagment.whiteControlArea;
            opponentColor = "B";
        }
             
        //calcul des mvts autorisés pour le roi
        List<GameObject> tilesAuhorizedForKing = new List<GameObject>();
        tilesAuhorizedForKing = kingMovementAuthorized(playerColor);
        movementAuthorized.Add(tilesAuhorizedForKing);


        //Recherche des cases à occuper pour annuler l'échec au roi (interception ou prise de pièce)
        foreach (GameObject piece in opponentControlArea[iKing, jKing])
        {
            int iPiece = BoardManagment.searchIndexIOfPiece(piece);
            int jPiece = BoardManagment.searchIndexJOfPiece(piece);
            GameObject tileOfPiece = GameObject.Find("Tile" + iPiece + jPiece);
            movementAuthorized.Add(new List<GameObject> { tileOfPiece });
            List<GameObject> pathToKing = new List<GameObject>();
            pathToKing = searchPathToKing(piece, playerColor, iKing, jKing);
            foreach (GameObject tile in pathToKing)
            {
                movementAuthorized.Add(new List<GameObject> { tile });
            }
        }
        //Recherche des pièces pouvant se rendre sur ces cases stockées dans une liste dont le premier élément est la case accessible à ces pièces)
        foreach (List<GameObject> list in movementAuthorized)
        {
            GameObject tileTarget = list[0];
            if (tileTarget.name.Substring(1, 1) == "k") continue;
            string iTileString = tileTarget.name.Substring(4, 1);
            string jTileString = tileTarget.name.Substring(5, 1);
            int iTile = Int32.Parse(iTileString);
            int jTile = Int32.Parse(jTileString);
            int pawnDirectionMovement;
            //Cas du pion en déplacement en ligne droite
            if (playerColor == "Black") pawnDirectionMovement = -1;
            else pawnDirectionMovement = 1;
            GameObject isItaPawn = BoardManagment.tabPosition[iTile-pawnDirectionMovement, jTile];
            if (isItaPawn!=null&&isItaPawn.name.Substring(1, 4) == "pawn") list.Add(isItaPawn);
            foreach (GameObject piece in playerControlArea[iTile, jTile])
            {
                int jPiece = BoardManagment.searchIndexJOfPiece(piece);
                //Si la pièce est un pion, traitement des déplacements en diagonales: valide uniquement si une pièce adverse est présente sur le tile visé
                if (piece.name.Substring(1, 4) == "pawn" && jTile != jPiece)
                {
                    if (BoardManagment.tabPosition[iTile, jTile] != null && BoardManagment.tabPosition[iTile, jTile].name.Substring(0, 1) == opponentColor)
                    {
                        list.Add(piece);
                    }
                }
                //traitement des autres pièces
                else if(piece.name.Substring(1,4)!="king")
                {
                    list.Add(piece);
                    
                }
            }

        }
        //private static List<List<GameObject>> movementAuthorized = new List<List<GameObject>>();
        foreach (List<GameObject> list in movementAuthorized)
        {
            foreach(GameObject obj in list)
            {
                Debug.Log(obj.name);
            }
        }
    }

    public static bool moveAuthorizedWhenKingInCheck(string pieceName, GameObject Target)
    {
        GameObject piece = GameObject.Find(pieceName);
        if(Target.name.Substring(0,4)!="Tile")
        {
            int iTarget = BoardManagment.searchIndexIOfPiece(Target);
            int jTarget = BoardManagment.searchIndexJOfPiece(Target);
            Target = GameObject.Find("Tile" + iTarget + jTarget);
        }

        if (pieceName.Substring(1, 4) == "king" && movementAuthorized[0].IndexOf(Target) >= 0) return true;
        foreach(List<GameObject> list in movementAuthorized)
        {
            if(list[0]==Target)
            {
                //Debug.Log(Target.name+"cible valable");
                //Debug.Log("Index piece=" + list.IndexOf(piece));
                //Debug.Log("taille liste"+Target.name+"="+list.Count);
                foreach(GameObject obj in list)
                {
                    //Debug.Log(obj.name);
                }
                if (list.IndexOf(piece) >= 1) return true;
            }
        }
        return false;

    }
}