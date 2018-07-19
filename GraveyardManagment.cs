using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Les tiles du Graveyard sont représentés par une case d'un tableau d'entier (=1 si occupée, =0 si libre).
public class GraveyardManagment
{
    int[] blackGraveyard = new int[16];
    int[] whiteGraveyard = new int[16];
    int i;


    //initialisation des tableaux représentant les cimetières à 0
    public void initialisationTab()
    {
        for (i = 0; i < 16; i++)
        {
            whiteGraveyard[i] = 0;
            blackGraveyard[i] = 0;
        }
    }

    //Recherche d'un tile libre
    public int addPiece(string color)
    {
        i = 0;
        if (color == "White")
        {
            while (whiteGraveyard[i] == 1)
            {
                i++;
            }
            whiteGraveyard[i] = 1;
            return i;
        }

        else if (color == "Black")
        {
            while (blackGraveyard[i] == 1)
            {
                i++;
            }
            blackGraveyard[i] = 1;
            return i;

        }

        else return -1;

    }

}
