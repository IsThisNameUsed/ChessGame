using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Crée et met à jour des listes de piece pour chaque case indiquant quelles pièces la contrôlent.
public class BoardStateManagment {

    public static List<GameObject>[,] whiteControlArea = new List<GameObject>[9, 9];
    public static List<GameObject>[,] blackControlArea = new List<GameObject>[9, 9];

    public static void initializeControlArea()
    {
        for (int i = 1; i <= 8; i++)
        {
            for (int j = 1; j <= 8; j++)
            {
                blackControlArea[i, j] = new List<GameObject>();
                whiteControlArea[i, j] = new List<GameObject>();
            }
        }
        //Initialize control area of pawn
        for(int i=1;i<=8;i++)
        {
            pawnCalculateControlArea(GameObject.Find("Bpawn"+i));
            pawnCalculateControlArea(GameObject.Find("Wpawn"+i));
        }
        //initialize control area of knights
        whiteControlArea[3, 6].Add(BoardManagment.tabPosition[1, 7]);
        whiteControlArea[3, 8].Add(BoardManagment.tabPosition[1, 7]);
        whiteControlArea[3, 3].Add(BoardManagment.tabPosition[1, 2]);
        whiteControlArea[3, 1].Add(BoardManagment.tabPosition[1, 2]);
        blackControlArea[6, 8].Add(BoardManagment.tabPosition[8, 7]);
        blackControlArea[6, 6].Add(BoardManagment.tabPosition[8, 7]);
        blackControlArea[6, 3].Add(BoardManagment.tabPosition[8, 2]);
        blackControlArea[6, 1].Add(BoardManagment.tabPosition[8, 2]);
        //Tower
        whiteControlArea[2, 8].Add(BoardManagment.tabPosition[1, 8]);
        whiteControlArea[1, 7].Add(BoardManagment.tabPosition[1, 8]);
        whiteControlArea[2, 1].Add(BoardManagment.tabPosition[1, 1]);
        whiteControlArea[1, 2].Add(BoardManagment.tabPosition[1, 1]);
        blackControlArea[7, 1].Add(BoardManagment.tabPosition[8, 1]);
        blackControlArea[8, 2].Add(BoardManagment.tabPosition[8, 1]);
        blackControlArea[8, 7].Add(BoardManagment.tabPosition[8, 8]);
        blackControlArea[7, 8].Add(BoardManagment.tabPosition[8, 8]);
        //Bishop
        whiteControlArea[2, 4].Add(BoardManagment.tabPosition[1, 3]);
        whiteControlArea[2, 2].Add(BoardManagment.tabPosition[1, 3]);
        whiteControlArea[2, 7].Add(BoardManagment.tabPosition[1, 6]);
        whiteControlArea[2, 5].Add(BoardManagment.tabPosition[1, 6]);
        blackControlArea[7, 2].Add(BoardManagment.tabPosition[8, 3]);
        blackControlArea[7, 4].Add(BoardManagment.tabPosition[8, 3]);
        blackControlArea[7, 5].Add(BoardManagment.tabPosition[8, 6]);
        blackControlArea[7, 7].Add(BoardManagment.tabPosition[8, 6]);
        //Queen
        whiteControlArea[1, 3].Add(BoardManagment.tabPosition[1, 4]);
        whiteControlArea[2, 3].Add(BoardManagment.tabPosition[1, 4]);
        whiteControlArea[2, 4].Add(BoardManagment.tabPosition[1, 4]);
        whiteControlArea[2, 5].Add(BoardManagment.tabPosition[1, 4]);
        whiteControlArea[1, 5].Add(BoardManagment.tabPosition[1, 4]);
        blackControlArea[8, 3].Add(BoardManagment.tabPosition[8, 4]);
        blackControlArea[7, 3].Add(BoardManagment.tabPosition[8, 4]);
        blackControlArea[7, 4].Add(BoardManagment.tabPosition[8, 4]);
        blackControlArea[7, 5].Add(BoardManagment.tabPosition[8, 4]);
        blackControlArea[8, 5].Add(BoardManagment.tabPosition[8, 4]);
        //King
        whiteControlArea[1, 4].Add(BoardManagment.tabPosition[1, 5]);
        whiteControlArea[2, 4].Add(BoardManagment.tabPosition[1, 5]);
        whiteControlArea[2, 4].Add(BoardManagment.tabPosition[1, 5]);
        whiteControlArea[2, 6].Add(BoardManagment.tabPosition[1, 5]);
        whiteControlArea[1, 6].Add(BoardManagment.tabPosition[1, 5]);
        blackControlArea[8, 4].Add(BoardManagment.tabPosition[8, 5]);
        blackControlArea[7, 4].Add(BoardManagment.tabPosition[8, 5]);
        blackControlArea[7, 5].Add(BoardManagment.tabPosition[8, 5]);
        blackControlArea[7, 6].Add(BoardManagment.tabPosition[8, 5]);
        blackControlArea[8, 6].Add(BoardManagment.tabPosition[8, 5]);
    }

    private static void colorTileRed(int i, int j)
    {
        Material redMaterial = Resources.Load("Red") as Material;
        GameObject Tile = GameObject.Find("Tile" + i + j);
        Tile.GetComponent<Renderer>().material = redMaterial;
    }

    private static void colorTileOriginalColor(int i, int j)
    {
        Material originalMaterial;
        if ((i % 2 == 0 && j % 2 == 0) || (i % 2 != 0 && j % 2 != 0)) originalMaterial = Resources.Load("Black") as Material;
        else originalMaterial = Resources.Load("White") as Material;
        GameObject Tile = GameObject.Find("Tile" + i + j);
        Tile.GetComponent<Renderer>().material = originalMaterial;
    }
    public static void eraseControlAreaOf(GameObject piece)
    {
        string pieceColor = piece.name.Substring(0, 1);
        for (int i = 1; i <= 8; i++)
        {
            for (int j = 1; j <= 8; j++)
            {
                if (pieceColor == "W")
                {
                    if (whiteControlArea[i, j].Contains(piece)) colorTileOriginalColor(i, j);
                }
                if (pieceColor == "B")
                {
                    if (blackControlArea[i, j].Contains(piece)) colorTileOriginalColor(i, j);
                }
            }
        }
    }
    public static void displayControlAreaOf(GameObject piece)
    {
        string pieceColor = piece.name.Substring(0, 1);
        for (int i = 1; i <= 8; i++)
        {
            for (int j = 1; j <= 8; j++)
            {
                if (pieceColor == "W")
                {
                    if (whiteControlArea[i, j].Contains(piece)) colorTileRed(i, j);
                }
                if (pieceColor == "B")
                {
                    if (blackControlArea[i, j].Contains(piece)) colorTileRed(i, j);
                }
            }
        }
    }

    private static GameObject[] copyListInTab(List<GameObject> list)
    {
        int nbObject = list.Count;
        GameObject[] pieceTab = new GameObject[nbObject];
        list.CopyTo(pieceTab);
        return pieceTab;
    }

    private static void clearControlArea(GameObject piece)
    {
        List<GameObject>[,] controlArea = new List<GameObject>[9, 9];
        if (piece.name.Substring(0, 1) == "W") controlArea = whiteControlArea;
        else controlArea = blackControlArea;
        for(int i=1;i<=8;i++)
        {
            for(int j=1;j<=8;j++)
            {
                controlArea[i,j].Remove(piece);
            }
        }
    }
    private static void pawnCalculateControlArea(GameObject pawn)
    {
        int iPawn = BoardManagment.searchIndexIOfPiece(pawn);
        int jPawn = BoardManagment.searchIndexJOfPiece(pawn);
        int iTarget;
        int jTarget = jPawn;
        iTarget=(pawn.name.Substring(0, 1)) == "W" ?iPawn + 2 : iPawn - 2;
        GameObject target = GameObject.Find("Tile" + iTarget + jTarget);
        if (Move.moveAuthorized(iPawn, jPawn, iTarget, jTarget, pawn.name, target))
        {
            if (pawn.name.Substring(0, 1) == "W") whiteControlArea[iTarget, jTarget].Add(pawn);
            else blackControlArea[iTarget, jTarget].Add(pawn);
        }
        if (pawn.name.Substring(0, 1) == "W")
        {
            for(int j = jPawn-1;j<=jPawn+1;j++)
            {
                if (j>8 || j<1) continue;
                whiteControlArea[iPawn + 1,j].Add(pawn);
            }         
        }
        else
        {
            for (int j = jPawn - 1; j <= jPawn + 1; j++)
            {
                if (j>8 || j<1) continue;
                blackControlArea[iPawn -1, j].Add(pawn);
            }
        }

    }
    private static int calculateControlArea(GameObject piece)
    {
        if (piece.name.Substring(1, 1) == "p")
        {
            pawnCalculateControlArea(piece);
            return 0;
        }

        for(int iTarget=1;iTarget<=8;iTarget++)
        {
            for(int jTarget=1;jTarget<=8;jTarget++)
            {
                int iPiece = BoardManagment.searchIndexIOfPiece(piece);
                int jPiece = BoardManagment.searchIndexJOfPiece(piece);
                GameObject target= BoardManagment.tabPosition[iTarget, jTarget];
                if (target == null) target = GameObject.Find("Tile" + iTarget + jTarget);
                if (Move.moveAuthorized(iPiece, jPiece, iTarget, jTarget, piece.name, target))
                {
                    if (piece.name.Substring(0, 1) == "W") whiteControlArea[iTarget, jTarget].Add(piece);
                    else blackControlArea[iTarget, jTarget].Add(piece);
                }
            }
        }
        return 0;
    }

    //Quand une pièce est joué, recalcule les zone de contrôle de toutes les pièces qui contrôlaient les cases de départs et/ou d'arrivé
    public static void majBoard(int iDeparture, int jDeparture, int iDestination, int jDestination, GameObject playedPiece, GameObject target)
    {
        //Traitement de la pièce jouée
        clearControlArea(playedPiece);
        calculateControlArea(playedPiece);
        if (target.transform.tag.Substring(0,1) != "PT") clearControlArea(target);
        //Traitement des pièce contrôlant la case de départ avant le coup
        GameObject[] blackPieceControlDeparture = copyListInTab(blackControlArea[iDeparture, jDeparture]);
        GameObject[] whitePieceControlDeparture = copyListInTab(whiteControlArea[iDeparture, jDeparture]);
        blackControlArea[iDeparture, jDeparture].Clear();
        whiteControlArea[iDeparture, jDeparture].Clear();

        if (blackPieceControlDeparture.Length > 0)
        {
            for (int i = 0; i <= blackPieceControlDeparture.Length-1; i++)
            {
                clearControlArea(blackPieceControlDeparture[i]);
                calculateControlArea(blackPieceControlDeparture[i]);
            }
        }
        if (whitePieceControlDeparture.Length > 0)
        {
            for (int i = 0; i <= whitePieceControlDeparture.Length-1; i++)
            {
                clearControlArea(whitePieceControlDeparture[i]);
                calculateControlArea(whitePieceControlDeparture[i]);
            }
        }
        //Traitement des pièces controlant la case d'arrivé avant le coup
        GameObject[] blackPieceControlDestination = copyListInTab(blackControlArea[iDestination, jDestination]);
        GameObject[] whitePieceControlDestination = copyListInTab(whiteControlArea[iDestination, jDestination]);
        blackControlArea[iDestination, jDestination].Clear();
        whiteControlArea[iDestination, jDestination].Clear();

        if (blackPieceControlDestination.Length > 0)
        {
            for (int i = 0; i <= blackPieceControlDestination.Length-1; i++)
            {
                Debug.Log(i);
                clearControlArea(blackPieceControlDestination[i]);
                calculateControlArea(blackPieceControlDestination[i]);
            }
        }

        if (whitePieceControlDestination.Length > 0)
        {
            for (int i = 0; i <= whitePieceControlDestination.Length-1; i++)
            {
                clearControlArea(whitePieceControlDestination[i]);
                calculateControlArea(whitePieceControlDestination[i]);
            }
        }
    }
        
}

