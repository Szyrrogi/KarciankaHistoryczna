using UnityEngine;

public class DragCard : MonoBehaviour
{
    public GameObject pole;       // Pole, do którego karta ma się przyciągnąć
    private float snapDistanceX = 24f; // Maksymalna odległość przyciągania w osi X
    private float snapDistanceZ = 3.5f; // Maksymalna odległość przyciągania w osi Z

    private Vector3 startPosition;
    private bool isDragging = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        startPosition = transform.position;
    }

    void OnMouseDown()
    {
        isDragging = true;
    }

    void OnMouseUp()
    {
        isDragging = false;

        // Liczymy różnicę w X i Z
        float diffX = Mathf.Abs(transform.position.x - pole.transform.position.x);
        float diffZ = Mathf.Abs(transform.position.z - pole.transform.position.z);

        // Debug w konsoli ile brakuje
        Debug.Log($"Odległość X: {diffX:F3}, Odległość Z: {diffZ:F3}");

        // Sprawdzamy, czy mieści się w zakresie przyciągania
        if (diffX <= snapDistanceX && diffZ <= snapDistanceZ)
        {
            transform.position = pole.transform.position; // Przyciągnięcie
        }
        else
        {
            transform.position = startPosition; // Powrót do startu
        }
    }

    void Update()
    {
        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 hitPoint = ray.GetPoint(distance);
                transform.position = new Vector3(hitPoint.x, startPosition.y, hitPoint.z);
            }


            float diffX = Mathf.Abs(transform.position.x - pole.transform.position.x);
            float diffZ = Mathf.Abs(transform.position.z - pole.transform.position.z);
            // Zielony
            if (diffX <= snapDistanceX && diffZ <= snapDistanceZ)
            {
                pole.GetComponent<Renderer>().material.color = Color.green;
            }
            else
            {
                pole.GetComponent<Renderer>().material.color = Color.white;
            }

        }
    }
}
