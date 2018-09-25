using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class GameLoop : MonoBehaviour
{

    public Camera CurrentCamera;
    public GameObject pieceSelected, target;
    public string color, vscolor, temp;
    int moveOK;
    GameObject selectionCircle;
    ParticleSystem particleSelectionCircle;
    public Canvas endGameCanvas;
    private Camera camera;
    string camPosition;
    public Quaternion BlackRotation = Quaternion.Euler(70, 180, 0);
    public Quaternion WhiteRotation = Quaternion.Euler(70, 0 ,0);
    Vector3 camBlackPosition= new Vector3(-29.6f, 44.82f, 5.62f), camWhitePosition=new Vector3(-30.25f, 45.8f, -5.31f);

    void Start()
    {
        Camera[] allCam = Camera.allCameras;
        camera = allCam[0];
        if (CurrentCamera == null)
        {
            CurrentCamera = camera;
        }
        selectionCircle = GameObject.Find("selectionCircle");
        particleSelectionCircle = selectionCircle.GetComponent<ParticleSystem>();
        particleSelectionCircle.Stop();
        GameObject tempObject = GameObject.Find("Canvas");
        endGameCanvas = tempObject.GetComponent<Canvas>();
        if (endGameCanvas == null) Debug.Log("CANVAS NULL");
        //initialisation du graveyard (toutes cases libres) et des positions des pièces
        BoardManagment.startPosition();
        VictoryConditions.initializeCheck();
        //Color sert a déterminer quel joueur à le focus
        color = "Black";
        vscolor = "White";
        camPosition = "Black";


    }

    IEnumerator rotatCam(Quaternion originalRotation, Quaternion finalRotation, Vector3 originalPosition, Vector3 finalPosition)
    {

        float startTime = Time.time;
        float endTime = startTime + 1.80f;
        camera.transform.rotation = originalRotation;
        yield return null;
        while (Time.time < endTime)
        {
            float progress = (Time.time - startTime) / 1.80f;
            // progress will equal 0 at startTime, 1 at endTime.
            camera.transform.rotation = Quaternion.Slerp(originalRotation, finalRotation, progress);
            camera.transform.position = Vector3.Slerp(originalPosition, finalPosition, progress);
            yield return null;
        }
        camera.transform.rotation = finalRotation;
        camera.transform.position = finalPosition;
       
        yield break;
    }

    void Update()
    {
        //Gestion de la camera
        if (color != camPosition && color == "White")
        {
            StartCoroutine(rotatCam(BlackRotation, WhiteRotation, camBlackPosition, camWhitePosition));
            print("change camPosition");
            camPosition = color;
        }
        if (color != camPosition && color == "Black")
        {
            StartCoroutine(rotatCam(WhiteRotation, BlackRotation, camWhitePosition, camBlackPosition));
            print("change camPosition");
            camPosition = color;
        }
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
                    }
                    //La nouvelle pièce ciblé devient sélectionnée             
                    pieceSelected = GameObject.Find(hit.collider.transform.name);
                    BoardManagment.moveSelectionCircle(pieceSelected, selectionCircle);
                    particleSelectionCircle.Play();
                    /*BoardStateManagment.displayControlAreaOf(pieceSelected); */
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
                else moveOK = -1;
            }
            if (moveOK == 0)
            {
                particleSelectionCircle.Stop();
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
                    if(VictoryConditions.kingIsCheckMate())
                    {
                        GameObject victoryText = GameObject.Find("VictoryMessage");
                        Text myText = victoryText.GetComponent<Text>();
                        myText.text = vscolor + "player win the game, click to return menu";
                        endGameCanvas.enabled = true;
                    }
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
