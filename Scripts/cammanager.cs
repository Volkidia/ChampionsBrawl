using UnityEngine;
using System.Collections;

public class cammanager : MonoBehaviour {
    private GameObject[] maps;
    GameObject[] players;
    private float mapminY = Mathf.Infinity;
    private float mapmaxY = Mathf.NegativeInfinity;
    private float mapminZ = Mathf.Infinity;
    private float mapmaxZ = Mathf.NegativeInfinity;
    private Vector3 finalCameraCenter = Vector3.zero;
    private bool init = true;
    private bool AnimStart = false;
    public float camSpeed = 10f;
	Bounds MapBox;
    Bounds BoxPlayer;
    Camera m_cam;
    public Camera CamRender;
    public int MaxPlayer;
    // Use this for initialization
    void Start () {
        m_cam = Camera.main;
        Debug.Log(m_cam.transform.position.z);
    }

    // Update is called once per frame
    void FixedUpdate() {
        
       if (ReplaceObj.AlreadyAuthority && init) {
            players = GameObject.FindGameObjectsWithTag("player");
            if (players.Length == MaxPlayer & !AnimStart)
            {
                StartAnimAwake();
            }
        }

        float d = 0;
        float playerMaxDistY = 14;
        Vector3 PlayerPointMinX = Vector3.zero;
        Vector3 PlayerPointMaxX = Vector3.zero;

        if (!init)
        {
            players = GameObject.FindGameObjectsWithTag("player");
            bool firstPlyr = false;
            foreach (GameObject player in players) //calcule a chaque farme les X Y min et max des joueurs (les joueurs doivent etre tagués player)
            {
                if (!firstPlyr)
                {
                    BoxPlayer = new Bounds(player.transform.position, Vector3.zero);
                    firstPlyr = true;
                }
                else BoxPlayer.Encapsulate(player.GetComponent<CharacterController>().bounds);
            }

            Debug.Log("bidul");

            PlayerPointMaxX.z = BoxPlayer.max.y;
            PlayerPointMinX.z = BoxPlayer.min.y;

            d = Vector3.Distance(BoxPlayer.min, BoxPlayer.max)/ playerMaxDistY; //calcule de la distance max entre les joueurs les plus eloigné, on divise par l'ecart max en y pour obtenir un indice.
            m_cam.fieldOfView = Mathf.Lerp(9, 30, d); // on utilise l'indice pour modfier le FOV en fonction de la distance entre les joueurs.
            CamRender.fieldOfView = m_cam.fieldOfView;
            Debug.Log(m_cam.fieldOfView);
            float TopToCenter = BoxPlayer.max.y - BoxPlayer.center.y;
            float CamtoPlayer = playerMaxDistY - BoxPlayer.center.y;
            float PosCamY = (TopToCenter <= CamtoPlayer)? BoxPlayer.center.y : m_cam.transform.position.y;
            
            finalCameraCenter = new Vector3(m_cam.transform.position.x, PosCamY, BoxPlayer.center.z);

            m_cam.transform.position = Vector3.Lerp(m_cam.transform.position, finalCameraCenter, camSpeed * Time.deltaTime);

            
        }

       
        /*float frustumWidth = Mathf.Tan((m_cam.fieldOfView / 2) * Mathf.Deg2Rad) * Mathf.Abs(m_cam.transform.position.x) * m_cam.aspect; //rayon de la largeur du champ de vision au niveau de la map
        float ZminimalDistCamBord;
        float YminimalDistCamBord;
        float camRayHeight = Mathf.Abs(frustumWidth / m_cam.aspect); //moitié de la hauteur de la vision de la cam
        float camRayWidth = Mathf.Abs(frustumWidth);

        if ((mapmaxZ - playerCenter.z) <= (mapminZ + playerCenter.z)) // calcule de la distance entre le centre des joueurs et le bord de la map en x et en y
            ZminimalDistCamBord = mapmaxZ - playerCenter.z;
        else
            ZminimalDistCamBord = mapminZ + playerCenter.z;

        if ((mapmaxY - playerCenter.y) <= (mapminY + playerCenter.y))
            YminimalDistCamBord = (mapmaxY - playerCenter.y);
        else
            YminimalDistCamBord = (mapminY + playerCenter.y);

        if (ZminimalDistCamBord > camRayWidth || !init) //si la distance joueur/bord est superieur au rayon de la camera, on ne depassera pas de la map, on se centre sur les joueurs
        {
            finalCameraCenter.z = playerCenter.z;
        }
        else{                                           //sinon on décale la cam de facon a ne pas voir le vide
            if ((playerCenter.z + camRayWidth) >= mapmaxZ)
            {
                finalCameraCenter.z = mapmaxZ - camRayWidth;

            }
            else if ((playerCenter.z - camRayWidth) <= mapminZ)
            {
                finalCameraCenter.z = mapminZ + camRayWidth;
            }
        }
        if (YminimalDistCamBord > camRayHeight || !init)
        {
            finalCameraCenter.y = playerCenter.y;
        }
        else
        {
            if ((playerCenter.y + camRayHeight) >= mapmaxY)
            {
                finalCameraCenter.y = mapmaxY - camRayHeight;

            }
            else if ((playerCenter.y - camRayHeight) <= mapminY)
            {
                finalCameraCenter.y = mapminY + camRayHeight;
            }
        }
        finalCameraCenter.x = playerCenter.x; //la valeur de la camera en x ne change jamais.*/
         //la camera se deplace de son ancienne position a la nouvelle à une vitesse dependant de camspeed;
        //init = true;
    }

    void StartAnimAwake()
    {
        m_cam.GetComponent<Animator>().SetBool("StartCam", true);
        AnimStart = true;
    }
    void EndOfStartAnim()
    {
        foreach (GameObject player in players)
        {
            player.GetComponent<CharacterMove>().PublicUnset(CharacterMove.CharMoveState._cmNoInputs);
        }
        init = false;
        m_cam.GetComponent<Animator>().SetBool("StartCam", false);
        m_cam.GetComponent<Animator>().enabled = false;
        //AnimStart = false;
        
    }
}
