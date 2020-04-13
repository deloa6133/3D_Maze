using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera_Script : MonoBehaviour
{
    private float cameraPositionX = Spawn_Script.xSize;
    private float cameraPositionY = Spawn_Script.ySize;

    public void moveCameraLocation()
    {
        //changes the camera orthographic size and location based on size of maze
        if (cameraPositionX > cameraPositionY)
        {
            Camera.main.orthographicSize = cameraPositionX * 1.2f;
            transform.position = new Vector3(0, cameraPositionX / 2, cameraPositionY / 2 - 1);
        }
        else if (cameraPositionY > cameraPositionX)
        {
            Camera.main.orthographicSize = cameraPositionY * 1.2f;
            transform.position = new Vector3(0, cameraPositionY, cameraPositionY / 2 - 1);
        }
        else if (cameraPositionX == cameraPositionY)
        {
            Camera.main.orthographicSize = cameraPositionX * 1.2f;
            transform.position = new Vector3(0, cameraPositionX, cameraPositionY / 2 - 1);
        }
    }

    //retrieves information from the sliders to determine size of the orthographic camera
    public void Change_xSize(float x)
    {
        cameraPositionX = (int)x;
    }
    public void Change_ySize(float y)
    {
        cameraPositionY = (int)y;
    }
}
