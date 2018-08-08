using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Move
{


    public static void freeMove(GameObject tileTarget, GameObject pieceSelectedNow)
    {
        Vector3 position = new Vector3(0, 0, 0);
        position = tileTarget.transform.position;
        position.y = (float)1.20;
        pieceSelectedNow.transform.position = position;
    }

    private static int calculateDistance(int targetCoordinate, int pieceCoordinate)
    {
        return (targetCoordinate - pieceCoordinate);
    }

    public static bool moveAuthorized(int iPiece, int jPiece, int iTarget, int jTarget, string pieceName, GameObject Target)
    {
        string playerColor = (pieceName.Substring(0, 1));
        //Traitement spécial des mouvements autorisés en cas d'échec au roi
        if (VictoryConditions.whiteKingInCheck == true || VictoryConditions.blackKingInCheck == true)
        {
            Debug.Log("VictoryConditions.moveAuthorizedWhenKingInCheck" + playerColor);
            if (VictoryConditions.moveAuthorizedWhenKingInCheck(pieceName, Target))
            {
                VictoryConditions.kingNoLongerInCheck();
                return true;
            }
            else return false;
        }
        switch (pieceName.Substring(1, 3))
        {
            case "paw":
                if (playerColor == "B") return pawnMoveAuthorized(iPiece, jPiece, iTarget, jTarget, Target, "W");
                else return pawnMoveAuthorized(iPiece, jPiece, iTarget, jTarget, Target, "B");
            case "bis":
                return bishopMoveAuthorized(iPiece, jPiece, iTarget, jTarget);
            case "roo":
                return rookMoveAuthorized(iPiece, jPiece, iTarget, jTarget);
            case "kni":
                return knightMoveAuthorized(iPiece, jPiece, iTarget, jTarget);
            case "kin":
                return kingMoveAuthorized(iPiece, jPiece, iTarget, jTarget, playerColor);
            case "que":
                return queenMoveAuthorized(iPiece, jPiece, iTarget, jTarget);
            default:
                Debug.Log("Piece non valide");
                return false;
        }
    }

    private static bool freePath(int[,] path, int indexMax)
    {
        int k = 0;
        while (k <= indexMax)
        {
            int i = path[0, k];
            int j = path[1, k];
            if (BoardManagment.tabPosition[i, j] != null)
            {
                //Debug.Log(i.ToString() + "" + j.ToString() + "full");
                return false;
            }
            //Debug.Log(i.ToString() + "" + j.ToString() + "free");
            k++;
        }
        return true;
    }

    public static int[,] pathToTarget(int iPiece, int jPiece, int iTarget, int jTarget,int iIncrement, int jIncrement)
    {
        int i = iPiece + iIncrement, j = jPiece + jIncrement, pathIndex=0;
        int[,] path = new int[2, 9];
        if (iIncrement != 0)
        {
            while (i != iTarget)
            {                
                path[0, pathIndex] = i;
                path[1, pathIndex] = j;
                i += iIncrement;
                j += jIncrement;
                pathIndex += 1;
            }
            path[0, 8] = pathIndex;
            return path;
        }
        else
        {
            while (j != jTarget)
            {
                path[0, pathIndex] = i;
                path[1, pathIndex] = j;
                j += jIncrement;
                pathIndex += 1;
            }
            path[0, 8] = pathIndex;
            return path;
        }
    }
    public static bool rookMoveAuthorized(int iRook, int jRook, int iTarget, int jTarget)
    {
        int[,] path = new int[2, 9];
        int iDistance = calculateDistance(iTarget, iRook);
        int jDistance = calculateDistance(jTarget, jRook);
        int increment;
        if (iDistance == 0 && jDistance != 0)
        {
            if (jDistance < 0) increment = -1;
            else increment = 1;
            path = pathToTarget(iRook, jRook, iTarget, jTarget, 0, increment);
            int pathIndex = path[0, 8];
            if (freePath(path, pathIndex)) return true;
        }
        else if (iDistance != 0 && jDistance == 0)
        {
            if (iDistance < 0) increment = -1;
            else increment = 1;          
            path = pathToTarget(iRook, jRook, iTarget, jTarget, increment, 0);
            int pathIndex = path[0, 8];
            if (freePath(path, pathIndex)) return true;
        }
        return false;

    }

    public static bool pawnMoveAuthorized(int iPawn, int jPawn, int iTarget, int jTarget, GameObject Target, string opponentColor)
    {
        int iDistance = calculateDistance(iTarget, iPawn);
        int jDistance = calculateDistance(jTarget, jPawn);
        int valid_iDistance;
        bool doubleMove;
        string playerColor;
        if (iTarget > 8 || iTarget < 1) return false;
        if (opponentColor == "B")
        {
            playerColor = "W";
            valid_iDistance = 1;
            if (iPawn==2) doubleMove = true;
            else doubleMove = false;
        }
        else
        {
            playerColor = "B";
            valid_iDistance = -1;
            if (iPawn == 7) doubleMove = true;
            else doubleMove = false;
        }
        if (Target.name.Substring(0, 1) == opponentColor)
        {
            if (iDistance == valid_iDistance && (jDistance == 1 || jDistance == -1)) return true;
            else return false;
        }
        /*Maj controlArea, condition atteignable uniquement depuis l'appel par BoardStateManagment*/
        else if (Target.name.Substring(0, 1) == playerColor && iDistance == valid_iDistance && jDistance == 0) return true;
        else if (iDistance == valid_iDistance && jDistance == 0) return true;
        else if (iDistance == valid_iDistance * 2 && jDistance == 0 && doubleMove == true)
        {
            int[,] path = new int[2, 1];
            path[0, 0] = iPawn + valid_iDistance;
            path[1, 0] = jPawn;
            if (freePath(path, 0)) return true;
            else return false;
        }
        else return false;
    }


    public static bool bishopMoveAuthorized(int iBishop, int jBishop, int iTarget, int jTarget)
    {
        int iDistance = Mathf.Abs(calculateDistance(iTarget, iBishop));
        int jDistance = Mathf.Abs(calculateDistance(jTarget, jBishop));
        int[,] path = new int[2, 9];
        int pathIndex = 0;

        if (iDistance == 1 && jDistance == 1) return true;
        if (jDistance == iDistance && iDistance != 0)
        {
            int iPath, iIncrement, jPath, jIncrement;
            if (iTarget > iBishop)
            {
                iPath = iBishop + 1;
                iIncrement = 1;
               
            }
            else
            {
                iPath = iBishop - 1;
                iIncrement = -1;
            }

            if (jTarget > jBishop)
            {
                jPath = jBishop + 1;
                jIncrement = 1;
            }
            else
            {
                jPath = jBishop - 1;
                jIncrement = -1;
            }

            while (iPath != iTarget)
            {
                path[0, pathIndex] = iPath;
                path[1, pathIndex] = jPath;
                iPath += iIncrement;
                jPath += jIncrement;
                pathIndex += 1;
            }
            if (freePath(path, pathIndex)) return true;
            else return false;

        }
        else return false;

    }

    public static bool knightMoveAuthorized(int iKnight, int jKnight, int iTarget, int jTarget)
    {
        int iDistance = Mathf.Abs(calculateDistance(iTarget, iKnight));
        int jDistance = Mathf.Abs(calculateDistance(jTarget, jKnight));
        if ((iDistance == 2 && jDistance == 1) || (iDistance == 1 && jDistance == 2)) return true;

        else return false;

    }

    public static bool kingMoveAuthorized(int iKing, int jKing, int iTarget, int jTarget, string playerColor)
    {
        int iDistance = Mathf.Abs(calculateDistance(iTarget, iKing));
        int jDistance = Mathf.Abs(calculateDistance(jTarget, jKing));
        if (playerColor == "B" && BoardStateManagment.whiteControlArea[iTarget, jTarget].Count != 0) return false;
        else if (playerColor == "W" && BoardStateManagment.blackControlArea[iTarget, jTarget].Count != 0) return false;
        else if (iDistance <= 1 && jDistance <= 1 && (iDistance == 1 || jDistance == 1)) return true;
        else return false;
    }

    public static bool queenMoveAuthorized(int iQueen, int jQueen, int iTarget, int jTarget)
    {
        int iDistance = Mathf.Abs(calculateDistance(iTarget, iQueen));
        int jDistance = Mathf.Abs(calculateDistance(jTarget, jQueen));
        if (jDistance == iDistance && iDistance != 0) return bishopMoveAuthorized(iQueen, jQueen, iTarget, jTarget);
        else if ((iDistance == 0 && jDistance > 0) || (iDistance > 0 && jDistance == 0)) return rookMoveAuthorized(iQueen, jQueen, iTarget, jTarget);
        else return false;
    }
}
