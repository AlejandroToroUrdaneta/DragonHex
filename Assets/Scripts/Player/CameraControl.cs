using Unity.VisualScripting;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float speed = 8f;
    public Camera cam;
    public GameObject player;

    public bool worldView = true;

    void Update()
    {
        if (worldView)
        {
            //Debug.Log("No hay player todav�a");
        }
        else
        {
            UpdateCamera();
            ChangeWorldToCharacter();
        }
        
    }

    private void UpdateCamera()
    {
        // Calcula la nueva posici�n de la c�mara
        Vector3 newPos = player.transform.position - player.transform.forward * 6 + Vector3.up * this.transform.position.y;
        this.transform.position = newPos;

        // Obtiene los valores de entrada para la rotaci�n
        Vector3 input = InputValues(out int yRotation).normalized;

        // Ajusta el campo de visi�n de la c�mara
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView + input.y * 2, 30, 110);

        // Aplica movimiento y zoom a la c�mara
        transform.Translate(input.Flat() * speed * Time.deltaTime);

        // Gira la c�mara alrededor del jugador
        if (yRotation != 0)
        {
            transform.RotateAround(player.transform.position, Vector3.up, yRotation * Time.deltaTime * speed * 4);
        }
    }

    public void ChangeWorldToCharacter()
    {
        // Obtener la posici�n actual del jugador
        Vector3 playerPosition = player.transform.position;

        // Cambiar la altura (coordenada Y) a 12, manteniendo las coordenadas X y Z del jugador
        Vector3 cameraPosition = new Vector3(playerPosition.x, 6, playerPosition.z - 5);

        // Establecer la nueva posici�n de la c�mara
        cam.transform.position = cameraPosition;
    }


    private Vector3 InputValues(out int y)
    {
        //Move and zoom
        Vector3 values = new Vector3();
        values.x = Input.GetAxis("Horizontal");
        values.z = Input.GetAxis("Vertical");
        values.y = -Input.GetAxis("Mouse ScrollWheel");

        //Rotation
        y = 0;
        if (Input.GetKey(KeyCode.Q))
            y = -1;
        else if (Input.GetKey(KeyCode.E))
            y = 1;

        return values;
    }
}

