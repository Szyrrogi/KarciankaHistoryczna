using UnityEngine;

public class DragCard : MonoBehaviour
{
    public GameObject pole;       // Pole, do którego karta ma się przyciągnąć
    private float snapDistanceX = 24f; // Maksymalna odległość przyciągania w osi X
    private float snapDistanceZ = 3.5f; // Maksymalna odległość przyciągania w osi Z

    private bool isDragging = false;
    private Camera mainCamera;

    public static GameObject ChosenCard;

    void Start()
    {
        pole = ArenaManager.arenaManager.arena;
        mainCamera = Camera.main;
    }

    Vector3 originalEuler;
    Vector3 originalScale;
    Vector3 originalPosition;

    void OnMouseEnter() //powiększa
    {
        if (ChosenCard == null && isDragging == false && BattleManager.battleManager.Hand.Contains(this.gameObject))
        {
            ChosenCard = this.gameObject;

            // zapamiętujemy oryginalne wartości
            originalEuler = transform.eulerAngles;
            originalScale = transform.localScale;
            originalPosition = transform.position;

            // powiększamy
            transform.localScale *= 1.4f;

            // przesuwamy
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y + 0.1f,
                transform.position.z - 3f
            );

            // ustawiamy rotację na 0 w osi Y
            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                0f,
                transform.eulerAngles.z
            );
        }
    }

    void OnMouseExit()  //pomniejsza
    {
        if (ChosenCard == this.gameObject && isDragging == false)
        {
            ChosenCard = null;

            // przywracamy oryginalne wartości
            transform.localScale = originalScale;
            transform.position = originalPosition;
            transform.eulerAngles = originalEuler;
        }
    }

    void OnMouseDown()  //klikniaesz
    {
        if(BattleManager.battleManager.Hand.Contains(this.gameObject))
        {
            isDragging = true;
            OnMouseExit();
        }
    }

    void OnMouseUp()    //spada
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
            BattleManager.battleManager.PutOnBattlefild(this.gameObject);
        }
        else
        {
            BattleManager.battleManager.ArrangeHand();
        }
    }

    void Update()   //ruszać
    {
        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 hitPoint = ray.GetPoint(distance);
                transform.position = new Vector3(hitPoint.x, transform.position.y, hitPoint.z);
            }


    //         float diffX = Mathf.Abs(transform.position.x - pole.transform.position.x);
    //         float diffZ = Mathf.Abs(transform.position.z - pole.transform.position.z);
    //         // Zielony
    //         if (diffX <= snapDistanceX && diffZ <= snapDistanceZ)
    //         {
    //             pole.GetComponent<Renderer>().material.color =  new Color(0, 1f, 0, 0.5f);
    //         }
    //         else
    //         {
    //             pole.GetComponent<Renderer>().material.color = Color.white;
    //         }

        }
    }
}
