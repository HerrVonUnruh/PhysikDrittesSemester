using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Hauptklasse f�r die Ball-Interaktionen
public class MainBall : MonoBehaviour
{
    public GameObject[] balls; // Array Balls
    public Wall[] walls; // Array Walls
    public float ballRadius = 0.0285f; // Radius der B�lle
    public Color collisionColor = Color.blue; // Farbe bei Kollision
    public float collisionDuration = 0.1f; // Dauer Kollisionsfarb�nderung
    public float repulsionForce = 0.5f; // Absto�ungskraft Kollisionen
    public float initialSpeed = 2f; // Startgeschwindigkeit B�lle
    public float friction = 0.1f; // Reibung

    private Color[] originalColors; // Urspr�nglichsfarbe B�lle
    private Vector2[] ballVelocities; // B�llegeschwindigkeit

 
    void Start()
    {
        FindBalls(); // B�lle suchen und initialisieren
        FindWalls(); // W�nde suchen

        for (int i = 1; i < balls.Length; i++)
        {
            ballVelocities[i] = Vector2.zero; // Geschwindigkeiten aller B�lle, au�er dem wei�en Ball auf Null setzen
        }
    }

 
    void Update()
    {
        MoveBalls(); // Bewegt B�lle
        CheckCollisions(); // �berpr�ft Kollisionen

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PushWhiteBallTowardsMouse(); // Wei�e Kugel wird zur Mausposition geschoben abh�ngig von der Bildschirmmitte
        }
    }

    // alle B�lle suchen und Originalfarbe speichern, initialisieren der Geschwindigkeit
    void FindBalls()
    {
        balls = GameObject.FindGameObjectsWithTag("Ball");

        originalColors = new Color[balls.Length];
        ballVelocities = new Vector2[balls.Length];

        for (int i = 0; i < balls.Length; i++)
        {
            SpriteRenderer renderer = balls[i].GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                originalColors[i] = renderer.color; // Speichert Originalfarbe
            }

            ballVelocities[i] = Vector2.zero; // Initialisiert Geschwindigkeit = 0
        }
    }

    // Alle W�nde suchen
    void FindWalls()
    {
        walls = FindObjectsOfType<Wall>(); // Alle "Walls finden
    }

    // Alle b�lle bewegen nach geschwindigkeit und Zeit
    void MoveBalls()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].transform.position += (Vector3)ballVelocities[i] * Time.deltaTime; // anpassen der Position

            ballVelocities[i] *= (1 - friction * Time.deltaTime); //Geschwindigkeit nach reibung anpassen                                                             
        }
    }

    //Pr�ft Kollision von B�llen mit W�nden und anderen Kugeln
    void CheckCollisions()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            // Kollisionsbehandlung mit W�nden
            foreach (var wall in walls)
            {
                if (wall.IsCollidingWithBall(balls[i], ballRadius))
                {
                    Vector2 normal = wall.GetCollisionNormal(balls[i].transform.position, ballRadius);
                    Vector2 incomingVelocity = ballVelocities[i];
                    Vector2 reflectedVelocity = Vector2.Reflect(incomingVelocity, normal);
                    ballVelocities[i] = reflectedVelocity;

                    Vector2 newPosition = (Vector2)balls[i].transform.position + reflectedVelocity * Time.deltaTime;
                    balls[i].transform.position = (Vector3)newPosition;

                    StartCoroutine(ResetColorAfterDelay(i)); // Farbe nach Verz�gerung wieder zur�cksetzen
                }
            }

            // Kollision mit anderen B�llen
            for (int j = 0; j < balls.Length; j++)
            {
                if (i != j && AreBallsColliding(balls[i], balls[j], ballRadius * 2))
                {
                    Vector2 direction = (balls[i].transform.position - balls[j].transform.position).normalized;
                    Vector2 repulsionVelocity = direction * repulsionForce;

                    //Absto�kraft auf beide B�lle anwenden, bei Kollision
                   
                    ballVelocities[i] += repulsionVelocity * Time.deltaTime;
                    ballVelocities[j] -= repulsionVelocity * Time.deltaTime;

                    // �ndert die Farbe der kollidierenden B�lle
                    ChangeColor(i);
                    ChangeColor(j);
                }
            }

            // Kollision mit der weissen Kugel
            if (i != 0 && AreBallsColliding(balls[i], balls[0], ballRadius * 2))
            {
                PushBallAwayFromWhiteBall(i);
                StartCoroutine(ResetColorAfterDelay(i)); // Farbe zur�cksetzen nach Verz�gerung
            }
        }
    }

    // Checkt ob zwei B�lle kollidieren in Bezug auf ihren Abstand zueinander
    bool AreBallsColliding(GameObject ball1, GameObject ball2, float collisionDistance)
    {
        float distance = Vector2.Distance(ball1.transform.position, ball2.transform.position);
        return distance <= collisionDistance;
    }

    // warte Zeit ab, dann Farbe des Balls wieder auf Originalfarbe setzen
    IEnumerator ResetColorAfterDelay(int index)
    {
        yield return new WaitForSeconds(collisionDuration);

        SpriteRenderer renderer = balls[index].GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = originalColors[index]; // setzt Originalfarbe zur�ck
        }
    }

    // weisse Kugel richtung Mausposition bewegen
    void PushWhiteBallTowardsMouse()
    {
        GameObject whiteBall = balls[0];

        // Mausposition in Weltkoordinaten umrechnen
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Sicherstellen, dass die Mausposition die selbe Z-Koordinate wie der wei�e Ball hat

        // Berechnet die Richtung von der weissen Kugel zur Mausposition
        Vector2 direction = ((Vector2)mousePosition - (Vector2)whiteBall.transform.position).normalized;

        // Y-Komponente invertieren, um die korrekte Bewegungsrichtung zu gew�hrleisten
        direction.y *= -1;

        // Absto�ungskraft in berechnete Richtung nutzen
        ballVelocities[0] += direction * repulsionForce * Time.deltaTime;
    }

    // Schiebt jeweiligen Ball weg vom weissen Ball
    void PushBallAwayFromWhiteBall(int index)
    {
        GameObject whiteBall = balls[0];
        Vector2 direction = ((Vector2)balls[index].transform.position - (Vector2)whiteBall.transform.position).normalized;
        Vector2 repulsionVelocity = direction * repulsionForce * 2;

        float maxVelocityChange = 0.5f;
        Vector2 currentVelocity = ballVelocities[index];
        Vector2 newVelocity = currentVelocity + repulsionVelocity;
        if (newVelocity.magnitude > currentVelocity.magnitude + maxVelocityChange)
        {
            newVelocity = currentVelocity + (repulsionVelocity.normalized * maxVelocityChange);
        }

        ballVelocities[index] = newVelocity;
    }

    // �ndert Farbe des Balls bei Kollision
    void ChangeColor(int index)
    {
        SpriteRenderer renderer = balls[index].GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = collisionColor; // �ndert Farbe bei Kollisionm
        }
    }
}
