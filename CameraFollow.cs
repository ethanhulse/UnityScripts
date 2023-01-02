//Author: Ethan H/thvle 2022
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Variables
    public GameObject obj;
    public Camera cam;
    public Vector3 offset;
    [Range(1,10)]
    public float smoothFactor;

    //MIN/MAX values allow the camera to be limited to a certain space so camera will not pan out of bounds/beyond walls or floor. Will differ depending on aspect ration and resolution of game.
    public Vector3 minValues, maxValues;
    #endregion
    // Update is called once per frame
    private void FixedUpdate()
    {
        Follow();
    }
    void Follow()
    {
            Vector3 targetPosition = obj.transform.position + offset;

            Vector3 boundPosition = new Vector3(
                Mathf.Clamp(targetPosition.x, minValues.x, maxValues.x),
                Mathf.Clamp(targetPosition.y, minValues.y, maxValues.y),
                Mathf.Clamp(targetPosition.z, minValues.z, maxValues.z)
            );

            transform.position = Vector3.Lerp(transform.position, boundPosition, smoothFactor * Time.fixedDeltaTime);
    }
}
