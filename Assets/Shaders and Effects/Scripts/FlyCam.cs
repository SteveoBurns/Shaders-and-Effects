using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



    /// <summary>
    /// This class handles the movement and pickups for the fly camera.
    /// </summary>
    public class FlyCam : MonoBehaviour
    {
        [Header("Camera Movement Variables")]
        [SerializeField] private float cameraSensitivity = 300;
        [SerializeField] private float moveSpeed = 15;
        [SerializeField] private float xRot = 10;
        [SerializeField] private float yRot = 10;
        
        
        
        // Update is called once per frame
        void Update()
        {
            xRot += Input.GetAxisRaw("Mouse X") * cameraSensitivity * Time.deltaTime;
            yRot += Input.GetAxisRaw("Mouse Y") * cameraSensitivity * Time.deltaTime;
            yRot = Mathf.Clamp(yRot, -90, 90);
            
            transform.localRotation = Quaternion.AngleAxis(xRot, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(yRot, Vector3.left);

            transform.position += transform.forward * moveSpeed * Input.GetAxisRaw("Vertical") * Time.deltaTime;
            transform.position += transform.right * moveSpeed * Input.GetAxisRaw("Horizontal") * Time.deltaTime;

            if(Input.GetKey(KeyCode.E))
            {
                transform.position += transform.up * moveSpeed * Time.deltaTime;
            }
            if(Input.GetKey(KeyCode.Q))
            {
                transform.position += -transform.up * moveSpeed * Time.deltaTime;
            }
        }
        
        
    }
