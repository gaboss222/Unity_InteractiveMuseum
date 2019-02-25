using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
 * @Author : Griesser Gabriel
 * @Date : 27.04.2018
 */


/*
 * This class is for generate museum
 */
public class SceneGenerator : MonoBehaviour
{

    List<Room> _rooms;
    List<GameObject> _roomGameObject;
    List<GameObject> _wallBetweenRight;
    List<GameObject> _wallBetweenLeft;
    List<GameObject> _roomRight;
    List<GameObject> _roomLeft;

    float[] _roomDim, _roomDimMinimal;
    public GameObject roomPrefab, paintPrefab, corridor, wallBetweenRoomPrefab, wallAboveRoomPrefab, roof;
    Vector3 corridorDim;


    /*
     * Use this for initialization
     */
    void Start()
    {
        JsonReader js = new JsonReader("dbv.json");

        _rooms = js.GetRoomList();
        _roomGameObject = new List<GameObject>();

        _roomRight = new List<GameObject>();
        _roomLeft = new List<GameObject>();

        _roomDim = new float[3];
        _roomDimMinimal = new float[3];

        _roomDimMinimal[0] = 5f;
        _roomDimMinimal[1] = 3f;
        _roomDimMinimal[2] = 5f;

        corridorDim = corridor.transform.localScale;

        CreateAndInitializeRooms();
        SeparateAndPlaceRoom();

    }

    /*
     * Create a room for each author
     * Initialize them
     * Place them along the corridor
     */
    void CreateAndInitializeRooms()
    {
        foreach (Room room in _rooms)
        {


            //a room = GameObject from prefab
            GameObject roomGameObject = GameObject.Instantiate(roomPrefab);
            roomGameObject.name = "room : " + room.GetNameAuthor();

            //Room initialization and calculate width for each Wall
            room.SetPrefab(roomGameObject, paintPrefab);
            room.InitializingRoom();

            //Get room size
            float xSize = room.GetWallLeftRightSize();
            _roomDim[0] = xSize;
            float ySize = room.GetMaxHeight();
            _roomDim[1] = ySize;
            float zSize = room.GetWallBottomSize();
            _roomDim[2] = zSize;

            //Test scale with the minimum dimension
            for (int i = 0; i < _roomDim.Length; i++)
            {
                if (_roomDim[i] < _roomDimMinimal[i])
                    _roomDim[i] = _roomDimMinimal[i];
            }


            roomGameObject.transform.localScale = new Vector3(_roomDim[0], _roomDim[1], _roomDim[2]);

            //Add the paints and position them
            foreach (Paint paint in room._paints)
            {
                room.AddPaint(paint, paint.wall);
            }

            room.PaintSpacing(roomGameObject.transform.Find("WallLeft"));
            room.PaintSpacing(roomGameObject.transform.Find("WallRight"));
            room.PaintSpacing(roomGameObject.transform.Find("WallBottom"));


            _roomGameObject.Add(roomGameObject);
        }

    }

    /*
     * "Smartly" separate the list of rooms in two list :
     * One list is for rooms to the right of the corridor 
     * And the second is for the left of the corridor
     * The difference in length between the right and left room is small --> "Smartly separe"
    */
    void SeparateAndPlaceRoom()
    {
        
        _roomGameObject.Sort((r1, r2) => r1.transform.localScale.z.CompareTo(r2.transform.localScale.z) * -1);
              

        float sumWidthRoomRight = _roomGameObject[0].transform.localScale.z; ;
        float sumWidthRoomLeft = 0;

        _roomRight.Add(_roomGameObject[0]);

        //Fill the two lists
        {
            for (int i = 1; i < _roomGameObject.Count; i++)
            {
                if (sumWidthRoomRight < sumWidthRoomLeft)
                {
                    sumWidthRoomRight += _roomGameObject[i].transform.localScale.z;
                    _roomRight.Add(_roomGameObject[i]);
                }
                else if (sumWidthRoomLeft <= sumWidthRoomRight)
                {
                    sumWidthRoomLeft += _roomGameObject[i].transform.localScale.z;
                    _roomLeft.Add(_roomGameObject[i]);
                }
            }
        }
     
        RoomSpacing();
    }

    /*
     * Set position for all room and wall betweem them. Need a "smart" space between each room
     */
    void RoomSpacing()
    {

        Vector3 roomDim;
        int nbRoomRight = _roomRight.Count;
        int nbRoomLeft = _roomLeft.Count;

        float sumRight = _roomRight.Sum(g => g.transform.localScale.z);
        float sumLeft = _roomLeft.Sum(g => g.transform.localScale.z);
        float sum = (sumRight > sumLeft) ? sumRight : sumLeft;


        //Adjust corridor and roof size
        if (corridorDim.z < sum)
            corridor.transform.localScale = new Vector3(3, 0.1f, sum * 1.5f);

        corridorDim = corridor.transform.localScale;
        roof.transform.localScale = corridorDim;

        //Place the rooms at the beginning right of the corridor
        {
            foreach (GameObject roomGameObject in _roomGameObject)
            {
                roomGameObject.transform.localPosition = new Vector3(corridorDim.x / 2 + roomGameObject.transform.localScale.x / 2, roomGameObject.transform.localScale.y / 2 + 0.01f, -(corridor.transform.localScale.z / 2 - roomGameObject.transform.localScale.z / 2));
            }

            //_roomLeft need to be rotated and placed to the left of the corridor
            {
                foreach (GameObject roomGameObject in _roomLeft)
                {
                    roomGameObject.transform.localRotation = new Quaternion(0, 180f, 0, 0);
                    roomGameObject.transform.localPosition += new Vector3(-(corridorDim.x + roomGameObject.transform.localScale.x), 0, 0);
                }
            }
        }


        float spaceRight = (corridorDim.z - sumRight) / (nbRoomRight + 1);
        float spaceLeft = (corridorDim.z - sumLeft) / (nbRoomLeft + 1);

       
        PlaceWall(spaceRight, spaceLeft);

        //spacing right Rooms and spacing wall between rooms right
        {
            Vector3 roomPositionRight = new Vector3();
            Vector3 wallBetweenRoomRightPos = new Vector3();

            for (int i = 0; i < nbRoomRight; i++)
            {
                GameObject wallBetweenRoom = _wallBetweenRight[i];

                GameObject room = _roomRight[i];
                roomDim = room.transform.localScale;

                roomPositionRight.z += spaceRight;
                room.transform.position += roomPositionRight;
                roomPositionRight.z += room.transform.localScale.z;

                wallBetweenRoomRightPos.z += roomDim.z + wallBetweenRoom.transform.localScale.z;
                wallBetweenRoom.transform.position += wallBetweenRoomRightPos;
            }
        }

        //spacing left Rooms and spacing wall between rooms left
        {
            Vector3 roomPositionLeft = new Vector3();
            Vector3 wallBetweenRoomLeftPos = new Vector3();

            for (int i = 0; i < nbRoomLeft; i++)
            {
                GameObject wallBetweenRoom = _wallBetweenLeft[i];

                GameObject room = _roomLeft[i];
                roomDim = room.transform.localScale;

                roomPositionLeft.z += spaceLeft;
                room.transform.position += roomPositionLeft;
                roomPositionLeft.z += room.transform.localScale.z;

                wallBetweenRoomLeftPos.z += roomDim.z + wallBetweenRoom.transform.localScale.z;
                wallBetweenRoom.transform.position += wallBetweenRoomLeftPos;
            }
        }
      
        PlaceWallAboveEachRoom();
    }

    /*
     * Instantiate and place the wall between each room at the beginning of the corridor
     */
    void PlaceWall(float spaceRight, float spaceLeft)
    {
        _wallBetweenLeft = new List<GameObject>();
        _wallBetweenRight = new List<GameObject>();


        //Wal to the right of the corridor
        for (int i = 0; i < _roomRight.Count + 1; i++)
        {
            GameObject wallBetweenRoom = GameObject.Instantiate(wallBetweenRoomPrefab);
            wallBetweenRoom.name = "WallBetweenRoomRight";

            wallBetweenRoom.transform.localScale = new Vector3(0.3f, 7f, spaceRight);
            Vector3 wallBetweenRoomDim = wallBetweenRoom.transform.localScale;

            wallBetweenRoom.transform.localPosition = new Vector3(corridorDim.x / 2 + wallBetweenRoomDim.x / 2, wallBetweenRoomDim.y / 2, -(corridorDim.z / 2 - wallBetweenRoomDim.z / 2));
            _wallBetweenRight.Add(wallBetweenRoom);
        }

        //and to the left
        for (int i = 0; i < _roomLeft.Count + 1; i++)
        {
            GameObject wallBetweenRoom = GameObject.Instantiate(wallBetweenRoomPrefab);
            wallBetweenRoom.name = "WallBetweenRoomLeft";

            wallBetweenRoom.transform.localScale = new Vector3(0.3f, 7f, spaceLeft);
            Vector3 wallBetweenRoomDim = wallBetweenRoom.transform.localScale;

            wallBetweenRoom.transform.localPosition = new Vector3(-(corridorDim.x / 2 + wallBetweenRoomDim.x / 2), wallBetweenRoomDim.y / 2, -(corridorDim.z / 2 - wallBetweenRoomDim.z / 2));
            _wallBetweenLeft.Add(wallBetweenRoom);
        }
    }

    /*
     * Instantiate and place wall above each room
     */
    void PlaceWallAboveEachRoom()
    {
        foreach (GameObject room in _roomRight)
        {
            Vector3 roomDim = new Vector3(room.transform.localScale.x, room.transform.localScale.y, room.transform.localScale.z);

            GameObject wallAboveRoom = GameObject.Instantiate(wallAboveRoomPrefab);
            wallAboveRoom.name = "WallAboveRoomRight";
            wallAboveRoom.transform.localScale = new Vector3(0.3f, 7 - roomDim.y, roomDim.z);
            wallAboveRoom.transform.position = new Vector3(corridorDim.x / 2 + wallAboveRoom.transform.localScale.x / 2, wallAboveRoom.transform.localScale.y / 2 + roomDim.y, room.transform.position.z);
        }

        foreach (GameObject room in _roomLeft)
        {
            Vector3 roomDim = new Vector3(room.transform.localScale.x, room.transform.localScale.y, room.transform.localScale.z);

            GameObject wallAboveRoom = GameObject.Instantiate(wallAboveRoomPrefab);
            wallAboveRoom.name = "WallAboveRoomLeft";
            wallAboveRoom.transform.localScale = new Vector3(0.3f, 7 - roomDim.y, roomDim.z);
            wallAboveRoom.transform.position = new Vector3(-(corridorDim.x / 2 + wallAboveRoom.transform.localScale.x / 2), wallAboveRoom.transform.localScale.y / 2 + roomDim.y, room.transform.position.z);
        }
    }
}
