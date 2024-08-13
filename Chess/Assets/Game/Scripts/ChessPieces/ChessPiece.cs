using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public enum ChessPieceType
{
    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6
}

public class ChessPiece : MonoBehaviourPun
{
    public Chessboard chessboard;
    public int team;
    public int currentX;
    public int currentY;
    public Vector2Int currentPos;
    public ChessPieceType type;
    public bool isDead = false;

    [SerializeField] private Vector3 desiredPosition;
    [SerializeField] private float positionOffset = 0.1f;
    private Vector3 desiredScale = Vector3.one * 5;

    Camera mainCamera;

    public Vector3 desiredPositionRef{
        get{
            return desiredPosition;
        }
    }
    public Vector3 desiredScaleRef{
        get{
            return desiredScale;
        }
    }

    private void Start(){
        isDead = false;
        mainCamera = Camera.main;
        chessboard = GameObject.Find("Chessboard").GetComponent<Chessboard>();

        if(!PhotonNetwork.LocalPlayer.IsMasterClient)
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
    }

    private void Update(){

        currentPos = new Vector2Int(currentX,currentY);
        if(isDead){
            transform.position = desiredPosition;
            transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 20);
        }
        if((chessboard.draggingPiece != null && Mouse.current.leftButton.isPressed)){

            return;
        }
        // if(chessboard.specialMove == SpecialMove.Promotion && ((name == "050-1" && currentPos == chessboard.moveList[chessboard.moveList.Count - 1][1]))){
            
        //     MoveDownATad();
        //     return;
        // }
        if(chessboard.specialMove == SpecialMove.Promotion && team == 1 && type == ChessPieceType.Queen && name == "150-1"){
            return;
        }
        // if(chessboard.draggingPiece.team == team){return;}
        // if(team == 0 && !PhotonNetwork.LocalPlayer.IsMasterClient){
        //     transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 2);
        //     transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 2);
        // } else if(team == 1 && PhotonNetwork.LocalPlayer.IsMasterClient){
        //     transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 2);
        //     transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 2);
        // }
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 20);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 20);
        chessboard.RemoveHighlightTiles();
    }

    bool moveDownATadBool;
    private void MoveDownATad(){

        if(!moveDownATadBool && MainMenu._34View){

            transform.position += Vector3.down * positionOffset;
            moveDownATadBool = true;
        }
    }

    public virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY){

        List<Vector2Int> r = new List<Vector2Int>();

        r.Add(new Vector2Int(3,3));
        r.Add(new Vector2Int(3,4));
        r.Add(new Vector2Int(4,3));
        r.Add(new Vector2Int(4,4));

        return r;
    }

    public virtual SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves){

        return SpecialMove.None;
    }

    public void SetPosition(Vector3 position, bool force = false){

        if(team == 0 && PhotonNetwork.LocalPlayer.IsMasterClient && !isDead){
            desiredPosition = position + ((PhotonNetwork.LocalPlayer.IsMasterClient && MainMenu._34View) ? (Vector3.up * positionOffset) : (!PhotonNetwork.LocalPlayer.IsMasterClient && MainMenu._34View) ? (Vector3.down * positionOffset) : Vector3.zero);
            transform.position = desiredPosition;
        }
        if(team == 1 && !PhotonNetwork.LocalPlayer.IsMasterClient && !isDead){
            desiredPosition = position + ((PhotonNetwork.LocalPlayer.IsMasterClient && MainMenu._34View) ? (Vector3.up * positionOffset) : (!PhotonNetwork.LocalPlayer.IsMasterClient && MainMenu._34View) ? (Vector3.down * positionOffset) : Vector3.zero);
            transform.position = desiredPosition;
        }
        if(isDead){
            desiredPosition = position + ((PhotonNetwork.LocalPlayer.IsMasterClient && MainMenu._34View) ? (Vector3.up * positionOffset) : (!PhotonNetwork.LocalPlayer.IsMasterClient && MainMenu._34View) ? (Vector3.down * positionOffset) : Vector3.zero);
        }

        photonView.RPC(nameof(SyncPos), RpcTarget.AllBufferedViaServer, position.x, position.y, position.z, photonView.ViewID, force);
    }

    public void SetScale(Vector3 scale, bool force = false){

        photonView.RPC(nameof(SyncScale), RpcTarget.AllBufferedViaServer, scale.x, scale.y, scale.z, photonView.ViewID, force);
    }

    [PunRPC]
    public void SyncPos(float x, float y, float z, int viewID, bool force){

        ChessPiece cp = PhotonView.Find(viewID).GetComponent<ChessPiece>();

        Vector3 position = new Vector3(x,y,z);

        cp.desiredPosition = position + ((PhotonNetwork.LocalPlayer.IsMasterClient && MainMenu._34View) ? (Vector3.up * cp.positionOffset) : (!PhotonNetwork.LocalPlayer.IsMasterClient && MainMenu._34View) ? (Vector3.down * cp.positionOffset) : Vector3.zero);

        if(force)
            cp.transform.position = cp.desiredPosition;
    
    }
    [PunRPC]
    public void SyncScale(float x, float y, float z, int viewID, bool force){

        ChessPiece cp = PhotonView.Find(viewID).GetComponent<ChessPiece>();

        Vector3 scale = new Vector3(x,y,z);

        cp.desiredScale = scale;

        if(force)
            cp.transform.localScale = desiredScale;

    }
}
